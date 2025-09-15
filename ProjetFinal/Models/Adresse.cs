using System.ComponentModel.DataAnnotations;

namespace ProjetFinal.Models
{
    public class Adresse
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Pays { get; set; } = "";

        [Required, StringLength(100)]
        public string Province { get; set; } = "";

        [Required, StringLength(100)]
        public string Ville { get; set; } = "";

        [Required, StringLength(150)]
        public string Voie { get; set; } = "";

        [Required, StringLength(20)]
        public string Numero { get; set; } = "";

        [Required, StringLength(20)]
        public string CodePostal { get; set; } = "";

        // 1–1 avec Entreprise (inverse navigation)
        public Entreprise? Entreprise { get; set; }
    }
}
