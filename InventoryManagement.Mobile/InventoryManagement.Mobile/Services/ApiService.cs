using InventoryManagement.Mobile.Helpers;
using InventoryManagement.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace InventoryManagement.Mobile.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions jsonOptions;

        public ApiService(HttpClient httpClient)
        {
            // DO NOT DELETE - Use the following API link for live debugging of the api with the Emulator 
            //var ApiBaseUri = Preferences.Get("ApiBaseUri", "http://10.0.2.2:5129/api/");

            // DO NOT DELETE - Use the following API link when you need to use your actual device as
            // the above api can't be accessed from an actual device
            // var ApiBaseUri = "http://10.0.2.2:5129/api/";
            var ApiBaseUri = Preferences.Get("ApiBaseUri", "http://192.168.1.2/InvMan/api/");

            httpClient.BaseAddress = new Uri(ApiBaseUri);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "InventoryManagement/1.0 API Consumer");
            _client = httpClient;

            jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new LowerCaseNamingPolicy()
            };
        }

        #region Record
        public async Task<IEnumerable<Record>> GetRecordsAsync()
        {
            var response = await _client.GetAsync("Record");
            string responseMessage;
            if (!response.IsSuccessStatusCode)
            {
                if (response.Headers.Any(h => h.Key == "X-Status-Reason"))
                {
                    responseMessage = response.Headers.First(h => h.Key == "X-Status-Reason").Value.First();
                    if (string.IsNullOrWhiteSpace(responseMessage))
                    {
                        responseMessage = response.ReasonPhrase;
                    }
                }
                else
                {
                    responseMessage = response.ReasonPhrase;
                }

                throw new IMHttpRequestException(responseMessage, response.StatusCode);
            }

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<Record>>(responseString, jsonOptions);
        }

        public async Task<Record> GetRecordAsync(string recordId)
        {
            var response = await _client.GetAsync($"Record/{recordId}");

            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = response.Headers.FirstOrDefault(h => h.Key == "X-Status-Reason").Value.First();
                if (string.IsNullOrWhiteSpace(responseMessage))
                {
                    responseMessage = response.ReasonPhrase;
                }

                throw new IMHttpRequestException(responseMessage, response.StatusCode);
            }

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Record>(responseString, jsonOptions);
        }

        public async Task PostRecordAsync(Record record)
        {
            string jsonString = JsonSerializer.Serialize(record);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"Record/", httpContent);

            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = response.Headers.FirstOrDefault(h => h.Key == "X-Status-Reason").Value.First();
                if (string.IsNullOrWhiteSpace(responseMessage))
                {
                    responseMessage = response.ReasonPhrase;
                }

                throw new IMHttpRequestException(responseMessage, response.StatusCode);
            }
        }

        public async Task PutRecordAsync(Record record)
        {
            var response = await _client.PutAsync($"Record/", new StringContent(JsonSerializer.Serialize(record), Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = response.Headers.FirstOrDefault(h => h.Key == "X-Status-Reason").Value.First();
                if (string.IsNullOrWhiteSpace(responseMessage))
                {
                    responseMessage = response.ReasonPhrase;
                }

                throw new IMHttpRequestException(responseMessage, response.StatusCode);
            }

            throw new NotImplementedException();
        }

        public async Task DeleteRecordAsync(Record record)
        {
            //try
            //{
            //    string jsonString = JsonSerializer.Serialize(record);
            //    var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            //    var response = await _client.DeleteAsync($"Record/{record.Id}");
            //    response.EnsureSuccessStatusCode();
            //}
            //catch (HttpRequestException ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
            throw new NotImplementedException();
        }
        #endregion

        #region Record Item
        public async Task<IEnumerable<RecordItem>> GetRecordItemsAsync(string recordId)
        {
            var response = await _client.GetAsync($"RecordItem/Record/{recordId}");

            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = response.Headers.FirstOrDefault(h => h.Key == "X-Status-Reason").Value.First();
                if (string.IsNullOrWhiteSpace(responseMessage))
                {
                    responseMessage = response.ReasonPhrase;
                }

                throw new IMHttpRequestException(responseMessage, response.StatusCode);
            }

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<RecordItem>>(responseString, jsonOptions);
        }

        public async Task<RecordItem> GetRecordItemAsync(string recordItemId)
        {
            var response = await _client.GetAsync($"RecordItem/{recordItemId}");

            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = response.Headers.FirstOrDefault(h => h.Key == "X-Status-Reason").Value.First();
                if (string.IsNullOrWhiteSpace(responseMessage))
                {
                    responseMessage = response.ReasonPhrase;
                }

                throw new IMHttpRequestException(responseMessage, response.StatusCode);
            }

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<RecordItem>(responseString, jsonOptions);
        }

        public async Task<RecordItem> GetRecordItemAsync(string recordId, string productId)
        {
            var response = await _client.GetAsync($"RecordItem/Record/{recordId}/Product/{productId}");
            var responseString = string.Empty;

            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = response.Headers.FirstOrDefault(h => h.Key == "X-Status-Reason").Value.First();
                if (string.IsNullOrWhiteSpace(responseMessage))
                {
                    responseMessage = response.ReasonPhrase;
                }

                throw new IMHttpRequestException(responseMessage, response.StatusCode);
            }
            else
            {
                responseString = await response.Content.ReadAsStringAsync();
            }

            return JsonSerializer.Deserialize<RecordItem>(responseString, jsonOptions);
        }

        public async Task<IEnumerable<RecordItem>> GetRecordItemsForProduct(string productId)
        {
            var response = await _client.GetAsync($"RecordItem/Product/{productId}");
            var responseString = string.Empty;

            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = response.Headers.FirstOrDefault(h => h.Key == "X-Status-Reason").Value.First();
                if (string.IsNullOrWhiteSpace(responseMessage))
                {
                    responseMessage = response.ReasonPhrase;
                }

                throw new IMHttpRequestException(responseMessage, response.StatusCode);
            }
            else
            {
                responseString = await response.Content.ReadAsStringAsync();
            }

            return JsonSerializer.Deserialize<IEnumerable<RecordItem>>(responseString, jsonOptions);
        }

        public async Task PostRecordItemAsync(RecordItem recordItem)
        {
            string jsonString = JsonSerializer.Serialize(recordItem);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"RecordItem/", httpContent);

            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = response.Headers.FirstOrDefault(h => h.Key == "X-Status-Reason").Value.First();
                if (string.IsNullOrWhiteSpace(responseMessage))
                {
                    responseMessage = response.ReasonPhrase;
                }

                throw new IMHttpRequestException(responseMessage, response.StatusCode);
            }
        }

        public async Task PutRecordItemAsync(RecordItem recordItem)
        {
            var response = await _client.PutAsync($"RecordItem/", new StringContent(JsonSerializer.Serialize(recordItem), Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = response.Headers.FirstOrDefault(h => h.Key == "X-Status-Reason").Value.First();
                if (string.IsNullOrWhiteSpace(responseMessage))
                {
                    responseMessage = response.ReasonPhrase;
                }

                throw new IMHttpRequestException(responseMessage, response.StatusCode);
            }
        }

        #endregion

        #region Product
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            var response = await _client.GetAsync("Product");

            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = response.Headers.FirstOrDefault(h => h.Key == "X-Status-Reason").Value.First();
                if (string.IsNullOrWhiteSpace(responseMessage))
                {
                    responseMessage = response.ReasonPhrase;
                }

                throw new IMHttpRequestException(responseMessage, response.StatusCode);
            }

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<Product>>(responseString, jsonOptions);
        }

        public async Task<Product> GetProductByIdAsync(string productId)
        {
            var response = await _client.GetAsync($"Product/Id/{productId}");

            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = response.Headers.FirstOrDefault(h => h.Key == "X-Status-Reason").Value.First();
                if (string.IsNullOrWhiteSpace(responseMessage))
                {
                    responseMessage = response.ReasonPhrase;
                }

                throw new IMHttpRequestException(responseMessage, response.StatusCode);
            }

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Product>(responseString, jsonOptions);
        }

        public async Task<Product> GetProductByNameAsync(string name)
        {
            var response = await _client.GetAsync($"Product/Name/{name}");

            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = response.Headers.FirstOrDefault(h => h.Key == "X-Status-Reason").Value.First();
                if (string.IsNullOrWhiteSpace(responseMessage))
                {
                    responseMessage = response.ReasonPhrase;
                }

                throw new IMHttpRequestException(responseMessage, response.StatusCode);
            }

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Product>(responseString, jsonOptions);
        }

        public async Task<Product> GetProductByUpcAsync(string upc)
        {
            var response = await _client.GetAsync($"Product/Upc/{upc}");

            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = response.Headers.FirstOrDefault(h => h.Key == "X-Status-Reason").Value.First();
                if (string.IsNullOrWhiteSpace(responseMessage))
                {
                    responseMessage = response.ReasonPhrase;
                }

                throw new IMHttpRequestException(responseMessage, response.StatusCode);
            }

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Product>(responseString, jsonOptions);
        }

        public async Task PostProductAsync(Product product)
        {
            #region used in development DO NOT DELETE
            // DO NOT DELETE - The following code can be used to view any validation errors that occur
            // before the method in the API Controller is called (when using _client.PostAsync)
            //var json = JsonSerializer.Serialize(product);
            //var content = new StringContent(json, Encoding.UTF8, "application/json");
            //var requestUri = UserSettings.Instance.ApiBaseUri + "Product/";
            //var webRequest = new HttpRequestMessage(HttpMethod.Post, requestUri)
            //{
            //    Content = content
            //};
            //var response = await _client.SendAsync(webRequest);
            //var reader = new StreamReader(await response.Content.ReadAsStreamAsync());
            //var responseString = reader.ReadToEnd();
            //response.EnsureSuccessStatusCode();
            #endregion

            var json = JsonSerializer.Serialize(product);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("Product/", content);

            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = response.Headers.FirstOrDefault(h => h.Key == "X-Status-Reason").Value.First();
                if (string.IsNullOrWhiteSpace(responseMessage))
                {
                    responseMessage = response.ReasonPhrase;
                }

                throw new IMHttpRequestException(responseMessage, response.StatusCode);
            }
        }

        public async Task PutProductAsync(Product product)
        {
            var response = await _client.PutAsync($"Product/", new StringContent(JsonSerializer.Serialize(product, jsonOptions), Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
            {
                if (!response.IsSuccessStatusCode)
                {
                    var responseMessage = response.Headers.FirstOrDefault(h => h.Key == "X-Status-Reason").Value.First();
                    if (string.IsNullOrWhiteSpace(responseMessage))
                    {
                        responseMessage = response.ReasonPhrase;
                    }

                    throw new IMHttpRequestException(responseMessage, response.StatusCode);
                }
            }
        }
        #endregion

        #region Group
        public async Task<IEnumerable<Group>> GetGroupsAsync()
        {
            var response = await _client.GetAsync("Group");

            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = response.Headers.FirstOrDefault(h => h.Key == "X-Status-Reason").Value.First();
                if (string.IsNullOrWhiteSpace(responseMessage))
                {
                    responseMessage = response.ReasonPhrase;
                }

                throw new IMHttpRequestException(responseMessage, response.StatusCode);
            }

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<Group>>(responseString, jsonOptions);
        }

        public async Task<Group> GetGroupAsync(string groupId)
        {
            var response = await _client.GetAsync($"Group/{groupId}");

            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = response.Headers.FirstOrDefault(h => h.Key == "X-Status-Reason").Value.First();
                if (string.IsNullOrWhiteSpace(responseMessage))
                {
                    responseMessage = response.ReasonPhrase;
                }

                throw new IMHttpRequestException(responseMessage, response.StatusCode);
            }

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Group>(responseString, jsonOptions);
        }

        public async Task PostGroupAsync(Group group)
        {
            var json = JsonSerializer.Serialize(group);
            var response = await _client.PostAsync($"Group", new StringContent(json, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = response.Headers.FirstOrDefault(h => h.Key == "X-Status-Reason").Value.First();
                if (string.IsNullOrWhiteSpace(responseMessage))
                {
                    responseMessage = response.ReasonPhrase;
                }

                throw new IMHttpRequestException(responseMessage, response.StatusCode);
            }
        }

        public async Task PutGroupAsync(Group group)
        {
            var response = await _client.PutAsync($"Group", new StringContent(JsonSerializer.Serialize(group, jsonOptions), Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = response.Headers.FirstOrDefault(h => h.Key == "X-Status-Reason").Value.First();
                if (!string.IsNullOrWhiteSpace(responseMessage))
                {
                    responseMessage = response.ReasonPhrase;
                }

                throw new IMHttpRequestException(responseMessage, response.StatusCode);
            }
        }
        #endregion

        #region Reports
        public async Task<IEnumerable<GroupTotalsByRecord>> GetGroupTotalsByRecordAsync(string recordId)
        {
            var response = await _client.GetAsync($"Reports/GroupTotalsByRecord/{recordId}");
            string responseMessage;
            if (!response.IsSuccessStatusCode)
            {
                if (response.Headers.Any(h => h.Key == "X-Status-Reason"))
                {
                    responseMessage = response.Headers.First(h => h.Key == "X-Status-Reason").Value.First();
                    if (string.IsNullOrWhiteSpace(responseMessage))
                    {
                        responseMessage = response.ReasonPhrase;
                    }
                }
                else
                {
                    responseMessage = response.ReasonPhrase;
                }

                throw new IMHttpRequestException(responseMessage, response.StatusCode);
            }

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<GroupTotalsByRecord>>(responseString, jsonOptions);
        }
        #endregion
    }
}