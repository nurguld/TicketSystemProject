using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace TicketSystemProject.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        public string? Name { get; set; }

        // Navigation property
        public ICollection<Ticket>? Tickets { get; set; }
    }
}