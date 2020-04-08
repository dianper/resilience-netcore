namespace WebApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        // GET api/products/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return new JsonResult(new { Id = id, Name = "Description" });
        }
    }
}
