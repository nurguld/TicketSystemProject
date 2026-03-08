using System.ComponentModel.DataAnnotations;

namespace TicketSystemProject.ViewModels
{
    public class ProfileViewModel
    {
        [Required]
        [Display(Name = "Ad Soyad")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Kullanıcı Adı")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

}
