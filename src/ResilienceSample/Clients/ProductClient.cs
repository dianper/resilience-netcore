namespace ResilienceSample.Clients
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using ResilienceSample.Model;

    public class ProductClient : IProductClient
    {
        private readonly HttpClient httpClient;

        public ProductClient(IHttpClientFactory httpClientFactory)
        {
            this.httpClient = httpClientFactory.CreateClient("productApi");
        }

        public async Task<ProductModel> GetAsync(int id)
        {
            var product = default(ProductModel);

            try
            {
                var response = await this.httpClient.GetAsync($"api/products/{id}");
                if (response.IsSuccessStatusCode) 
                {
                    var json = await response.Content.ReadAsStringAsync();
                    product = JsonConvert.DeserializeObject<ProductModel>(json);
                }
            }
            catch (Exception ex)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{DateTime.Now.ToLongTimeString()} - Error: [ProductClient.GetAsync]: {ex.Message}");
                Console.ResetColor();

                throw ex;
            }

            return product;
        }
    }
}
