using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataLayer.Models;
using UI.Data;
using DataLayer.Models.ViewModels;

namespace UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        [TempData]
        public string StatusMessage { get; set; }

        public SubCategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/SubCategory
        public async Task<IActionResult> Index()
        {
            var subCategories = await _context.SubCategory.Include(s => s.Category).ToListAsync();
            return View(subCategories);
        }

        // GET: Admin/SubCategory/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subCategory = await _context.SubCategory.Include(s => s.Category).SingleOrDefaultAsync(m => m.SubCategoryID == id);
            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);
        }

        

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();
            subCategories = await (from subCategory in _context.SubCategory
                           where subCategory.CategoryID == id
                           select subCategory).ToListAsync();
            return Json(new SelectList(subCategories, "SubCategoryID", "Name"));
        }
        // GET: Admin/SubCategory/Create
        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _context.Category.ToListAsync(),
                SubCategory = new SubCategory(),
                SubCategoryList = await _context.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };

            //ViewData["CategoryID"] = new SelectList(_context.Category, "CategoryID", "Name");
            return View(model);
        }
        // POST: Admin/SubCategory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("SubCategoryID,Name,CategoryID")] SubCategory subCategory)
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExist = _context.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.CategoryID == model.SubCategory.CategoryID);
                if(doesSubCategoryExist.Count() > 0)
                {
                    //Error
                    StatusMessage = "Error : the Sub Category exist under " + doesSubCategoryExist.First().Category.Name + " category. Please use another name.";
                }
                else
                {
                    _context.SubCategory.Add(model.SubCategory);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            //ViewData["CategoryID"] = new SelectList(_context.Category, "CategoryID", "Name", /*s*/ubCategory.CategoryID);
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _context.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _context.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync(),
                StatusMessage = StatusMessage
            };   
            return View(modelVM);
        }


        // GET: Admin/SubCategory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var subCategory = await _context.SubCategory.SingleOrDefaultAsync(m => m.SubCategoryID == id);
            if (subCategory == null)
            {
                return NotFound();
            }
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _context.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _context.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };

            //ViewData["CategoryID"] = new SelectList(_context.Category, "CategoryID", "Name");
            return View(model);
        }

        // POST: Admin/SubCategory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExist = _context.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.CategoryID == model.SubCategory.CategoryID);
                if (doesSubCategoryExist.Count() > 0)
                {
                    //Error
                    StatusMessage = "Error : the Sub Category exist under " + doesSubCategoryExist.First().Category.Name + " category. Please use another name.";
                }
                else
                {
                    var subCaFromDb = await _context.SubCategory.FindAsync(model.SubCategory.SubCategoryID);
                    subCaFromDb.Name = model.SubCategory.Name;

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            //ViewData["CategoryID"] = new SelectList(_context.Category, "CategoryID", "Name", /*s*/ubCategory.CategoryID);
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _context.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _context.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync(),
                StatusMessage = StatusMessage
            };
            return View(modelVM);
        }

        // GET: Admin/SubCategory/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subCategory = await _context.SubCategory.Include(s => s.Category).SingleOrDefaultAsync(m => m.SubCategoryID == id);
            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);
        }

        // POST: Admin/SubCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subCategory = await _context.SubCategory.SingleOrDefaultAsync(m => m.SubCategoryID == id);
            _context.SubCategory.Remove(subCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        private bool SubCategoryExists(int id)
        {
            return _context.SubCategory.Any(e => e.SubCategoryID == id);
        }
    }
}
