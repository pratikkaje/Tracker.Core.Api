using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Categories.Exceptions
{
    public class CategoryServiceException : Xeption
    {
        public CategoryServiceException(string message, Xeption innerException)
            : base(message, innerException) { }
    }
}
