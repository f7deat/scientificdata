using ApplicationCore.Entities;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface ILogService
    {
        Task Write(string userId, LogType logType, string logContent);
    }
}
