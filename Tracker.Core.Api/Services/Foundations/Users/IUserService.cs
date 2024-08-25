using System.Threading.Tasks;
using Tracker.Core.Api.Models.Foundations.Users;

namespace Tracker.Core.Api.Services.Foundations.Users
{
    public interface IUserService
    {
        ValueTask<User> AddCategoryAsync(User user);
    }
}
