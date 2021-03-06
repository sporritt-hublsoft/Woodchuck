﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Woodchuck.Models;

namespace Woodchuck.Migrations
{
    [DbContext(typeof(WoodchuckContext))]
    [Migration("20201023080231_add-extra-columns")]
    partial class addextracolumns
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Woodchuck.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("Woodchuck.Models.Log", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccountName")
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("Environment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EventId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset?>("EventTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("MessageLevel")
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("ShortMessage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("User")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("Xid")
                        .HasColumnName("xid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("Log");
                });

            modelBuilder.Entity("Woodchuck.Models.LogCategory", b =>
                {
                    b.Property<int>("LogId")
                        .HasColumnType("int");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.HasKey("LogId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("LogCategory");
                });

            modelBuilder.Entity("Woodchuck.Models.LogCategory", b =>
                {
                    b.HasOne("Woodchuck.Models.Category", "Category")
                        .WithMany("LogCategory")
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("FK_LogCategory_Category")
                        .IsRequired();

                    b.HasOne("Woodchuck.Models.Log", "Log")
                        .WithMany("LogCategory")
                        .HasForeignKey("LogId")
                        .HasConstraintName("FK_LogCategory_Log")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
