using Microsoft.AspNetCore.Mvc;

namespace test.Modules.Users
{
    [ApiController]
    [Route("[controller]")]
    public class UserController
    {
        public IUserService _UserService;
    }
}