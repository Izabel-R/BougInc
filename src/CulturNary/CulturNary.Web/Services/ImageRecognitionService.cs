using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;
using System.Net.Http.Headers;
using SpecFlow.Internal.Json;
using System.Web.Helpers;
using Newtonsoft.Json;
using System.Text;

namespace CulturNary.Web.Services
{
    public interface IImageRecognitionService
    {
        Task<string> ImageRecognitionAsync(string imagePath, string requestText);
    }


    public class ImageRecognitionService : IImageRecognitionService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ImageRecognitionService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> ImageRecognitionAsync(string imagePath, string requestText)
        {
            try{
                string apiKey = _configuration["OpenAI:ImageRecognitionAppKey"];
                string base64_image = imagePath;

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                string textPrompt;

                if(requestText == "default")
                {
                    textPrompt = "What kind of food or ingredients is in this picture?";
                }
                else
                {
                    textPrompt = $"What would the price be to purchase the ingredients in this picture, or the ingredients to make the food in this picture, in US dollars for a person living in the Zip Code: {requestText}?";
                }

                var payload = new
                        {
                            model = "gpt-4-turbo",
                            messages = new[]
                            {
                                new
                                {
                                    role = "user",
                                    content = new object[]
                                    {
                                        new
                                        {
                                            type = "text",
                                            text = textPrompt
                                        },
                                        new
                                        {
                                            type = "image_url",
                                            image_url = new
                                            {
                                                url = $"data:image/jpeg;base64,{base64_image}",
                                                detail = "low"
                                            }
                                        }
                                    }
                                }
                            },
                            max_tokens = 800
                        };

                var jsonPayload = JsonConvert.SerializeObject(payload);

                var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                //Console.WriteLine("Initiating API Call to OpenAI Image Recognition API");

                HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    //Console.WriteLine("Image Recognition API call successful");
                    Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    //Console.WriteLine("Image Recognition API call failed");
                    Console.WriteLine(response.ToString());
                    Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    throw new HttpRequestException($"Error fetching image recognition result: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in Image Recognition Service: " + ex.Message);
                throw new HttpRequestException($"Error fetching image recognition result: {ex.Message}");
            }
        }
    }

    //     public async Task<string> ImageRecognitionAsync(string imagePath)
    //     {
    //         var appKey = _configuration["FoodVisor:ImageRecognitionAppKey"];
    //         var baseUrl = "https://vision.foodvisor.io/api/1.0/en/analysis/";

    //         // byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);

    //         // MultipartFormDataContent content = new MultipartFormDataContent();
    //         // ByteArrayContent imageContent = new ByteArrayContent(imageBytes);
    //         // content.Add(imageContent, "image", "image.jpg");

    //         _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue($"Api-Key", appKey);
    //         Console.WriteLine(_httpClient.DefaultRequestHeaders.Authorization);

    //         HttpResponseMessage response = await _httpClient.PostAsync(baseUrl, new StringContent("image=" + imagePath));

    //         string responseContent = await response.Content.ReadAsStringAsync();
    //         Console.WriteLine($"Image Recognition Result: {response.StatusCode}, {responseContent}");

    //         if (response.IsSuccessStatusCode)
    //         {
    //             return await response.Content.ReadAsStringAsync();
    //         }
    //         else
    //         {
    //             throw new HttpRequestException($"Error fetching image recognition result: {response.ReasonPhrase}");
    //         }
    //     }
    // }

}