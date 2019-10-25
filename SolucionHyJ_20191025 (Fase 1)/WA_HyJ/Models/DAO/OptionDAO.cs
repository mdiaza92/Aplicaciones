using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using WA_HyJ.Interfaces;
using WA_HyJ.Models.InternalModels;

namespace WA_HyJ.Models.DAO
{
    public class OptionDAO : IOption
    {
        private static readonly string _UriOption_GetIGV = ConfigurationManager.AppSettings["URIOption_GetIGV"];
        private static readonly string _UriOption_GetUnidadMedida = ConfigurationManager.AppSettings["URIOption_GetUnidadMedida"];
        private static readonly string _UriOption_GetTipoMoneda = ConfigurationManager.AppSettings["URIOption_GetTipoMoneda"];
        private static readonly string _UriOption_GetTipoComprobante = ConfigurationManager.AppSettings["URIOption_GetTipoComprobante"];
        private static readonly string _URIOption_GetTipoPago = ConfigurationManager.AppSettings["URIOption_GetTipoPago"];

        public async Task<double> GetIGVAsync(string token = null)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_UriOption_GetIGV)))
            {
                client.DefaultRequestHeaders.Clear();

                if (token != null) client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var response = await client
                        .SendAsync(request)
                        .ConfigureAwait(false))
                {
                    if (response.IsSuccessStatusCode)
                        return JsonConvert.DeserializeObject<double>(await response.Content.ReadAsStringAsync());
                    else
                        throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }
        }

        public async Task<List<EnumModel>> GetTipoMoneda(string token = null)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_UriOption_GetTipoMoneda)))
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
                        EnumModel[] result = JsonConvert.DeserializeObject<EnumModel[]>(await response.Content.ReadAsStringAsync());

                        List<EnumModel> rsp = new List<EnumModel>();

                        foreach (var item in result)
                            rsp.Add(item);

                        return rsp;
                    }
                    else
                        throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }
        }

        public async Task<List<EnumModel>> GetTipoPago(string token = null)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_URIOption_GetTipoPago)))
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
                        EnumModel[] result = JsonConvert.DeserializeObject<EnumModel[]>(await response.Content.ReadAsStringAsync());

                        List<EnumModel> rsp = new List<EnumModel>();

                        foreach (var item in result)
                            rsp.Add(item);

                        return rsp;
                    }
                    else
                        throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }
        }

        public async Task<List<EnumModel>> GetTipoRecibo(string token = null)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_UriOption_GetTipoComprobante)))
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
                        EnumModel[] result = JsonConvert.DeserializeObject<EnumModel[]>(await response.Content.ReadAsStringAsync());

                        List<EnumModel> rsp = new List<EnumModel>();

                        foreach (var item in result)
                            rsp.Add(item);

                        return rsp;
                    }
                    else
                        throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }
        }

        public async Task<List<EnumModel>> GetUnidadMedida(string token = null)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_UriOption_GetUnidadMedida)))
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
                        EnumModel[] result = JsonConvert.DeserializeObject<EnumModel[]>(await response.Content.ReadAsStringAsync());

                        List<EnumModel> rsp = new List<EnumModel>();

                        foreach (var item in result)
                            rsp.Add(item);

                        return rsp;
                    }
                    else
                        throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }
        }
    }
}