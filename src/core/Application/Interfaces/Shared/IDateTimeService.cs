using System;

namespace Application.Interfaces.Shared
{
    public interface IDateTimeService
    {
        DateTime NowUtc { get; }
        DateTime Now { get; }
    }
}