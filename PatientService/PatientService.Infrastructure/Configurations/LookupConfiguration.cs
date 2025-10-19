using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientService.Domain.Entities;

namespace PatientService.Infrastructure.Configurations;

public class LookupConfiguration<TLookup> : IEntityTypeConfiguration<TLookup> where TLookup : LookupEntity
{
    public void Configure(EntityTypeBuilder<TLookup> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Name).HasMaxLength(150).IsRequired();
        builder.Property(l => l.IsActive).HasDefaultValue(true);
        builder.Property(l => l.SortOrder).HasDefaultValue(0);
        builder.HasIndex(l => l.Name).IsUnique();
    }
}
