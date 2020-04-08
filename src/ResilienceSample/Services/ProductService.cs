namespace ResilienceSample.Services
{
    using System.Threading.Tasks;
    using ResilienceSample.Clients;
    using ResilienceSample.Model;

    public class ProductService : IProductService
    {
        private readonly IProductClient productClient;
        private readonly IResilienceService resilienceService;

        public ProductService(
            IProductClient productClient,
            IResilienceService resilienceService)
        {
            this.productClient = productClient;
            this.resilienceService = resilienceService;
        }

        public async Task<ProductModel> GetProduct(int id)
        {
            return await this.resilienceService.ExecuteAsync(
                baseAction: () => this.productClient.GetAsync(id),
                fallbackAction: () => this.GetProductFallback());
        }

        private Task<ProductModel> GetProductFallback()
        {
            return Task.FromResult(new ProductModel { Id = 0, Name = "Product Fallback" });
        }
    }
}
