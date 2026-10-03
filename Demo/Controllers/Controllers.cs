using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Controllers : ControllerBase
    {
        [Route("")]
        [Authorizer.GrpcClient.InterAuthorizer()]
        public async Task<bool> Test()
        {
          return true;
        }
    }
}
