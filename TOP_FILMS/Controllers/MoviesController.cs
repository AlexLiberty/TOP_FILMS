using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TOP_FILMS.Models;
using static System.Net.WebRequestMethods;

namespace TOP_FILMS.Controllers
{
    public class MoviesController : Controller
    {
        private readonly MovieContext _context;
        private readonly IWebHostEnvironment _appEnvironment;

        public MoviesController(MovieContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movies.ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(1000000000)]
        public async Task<IActionResult> Create([Bind("Id,Title,Genre,ReleaseYear,Description")] Movie movie, IFormFile? imageFile)
        {
            ModelState.Remove("ImagePath");
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_appEnvironment.WebRootPath, "Files");
                    var uniqueFileName = $"{Guid.NewGuid().ToString()}_{imageFile.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    movie.ImagePath = $"/Files/{uniqueFileName}";
                }
                else
                {
                    movie.ImagePath = "https://ikinogo.biz/uploads/mini/short/21/3b7daac61aa607060ec1a212750594.webp";
                }

                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(1000000000)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Genre,ReleaseYear,ImagePath,Description")] Movie movie, IFormFile? imageFile)
        {
            if (id != movie.Id)
            {                
                return NotFound();
            }

            ModelState.Remove("ImagePath");

            if (ModelState.IsValid)
            {
                var existingMovie = await _context.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
                if (existingMovie == null)
                {
                    return NotFound();
                }

                if (imageFile != null)
                {
                    var uploadsFolder = Path.Combine(_appEnvironment.WebRootPath, "Files");
                    var uniqueFileName = $"{Guid.NewGuid().ToString()}_{imageFile.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    movie.ImagePath = $"/Files/{uniqueFileName}";
                }
                else
                {
                    movie.ImagePath = existingMovie.ImagePath;
                }

                _context.Update(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}
