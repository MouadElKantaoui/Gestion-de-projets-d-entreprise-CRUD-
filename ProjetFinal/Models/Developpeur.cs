using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore; // pour [Index] si tu veux l'unicité

namespace ProjetFinal.Models
{
    // (Optionnel) Rendre le Code unique en base
    [Index(nameof(Code), IsUnique = true)]
    public class Developpeur
    {
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Nom { get; set; } = "";

        [Display(Name = "Code (identifiant)")]
        [Range(1, int.MaxValue, ErrorMessage = "Le code doit être un entier positif.")]
        public int Code { get; set; }

        [Required, StringLength(100)]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string MotDePasse { get; set; } = "";

        [Required, StringLength(10)]
        [Display(Name = "Ancienneté")]
        [RegularExpression("^(Senior|Junior)$", ErrorMessage = "Ancienneté doit être 'Senior' ou 'Junior'.")]
        public string Anciennete { get; set; } = ""; // Senior / Junior

        [Display(Name = "Entreprise")]
        public int EntrepriseId { get; set; }
        public Entreprise? Entreprise { get; set; }

        public List<Projet> Projets { get; set; } = new();
    }
}
