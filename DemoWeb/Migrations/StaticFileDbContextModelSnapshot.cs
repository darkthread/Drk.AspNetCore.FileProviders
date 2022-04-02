﻿// <auto-generated />
using System;
using Drk.AspNetCore.FileProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DemoWeb.Migrations
{
    [DbContext(typeof(StaticFileDbContext))]
    partial class StaticFileDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.3");

            modelBuilder.Entity("Drk.AspNetCore.FileProviders.StaticFileData", b =>
                {
                    b.Property<int>("FileDataId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClientIp")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("Content")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("TEXT");

                    b.Property<string>("Remark")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(1)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.HasKey("FileDataId");

                    b.ToTable("StaticFileDatas");
                });

            modelBuilder.Entity("Drk.AspNetCore.FileProviders.StaticFileIndex", b =>
                {
                    b.Property<string>("Path")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<int>("FileDataId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastUpdate")
                        .HasColumnType("TEXT");

                    b.Property<int>("Size")
                        .HasColumnType("INTEGER");

                    b.HasKey("Path");

                    b.ToTable("StaticFileIndices");
                });
#pragma warning restore 612, 618
        }
    }
}