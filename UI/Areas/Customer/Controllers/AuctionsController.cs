using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataLayer.Models;
using UI.Data;

namespace UI.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AuctionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuctionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customer/Auctions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Auction.Include(a => a.Announce);
            var list = applicationDbContext.ToList();
            for(int i = 0; i < list.Count ;i++)
            {
                list[i].End = list[i].Start.AddHours(list[i].Length);
            }

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Customer/Auctions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auction = await _context.Auction
                .Include(a => a.Announce)
                .FirstOrDefaultAsync(m => m.AuctionID == id);
            if (auction == null)
            {
                return NotFound();
            }

            return View(auction);
        }

        // GET: Customer/Auctions/Create
        public IActionResult Create()
        {
            ViewData["AnnounceID"] = new SelectList(_context.Announce, "AnnounceID", "Title");
            return View();
        }

        // POST: Customer/Auctions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AuctionID,AnnounceID,Length,Start,Price")] Auction auction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(auction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AnnounceID"] = new SelectList(_context.Announce, "AnnounceID", "Title", auction.AnnounceID);
            return View(auction);
        }

        // GET: Customer/Auctions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auction = await _context.Auction.FindAsync(id);
            if (auction == null)
            {
                return NotFound();
            }
            ViewData["AnnounceID"] = new SelectList(_context.Announce, "AnnounceID", "Title", auction.AnnounceID);
            return View(auction);
        }

        // POST: Customer/Auctions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AuctionID,AnnounceID,Length,Start,Price")] Auction auction)
        {
            if (id != auction.AuctionID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(auction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuctionExists(auction.AuctionID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AnnounceID"] = new SelectList(_context.Announce, "AnnounceID", "Title", auction.AnnounceID);
            return View(auction);
        }

        // GET: Customer/Auctions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auction = await _context.Auction
                .Include(a => a.Announce)
                .FirstOrDefaultAsync(m => m.AuctionID == id);
            if (auction == null)
            {
                return NotFound();
            }

            return View(auction);
        }

        // POST: Customer/Auctions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var auction = await _context.Auction.FindAsync(id);
            _context.Auction.Remove(auction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuctionExists(int id)
        {
            return _context.Auction.Any(e => e.AuctionID == id);
        }
        public async Task<IActionResult> Bid(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var auction = await _context.Auction.FindAsync(id);

            var auction = await _context.Auction
                .Include(a => a.Announce)
                .FirstOrDefaultAsync(m => m.AuctionID == id);

            if (auction == null)
            {
                return NotFound();
            }
            return View(auction);
        }
        public async Task<IActionResult> BidConfirm(Auction act)
        {
            //if (act.AuctionID == 0)
            //{
            //    return NotFound();
            //}

            //var auction = await _context.Auction.FindAsync(id);

            var auction = await _context.Auction
                .Include(a => a.Announce)
                .FirstOrDefaultAsync(m => m.AuctionID == act.AuctionID);

            if (auction == null)
            {
                return NotFound();
            }


            //idealmente esto se deberia controlar en la vista
            if (act.Price <= auction.Price)
                return RedirectToAction(nameof(Index));

            auction.Price = act.Price;
            _context.Entry(auction).State = EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
