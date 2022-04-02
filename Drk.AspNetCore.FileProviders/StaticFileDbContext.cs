using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Sqlite;
using Dapper;
using System.Diagnostics.CodeAnalysis;

namespace Drk.AspNetCore.FileProviders
{

    public class StaticFileDbContext : DbContext
    {
        const int DIRECTORY_IDX = -1;
        public DbSet<StaticFileIndex> StaticFileIndices { get; set; } = null!;
        public DbSet<StaticFileData> StaticFileDatas { get; set; } = null!;
        
        public StaticFileDbContext(DbContextOptions<StaticFileDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // optionsBuilder.UseSqlite(@"Data Source=static-files.sqlite");
            // optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Initial Catalog=StaticFiles;Integrated Security=True");
        }



    }

}