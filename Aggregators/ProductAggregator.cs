using Ocelot.Middleware;
using Ocelot.Multiplexer;
using Ocelot.Configuration;
using System.Text.Json;
using System.Net.Http.Headers;

using ApiGateway.Models;

namespace ApiGateway.Aggregators
{
    public class ProductAggregator : IDefinedAggregator
    {
        public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
        {
            CatalogProductModel catalogProduct = null;
            CartProductModel cartProduct = null;

            foreach (var response in responses)
            {
                var routeKey = ((DownstreamRoute)response.Items["DownstreamRoute"]).Key;
                var responseBody = await response.Items.DownstreamResponse().Content.ReadAsStringAsync();

                if (routeKey == "Catalog")
                {
                    catalogProduct = JsonSerializer.Deserialize<CatalogProductModel>(
                        responseBody, 
                        new JsonSerializerOptions() { DictionaryKeyPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true }
                    );
                }

                if (routeKey == "Cart")
                {
                    cartProduct = JsonSerializer.Deserialize<CartProductModel>(
                        responseBody,
                        new JsonSerializerOptions() { DictionaryKeyPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true }
                    );
                }
            }

            var jsonResponse = JsonSerializer.Serialize(MergeProduct(cartProduct, catalogProduct));
            var stringContent = new StringContent(jsonResponse) { Headers = { ContentType = new MediaTypeHeaderValue("application/json") } };

            return new DownstreamResponse(stringContent, System.Net.HttpStatusCode.OK, new List<Header>(), "OK");
        }

        private ResponseModel MergeProduct(CartProductModel cartModel, CatalogProductModel catalogModel)
        {
            return new ResponseModel
            {
                Id = cartModel.Id,
                Name = catalogModel.Name,
                Price = catalogModel.Price,
                Amount = cartModel.Amount
            };
        }
    }
}
