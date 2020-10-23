using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Woodchuck.Models;

namespace Woodchuck
{
    public class FetcherWorker : BackgroundService
    {
        private readonly ILogger<FetcherWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly LogQueue _logs;
        private readonly IHttpClientFactory _clientFactory;

        #region LogCollector Settings

        private static readonly Dictionary<string, List<JObject>> _hits = new Dictionary<string, List<JObject>>();
        private static readonly DateTimeOffset _startDate = DateTimeOffset.UtcNow.AddDays(-1);
        private static readonly TimeSpan _duration = TimeSpan.FromDays(15);
        private static readonly DateTimeOffset _endDate = _startDate.Add(_duration);

        private const string _baseUri = "https://f5df450f-dc8b-4dea-94c2-06db6e2a2aae-es.logit.io/";
        private const string _apiKey = "e0142d36-46af-4d94-b901-ca261dee9b58";
        private const string _basePath = "/Users/sporritt.HS/queries";
        private const string _matchField = "account";
        private const string _matchValue = "GS";
        private static readonly string _jsonFile = CalculateFileName();

        private static string CalculateFileName() =>
            $"{_basePath}/{_startDate.Year}{_startDate.Month:D2}{_startDate.Day:D2}-{_duration.Days}day.json";

        #endregion

        public FetcherWorker(ILogger<FetcherWorker> logger, IServiceScopeFactory scopeFactory, LogQueue logs, IHttpClientFactory clientFactory)
        {
            _logs = logs;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _clientFactory = clientFactory;
        }

        private string Parameterise(string raw) => raw
            .Replace("\"gte\": \"\"", $"\"gte\": \"{_startDate:o}\"")
            .Replace("\"lte\": \"\"", $"\"lte\": \"{_endDate:o}\"")
            .Replace("\"match_phrase\": { \"\": \"\" }", $"\"match_phrase\": {{ \"{_matchField}\": \"{_matchValue}\" }}");

        private Log JSonToLog(JObject jsonLog)
        {
            var js = new JsonSerializer();

            //foreach (var xid in _hits.Keys.ToArray())
            //{
            //    var list = _hits[xid];

            //    foreach (var item in list)
            //    {
            try
            {
                var log = js.Deserialize<Log>(jsonLog.CreateReader());
                return log;
            }
            catch
            {
                // TODO : An exception occurs when we receive "<none>" for Xid when we're expecting a Guid
                // We should handle that with a replace before we attept to Deserialize
                return null;
            }
        }

        private string ResourceToString(string name) => new StreamReader(name).ReadToEnd();

        private string InitiateSearch(HttpClient client, string queryName)
        {
            // configure the request string and query
            var searchUri = new Uri($"{_baseUri}_search?apikey={_apiKey}&scroll=1m");
            var query = Parameterise(ResourceToString($"queries/{queryName}.json"));

            _logger.LogInformation($"Sending query: {query}");

            // build the query
            var queryContent = new StringContent(query, Encoding.UTF8, "application/json");

            // send the query and get a response
            var searchResponse = client.PostAsync(searchUri, queryContent).Result;

            // read and parse the response
            var json = searchResponse.Content.ReadAsStringAsync().Result;
            var result = JObject.Parse(json);

            // store any hits
            QueueResults(result);

            // return the scrollId
            return result["_scroll_id"].Value<string>();
        }

        private bool ScrollResults(HttpClient client, string scrollId)
        {
            _logger.LogInformation("Scrolling results.");

            // configure the request string
            var scrollUri = new Uri($"{_baseUri}_search/scroll?apikey={_apiKey}");

            // load the scroll.json template, injecting the current scrollId
            var scrollTemplate = ResourceToString("scroll.json").Replace("{scrollId}", scrollId);
            var scrollContent = new StringContent(scrollTemplate, Encoding.UTF8, "application/json");

            // send the request and read the response
            var scrollResponse = client.PostAsync(scrollUri, scrollContent).Result;
            var json = scrollResponse.Content.ReadAsStringAsync().Result;

            // parse the response as a JObject
            var result = JObject.Parse(json);

            return QueueResults(result);
        }

        private bool QueueResults(JObject result)
        {
            _logger.LogInformation("Queing logs.");

            var found = result["hits"]["hits"]
                .Values<JObject>()
                .Select(o => o["_source"]
                .Value<JObject>());

            foreach (var jsonObject in found)
            {
                // convert this item to a Log
                var log = JSonToLog(jsonObject);

                // queue it up to be processed
                if (log != null)
                    _logs.Enqueue(log);

                //var xid = jsonObject["xid"].Value<string>();
                //// find or add a parent item with the current xid
                //if (!_hits.ContainsKey(xid))
                //    _hits.Add(xid, new List<JObject>());
                //var parent = _hits[xid];
                //parent.Add(jsonObject);
            }

            return found.Any();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var maxAttempts = 100;
            var currentAttempt = 0;
            using var client = _clientFactory.CreateClient();

            _logger.LogInformation("Starting initial search");
            var scrollId = InitiateSearch(client, "simple");

            do
            {
                _logger.LogInformation($"Scrolling results page { currentAttempt }.");

                // do some work
                _logs.IsFetchingComplete = !ScrollResults(client, scrollId);

                // now we're tired from all that work, so have a snooze
                await Task.Delay(10, stoppingToken);
            } while (!stoppingToken.IsCancellationRequested && !_logs.IsFetchingComplete && ++currentAttempt < maxAttempts);

            _logger.LogInformation("Fetching logs complete.");
        }
    }
}
