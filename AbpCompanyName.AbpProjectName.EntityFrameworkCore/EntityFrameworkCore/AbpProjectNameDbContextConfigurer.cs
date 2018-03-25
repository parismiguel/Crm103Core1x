using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace AbpCompanyName.AbpProjectName.EntityFrameworkCore
{
    public static class AbpProjectNameDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<AbpProjectNameDbContext> builder, string connectionString)
        {
            //TODO: SQL to PostGreSQL
            //builder.UseSqlServer(connectionString);
            builder.UseNpgsql(connectionString);
        }

        //public static void Configure(DbContextOptionsBuilder<AbpProjectNameDbContext> builder, DbConnection connection)
        //{
        //    builder.UseNpgsql(connection);
        //}
    }
}