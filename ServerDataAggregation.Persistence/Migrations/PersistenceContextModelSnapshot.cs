﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ServerDataAggregation.Persistence;

#nullable disable

namespace ServerDataAggregation.Persistence.Migrations
{
    [DbContext(typeof(PersistenceContext))]
    partial class PersistenceContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ServerDataAggregation.Persistence.Models.PlayerMatch", b =>
                {
                    b.Property<int>("PlayerMatchId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("player_match_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("PlayerMatchId"));

                    b.Property<int>("Frags")
                        .HasColumnType("integer")
                        .HasColumnName("frags");

                    b.Property<string>("Model")
                        .HasColumnType("text")
                        .HasColumnName("model");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("NameRaw")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name_raw");

                    b.Property<int>("Number")
                        .HasColumnType("integer")
                        .HasColumnName("number");

                    b.Property<int>("PantColor")
                        .HasColumnType("integer")
                        .HasColumnName("pant_color");

                    b.Property<DateTime?>("PlayerMatchEnd")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("player_match_end");

                    b.Property<DateTime?>("PlayerMatchStart")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("player_match_start");

                    b.Property<int>("PlayerType")
                        .HasColumnType("integer")
                        .HasColumnName("type");

                    b.Property<int>("ShirtColor")
                        .HasColumnType("integer")
                        .HasColumnName("shirt_color");

                    b.Property<string>("Skin")
                        .HasColumnType("text")
                        .HasColumnName("skin");

                    b.Property<int>("server_match_id")
                        .HasColumnType("integer");

                    b.HasKey("PlayerMatchId");

                    b.HasIndex("server_match_id");

                    b.ToTable("player_match");
                });

            modelBuilder.Entity("ServerDataAggregation.Persistence.Models.PlayerMatchProgress", b =>
                {
                    b.Property<int>("PlayerMatchProgressId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("player_match_progress_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("PlayerMatchProgressId"));

                    b.Property<int>("Frags")
                        .HasColumnType("integer")
                        .HasColumnName("frags");

                    b.Property<string>("Model")
                        .HasColumnType("text")
                        .HasColumnName("model");

                    b.Property<int>("PantColor")
                        .HasColumnType("integer")
                        .HasColumnName("pant_color");

                    b.Property<int>("ShirtColor")
                        .HasColumnType("integer")
                        .HasColumnName("shirt_color");

                    b.Property<string>("Skin")
                        .HasColumnType("text")
                        .HasColumnName("skin");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("timestamp");

                    b.Property<int>("player_match_id")
                        .HasColumnType("integer");

                    b.Property<int>("server_match_id")
                        .HasColumnType("integer");

                    b.HasKey("PlayerMatchProgressId");

                    b.HasIndex("player_match_id");

                    b.HasIndex("server_match_id");

                    b.ToTable("player_match_progress");
                });

            modelBuilder.Entity("ServerDataAggregation.Persistence.Models.Server", b =>
                {
                    b.Property<int>("ServerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("server_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ServerId"));

                    b.Property<bool>("Active")
                        .HasColumnType("boolean")
                        .HasColumnName("active");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("address");

                    b.Property<string>("ApiKey")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("api_key");

                    b.Property<string>("Country")
                        .HasColumnType("text")
                        .HasColumnName("country");

                    b.Property<int>("GameId")
                        .HasColumnType("integer")
                        .HasColumnName("game_id");

                    b.Property<string>("Locality")
                        .HasColumnType("text")
                        .HasColumnName("locality");

                    b.Property<string>("Metadata")
                        .HasColumnType("text")
                        .HasColumnName("metadata");

                    b.Property<string>("Mod")
                        .HasColumnType("text")
                        .HasColumnName("mod");

                    b.Property<string>("Parameters")
                        .HasColumnType("text")
                        .HasColumnName("parameters");

                    b.Property<int>("Port")
                        .HasColumnType("integer")
                        .HasColumnName("port");

                    b.Property<int>("QueryInterval")
                        .HasColumnType("integer")
                        .HasColumnName("query_interval");

                    b.HasKey("ServerId");

                    b.ToTable("server");
                });

            modelBuilder.Entity("ServerDataAggregation.Persistence.Models.ServerMatch", b =>
                {
                    b.Property<int>("ServerMatchId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("server_match_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ServerMatchId"));

                    b.Property<string>("Map")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("map");

                    b.Property<DateTime?>("MatchEnd")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("match_end");

                    b.Property<DateTime>("MatchStart")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("match_start");

                    b.Property<string>("Mod")
                        .HasColumnType("text")
                        .HasColumnName("mod");

                    b.Property<string>("Mode")
                        .HasColumnType("text")
                        .HasColumnName("mode");

                    b.Property<int>("server_id")
                        .HasColumnType("integer");

                    b.HasKey("ServerMatchId");

                    b.HasIndex("server_id");

                    b.ToTable("server_match");
                });

            modelBuilder.Entity("ServerDataAggregation.Persistence.Models.ServerSnapshot", b =>
                {
                    b.Property<int>("ServerSnapshotId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("server_snapshot_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ServerSnapshotId"));

                    b.Property<string>("Hostname")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("hostname");

                    b.Property<string>("IpAddress")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("ip_address");

                    b.Property<string>("Map")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("map");

                    b.Property<int>("MaxPlayers")
                        .HasColumnType("integer")
                        .HasColumnName("max_players");

                    b.Property<string>("Mod")
                        .HasColumnType("text")
                        .HasColumnName("mod");

                    b.Property<string>("Mode")
                        .HasColumnType("text")
                        .HasColumnName("mode");

                    b.Property<string>("Players")
                        .IsRequired()
                        .HasColumnType("jsonb")
                        .HasColumnName("players");

                    b.Property<int>("ServerId")
                        .HasColumnType("integer")
                        .HasColumnName("server_id");

                    b.Property<string>("ServerSettings")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("server_settings");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("timestamp");

                    b.HasKey("ServerSnapshotId");

                    b.ToTable("server_snapshot");
                });

            modelBuilder.Entity("ServerDataAggregation.Persistence.Models.ServerState", b =>
                {
                    b.Property<int>("ServerStateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("server_state_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ServerStateId"));

                    b.Property<int>("FailedQueryAttempts")
                        .HasColumnType("integer")
                        .HasColumnName("failed_query_attempts");

                    b.Property<string>("Hostname")
                        .HasColumnType("text")
                        .HasColumnName("hostname");

                    b.Property<string>("IpAddress")
                        .HasColumnType("text")
                        .HasColumnName("ip_address");

                    b.Property<DateTime?>("LastQuery")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_query");

                    b.Property<int>("LastQueryResult")
                        .HasColumnType("integer")
                        .HasColumnName("query_result");

                    b.Property<string>("Map")
                        .HasColumnType("text")
                        .HasColumnName("map");

                    b.Property<int>("MaxPlayers")
                        .HasColumnType("integer")
                        .HasColumnName("max_players");

                    b.Property<string>("Mod")
                        .HasColumnType("text")
                        .HasColumnName("mod");

                    b.Property<string>("Mode")
                        .HasColumnType("text")
                        .HasColumnName("mode");

                    b.Property<string>("Players")
                        .HasColumnType("jsonb")
                        .HasColumnName("players");

                    b.Property<string>("ServerSettings")
                        .HasColumnType("text")
                        .HasColumnName("server_settings");

                    b.Property<DateTime?>("TimeStamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("timestamp");

                    b.Property<int>("server_id")
                        .HasColumnType("integer");

                    b.HasKey("ServerStateId");

                    b.HasIndex("server_id");

                    b.ToTable("server_state");
                });

            modelBuilder.Entity("ServerDataAggregation.Persistence.Models.PlayerMatch", b =>
                {
                    b.HasOne("ServerDataAggregation.Persistence.Models.ServerMatch", "ServerMatch")
                        .WithMany("PlayerMatches")
                        .HasForeignKey("server_match_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ServerMatch");
                });

            modelBuilder.Entity("ServerDataAggregation.Persistence.Models.PlayerMatchProgress", b =>
                {
                    b.HasOne("ServerDataAggregation.Persistence.Models.PlayerMatch", "PlayerMatch")
                        .WithMany()
                        .HasForeignKey("player_match_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ServerDataAggregation.Persistence.Models.ServerMatch", "ServerMatch")
                        .WithMany()
                        .HasForeignKey("server_match_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PlayerMatch");

                    b.Navigation("ServerMatch");
                });

            modelBuilder.Entity("ServerDataAggregation.Persistence.Models.ServerMatch", b =>
                {
                    b.HasOne("ServerDataAggregation.Persistence.Models.Server", "Server")
                        .WithMany()
                        .HasForeignKey("server_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");
                });

            modelBuilder.Entity("ServerDataAggregation.Persistence.Models.ServerState", b =>
                {
                    b.HasOne("ServerDataAggregation.Persistence.Models.Server", "ServerDefinition")
                        .WithMany()
                        .HasForeignKey("server_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ServerDefinition");
                });

            modelBuilder.Entity("ServerDataAggregation.Persistence.Models.ServerMatch", b =>
                {
                    b.Navigation("PlayerMatches");
                });
#pragma warning restore 612, 618
        }
    }
}
