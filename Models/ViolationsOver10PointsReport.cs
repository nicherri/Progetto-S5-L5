namespace ServerPoliziaApp.Models
{
    public class ViolationsOver10PointsReport
    {
        public decimal Importo { get; set; }
        public string Cognome { get; set; }
        public string Nome { get; set; }
        public DateTime DataViolazione { get; set; }
        public int PuntiDecurtati { get; set; }
    }
}
