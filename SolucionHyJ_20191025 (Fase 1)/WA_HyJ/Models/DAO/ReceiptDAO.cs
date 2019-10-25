using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using WA_HyJ.Interfaces;
using WA_HyJ.Models.Request;
using WA_HyJ.Models.Response;

namespace WA_HyJ.Models.DAO
{
    public class ReceiptDAO : IReceipt
    {
        private static readonly string _Uri = ConfigurationManager.AppSettings["URIReceipt"];
        private static readonly string _URIReceipt_AddLetterPaymentMethod = ConfigurationManager.AppSettings["URIReceipt_AddLetterPaymentMethod"];
        private static readonly string _URIReceipt_AddOtherPaymentMethod = ConfigurationManager.AppSettings["URIReceipt_AddOtherPaymentMethod"];

        public async Task<Uri> AddAsync(ReceiptRequest model, string token = null)
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

        public async Task<object> AddOtherPaymentMethodAsync(string id, OtherMethodPaymentRequest model, string token = null)
        {
            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            {
                client.DefaultRequestHeaders.Clear();

                if (token != null) client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpContent hcTipoPago = new StringContent(model.TipoPago.ToString());

                formData.Add(hcTipoPago, "TipoPago");

                //Enviar Imagen
                if (model.Imagen != null)
                {
                    HttpContent fileStreamContent = new StreamContent(model.Imagen.InputStream);

                    fileStreamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "Imagen", FileName = model.Imagen.FileName };
                    fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    formData.Add(fileStreamContent);
                }

                var URI = new Uri(_URIReceipt_AddOtherPaymentMethod + $"/{id}");

                using (var response = await client.PostAsync(URI, formData).ConfigureAwait(false))
                {
                    if (response.IsSuccessStatusCode)
                        return response.StatusCode;
                    else
                        return response.Content.ReadAsStringAsync().Result;
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

        public async Task<ReceiptResponse> DetailsAsync(string id, string token = null)
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
                    ReceiptResponse model = null;

                    if (response.IsSuccessStatusCode)
                        model = JsonConvert.DeserializeObject<ReceiptResponse>(await response.Content.ReadAsStringAsync());

                    return model;
                }
            }
        }

        public async Task<List<ReceiptResponse>> GetAllAsync(string token = null)
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
                        ReceiptResponse[] _result = JsonConvert.DeserializeObject<ReceiptResponse[]>(await response.Content.ReadAsStringAsync());

                        List<ReceiptResponse> lista = new List<ReceiptResponse>();

                        foreach (ReceiptResponse item in _result)
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

        public async Task<HttpStatusCode> UpdateAsync(string id, ReceiptRequest model, string token = null)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Put, new Uri(_Uri + $"/{id}")))
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

        public async Task<object> AddLetterPaymentMethodAsync(string id, LetterMethodPaymentRequest model, string token = null)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_URIReceipt_AddLetterPaymentMethod + $"/{id}")))
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
                        if (response.IsSuccessStatusCode)
                            return response.StatusCode;
                        else
                            return response.Content.ReadAsStringAsync().Result;
                    }
                }
            }
        }
    }
}