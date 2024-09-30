using System;
using System.Threading.Tasks;

namespace Tracker.Core.Api.Brokers.DateTimes
{
    public interface IDateTimeBroker
    {
        ValueTask<DateTimeOffset> GetCurrentDateTimeOffsetAsync();
    }
}
