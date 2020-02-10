using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class LogRepository : EfRepository<Log>, IAsyncRepository<Log>
    {
        public LogRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
