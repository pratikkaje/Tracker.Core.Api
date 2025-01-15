using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Categories.Exceptions
{
    public class CategoryDependencyException : Xeption
    {
        public CategoryDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
