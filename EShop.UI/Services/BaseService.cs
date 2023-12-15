using EShop.UI.Models;
using EShop.UI.Services.IService;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;
using static EShop.UI.Utility.StaticDetails;

namespace EShop.UI.Services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;
        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider) 
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }

        public async Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("EShopAPI");
                HttpRequestMessage message = new();
                message.Headers.Add("Accept", "application/json");
                //token
                if(withBearer)
                {
                    var token = _tokenProvider.GetToken();
                    message.Headers.Add("Authorization", $"Bearer {token}");
                }

                message.RequestUri = new Uri(requestDto.Url);
                if (requestDto.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
                }

                HttpResponseMessage? apiResponse = null;

                switch (requestDto.ApiType)
                {
                    case ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    case ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                apiResponse = await client.SendAsync(message);

                switch (apiResponse.StatusCode)
                {
                    case System.Net.HttpStatusCode.NotFound:
                        return new() { isSuccess = false, Message = "Not Found" };
                    case System.Net.HttpStatusCode.Unauthorized:
                        return new() { isSuccess = false, Message = "Unauthorized" };
                    case System.Net.HttpStatusCode.Forbidden:
                        return new() { isSuccess = false, Message = "Access Denied" };
                    case System.Net.HttpStatusCode.InternalServerError:
                        return new() { isSuccess = false, Message = "Internal Server Error" };
                    default:
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                        return apiResponseDto;
                }
            }
            catch (Exception ex)
            {
                var Dto = new ResponseDto
                {
                    Message = ex.Message.ToString(),
                    isSuccess = false
                };
                return Dto;
            }
            
        }
    }
}
