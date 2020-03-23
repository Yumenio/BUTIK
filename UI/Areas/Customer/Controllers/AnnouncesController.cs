using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataLayer.Models;
using UI.Data;
using Microsoft.AspNetCore.Hosting;
using DataLayer.Models.ViewModels;
using System.IO;
using UI.Utility;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace UI.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AnnouncesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnviroment;
        [BindProperty]
        public ShoppingCart ShopingCart { get; set; }

        public AnnouncesController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnviroment = hostingEnvironment;
            ShopingCart = new ShoppingCart()
            {
                Announce = new Announce()
            };
        }


        //To Do: cambiar la vista index para que se vea solo el titulo y el precio
        // GET: Customer/Announces
        public async Task<IActionResult> Index()
        {
            var listOfAnnounce = _context.Announce.Include(p => p.Product).ToListAsync();
            for (int i = 0; i < listOfAnnounce.Result.Count(); i++)
            {
                int cid = listOfAnnounce.Result[i].Product.CategoryID;
                int scid = listOfAnnounce.Result[i].Product.SubCategoryID;
                var cat = await _context.Category.FindAsync(cid);
                var scat = await _context.SubCategory.FindAsync(scid);
                listOfAnnounce.Result[i].Product.Category = cat;
                listOfAnnounce.Result[i].Product.SubCategory = scat;
            }
            return View(await listOfAnnounce);
        }

        [Authorize]
        // GET: Customer/Announces/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //AnnounceVM.Announce.Product = await _context.Products.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.ProductID == id);
            ShopingCart.Announce = await _context.Announce.SingleOrDefaultAsync(m => m.AnnounceID == id);
            var prodId = ShopingCart.Announce.ProductID;
            var prod = await _context.Products.FindAsync(prodId);
            int cid = prod.CategoryID;
            int scid = prod.SubCategoryID;
            var cat = await _context.Category.FindAsync(cid);
            var scat = await _context.SubCategory.FindAsync(scid);
            prod.Category = cat;
            prod.SubCategory = scat;
            ShopingCart.Announce.Product = prod;
            ShopingCart.AnnounceID = ShopingCart.Announce.AnnounceID;
            if (ShopingCart.Announce == null)
            {
                return NotFound();
            }

            return View(ShopingCart);
        }

        //[Authorize]
        //[HttpPost,ActionName("Details")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DetailsPost(int id)
        {
            ShoppingCart cartObj = new ShoppingCart(); 
            cartObj.ShoppingCartID = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cartObj.ApplicationUserID = claim.Value;

                ShoppingCart cartFromDb = await _context.ShoppingCart.Where(m => m.ApplicationUserID == cartObj.ApplicationUserID && m.AnnounceID == id).FirstOrDefaultAsync();
                cartObj.AnnounceID = id;
                if (cartFromDb == null)
                    await _context.ShoppingCart.AddAsync(cartObj);
                else
                    cartFromDb.Count += cartObj.Count; 

                await _context.SaveChangesAsync();
                
                var sc = await _context.ShoppingCart.Where(m => m.ApplicationUserID == cartObj.ApplicationUserID).ToListAsync();

                var count = 0;

                foreach (var cart in sc)
                {
                    count += cart.Count;
                }

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


        // GET: Customer/Announces/Create
        public IActionResult Create()
        {
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "Name");
            return View(ShopingCart);
        }

        // POST: Customer/Announces/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            //AnnounceVM.Announce.Product.SubCategoryID = Convert.ToInt32(Request.Form["SubCategoryID"].ToString());
            var prodId = ShopingCart.Announce.Product.ProductID;
            var prod = await _context.Products.FindAsync(prodId);
            int cid = prod.CategoryID;
            int scid = prod.SubCategoryID;
            var cat = await _context.Category.FindAsync(cid);
            var scat = await _context.SubCategory.FindAsync(scid);
            prod.Category = cat;
            prod.SubCategory = scat;
            var announce = new Announce()
            {
                Product = prod,
                Description = ShopingCart.Announce.Description,
                Image = ShopingCart.Announce.Image,
                Amount = ShopingCart.Announce.Amount,
                Price = ShopingCart.Announce.Price,
                Title = ShopingCart.Announce.Title
            };
            //announce.Product.Category = await _context.Category.FindAsync(announce.Product.CategoryID);
            //announce.Product.SubCategory = await _context.SubCategory.FindAsync(announce.Product.SubCategoryID);
            bool isValid = true;
            var a = ModelState.Values.ToList();
            for (int i = 0; i < a.Count(); i++)
            {
                var s = a[i].ValidationState.ToString();
                if (i != 4 && a[i].ValidationState.ToString() != "Valid")
                {
                    isValid = false;
                }

            }
            //var b = ModelState.IsValid;
            ShopingCart.Announce = announce;
            if (isValid/*ModelState.IsValid*/)
            {

                _context.Add(ShopingCart.Announce);
                await _context.SaveChangesAsync();

                //Work on the image saving section

                string webRootPath = _hostingEnviroment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var AnnounceFromDb = await _context.Announce.FindAsync(ShopingCart.Announce.AnnounceID);

                if (files.Count > 0)
                {
                    //files has been uploaded
                    var uploads = Path.Combine(webRootPath, "Images");
                    var extension = Path.GetExtension(files[0].FileName);

                    using (var filesStream = new FileStream(Path.Combine(uploads, ShopingCart.Announce.AnnounceID + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filesStream);
                    }
                    AnnounceFromDb.Image = @"\Images\" + ShopingCart.Announce.AnnounceID + extension;
                }
                else
                {
                    //no file was uploaded, so use default
                    var uploads = Path.Combine(webRootPath, @"Images\" + SD.DefaultProductImage);
                    System.IO.File.Copy(uploads, webRootPath + @"\Images\" + ShopingCart.Announce.AnnounceID + ".png");
                    AnnounceFromDb.Image = @"\Images\" + ShopingCart.Announce.AnnounceID + ".png";
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "Name", announce.ProductID);
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "Name");
            return View(ShopingCart);
        }

        [Authorize("Admin")]
        // GET: Customer/Announces/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();
            ShopingCart.Announce = await _context.Announce.SingleOrDefaultAsync(m => m.AnnounceID == id);

            ShopingCart.Announce.Product = await _context.Products.SingleOrDefaultAsync(m => m.ProductID == ShopingCart.Announce.ProductID);

            if (ShopingCart.Announce == null)
            {
                return NotFound();
            }
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "Name");
            return View(ShopingCart);
        }

        // POST: Customer/Announces/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            //ProductVM.Product.SubCategoryID = Convert.ToInt32(Request.Form["SubCategoryID"].ToString());

            bool isValid = true;
            var a = ModelState.Values.ToList();
            for (int i = 0; i < a.Count(); i++)
            {
                if (i != 6 && a[i].ValidationState.ToString() != "Valid")
                {
                    isValid = false;
                }

            }

            var prodId = ShopingCart.Announce.Product.ProductID;
            var prod = await _context.Products.FindAsync(prodId);
            int cid = prod.CategoryID;
            int scid = prod.SubCategoryID;
            var cat = await _context.Category.FindAsync(cid);
            var scat = await _context.SubCategory.FindAsync(scid);
            prod.Category = cat;
            prod.SubCategory = scat;
            var announce = new Announce()
            {
                Product = prod,
                Description = ShopingCart.Announce.Description,
                Image = ShopingCart.Announce.Image,
                Amount = ShopingCart.Announce.Amount,
                ProductID = ShopingCart.Announce.ProductID,
                Price = ShopingCart.Announce.Price,
                Title = ShopingCart.Announce.Title
            };
            //ProductVM.SubCategory = await _context.SubCategory.Where(s => s.CategoryID == ProductVM.Product.CategoryID).ToListAsync();
            if (!isValid/*ModelState.IsValid*/)
            {

                ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "Name");
                ShopingCart.Announce = announce;

                return View(ShopingCart);
            }

            //_context.Products.Add(ProductVM.Product);
            //await _context.SaveChangesAsync();

            //Work on the image saving section

            string webRootPath = _hostingEnviroment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var announceFromDb = await _context.Announce.FindAsync(ShopingCart.Announce.AnnounceID);

            if (files.Count > 0)
            {
                //files has been uploaded
                var uploads = Path.Combine(webRootPath, "Images");
                var extension_new = Path.GetExtension(files[0].FileName);

                //Delete the original file
                var imagePath = Path.Combine(webRootPath, announceFromDb.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                using (var filesStream = new FileStream(Path.Combine(uploads, ShopingCart.Announce.AnnounceID + extension_new), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                announceFromDb.Image = @"\Images\" + ShopingCart.Announce.AnnounceID + extension_new;
            }
            announceFromDb.Product = announce.Product;
            announceFromDb.Price = ShopingCart.Announce.Price;
            announceFromDb.Amount = ShopingCart.Announce.Amount;
            announceFromDb.Title = ShopingCart.Announce.Title;
            //announceFromDb.Image = AnnounceVM.Announce.Image;
            //announceFromDb.Price = AnnounceVM.Announce.Price;

            announceFromDb.Description = ShopingCart.Announce.Description;
            //announceFromDb.Product.CategoryID = AnnounceVM.Announce.Product.CategoryID;
            //announceFromDb.Product.SubCategoryID = AnnounceVM.Announce.Product.SubCategoryID;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        // GET: Customer/Announces/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var announce = await _context.Announce
                .Include(a => a.Product)
                .FirstOrDefaultAsync(m => m.AnnounceID == id);
            ShopingCart.Announce = announce;
            if (announce == null)
            {
                return NotFound();
            }

            return View(ShopingCart);
        }

        // POST: Customer/Announces/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string webRootPath = _hostingEnviroment.WebRootPath;
            //Product product = await _context.Products.FindAsync(id);
            var announce = await _context.Announce.FindAsync(id);

            if (announce != null)
            {
                var imagePath = Path.Combine(webRootPath, announce.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                _context.Announce.Remove(announce);
                await _context.SaveChangesAsync();

            }

            return RedirectToAction(nameof(Index));
        }

        private bool AnnounceExists(int id)
        {
            return _context.Announce.Any(e => e.AnnounceID == id);
        }
    }
}
