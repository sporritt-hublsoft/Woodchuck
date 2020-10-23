using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Woodchuck.Models;

namespace Woodchuck
{
    public class SaverWorker : BackgroundService
    {
        private readonly ILogger<SaverWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly LogQueue _logs;

        public SaverWorker(ILogger<SaverWorker> logger, IServiceScopeFactory scopeFactory, LogQueue logs)
        {
            _logs = logs;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        /// <summary>
        /// Dequeues all logs from the _logs Queue and inserts them into the database
        /// </summary>
        private void SaveLogs()
        {
            var batchLimit = 1000;
            var itemsToSave = 0;
            var logsToSave = new List<Log>();

            while (_logs.Count > 0 && itemsToSave < batchLimit)
            {
                var log = _logs.Dequeue();
                if (log != null)
                {
                    logsToSave.Add(log);
                    itemsToSave++;
                }
            }

            if (logsToSave.Any())
            {
                _logger.LogInformation("Saving logs @ {time}", DateTimeOffset.Now);
                using var scope = _scopeFactory.CreateScope();
                using var dbContext = scope.ServiceProvider.GetRequiredService<WoodchuckContext>();

                dbContext.AddRange(logsToSave);
                dbContext.SaveChanges();
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested && !_logs.IsProcessingComplete)
            {
                SaveLogs();
                await Task.Delay(10, stoppingToken);
            }

            _logger.LogInformation("Saving logs complete.");
        }
    }
}
