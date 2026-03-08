using System.ComponentModel.DataAnnotations;

namespace TicketSystemProject.Models.ViewModels
{
    public class TicketCreateViewModel
    {
        [Required(ErrorMessage = "Lütfen bir konu başlığı giriniz.")]
        [StringLength(100, ErrorMessage = "Başlık en fazla 100 karakter olabilir.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Lütfen detaylı bir açıklama yazınız.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Lütfen öncelik durumunu seçiniz.")]
        public string Priority { get; set; }
    }
}