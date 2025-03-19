using System.Diagnostics;
using CharlaReCaptcha.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CharlaReCaptcha.Controllers
{
    public class HomeController : Controller
    {
        private readonly reCAPTCHAService reCaptchaService;

        public HomeController(reCAPTCHAService reCaptchaService)
        {
            this.reCaptchaService = reCaptchaService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
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
            // Validar el token de reCAPTCHA
            var isValid = await this.reCaptchaService.ValidateCaptcha(recaptchaToken);
            if (isValid)
            {
                return RedirectToAction("Success");
            }

            // Si la validación falla
            ViewBag.ErrorMessage = "La validación de reCAPTCHA falló. Inténtalo nuevamente.";
            return View("Index"); // Volver a mostrar el formulario
        }

        public IActionResult Success()
        {
            // Esta acción se muestra cuando el reCAPTCHA fue validado correctamente
            return View();
        }
    }
}
