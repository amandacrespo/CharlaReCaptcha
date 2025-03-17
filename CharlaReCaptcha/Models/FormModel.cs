using System.ComponentModel.DataAnnotations;

namespace CharlaReCaptcha.Models
{
    public class FormModel
    {
        public string Name { get; set; }
        public string Email { get; set; }

        [Required]
        public string ReCaptchaToken { get; set; }
    }
}
