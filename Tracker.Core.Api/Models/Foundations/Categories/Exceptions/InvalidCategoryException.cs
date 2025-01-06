using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Categories.Exceptions
{
    public class InvalidCategoryException : Xeption
    {
        public InvalidCategoryException(string message) : base(message)
        { }
    }
}
