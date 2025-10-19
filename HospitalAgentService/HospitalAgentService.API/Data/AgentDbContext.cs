using Microsoft.EntityFrameworkCore;
using HospitalAgentService.API.Models;

namespace HospitalAgentService.API.Data;

/// <summary>
/// Database context for Hospital Agent Service (ag schema)
/// </summary>
public class AgentDbContext : DbContext
{
    public AgentDbContext(DbContextOptions<AgentDbContext> options) : base(options)
    {
    }

    public DbSet<HospitalAgent> Agents { get; set; }
    public DbSet<AgentSession> Sessions { get; set; }
    public DbSet<IntakeRequest> IntakeRequests { get; set; }
    public DbSet<TransferLog> TransferLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("ag");

        modelBuilder.Entity<HospitalAgent>(entity =>
        {
            entity.HasKey(e => e.AgentId);
            entity.Property(e => e.AgentId).ValueGeneratedOnAdd();
            entity.Property(e => e.MachineName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.HospitalName).HasMaxLength(300).IsRequired();
        });

        modelBuilder.Entity<AgentSession>(entity =>
        {
            entity.HasKey(e => e.SessionId);
            entity.Property(e => e.SessionId).ValueGeneratedOnAdd();
            entity.HasOne(e => e.Agent)
                .WithMany()
                .HasForeignKey(e => e.AgentId);
        });

        modelBuilder.Entity<IntakeRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId);
            entity.Property(e => e.RequestId).ValueGeneratedOnAdd();
            entity.HasOne(e => e.Agent)
                .WithMany()
                .HasForeignKey(e => e.AgentId);
        });

        modelBuilder.Entity<TransferLog>(entity =>
        {
            entity.HasKey(e => e.LogId);
            entity.Property(e => e.LogId).ValueGeneratedOnAdd();
            entity.HasOne(e => e.Agent)
                .WithMany()
                .HasForeignKey(e => e.AgentId);
        });
    }
}

