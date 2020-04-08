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

namespace UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminUser)]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnviroment;
        [BindProperty]
        public ProductViewModel ProductVM { get; set; }
        public ProductsController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnviroment = hostingEnvironment;
            ProductVM = new ProductViewModel()
            {
                Category = _context.Category,
                Product = new Product()
            };

        }

        // GET: Admin/Products
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products.Include(p => p.Category).Include(p => p.SubCategory);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductVM.Product = await _context.Products.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.ProductID == id);

            if (ProductVM.Product == null)
            {
                return NotFound();
            }

            return View(ProductVM);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            //ViewData["CategoryID"] = new SelectList(_context.Category, "CategoryID", "Name");
            //ViewData["SubCategoryID"] = new SelectList(_context.SubCategory, "SubCategoryID", "Name");
            return View(ProductVM);
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            ProductVM.Product.SubCategoryID = Convert.ToInt32(Request.Form["SubCategoryID"].ToString());

            if (!ModelState.IsValid)
            {
                return View(ProductVM);
            }

            _context.Products.Add(ProductVM.Product);
            await _context.SaveChangesAsync();

            ////Work on the image saving section

            //string webRootPath = _hostingEnviroment.WebRootPath;
            //var files = HttpContext.Request.Form.Files;

            //var productFromDb = await _context.Products.FindAsync(ProductVM.Product.ProductID);

            //if (files.Count > 0)
            //{
            //    //files has been uploaded
            //    var uploads = Path.Combine(webRootPath, "Images");
            //    var extension = Path.GetExtension(files[0].FileName);

            //    using (var filesStream = new FileStream(Path.Combine(uploads, ProductVM.Product.ProductID + extension), FileMode.Create))
            //    {
            //        files[0].CopyTo(filesStream);
            //    }
            //    productFromDb.Image = @"\Images\" + ProductVM.Product.ProductID + extension;
            //}
            //else
            //{
            //    //no file was uploaded, so use default
            //    var uploads = Path.Combine(webRootPath, @"Images\" + SD.DefaultProductImage);
            //    System.IO.File.Copy(uploads, webRootPath + @"\Images\" + ProductVM.Product.ProductID + ".png");
            //    productFromDb.Image = @"\Images\" + ProductVM.Product.ProductID + ".png";
            //}

            //await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        //Get: Admin/Product/Edit/4
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();
            ProductVM.Product = await _context.Products.Include(m=>m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m=>m.ProductID == id);
            ProductVM.SubCategory = await _context.SubCategory.Where(s => s.CategoryID == ProductVM.Product.CategoryID).ToListAsync();
            if(ProductVM.Product == null)
            {
                return NotFound();
            }
            return View(ProductVM);
        }

        // POST: Admin/Products/Edit/4
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }
            ProductVM.Product.SubCategoryID = Convert.ToInt32(Request.Form["SubCategoryID"].ToString());

            if (!ModelState.IsValid)
            {
                ProductVM.SubCategory = await _context.SubCategory.Where(s => s.CategoryID == ProductVM.Product.CategoryID).ToListAsync();
                return View(ProductVM);
            }

            _context.Products.Add(ProductVM.Product);
            await _context.SaveChangesAsync();

            //Work on the image saving section

            //string webRootPath = _hostingEnviroment.WebRootPath;
            //var files = HttpContext.Request.Form.Files;

            var productFromDb = await _context.Products.FindAsync(ProductVM.Product.ProductID);

            //if (files.Count > 0)
            //{
            //    //files has been uploaded
            //    var uploads = Path.Combine(webRootPath, "Images");
            //    var extension_new = Path.GetExtension(files[0].FileName);

            //    //Delete the original file
            //    var imagePath = Path.Combine(webRootPath, productFromDb.Image.TrimStart('\\'));

            //    if (System.IO.File.Exists(imagePath))
            //    {
            //        System.IO.File.Delete(imagePath);
            //    }

            //    using (var filesStream = new FileStream(Path.Combine(uploads, ProductVM.Product.ProductID + extension_new), FileMode.Create))
            //    {
            //        files[0].CopyTo(filesStream);
            //    }
            //    productFromDb.Image = @"\Images\" + ProductVM.Product.ProductID + extension_new;
            //}
            productFromDb.Name = ProductVM.Product.Name;
            //productFromDb.Price = ProductVM.Product.Price;
            //productFromDb.Description = ProductVM.Product.Description;
            productFromDb.CategoryID = ProductVM.Product.CategoryID;
            productFromDb.SubCategoryID = ProductVM.Product.SubCategoryID;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductVM.Product = await _context.Products.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.ProductID == id);

            if (ProductVM.Product == null)
            {
                return NotFound();
            }

            return View(ProductVM);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //string webRootPath = _hostingEnviroment.WebRootPath;
            Product product = await _context.Products.FindAsync(id);

            if (product != null)
            {
                //var imagePath = Path.Combine(webRootPath, product.Image.TrimStart('\\'));

                //if (System.IO.File.Exists(imagePath))
                //{
                //    System.IO.File.Delete(imagePath);
                //}
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }
        
        [ActionName("GetProduct")]
        public async Task<IActionResult> GetProduct(int id)
        {
            List<Product> products = new List<Product>();
            products = await (from product in _context.Products
                                   where product.SubCategoryID == id
                                   select product).ToListAsync();
            var js = Json(new SelectList(products, "ProductID", "Name"));
            return js;
        }
    }
}
