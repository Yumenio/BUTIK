using System.Threading.Tasks;
using UI.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataLayer.Models.ViewModels;
using UI.Utility;
using Microsoft.Extensions.Logging;
using DataLayer.Models;

namespace UI.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                //var cnt = _context.ShoppingCart.Where(u => u.ApplicationUserID == claim.Value).Count;

                var sc = await _context.ShoppingCart.Where(m => m.ApplicationUserID == claim.Value).ToListAsync();
                var count = 0;

                foreach (var cart in sc)
                {
                    count += cart.Count;
                }
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);
            }
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var announceFromDb = await _context.Announce.Where(m => m.AnnounceID == id).FirstOrDefaultAsync();
            var prod = await _context.Products.Where(m => m.ProductID == announceFromDb.ProductID).FirstOrDefaultAsync();
            announceFromDb.Product = prod;
            ShoppingCart cartObj = new ShoppingCart()
            {
                Announce = announceFromDb,
                AnnounceID = announceFromDb.AnnounceID
            };
            return View(cartObj);
        }
        [Authorize]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Details(ShoppingCart cartObj)
        {
            cartObj.ShoppingCartID = 0;
            if(ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cartObj.ApplicationUserID = claim.Value;

                ShoppingCart cartFromDb = await _context.ShoppingCart.Where(m => m.ApplicationUserID == cartObj.ApplicationUserID && m.AnnounceID == cartObj.AnnounceID).FirstOrDefaultAsync();

                if (cartFromDb == null)
                    await _context.ShoppingCart.AddAsync(cartObj);
                else
                    cartFromDb.Count += cartObj.Count; ;

                await _context.SaveChangesAsync();
                var count = _context.ShoppingCart.Where(m => m.ApplicationUserID == cartObj.ApplicationUserID).ToList().Count();
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);
                return RedirectToAction("Index");
            }
            else
            {
                var announceFromDb = await _context.Announce.Where(m => m.AnnounceID == cartObj.AnnounceID).FirstOrDefaultAsync();
                var prod = await _context.Products.Where(m => m.ProductID == announceFromDb.ProductID).FirstOrDefaultAsync();
                announceFromDb.Product = prod;
                ShoppingCart cart = new ShoppingCart()
                {
                    Announce = announceFromDb,
                    AnnounceID = announceFromDb.AnnounceID
                };
                return View(cart);
            }

        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
