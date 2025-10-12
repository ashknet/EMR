using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Entities;
using PatientService.Infrastructure.Data;
using Shared.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PatientService.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for Patient entity operations
    /// Implements HIPAA-compliant data access patterns with audit logging
    /// </summary>
    public class PatientRepository : IRepository<Patient>
    {
        private readonly PatientDbContext _context;

        public PatientRepository(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Patient?> GetByIdAsync(Guid id)
        {
            return await _context.Patients
                .Include(p => p.Documents)
                .Include(p => p.Notes)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _context.Patients
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Patient>> FindAsync(Expression<Func<Patient, bool>> predicate)
        {
            return await _context.Patients
                .Where(predicate)
                .Where(p => !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<Patient> AddAsync(Patient entity)
        {
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;
            
            await _context.Patients.AddAsync(entity);
            await _context.SaveChangesAsync();
            
            return entity;
        }

        public async Task<Patient> UpdateAsync(Patient entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            
            _context.Patients.Update(entity);
            await _context.SaveChangesAsync();
            
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var patient = await GetByIdAsync(id);
            if (patient == null)
                return false;

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            
            return true;
        }

        public async Task<bool> SoftDeleteAsync(Guid id, string deletedBy)
        {
            var patient = await GetByIdAsync(id);
            if (patient == null)
                return false;

            patient.IsDeleted = true;
            patient.DeletedAt = DateTime.UtcNow;
            patient.DeletedBy = deletedBy;
            
            await UpdateAsync(patient);
            
            return true;
        }

        public async Task<int> CountAsync(Expression<Func<Patient, bool>>? predicate = null)
        {
            if (predicate == null)
                return await _context.Patients.CountAsync(p => !p.IsDeleted);
            
            return await _context.Patients.CountAsync(predicate);
        }

        public async Task<bool> ExistsAsync(Expression<Func<Patient, bool>> predicate)
        {
            return await _context.Patients.AnyAsync(predicate);
        }

        // Custom methods for Patient-specific operations
        public async Task<Patient?> GetByEmailAsync(string email)
        {
            return await _context.Patients
                .FirstOrDefaultAsync(p => p.Email == email && !p.IsDeleted);
        }

        public async Task<Patient?> GetBySSNHashAsync(string ssnHash)
        {
            return await _context.Patients
                .FirstOrDefaultAsync(p => p.SSNHash == ssnHash && !p.IsDeleted);
        }

        public async Task<IEnumerable<Patient>> GetFamilyMembersAsync(Guid primaryAccountHolderId)
        {
            return await _context.Patients
                .Where(p => p.PrimaryAccountHolderId == primaryAccountHolderId && !p.IsDeleted)
                .OrderBy(p => p.ProfileType)
                .ThenBy(p => p.DateOfBirth)
                .ToListAsync();
        }

        public async Task<IEnumerable<Patient>> SearchPatientsAsync(string searchTerm)
        {
            searchTerm = searchTerm.ToLower();
            
            return await _context.Patients
                .Where(p => !p.IsDeleted && (
                    p.FirstName.ToLower().Contains(searchTerm) ||
                    p.LastName.ToLower().Contains(searchTerm) ||
                    p.Email!.ToLower().Contains(searchTerm) ||
                    p.MedicalRecordNumber!.Contains(searchTerm)
                ))
                .Take(50)
                .ToListAsync();
        }
    }
}
