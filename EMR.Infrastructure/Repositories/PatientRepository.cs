using EMR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EMR.Infrastructure.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly EMRDbContext _dbContext;

    public PatientRepository(EMRDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Patient?> GetByIdAsync(Guid patientId, CancellationToken cancellationToken)
    {
        return await _dbContext.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PatientId == patientId && !p.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(Patient patient, CancellationToken cancellationToken)
    {
        await _dbContext.Patients.AddAsync(patient, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Patient patient, CancellationToken cancellationToken)
    {
        _dbContext.Patients.Update(patient);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

