﻿namespace ServerPoliziaApp.Models
{
    public class ViolazioneOver400EuroViewModel
    {
        public string NomeTrasgressore { get; set; }
        public string CognomeTrasgressore { get; set; }
        public DateTime DataViolazione { get; set; }
        public decimal Importo { get; set; }
        public int PuntiDecurtati { get; set; }
    }
}
