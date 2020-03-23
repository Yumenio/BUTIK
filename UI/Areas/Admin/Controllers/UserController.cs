using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UI.Data;
using UI.Utility;

namespace UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminUser)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            return View(await _context.ApplicationUser.Where(u=>u.Id != claim.Value).ToListAsync());
        }
        public async Task<IActionResult> Lock(string id)
        {
            if (id == null)
                return NotFound();
            var appliationUser = await _context.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id );

            if (appliationUser == null)
                return NotFound();
            appliationUser.LockoutEnd = DateTime.Now.AddYears(1000);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> UnLock(string id)
        {
            if (id == null)
                return NotFound();
            var appliationUser = await _context.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);

            if (appliationUser == null)
                return NotFound();
            appliationUser.LockoutEnd = DateTime.Now;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}