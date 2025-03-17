using System.Diagnostics;
using CharlaReCaptcha.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CharlaReCaptcha.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _secretKey;

        public HomeController( IOptions<reCAPTCHASettings> settings)
        {
            //_logger = logger;
            _secretKey = settings.Value.SecretKey;
            //_reCaptchaService = reCaptchaService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Success()
        {
            // Esta acción se muestra cuando el reCAPTCHA fue validado correctamente
            return View(); 
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> SubmitForm(string recaptchaToken)
        {
            // Verificar reCAPTCHA
            var isValid = await ValidateCaptcha(recaptchaToken);
            if (isValid)
            {
                // Procesar el formulario
                return RedirectToAction("Success");
            }

            // Si la validación falla
            ViewBag.ErrorMessage = "La validación de reCAPTCHA falló. Inténtalo nuevamente.";
            return View("Index"); // Volver a mostrar el formulario
        }

        private async Task<bool> ValidateCaptcha(string recaptchaToken)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("secret", _secretKey),
                new KeyValuePair<string, string>("response", recaptchaToken)
            }));

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var googleResponse = JsonConvert.DeserializeObject<GoogleReCaptchaResponse>(jsonResponse);

                return googleResponse.Success && googleResponse.Score > 0.5; // Asegúrate de que la puntuación es suficiente
            }
        }

        public class GoogleReCaptchaResponse
        {
            public bool Success { get; set; }
            public float Score { get; set; }
        }
    }
}
