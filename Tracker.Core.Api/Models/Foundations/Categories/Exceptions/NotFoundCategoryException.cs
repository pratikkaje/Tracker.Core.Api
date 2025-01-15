using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Categories.Exceptions
{
    public class NotFoundCategoryException : Xeption
    {
        public NotFoundCategoryException(string message)
            : base(message)
        { }
    }
}
