using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PromptPlayground.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptPlayground.Services
{
    internal class DbStore : DbContext
    {
        static string defaultConnectionString;
        public static DbStore NewScoped
        {
            get
            {
                var options = new DbContextOptionsBuilder<DbStore>()
                               .UseSqlite(defaultConnectionString)
                               .Options;
                return new DbStore(options);
            }
        }

        static DbStore()
        {
            var path = new ProfileService<DbStore>("store.db").ProfilePath();
            defaultConnectionString = $"Data Source={path};Mode=ReadWriteCreate;Cache=Shared";

            var db = NewScoped;
            db.Database.Migrate();
        }

        public DbStore(DbContextOptions<DbStore> options) : base(options)
        {

        }

        public DbSet<GenerationResultStore> GenerationResultStores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GenerationResultStore>(
                entity =>
                {
                    entity.OwnsOne(_ => _.Usage, builder =>
                    {
                        builder.ToJson();
                    });
                });
        }
    }

    internal class DbStoreContextFactory : IDesignTimeDbContextFactory<DbStore>
    {
        public DbStore CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<DbStore>()
                              .UseSqlite("Data Source=design.db")
                              .Options;
            return new DbStore(options);
        }
    }
}
