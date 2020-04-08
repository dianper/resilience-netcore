namespace ResilienceSample.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using ResilienceSample.Model;
    using ResilienceSample.Services;

    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IProductService productService;

        public ValuesController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpGet]
        public async Task<ProductModel> Get()
        {
            var model = await this.productService.GetProduct(12345);

            return model;
        }
    }
}
