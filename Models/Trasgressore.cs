using System.ComponentModel.DataAnnotations;

namespace ServerPoliziaApp.Models
{
    public class Trasgressore
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public string Cognome { get; set; }

        [Required]
        public string Indirizzo { get; set; }

        [Required]
        public string Città { get; set; }

        [Required]
        public string CodiceFiscale { get; set; }
    }
}

