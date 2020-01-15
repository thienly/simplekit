﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OrderWorker.EntityConfigurations;

namespace OrderWorker.Migrations
{
    [DbContext(typeof(OrderDbContext))]
    [Migration("20200114075013_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("OrderWorker.Domains.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Order");
                });

            modelBuilder.Entity("SimpleKit.StateMachine.SagaStateProxy", b =>
                {
                    b.Property<Guid>("SagaId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CurrentState")
                        .IsRequired()
                        .HasColumnName("CurrentState")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Direction")
                        .HasColumnType("int");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("bit");

                    b.Property<string>("NextState")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SagaDefinitionType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SagaState")
                        .IsRequired()
                        .HasColumnName("Data")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SagaId", "Version");

                    b.ToTable("SagaState");
                });
#pragma warning restore 612, 618
        }
    }
}
