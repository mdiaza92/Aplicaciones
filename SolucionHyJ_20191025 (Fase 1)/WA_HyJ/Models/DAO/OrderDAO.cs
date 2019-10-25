using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.ModelBinding;
using WA_HyJ.Interfaces;
using WA_HyJ.Models.InternalModels;
using WA_HyJ.Models.Request;
using WA_HyJ.Models.Response;

namespace WA_HyJ.Models.DAO
{
    public class OrderDAO : IOrder
    {
        private static readonly string _Uri = ConfigurationManager.AppSettings["URIOrder"];
        private static readonly string _Uri_UpdateStatus = ConfigurationManager.AppSettings["URIOrder_UpdateStatus"];
        private static readonly string _URIOrder_UpdateOrderDetails = ConfigurationManager.AppSettings["URIOrder_UpdateOrderDetails"];

        public async Task<Uri> AddAsync(OrderRequest model, string token = null)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_Uri)))
            {
                client.DefaultRequestHeaders.Clear();

                if (token != null) client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var json = JsonConvert.SerializeObject(model);

                using (var stringcontent = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    request.Content = stringcontent;

                    using (var response = await client
                        .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                        .ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();                       

                        return response.Headers.Location;
                    }
                }
            }
        }

        public async Task<HttpStatusCode> DeleteAsync(string id, string token = null)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Delete, new Uri(_Uri + $"/{id}")))
            {
                client.DefaultRequestHeaders.Clear();

                if (token != null) client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var response = await client
                        .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                        .ConfigureAwait(false))
                {
                    return response.StatusCode;
                }
            }
        }

        public async Task<OrderResponse> DetailsAsync(string id, string token = null)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_Uri + $"/{id}")))
            {
                client.DefaultRequestHeaders.Clear();

                if (token != null) client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var response = await client
                        .SendAsync(request)
                        .ConfigureAwait(false))
                {
                    OrderResponse model = null;

                    if (response.IsSuccessStatusCode)
                        model = JsonConvert.DeserializeObject<OrderResponse>(await response.Content.ReadAsStringAsync());

                    return model;
                }
            }
        }

        public async Task<List<OrderResponse>> GetAllAsync(string token = null)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_Uri);

                client.DefaultRequestHeaders.Clear();

                if (token != null) client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(string.Empty, HttpCompletionOption.ResponseContentRead);

                if (response.IsSuccessStatusCode)
                {
                    OrderResponse[] _result = JsonConvert.DeserializeObject<OrderResponse[]>(await response.Content.ReadAsStringAsync());

                    List<OrderResponse> lista = new List<OrderResponse>();

                    foreach (OrderResponse item in _result)
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

        public Task<HttpStatusCode> UpdateAsync(OrderRequest model, string token = null)
        {
            throw new NotImplementedException();
        }

        public async Task<HttpStatusCode> UpdateOrderDetailsAsync(string idPedido, List<OrderDetailRequest> details, string token = null)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Put, new Uri(_URIOrder_UpdateOrderDetails + $"/{idPedido}")))
            {
                client.DefaultRequestHeaders.Clear();

                if (token != null) client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var json = JsonConvert.SerializeObject(details);

                using (var stringcontent = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    request.Content = stringcontent;

                    using (var response = await client
                        .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                        .ConfigureAwait(false))
                    {
                        return response.StatusCode;
                    }
                }
            }
        }

        public async Task<HttpStatusCode> UpdateStatus(string id, string token = null)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Put, new Uri(_Uri_UpdateStatus + $"/{id}")))
            {
                client.DefaultRequestHeaders.Clear();

                if (token != null) client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var response = await client
                        .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                        .ConfigureAwait(false))
                {
                    return response.StatusCode;
                }
            }
        }
    }
}