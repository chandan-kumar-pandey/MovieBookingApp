using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Models;
using Microsoft.Extensions.Configuration;

namespace Domain.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserDetails> UserDetails { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Ticket> Tickets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserDetails>()
        .HasIndex(u => u.LoginID)
        .IsUnique();

        modelBuilder.Entity<UserDetails>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Movie>()
            .HasIndex(m => m.MovieName)
            .IsUnique();
    }
}