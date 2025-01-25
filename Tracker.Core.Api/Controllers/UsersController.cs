using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using Tracker.Core.Api.Models.Foundations.Users;
using Tracker.Core.Api.Services.Foundations.Users;

namespace Tracker.Core.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : RESTFulController
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService) =>
            this.userService = userService;

        [HttpPost]
        public async ValueTask<ActionResult<User>> PostUserAsync(User user)
        {
            User addedUser =
                await this.userService.AddUserAsync(user);

            return Created(addedUser);
        }
    }
}
