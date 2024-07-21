using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data.SqlClient;
using ServerPoliziaApp.Models;

namespace ServerPoliziaApp.Controllers
{
    public class ViolazioniController : Controller
    {
        private readonly string _connectionString;

        public ViolazioniController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Index()
        {
            var violazioni = new List<Violazione>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Violazioni";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        violazioni.Add(new Violazione
                        {
                            Id = (int)reader["Id"],
                            Descrizione = (string)reader["Descrizione"],
                            ImportoMinimo = (decimal)reader["ImportoMinimo"],
                            PuntiDecurtati = (int)reader["PuntiDecurtati"]
                        });
                    }
                }
            }
            return View(violazioni);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Violazione violazione)
        {
            if (!ModelState.IsValid)
            {
                return View(violazione);
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "INSERT INTO Violazioni (Descrizione, ImportoMinimo, PuntiDecurtati) VALUES (@Descrizione, @ImportoMinimo, @PuntiDecurtati)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Descrizione", violazione.Descrizione);
                cmd.Parameters.AddWithValue("@ImportoMinimo", violazione.ImportoMinimo);
                cmd.Parameters.AddWithValue("@PuntiDecurtati", violazione.PuntiDecurtati);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
