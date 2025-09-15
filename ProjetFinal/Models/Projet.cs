using System.ComponentModel.DataAnnotations;

namespace ProjetFinal.Models
{
    public class Projet
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Titre { get; set; } = "";

        [Range(0, 100, ErrorMessage = "La progression doit être entre 0 et 100.")]
        [Display(Name = "Progression (%)")]
        public int Progression { get; set; }

        [DataType(DataType.Date)]
        public DateTime Deadline { get; set; }

        [Display(Name = "Entreprise")]
        public int EntrepriseId { get; set; }
        public Entreprise? Entreprise { get; set; }

        public List<Developpeur> Developpeurs { get; set; } = new();
    }
}
