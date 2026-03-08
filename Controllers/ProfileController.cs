using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketSystemProject.Models;
using TicketSystemProject.ViewModels;

namespace TicketSystemProject.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;


        public ProfileController(UserManager<AppUser> userManager,
                                   SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ================= MAİN PAGE PROFİLE =================
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var model = new ProfileViewModel
            {
                Name = user.Name,
                UserName = user.UserName,
                Email = user.Email
            };

            return View(model);
        }


        // ================= UPDATE PROFİLE =================

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            user.Name = model.Name;
            user.UserName = model.UserName;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("Index", model);
            }

            await _signInManager.RefreshSignInAsync(user);

            return RedirectToAction("Index");
        }

        // ================= UPDATE PASSWORD =================
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string OldPassword, string NewPassword, string ConfirmPassword)
        {
            // 1. Şifrelerin eşleşip eşleşmediğini kontrol et (Ekstra güvenlik)
            if (NewPassword != ConfirmPassword)
            {
                TempData["ErrorMessage"] = "New password and confirmation do not match.";
                return RedirectToAction("Index");
            }
            if (OldPassword != NewPassword)
            {
                TempData["ErrorMessage"] = "New password cannot be the same as the old password.";
                return RedirectToAction("Index");
            }
            if (string.IsNullOrEmpty(OldPassword) || string.IsNullOrEmpty(NewPassword))
            {
                TempData["ErrorMessage"] = "Password field cannot be empty.";
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            
            var result = await _userManager.ChangePasswordAsync(user, OldPassword, NewPassword);

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = string.Join("<br/>", result.Errors.Select(e => e.Description));
                return RedirectToAction("Index");
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["SuccessMessage"] = "Password succesfully updated.";
            return RedirectToAction("Index");
        }
        // ================= DELETE ACCOUNT =================
    }
}