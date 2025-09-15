using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetFinal.Data;
using ProjetFinal.Models;

namespace ProjetFinal.Controllers
{
    public class EntreprisesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EntreprisesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Entreprises
        public async Task<IActionResult> Index()
        {
            var entreprises = _context.Entreprises
                .Include(e => e.Adresse)
                .AsNoTracking();

            return View(await entreprises.ToListAsync());
        }

        // GET: Entreprises/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var entreprise = await _context.Entreprises
                .Include(e => e.Adresse)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (entreprise == null) return NotFound();

            return View(entreprise);
        }

        // Helpers
        private SelectList BuildAdresseSelectList(int? selectedId = null)
        {
            var items = _context.Adresses.AsNoTracking()
                .Select(a => new
                {
                    a.Id,
                    Libelle = a.Numero + " " + a.Voie + ", " + a.CodePostal + " " + a.Ville
                })
                .ToList();

            return new SelectList(items, "Id", "Libelle", selectedId);
        }

        // GET: Entreprises/Create
        public IActionResult Create()
        {
            ViewData["AdresseId"] = BuildAdresseSelectList();
            return View();
        }

        // POST: Entreprises/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nom,DateCreation,AdresseId")] Entreprise entreprise)
        {
            if (!ModelState.IsValid)
            {
                ViewData["AdresseId"] = BuildAdresseSelectList(entreprise.AdresseId);
                return View(entreprise);
            }

            _context.Add(entreprise);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Entreprises/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var entreprise = await _context.Entreprises.FindAsync(id);
            if (entreprise == null) return NotFound();

            ViewData["AdresseId"] = BuildAdresseSelectList(entreprise.AdresseId);
            return View(entreprise);
        }

        // POST: Entreprises/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nom,DateCreation,AdresseId")] Entreprise entreprise)
        {
            if (id != entreprise.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["AdresseId"] = BuildAdresseSelectList(entreprise.AdresseId);
                return View(entreprise);
            }

            try
            {
                _context.Update(entreprise);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntrepriseExists(entreprise.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Entreprises/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var entreprise = await _context.Entreprises
                .Include(e => e.Adresse)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (entreprise == null) return NotFound();

            return View(entreprise);
        }

        // POST: Entreprises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entreprise = await _context.Entreprises.FindAsync(id);
            if (entreprise != null)
            {
                // Note: avec DeleteBehavior.NoAction, la suppression échouera
                // s'il reste des Projets/Developpeurs liés. Supprime/retire-les d'abord.
                _context.Entreprises.Remove(entreprise);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EntrepriseExists(int id) => _context.Entreprises.Any(e => e.Id == id);
    }
}
