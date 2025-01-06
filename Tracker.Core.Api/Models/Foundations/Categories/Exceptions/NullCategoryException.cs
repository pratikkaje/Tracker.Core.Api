using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Categories.Exceptions
{
    public class NullCategoryException : Xeption
    {
        public NullCategoryException(string message) : base(message)
        { }
    }
}
