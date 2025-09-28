using EMR.Domain.Entities;
using EMR.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace EMR.Infrastructure;

public class EMRDbContext : DbContext
{
    public EMRDbContext(DbContextOptions<EMRDbContext> options) : base(options)
    {
    }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("patient");

        modelBuilder.Entity<Patient>(b =>
        {
            b.ToTable("Patients");
            b.HasKey(x => x.PatientId);
            b.Property(x => x.PatientId).ValueGeneratedNever();
            b.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            b.Property(x => x.MiddleName).HasMaxLength(100);
            b.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            b.Property(x => x.Gender).HasMaxLength(50);
            b.Property(x => x.Phone).HasMaxLength(50);
            b.Property(x => x.Email).HasMaxLength(200);
            b.Property(x => x.City).HasMaxLength(100);
            b.Property(x => x.State).HasMaxLength(100);
        });

        modelBuilder.Entity<OutboxMessage>(b =>
        {
            b.ToTable("OutboxMessages", schema: "core");
            b.HasKey(x => x.OutboxMessageId);
            b.Property(x => x.OutboxMessageId).ValueGeneratedNever();
            b.Property(x => x.MessageId).IsRequired();
            b.Property(x => x.Domain).HasMaxLength(50).IsRequired();
            b.Property(x => x.EventType).HasMaxLength(100).IsRequired();
            b.Property(x => x.Payload).IsRequired();
        });
    }
}

