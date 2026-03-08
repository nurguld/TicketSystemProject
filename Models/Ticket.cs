using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketSystemProject.Models
{
    public class Ticket
    {
        [Key] // 🔥 Primary Key açıkça tanımlandı
        public int TicketId { get; set; }

        [Required(ErrorMessage = "Konu başlığı boş bırakılamaz.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Açıklama alanı zorunludur.")]
        public string Description { get; set; }

        [Required]
        public string Priority { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string Status { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public AppUser User { get; set; }
    }
}
