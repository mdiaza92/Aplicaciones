using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using WA_HyJ.Interfaces;
using WA_HyJ.Models.InternalModels;

namespace WA_HyJ.Models.DAO
{
    public class KardexDAO : IKardex
    {
        private static readonly string _UriPI_GetStock = ConfigurationManager.AppSettings["URIPhysicalInventory_GetStock"];

        public async Task<List<ProductsByProvider>> GetStockAsync(string idProvider, string token = null)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_UriPI_GetStock + $"/{idProvider}")))
            {
                client.DefaultRequestHeaders.Clear();

                if (token != null) client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var response = await client
                        .SendAsync(request)
                        .ConfigureAwait(false))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        ProductsByProvider[] _result = JsonConvert.DeserializeObject<ProductsByProvider[]>(await response.Content.ReadAsStringAsync());

                        List<ProductsByProvider> lista = new List<ProductsByProvider>();

                        foreach (ProductsByProvider item in _result)
                        {
                            lista.Add(item);
                        }

                        return lista;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
    }
}