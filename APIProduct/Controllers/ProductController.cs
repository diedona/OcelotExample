using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIProduct.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        [HttpGet]
        public ActionResult<string[]> Get()
        {
            return Ok(new[] { "Apple", "Orange", "Milk" });
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        [Route("secret")]
        public ActionResult<string> GetSecretProduct()
        {
            return Ok("CHOCOLATE");
        }

        [HttpGet]
        [Authorize(Policy = "UserMustBeAbleToTalk")]
        [Route("talk")]
        public ActionResult<string> GetTalk()
        {
            return Ok("yes you can talk");
        }
    }
}
