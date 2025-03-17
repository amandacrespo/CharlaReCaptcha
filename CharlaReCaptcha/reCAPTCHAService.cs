using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CharlaReCaptcha
{
    public class reCAPTCHAService
    {
        private readonly string _secretKey;

        public reCAPTCHAService(IOptions<reCAPTCHASettings> settings)
        {
            _secretKey = settings.Value.SecretKey;
        }

        public async Task<bool> ValidateCaptcha(string token)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("secret", _secretKey),
                new KeyValuePair<string, string>("response", token)
            }));

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var googleResponse = JsonConvert.DeserializeObject<GoogleReCaptchaResponse>(jsonResponse);
                return googleResponse.Success && googleResponse.Score > 0.5;
            }
        }
    }
}

public class GoogleReCaptchaResponse
{
    public bool Success { get; set; }
    public float Score { get; set; }
    public string Action { get; set; }
    public string Challenge_ts { get; set; }
    public string Hostname { get; set; }
}
