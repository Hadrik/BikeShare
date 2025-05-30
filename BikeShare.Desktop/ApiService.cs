﻿using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace BikeShare.Desktop
{
    public static class ApiService
    {
        private static readonly HttpClient Client;
        private static readonly CookieContainer CookieContainer;
        private static string _baseUrl = "http://localhost:5289/api";
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        static ApiService()
        {
            CookieContainer = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = CookieContainer,
                UseCookies = true
            };
            
            Client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(5)
            };
        }

        public static void SetBaseUrl(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public static async Task<T?> GetAsync<T>(string endpoint)
        {
            Console.WriteLine($"GET: '{endpoint}' ->");
            var response = await Client.GetAsync($"{_baseUrl}/{endpoint}");
            response.EnsureSuccessStatusCode();
            var respContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"GET: '{endpoint}' <- {respContent}");
            return JsonSerializer.Deserialize<T>(respContent, Options);
        }

        public static async Task<T?> PostAsync<T>(string endpoint, object data)
        {
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            Console.WriteLine($"POST: '{endpoint}' -> {content}");
            var response = await Client.PostAsync($"{_baseUrl}/{endpoint}", content);
            response.EnsureSuccessStatusCode();
            var respContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"GET: '{endpoint}' <- {respContent}");
            return JsonSerializer.Deserialize<T>(respContent, Options);
        }

        public static async Task<HttpResponseMessage> PostAsync(string endpoint, object data)
        {
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            Console.WriteLine($"POST: '{endpoint}' -> {content}");
            var response = await Client.PostAsync($"{_baseUrl}/{endpoint}", content);
            response.EnsureSuccessStatusCode();
            return response;
        }

        public static async Task<T?> PutAsync<T>(string endpoint, object data)
        {
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            Console.WriteLine($"PUT: '{endpoint}' -> {content}");
            var response = await Client.PutAsync($"{_baseUrl}/{endpoint}", content);
            response.EnsureSuccessStatusCode();
            var respContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"GET: '{endpoint}' <- {respContent}");
            return JsonSerializer.Deserialize<T>(respContent, Options);
        }

        public static async Task<HttpResponseMessage> PutAsync(string endpoint, object data)
        {
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            Console.WriteLine($"PUT: '{endpoint}' -> {content}");
            var response = await Client.PutAsync($"{_baseUrl}/{endpoint}", content);
            response.EnsureSuccessStatusCode();
            return response;
        }
        
        public static async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            Console.WriteLine($"DELETE: '{endpoint}' ->");
            var response = await Client.DeleteAsync($"{_baseUrl}/{endpoint}");
            response.EnsureSuccessStatusCode();
            return response;
        }

        public static async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                var loginData = new { Username = username, Password = password };
                var response = await PostAsync("auth/login", loginData);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public static CookieCollection GetCookies(Uri uri)
        {
            return CookieContainer.GetCookies(uri);
        }
    }
}