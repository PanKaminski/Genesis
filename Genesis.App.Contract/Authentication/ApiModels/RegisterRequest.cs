using System.ComponentModel.DataAnnotations;

namespace Genesis.App.Contract.Authentication.ApiModels
{
    public class RegisterRequest
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string MiddleName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        public byte Gender { get; set; }

        public string CountryCode { get; set; }

        public string CityCode { get; set; }
    }
}
