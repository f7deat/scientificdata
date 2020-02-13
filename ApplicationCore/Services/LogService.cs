using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class LogService : ILogService
    {
        readonly IAsyncRepository<Log> _logRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LogService(IAsyncRepository<Log> logRepository, IHttpContextAccessor httpContextAccessor)
        {
            _logRepository = logRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Write(LogType logType, string logContent)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
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
