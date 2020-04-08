namespace ResilienceSample.Clients
{
    using System.Threading.Tasks;
    using ResilienceSample.Model;

    public interface IProductClient
    {
        Task<ProductModel> GetAsync(int id);
    }
}
