namespace ServerPoliziaApp.Models
{
    public class Verbale
    {
        public string NumeroVerbale { get; set; }
        public int TrasgressoreId { get; set; }
        public int ViolazioneId { get; set; }
        public DateTime DataViolazione { get; set; }
        public decimal Importo { get; set; }
        public int PuntiDecurtati { get; set; }

        public string TrasgressoreNome { get; set; }
        public string TrasgressoreCognome { get; set; }
    }
}
