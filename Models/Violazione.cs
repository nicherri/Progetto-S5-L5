using System.ComponentModel.DataAnnotations;

namespace ServerPoliziaApp.Models
{
    public class Violazione
    {
        public int Id { get; set; }

        [Required]
        public string Descrizione { get; set; }

        [Required]
        public decimal ImportoMinimo { get; set; }

        [Required]
        public int PuntiDecurtati { get; set; }
    }
}
