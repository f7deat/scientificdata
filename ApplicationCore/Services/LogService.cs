using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using System;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class LogService : ILogService
    {
        IAsyncRepository<Log> _logRepository;
        public LogService(IAsyncRepository<Log> logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task Write(string userId, LogType logType, string logContent)
        {
            var log = new Log
            {
                UserId = userId,
                LogType = logType,
                LogTime = DateTime.Now,
                LogContent = logContent
            };
           await _logRepository.AddAsync(log);
        }
    }
}
