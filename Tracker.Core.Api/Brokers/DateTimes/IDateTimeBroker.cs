using System.Threading.Tasks;
using System;

namespace Tracker.Core.Api.Brokers.DateTimes
{
    public interface IDateTimeBroker
    {
        ValueTask<DateTimeOffset> GetCurrentDateTimeOffsetAsync();
    }
}
