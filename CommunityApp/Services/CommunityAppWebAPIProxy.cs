﻿using CommunityApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
//using CommunityApp.Models;

namespace CommunityApp.Services
{
    public class CommunityWebAPIProxy
    {
        #region without tunnel
        /*
        //Define the serevr IP address! (should be realIP address if you are using a device that is not running on the same machine as the server)
        private static string serverIP = "localhost";
        private HttpClient client;
        private string baseUrl;
        public static string BaseAddress = (DeviceInfo.Platform == DevicePlatform.Android &&
            DeviceInfo.DeviceType == DeviceType.Virtual) ? "http://10.0.2.2:5110/api/" : $"http://{serverIP}:5110/api/";
        private static string ImageBaseAddress = (DeviceInfo.Platform == DevicePlatform.Android &&
            DeviceInfo.DeviceType == DeviceType.Virtual) ? "http://10.0.2.2:5110" : $"http://{serverIP}:5110";
        */
        #endregion

        #region with tunnel
        //Define the serevr IP address! (should be realIP address if you are using a device that is not running on the same machine as the server)
        private static string serverIP = "gvxbtwjc-7237.euw.devtunnels.ms";
        private HttpClient client;
        private string baseUrl;
        public static string BaseAddress = "https://gvxbtwjc-7237.euw.devtunnels.ms/api/";
        public static string ImageBaseAddress = "https://gvxbtwjc-7237.euw.devtunnels.ms/";
        #endregion

        public CommunityWebAPIProxy()
        {
            //Set client handler to support cookies!!
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = new System.Net.CookieContainer();

            this.client = new HttpClient(handler);
            this.baseUrl = BaseAddress;
        }

        public string GetImagesBaseAddress()
        {
            return CommunityWebAPIProxy.ImageBaseAddress;
        }

        public string GetDefaultProfilePhotoUrl()
        {
            return $"{CommunityWebAPIProxy.ImageBaseAddress}/profileImages/default.png";
        }
        public async Task<Account?> UploadProfileImage(string imagePath)
        {
            //Set URI to the specific function API
            string url = $"{this.baseUrl}uploadprofileimage";
            try
            {
                //Create the form data
                MultipartFormDataContent form = new MultipartFormDataContent();
                var fileContent = new ByteArrayContent(File.ReadAllBytes(imagePath));
                form.Add(fileContent, "file", imagePath);
                //Call the server API
                HttpResponseMessage response = await client.PostAsync(url, form);
                //Check status
                if (response.IsSuccessStatusCode)
                {
                    //Extract the content as string
                    string resContent = await response.Content.ReadAsStringAsync();
                    //Desrialize result
                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    Account? result = JsonSerializer.Deserialize<Account>(resContent, options);
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<Account?> Register(Account user)
        {
            //Set URI to the specific function API
            string url = $"{this.baseUrl}register";
            try
            {
                //Call the server API
                string json = JsonSerializer.Serialize(user);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);
                //Check status
                if (response.IsSuccessStatusCode)
                {
                    //Extract the content as string
                    string resContent = await response.Content.ReadAsStringAsync();
                    //Desrialize result
                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    Account? result = JsonSerializer.Deserialize<Account>(resContent, options);
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<Account?> LoginAsync(LoginInfo userInfo)
        {
            //Set URI to the specific function API
            string url = $"{this.baseUrl}login";
            try
            {
                //Call the server API
                string json = JsonSerializer.Serialize(userInfo);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);
                //Check status
                if (response.IsSuccessStatusCode)
                {
                    //Extract the content as string
                    string resContent = await response.Content.ReadAsStringAsync();
                    //Desrialize result
                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    Account? result = JsonSerializer.Deserialize<Account>(resContent, options);
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<List<MemberCommunity>?> GetUserCommunitiesAsync(int userId)
        {
            string url = $"{this.baseUrl}GetUserCommunities?id={userId}";
            try
            {
                // Call the API
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response content
                    string content = await response.Content.ReadAsStringAsync();
                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    // Deserialize into a list of MemberCommunity objects
                    return JsonSerializer.Deserialize<List<MemberCommunity>>(content, options);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return null;
            }
        }

        //public async Task<bool> SignInToCommunityAsync(int comId)
        //{
        //    string url = $"{this.baseUrl}SignInToCommunity";
        //    try
        //    {
        //        // Prepare the request content
        //        StringContent content = new StringContent(JsonSerializer.Serialize(comId), Encoding.UTF8, "application/json");

        //        // Send the POST request
        //        HttpResponseMessage response = await client.PostAsync(url, content);

        //        // Check if the response is successful
        //        return response.IsSuccessStatusCode;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log or handle the error
        //        return false;
        //    }
        //}
        public async Task<List<Notice>> GetCommunityNoticesAsync(int comId)
        {
            string url = $"{this.baseUrl}GetCommunityNotices?ComId={comId}";
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    return JsonSerializer.Deserialize<List<Notice>>(content, options) ?? new List<Notice>();
                }
                else
                {
                    return new List<Notice>(); // Return an empty list instead of null
                }
            }
            catch (Exception ex)
            {
                return new List<Notice>(); // Handle exceptions by returning an empty list
            }
        }

        public async Task<List<Report>> GetCommunityReportsAsync(int comId)
        {
            string url = $"{this.baseUrl}GetCommunityReports?ComId={comId}";
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    return JsonSerializer.Deserialize<List<Report>>(content, options) ?? new List<Report>();
                }
                else
                {
                    return new List<Report>();
                }
            }
            catch (Exception ex)
            {
                return new List<Report>();
            }
        }

        public async Task<bool> CreateReportAsync(Report r)
        {
            try
            {
                string url = $"{this.baseUrl}CreateReport";
                string json = JsonSerializer.Serialize(r);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);

                // If the request was successful, read the boolean response
                return response.IsSuccessStatusCode &&
                       bool.TryParse(await response.Content.ReadAsStringAsync(), out bool result) && result;
            }
            catch
            {
                return false; // If an error occurs, assume failure
            }
        }

        public async Task<int> GetCommunityIdAsync(string s)
        {
            try
            {
                string url = $"{this.baseUrl}GetCommunityId";
                string json = JsonSerializer.Serialize(s);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (int.TryParse(responseContent, out int communityId))
                    {
                        return communityId;
                    }
                }

                return -2;
            }
            catch
            {
                return -3; 
            }
        }

        public async Task<bool> JoinCommunityAsync(Member m)
        {
            try
            {
                string url = $"{this.baseUrl}JoinCommunity";
                string json = JsonSerializer.Serialize(m);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);

                return response.IsSuccessStatusCode &&
                       bool.TryParse(await response.Content.ReadAsStringAsync(), out bool result) && result;
            }
            catch
            {
                return false; 
            }
        }
        
        public async Task<MemberCommunity> CreateCommunityAsync(MemberCommunity memCom)
        {
            try
            {
                string url = $"{this.baseUrl}CreateCommunity";
                string json = JsonSerializer.Serialize(memCom);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    // Parse the response content
                    string result = await response.Content.ReadAsStringAsync();
                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    return JsonSerializer.Deserialize<MemberCommunity>(result, options);
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<MemberAccount>> GetCommunityMemberAccountsAsync(int comId)
        {
            string url = $"{this.baseUrl}GetCommunityMemberAccounts?ComId={comId}";
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    return JsonSerializer.Deserialize<List<MemberAccount>>(content, options) ?? new List<MemberAccount>();
                }
                else
                {
                    return new List<MemberAccount>();
                }
            }
            catch (Exception ex)
            {
                return new List<MemberAccount>();
            }
        }

    }
}