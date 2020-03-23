using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataLayer.Models;
using DataLayer.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UI.Data;
using UI.Utility;

namespace UI.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        [BindProperty]
        public OrderDetailsCart detailsCart { get; set; }
        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            detailsCart = new OrderDetailsCart()
            {
                OrderHeader = new OrderHeader()
            };
            detailsCart.OrderHeader.OrderTotal = 0;
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cart = _context.ShoppingCart.Where(m => m.ApplicationUserID == claim.Value);
            if (cart != null)
            {
                detailsCart.listCart = cart.ToList();
            }
            foreach (var list in detailsCart.listCart)
            {
                //revisar q este dando bien el id
                if (list.AnnounceID == 0)
                    continue;
                var announce = await _context.Announce.FirstOrDefaultAsync(m => m.AnnounceID == list.AnnounceID);
                var prod = await _context.Products.Where(m => m.ProductID == announce.ProductID).FirstOrDefaultAsync();
                announce.Product = prod;
                list.Announce = announce;
                detailsCart.OrderHeader.OrderTotal += (list.Count * list.Announce.Price);
            }
            return View(detailsCart);
        }

        public async Task<IActionResult> Summary()
        {
            detailsCart = new OrderDetailsCart()
            {
                OrderHeader = new OrderHeader()
            };
            detailsCart.OrderHeader.OrderTotal = 0;
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ApplicationUser applicationUser = await _context.ApplicationUser.Where(c => c.Id == claim.Value).FirstOrDefaultAsync();

            var cart = _context.ShoppingCart.Where(m => m.ApplicationUserID == claim.Value);
            if (cart != null)
            {
                detailsCart.listCart = cart.ToList();
            }
            foreach (var list in detailsCart.listCart)
            {
                //revisar q este dando bien el id
                if (list.AnnounceID == 0)
                    continue;
                var announce = await _context.Announce.FirstOrDefaultAsync(m => m.AnnounceID == list.AnnounceID);
                var prod = await _context.Products.Where(m => m.ProductID == announce.ProductID).FirstOrDefaultAsync();
                announce.Product = prod;
                list.Announce = announce;
                detailsCart.OrderHeader.OrderTotal += (list.Count * list.Announce.Price);
                list.Announce.Description = SD.ConvertToRawHtml(list.Announce.Description);
                if (list.Announce.Description.Length > 100)
                {
                    list.Announce.Description = list.Announce.Description.Substring(0, 99) + "...";
                }
            }
            detailsCart.OrderHeader.PickUpName = applicationUser.Name;
            detailsCart.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
            detailsCart.OrderHeader.PickUpDate = DateTime.Now;
            return View(detailsCart);
        }

        public async Task<IActionResult> Plus(int cartId)
        {
            var cart = await _context.ShoppingCart.FirstOrDefaultAsync(c => c.ShoppingCartID == cartId);
            var announce = await _context.Announce.FirstOrDefaultAsync(c => c.AnnounceID == cart.AnnounceID);
            if(cart.Count < announce.Amount)
                cart.Count += 1;
            
            var sc = await _context.ShoppingCart.Where(m => m.ApplicationUserID == cart.ApplicationUserID).ToListAsync();
            var count = 0;

            foreach (var c in sc)
            {
                count += c.Count;
            }
            HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var cart = await _context.ShoppingCart.FirstOrDefaultAsync(c => c.ShoppingCartID == cartId);
            if (cart.Count == 1)
            {
                _context.ShoppingCart.Remove(cart);
                await _context.SaveChangesAsync();

                //var cnt = _context.ShoppingCart.Where(u => u.ApplicationUserID == cart.ApplicationUserID).ToList().Count;
                var sc = await _context.ShoppingCart.Where(m => m.ApplicationUserID == cart.ApplicationUserID).ToListAsync();
                var count = 0;

                foreach (var c in sc)
                {
                    count += c.Count;
                }
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);
            }
            else
            {
                cart.Count -= 1;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int cartId)
        {
            var cart = await _context.ShoppingCart.FirstOrDefaultAsync(c => c.ShoppingCartID == cartId);

            _context.ShoppingCart.Remove(cart);
            await _context.SaveChangesAsync();

            //var cnt = _context.ShoppingCart.Where(u => u.ApplicationUserID == cart.ApplicationUserID).ToList().Count;
            var sc = await _context.ShoppingCart.Where(m => m.ApplicationUserID == cart.ApplicationUserID).ToListAsync();
            var count = 0;

            foreach (var c in sc)
            {
                count += c.Count;
            }
            HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);


            return RedirectToAction(nameof(Index));
        }
    }
}