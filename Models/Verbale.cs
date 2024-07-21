using System;
using System.ComponentModel.DataAnnotations;

namespace ServerPoliziaApp.Models
{
    public class Verbale
    {
        [Required]
        public string NumeroVerbale { get; set; }

        [Required]
        public int TrasgressoreId { get; set; }

        [Required]
        public int ViolazioneId { get; set; }

        [Required]
        public DateTime DataViolazione { get; set; }

        [Required]
        public decimal Importo { get; set; }

        [Required]
        public int PuntiDecurtati { get; set; }
    }
}
