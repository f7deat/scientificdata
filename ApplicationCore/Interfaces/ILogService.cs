using ApplicationCore.Entities;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface ILogService
    {
        Task Write(LogType logType, string logContent);
    }
}
