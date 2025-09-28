using EMR.Domain.Entities;

namespace EMR.Infrastructure.Repositories;

public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(Guid patientId, CancellationToken cancellationToken);
    Task AddAsync(Patient patient, CancellationToken cancellationToken);
    Task UpdateAsync(Patient patient, CancellationToken cancellationToken);
}

