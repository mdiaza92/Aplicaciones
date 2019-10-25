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
using WA_HyJ.Interfaces;
using WA_HyJ.Models.InternalModels;

namespace WA_HyJ.Models.DAO
{
    public class BrandDAO : IBrand
    {
        private static readonly string _Uri = ConfigurationManager.AppSettings["URIBrand"];

        public async Task<Uri> AddAsync(BrandModel model, string token = null)
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

        public async Task<BrandModel> DetailsAsync(string id, string token = null)
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
                    BrandModel model = null;

                    if (response.IsSuccessStatusCode)
                        model = JsonConvert.DeserializeObject<BrandModel>(await response.Content.ReadAsStringAsync());

                    return model;
                }
            }
        }

        public async Task<List<BrandModel>> GetAllAsync(string token = null)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_Uri)))
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
                        BrandModel[] _result = JsonConvert.DeserializeObject<BrandModel[]>(await response.Content.ReadAsStringAsync());

                        List<BrandModel> lista = new List<BrandModel>();

                        foreach (BrandModel item in _result)
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

        public async Task<HttpStatusCode> UpdateAsync(BrandModel model, string token = null)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Put, new Uri(_Uri + $"/{model.Id}")))
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
                        return response.StatusCode;
                    }
                }
            }
        }
    }
}