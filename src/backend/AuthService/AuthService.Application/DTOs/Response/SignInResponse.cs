using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AuthService.Application.DTOs.Response
{
    public class SignInResponse
    {
        [JsonPropertyName("status")]
        public int StatusCode { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
