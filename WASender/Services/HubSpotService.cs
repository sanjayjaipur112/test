using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WASender.Models;

namespace WASender.Services
{
    public class HubSpotService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.hubapi.com";
        
        public HubSpotService(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        
        public async Task<bool> ValidateApiKey()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"{BaseUrl}/crm/v3/objects/contacts?limit=1");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
        
        public async Task<List<HubSpotContact>> GetContacts(int limit = 100)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"{BaseUrl}/crm/v3/objects/contacts?limit={limit}&properties=firstname,lastname,phone,email");
                
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<HubSpotContactsResponse>(content);
                    return result.Results;
                }
                
                throw new Exception($"Failed to get contacts: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting HubSpot contacts: {ex.Message}");
            }
        }
        
        public async Task<HubSpotContact> CreateContact(HubSpotContact contact)
        {
            try
            {
                var properties = new Dictionary<string, string>();
                
                if (!string.IsNullOrEmpty(contact.Properties.Firstname))
                    properties.Add("firstname", contact.Properties.Firstname);
                    
                if (!string.IsNullOrEmpty(contact.Properties.Lastname))
                    properties.Add("lastname", contact.Properties.Lastname);
                    
                if (!string.IsNullOrEmpty(contact.Properties.Email))
                    properties.Add("email", contact.Properties.Email);
                    
                if (!string.IsNullOrEmpty(contact.Properties.Phone))
                    properties.Add("phone", contact.Properties.Phone);
                
                var requestBody = new { properties };
                string json = JsonConvert.SerializeObject(requestBody);
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync($"{BaseUrl}/crm/v3/objects/contacts", content);
                
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<HubSpotContact>(responseContent);
                }
                
                throw new Exception($"Failed to create contact: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating HubSpot contact: {ex.Message}");
            }
        }
        
        public async Task<bool> CreateNote(string contactId, string content)
        {
            try
            {
                var associations = new
                {
                    contactIds = new[] { contactId }
                };
                
                var requestBody = new
                {
                    properties = new
                    {
                        hs_note_body = content,
                        hs_timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    },
                    associations = associations
                };
                
                string json = JsonConvert.SerializeObject(requestBody);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                
                HttpResponseMessage response = await _httpClient.PostAsync($"{BaseUrl}/crm/v3/objects/notes", httpContent);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating HubSpot note: {ex.Message}");
            }
        }
        
        public async Task<List<HubSpotDeal>> GetDeals(int limit = 100)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"{BaseUrl}/crm/v3/objects/deals?limit={limit}&properties=dealname,amount,dealstage,closedate");
                
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<HubSpotDealsResponse>(content);
                    return result.Results;
                }
                
                throw new Exception($"Failed to get deals: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting HubSpot deals: {ex.Message}");
            }
        }
        
        public async Task<HubSpotDeal> CreateDeal(HubSpotDeal deal)
        {
            try
            {
                var properties = new Dictionary<string, string>();
                
                if (!string.IsNullOrEmpty(deal.Properties.Dealname))
                    properties.Add("dealname", deal.Properties.Dealname);
                    
                if (!string.IsNullOrEmpty(deal.Properties.Amount))
                    properties.Add("amount", deal.Properties.Amount);
                    
                if (!string.IsNullOrEmpty(deal.Properties.Dealstage))
                    properties.Add("dealstage", deal.Properties.Dealstage);
                    
                if (!string.IsNullOrEmpty(deal.Properties.Closedate))
                    properties.Add("closedate", deal.Properties.Closedate);
                
                var requestBody = new { properties };
                string json = JsonConvert.SerializeObject(requestBody);
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync($"{BaseUrl}/crm/v3/objects/deals", content);
                
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<HubSpotDeal>(responseContent);
                }
                
                throw new Exception($"Failed to create deal: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating HubSpot deal: {ex.Message}");
            }
        }
        
        public async Task<bool> AssociateDealWithContact(string dealId, string contactId)
        {
            try
            {
                var requestBody = new
                {
                    inputs = new[]