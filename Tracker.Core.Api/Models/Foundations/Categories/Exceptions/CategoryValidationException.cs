using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Categories.Exceptions
{
    public class CategoryValidationException : Xeption
    {
        public CategoryValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
