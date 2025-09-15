using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetFinal.Data;
using ProjetFinal.Models;

namespace ProjetFinal.Controllers
{
    public class AdressesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdressesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Adresses
        public async Task<IActionResult> Index()
        {
            var adresses = _context.Adresses.AsNoTracking();
            return View(await adresses.ToListAsync());
        }

        // GET: Adresses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var adresse = await _context.Adresses
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (adresse == null) return NotFound();

            return View(adresse);
        }

        // GET: Adresses/Create
        public IActionResult Create() => View();

        // POST: Adresses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Pays,Province,Ville,Voie,Numero,CodePostal")] Adresse adresse)
        {
            if (!ModelState.IsValid) return View(adresse);

            _context.Add(adresse);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Adresses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var adresse = await _context.Adresses.FindAsync(id);
            if (adresse == null) return NotFound();

            return View(adresse);
        }

        // POST: Adresses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Pays,Province,Ville,Voie,Numero,CodePostal")] Adresse adresse)
        {
            if (id != adresse.Id) return NotFound();

            if (!ModelState.IsValid) return View(adresse);

            try
            {
                _context.Update(adresse);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdresseExists(adresse.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Adresses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var adresse = await _context.Adresses
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (adresse == null) return NotFound();

            return View(adresse);
        }

        // POST: Adresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var adresse = await _context.Adresses.FindAsync(id);
            if (adresse == null) return RedirectToAction(nameof(Index));

            try
            {
                _context.Adresses.Remove(adresse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                // Arrive si l'adresse est référencée par une Entreprise (DeleteBehavior.NoAction)
                ModelState.AddModelError(string.Empty, "Impossible de supprimer cette adresse car elle est liée à une entreprise. Supprimez/modifiez d'abord l'entreprise.");
                // Revenir sur la vue Delete avec le message d’erreur
                return View("Delete", adresse);
            }
        }

        private bool AdresseExists(int id) => _context.Adresses.Any(e => e.Id == id);
    }
}
