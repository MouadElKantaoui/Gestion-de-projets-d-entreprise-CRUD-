using System.ComponentModel.DataAnnotations;

namespace ProjetFinal.Models
{
    public class Entreprise
    {
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Nom { get; set; } = "";

        [DataType(DataType.Date)]
        [Display(Name = "Date de création")]
        public DateTime DateCreation { get; set; }

        [Display(Name = "Adresse")]
        public int AdresseId { get; set; }

        public Adresse? Adresse { get; set; }

        public List<Projet> Projets { get; set; } = new();
        public List<Developpeur> Developpeurs { get; set; } = new();
    }
}
