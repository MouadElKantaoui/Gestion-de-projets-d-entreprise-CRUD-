using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetFinal.Data;
using ProjetFinal.Models;

namespace ProjetFinal.Controllers
{
    public class ProjetsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --------------------
        // LECTURE (publique)
        // --------------------

        // GET: Projets
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var projets = _context.Projets
                .Include(p => p.Entreprise)
                .Include(p => p.Developpeurs)
                .AsNoTracking();

            return View(await projets.ToListAsync());
        }

        // GET: Projets/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var projet = await _context.Projets
                .Include(p => p.Entreprise)
                .Include(p => p.Developpeurs)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (projet == null) return NotFound();

            return View(projet);
        }

        // --------------------
        // CRÉATION (Senior)
        // --------------------

        [Authorize(Policy = "CanEditProjects")]
        public IActionResult Create()
        {
            ViewData["EntrepriseId"] = new SelectList(_context.Entreprises.AsNoTracking(), "Id", "Nom");
            ViewData["DeveloppeursIds"] = new MultiSelectList(_context.Developpeurs.AsNoTracking(), "Id", "Nom");
            return View();
        }

        [Authorize(Policy = "CanEditProjects")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titre,Progression,Deadline,EntrepriseId")] Projet projet, int[] DeveloppeursIds)
        {
            if (projet.Progression < 0 || projet.Progression > 100)
                ModelState.AddModelError(nameof(Projet.Progression), "La progression doit être entre 0 et 100.");

            if (ModelState.IsValid)
            {
                // Attacher les développeurs sélectionnés (N–N)
                projet.Developpeurs = new List<Developpeur>();
                foreach (var devId in DeveloppeursIds.Distinct())
                {
                    var dev = await _context.Developpeurs.FindAsync(devId);
                    if (dev != null) projet.Developpeurs.Add(dev);
                }

                _context.Add(projet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["EntrepriseId"] = new SelectList(_context.Entreprises.AsNoTracking(), "Id", "Nom", projet.EntrepriseId);
            ViewData["DeveloppeursIds"] = new MultiSelectList(_context.Developpeurs.AsNoTracking(), "Id", "Nom", DeveloppeursIds);
            return View(projet);
        }

        // --------------------
        // ÉDITION (Senior)
        // --------------------

        [Authorize(Policy = "CanEditProjects")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var projet = await _context.Projets
                .Include(p => p.Developpeurs) // charger la N–N
                .FirstOrDefaultAsync(p => p.Id == id);

            if (projet == null) return NotFound();

            ViewData["EntrepriseId"] = new SelectList(_context.Entreprises.AsNoTracking(), "Id", "Nom", projet.EntrepriseId);
            ViewData["DeveloppeursIds"] = new MultiSelectList(
                _context.Developpeurs.AsNoTracking(),
                "Id",
                "Nom",
                projet.Developpeurs.Select(d => d.Id)
            );

            return View(projet);
        }

        [Authorize(Policy = "CanEditProjects")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titre,Progression,Deadline,EntrepriseId")] Projet formProjet, int[] DeveloppeursIds)
        {
            if (id != formProjet.Id) return NotFound();

            if (formProjet.Progression < 0 || formProjet.Progression > 100)
                ModelState.AddModelError(nameof(Projet.Progression), "La progression doit être entre 0 et 100.");

            if (!ModelState.IsValid)
            {
                ViewData["EntrepriseId"] = new SelectList(_context.Entreprises.AsNoTracking(), "Id", "Nom", formProjet.EntrepriseId);
                ViewData["DeveloppeursIds"] = new MultiSelectList(_context.Developpeurs.AsNoTracking(), "Id", "Nom", DeveloppeursIds);
                return View(formProjet);
            }

            // Charger l'entité existante + ses relations
            var projet = await _context.Projets
                .Include(p => p.Developpeurs)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (projet == null) return NotFound();

            // Mettre à jour les champs scalaires
            projet.Titre = formProjet.Titre;
            projet.Progression = formProjet.Progression;
            projet.Deadline = formProjet.Deadline;
            projet.EntrepriseId = formProjet.EntrepriseId;

            // Synchroniser la relation N–N
            projet.Developpeurs.Clear();
            foreach (var devId in DeveloppeursIds.Distinct())
            {
                var dev = await _context.Developpeurs.FindAsync(devId);
                if (dev != null) projet.Developpeurs.Add(dev);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjetExists(projet.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // --------------------
        // SUPPRESSION (Senior)
        // --------------------

        [Authorize(Policy = "CanEditProjects")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var projet = await _context.Projets
                .Include(p => p.Entreprise)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (projet == null) return NotFound();

            return View(projet);
        }

        [Authorize(Policy = "CanEditProjects")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projet = await _context.Projets
                .Include(p => p.Developpeurs)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (projet != null)
            {
                // Optionnel: vider les liaisons N–N avant suppression
                projet.Developpeurs.Clear();
                _context.Projets.Remove(projet);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProjetExists(int id) => _context.Projets.Any(e => e.Id == id);
    }
}
