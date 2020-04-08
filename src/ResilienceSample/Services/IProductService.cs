namespace ResilienceSample.Services
{
    using ResilienceSample.Model;
    using System.Threading.Tasks;

    public interface IProductService
    {
        Task<ProductModel> GetProduct(int id);
    }
}
