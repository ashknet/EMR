using Microsoft.EntityFrameworkCore;
using FamilyService.Domain.Entities;
using FamilyService.Infrastructure.Data;
using Shared.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FamilyService.Infrastructure.Repositories
{
    public class FamilyGroupRepository : IRepository<FamilyGroup>
    {
        private readonly FamilyDbContext _context;

        public FamilyGroupRepository(FamilyDbContext context)
        {
            _context = context;
        }

        public async Task<FamilyGroup?> GetByIdAsync(Guid id)
        {
            return await _context.FamilyGroups
                .Include(f => f.Members)
                .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted);
        }

        public async Task<IEnumerable<FamilyGroup>> GetAllAsync()
        {
            return await _context.FamilyGroups
                .Include(f => f.Members)
                .Where(f => !f.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<FamilyGroup>> FindAsync(Expression<Func<FamilyGroup, bool>> predicate)
        {
            return await _context.FamilyGroups
                .Include(f => f.Members)
                .Where(predicate)
                .Where(f => !f.IsDeleted)
                .ToListAsync();
        }

        public async Task<FamilyGroup> AddAsync(FamilyGroup entity)
        {
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;
            
            await _context.FamilyGroups.AddAsync(entity);
            await _context.SaveChangesAsync();
            
            return entity;
        }

        public async Task<FamilyGroup> UpdateAsync(FamilyGroup entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            
            _context.FamilyGroups.Update(entity);
            await _context.SaveChangesAsync();
            
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var familyGroup = await GetByIdAsync(id);
            if (familyGroup == null)
                return false;

            _context.FamilyGroups.Remove(familyGroup);
            await _context.SaveChangesAsync();
            
            return true;
        }

        public async Task<bool> SoftDeleteAsync(Guid id, string deletedBy)
        {
            var familyGroup = await GetByIdAsync(id);
            if (familyGroup == null)
                return false;

            familyGroup.IsDeleted = true;
            familyGroup.DeletedAt = DateTime.UtcNow;
            familyGroup.DeletedBy = deletedBy;
            
            await UpdateAsync(familyGroup);
            
            return true;
        }

        public async Task<int> CountAsync(Expression<Func<FamilyGroup, bool>>? predicate = null)
        {
            if (predicate == null)
                return await _context.FamilyGroups.CountAsync(f => !f.IsDeleted);
            
            return await _context.FamilyGroups.CountAsync(predicate);
        }

        public async Task<bool> ExistsAsync(Expression<Func<FamilyGroup, bool>> predicate)
        {
            return await _context.FamilyGroups.AnyAsync(predicate);
        }

        public async Task<FamilyGroup?> GetByPrimaryAccountHolderAsync(Guid primaryAccountHolderId)
        {
            return await _context.FamilyGroups
                .Include(f => f.Members)
                .FirstOrDefaultAsync(f => f.PrimaryAccountHolderId == primaryAccountHolderId && !f.IsDeleted);
        }
    }
}
