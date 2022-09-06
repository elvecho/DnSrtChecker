using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DnSrtChecker.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RT_ChecksContext _dbContext;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        public UnitOfWork(RT_ChecksContext dbContext, ILogger<UnitOfWork> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public async Task CompleteAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError($"Problem save data on database");
                throw;
            }
        }
        public void UpdateAsync(Object obj)
        {
            try
            {
                _dbContext.Update(obj);
                _logger.LogDebug($"{obj} is updated successfully ");
            }
            catch(Exception e)
            {
                _logger.LogError($"Error updating {obj} : {e.Message}");
            }
        }
        public void UpdateRangeAsync(IEnumerable<Object> obj)
        {
            try
            {
                _dbContext.UpdateRange(obj);
                _logger.LogDebug($"{obj} is updated successfully ");
            }
            catch (Exception e)
            {
                _logger.LogError($"Error updating {obj} : {e.Message}");
            }
        }
    }
}