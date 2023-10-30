﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProductParser.DAL;

public class IntegrationDbContextDesignFactory : IDesignTimeDbContextFactory<IntegrationDbContext>{
    public IntegrationDbContext CreateDbContext(string[] args) {
        var optionsBuilder = new DbContextOptionsBuilder<IntegrationDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=Test;Username=postgres;Password=123456;Port=5432");

        return new IntegrationDbContext(optionsBuilder.Options);
    }
}
public sealed class IntegrationDbContext : DbContext
{
    private readonly IConfiguration Configuration;
    public IntegrationDbContext(DbContextOptions<IntegrationDbContext> options) : base(options) {
        //Database.EnsureCreated();   // создаем базу данных при первом обращении
        //Database.Migrate();
    }
    private static DbContextOptions GetOptions(string connectionString) {
        return new DbContextOptionsBuilder().UseNpgsql(connectionString).Options;
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        //optionsBuilder.UseSqlServer("Server=.;Database=KafeDev;Trusted_Connection=True;");
        //optionsBuilder.UseSqlServer("Initial Catalog=KafeDev;Data Source=.\\Resto;User ID=resto;Password=resto#test;TrustServerCertificate=True;");
    }
    #region Models
    #endregion
}