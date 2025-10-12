using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PatientService.Infrastructure.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PatientService.Functions.Functions
{
    /// <summary>
    /// Azure Function for data cleanup and maintenance tasks
    /// Runs periodically to clean up expired data, optimize storage, and maintain compliance
    /// </summary>
    public class DataCleanupFunction
    {
        private readonly ILogger<DataCleanupFunction> _logger;
        private readonly PatientDbContext _dbContext;

        public DataCleanupFunction(
            ILogger<DataCleanupFunction> logger,
            PatientDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Timer-triggered function that runs weekly for data cleanup
        /// Schedule: Every Sunday at 3 AM UTC
        /// </summary>
        [Function("DataCleanupFunction")]
        public async Task Run([TimerTrigger("0 0 3 * * 0")] TimerInfo timerInfo)
        {
            _logger.LogInformation($"Data cleanup started at: {DateTime.UtcNow}");

            try
            {
                int totalCleaned = 0;

                // 1. Clean up expired shared documents
                totalCleaned += await CleanupExpiredShares();

                // 2. Archive old access logs (older than 7 years per HIPAA)
                totalCleaned += await ArchiveOldAccessLogs();

                // 3. Clean up soft-deleted records older than 90 days
                totalCleaned += await CleanupOldDeletedRecords();

                // 4. Remove expired reminders
                totalCleaned += await CleanupExpiredReminders();

                _logger.LogInformation($"Data cleanup completed. Total records processed: {totalCleaned}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in data cleanup function");
                throw;
            }
        }

        private async Task<int> CleanupExpiredShares()
        {
            _logger.LogInformation("Cleaning up expired document shares");

            var expiredShares = await _dbContext.PatientDocuments
                .Where(d => d.IsShared && d.SharedUntil.HasValue && d.SharedUntil < DateTime.UtcNow)
                .ToListAsync();

            foreach (var doc in expiredShares)
            {
                doc.IsShared = false;
                doc.SharedWith = null;
                doc.SharedUntil = null;
                doc.UpdatedAt = DateTime.UtcNow;
                doc.UpdatedBy = "system-cleanup";
            }

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Expired {expiredShares.Count} document shares");
            return expiredShares.Count;
        }

        private async Task<int> ArchiveOldAccessLogs()
        {
            _logger.LogInformation("Archiving old access logs");

            // HIPAA requires maintaining audit logs for 6 years
            // This would move logs older than 7 years to cold storage
            var cutoffDate = DateTime.UtcNow.AddYears(-7);

            var oldLogs = await _dbContext.PatientAccessLog
                .Where(log => log.AccessedAt < cutoffDate)
                .CountAsync();

            // In production, these would be moved to Azure Archive Storage
            // For now, just log the count
            _logger.LogInformation($"Found {oldLogs} access logs ready for archival");

            // TODO: Implement actual archival to Azure Archive Storage
            // await ArchiveToAzureStorage(oldLogs);

            return 0; // Return count of archived logs
        }

        private async Task<int> CleanupOldDeletedRecords()
        {
            _logger.LogInformation("Cleaning up old soft-deleted records");

            var cutoffDate = DateTime.UtcNow.AddDays(-90);

            // Clean up soft-deleted notes (non-PHI)
            var oldNotes = await _dbContext.PatientNotes
                .Where(n => n.IsDeleted && n.DeletedAt < cutoffDate)
                .ToListAsync();

            _dbContext.PatientNotes.RemoveRange(oldNotes);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Permanently deleted {oldNotes.Count} old notes");

            // Note: Patient records and documents should NEVER be permanently deleted
            // due to HIPAA requirements - they remain soft-deleted indefinitely

            return oldNotes.Count;
        }

        private async Task<int> CleanupExpiredReminders()
        {
            _logger.LogInformation("Cleaning up expired reminders");

            var expiredReminders = await _dbContext.PatientNotes
                .Where(n => n.ReminderDate.HasValue && 
                           n.ReminderDate < DateTime.UtcNow.AddDays(-30) &&
                           n.IsReminderSent)
                .ToListAsync();

            foreach (var note in expiredReminders)
            {
                note.ReminderDate = null;
                note.IsReminderSent = false;
            }

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Cleaned up {expiredReminders.Count} expired reminders");
            return expiredReminders.Count;
        }
    }
}
