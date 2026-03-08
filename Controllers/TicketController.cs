using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketSystemProject.Data;
using TicketSystemProject.Models;
using TicketSystemProject.Models.ViewModels;

namespace TicketSystemProject.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public TicketController(AppDbContext context,
                                UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string search, string status)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            var tickets = _context.Tickets
                                  .Include(t => t.User)
                                  .AsQueryable();

            // 🔒 Eğer admin değilse sadece kendi ticketlarını görsün
            if (!isAdmin)
            {
                tickets = tickets.Where(t => t.UserId == currentUser.Id);
            }

            // 🔍 Search
            if (!string.IsNullOrEmpty(search))
            {
                tickets = tickets.Where(t => t.Title.Contains(search));
            }

            // 📌 Status filter
            if (!string.IsNullOrEmpty(status))
            {
                tickets = tickets.Where(t => t.Status == status);
            }

            // 📊 Dashboard için istatistikler
            ViewBag.OpenCount = await tickets.CountAsync(t => t.Status == "Open");
            ViewBag.ClosedCount = await tickets.CountAsync(t => t.Status == "Closed");
            ViewBag.PendingCount = await tickets.CountAsync(t => t.Status == "Pending");

            ViewData["FilterStatus"] = status;
            ViewData["SearchTerm"] = search;

            var ticketList = await tickets
                                .OrderByDescending(t => t.CreatedDate)
                                .ToListAsync();

            return View(ticketList);
        }

        public IActionResult Details(int id)
        {
            var ticket = _context.Tickets
                                 .Include(t => t.User)   // Eğer ticket’ın user ilişkisi varsa
                                 .FirstOrDefault(t => t.TicketId == id);

            if (ticket == null)
                return NotFound();

            return View(ticket);
        }

        [HttpGet]
        public IActionResult Create()
        {
            // Boş ViewModel ile view’i render ediyoruz
            var vm = new TicketCreateViewModel();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TicketCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // ModelState geçersizse aynı sayfaya ViewModel ile dön
                return View(vm);
            }

            // Kullanıcıyı al
            var currentUser = await _userManager.GetUserAsync(User);

            // Ticket oluştur
            var ticket = new Ticket
            {
                Title = vm.Title,
                Description = vm.Description,
                Priority = vm.Priority,
                UserId = currentUser.Id,
                Status = "Open",
                CreatedDate = DateTime.UtcNow
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")] // Sadece adminler silebilir
        public async Task<IActionResult> Delete(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }





    }
}