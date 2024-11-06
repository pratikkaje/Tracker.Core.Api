using System.Threading.Tasks;
using Tracker.Core.Api.Models.Foundations.Users;
using Tracker.Core.Api.Models.Foundations.Users.Exceptions;

namespace Tracker.Core.Api.Services.Foundations.Users
{
    internal partial class UserService
    {
        private async ValueTask ValidateUserOnAddAsync(User user)
        {
            ValidateUserIsNotNull(user);
        }

        private static void ValidateUserIsNotNull(User user)
        {
            if (user is null)
            {
                throw new NullUserException(message: "User is null");
            }
        }
    }
}
