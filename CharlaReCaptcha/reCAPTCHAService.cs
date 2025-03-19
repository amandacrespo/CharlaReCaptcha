using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CharlaReCaptcha
{
    public class reCAPTCHAService
    {
        private readonly string _secretKey;

        // Inyectamos la configuración con la clave secreta
        public reCAPTCHAService(IOptions<reCAPTCHASettings> settings)
        {
            _secretKey = settings.Value.SecretKey;
        }

        // Método para validar el token de reCAPTCHA enviado desde el Front-End
        public async Task<bool> ValidateCaptcha(string token)
        {
            using (var client = new HttpClient())
            {
                // Enviar solicitud a Google para verificar el token
                var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("secret", _secretKey), // Se envía la clave secreta
                    new KeyValuePair<string, string>("response", token) // Se envía el token del usuario
                }));
                // Leer la respuesta de Google
                var jsonResponse = await response.Content.ReadAsStringAsync();
                // Deserializar la respuesta JSON de Google
                var googleResponse = JsonConvert.DeserializeObject<GoogleReCaptchaResponse>(jsonResponse);
                // Verificar si el reCAPTCHA fue exitoso y el score es aceptable
                return googleResponse.Success && googleResponse.Score > 0.5;
            }
        }
    }
}

// Clase para mapear la respuesta de Google reCAPTCHA
public class GoogleReCaptchaResponse
{
    public bool Success { get; set; }  // Indica si la verificación fue exitosa
    public float Score { get; set; }   // Puntaje de confianza (0.0 - 1.0)
    public string Action { get; set; } // Acción registrada (por ejemplo, 'submit')
    public string Challenge_ts { get; set; } // Timestamp del desafío
    public string Hostname { get; set; }  // Dominio donde se ejecutó reCAPTCHA
}
