
using Application.Interfaces.Shared;
using System;

namespace Infrastructure.Shared.Services
{
    public class SystemDateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
        public DateTime Now => DateTime.Now;
    }
}