using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data.SqlClient;
using ServerPoliziaApp.Models;

namespace ServerPoliziaApp.Controllers
{
    public class TrasgressoriController : Controller
    {
        private readonly string _connectionString;

        public TrasgressoriController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Index()
        {
            var trasgressori = new List<Trasgressore>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Trasgressori";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        trasgressori.Add(new Trasgressore
                        {
                            Id = (int)reader["Id"],
                            Nome = (string)reader["Nome"],
                            Cognome = (string)reader["Cognome"],
                            Indirizzo = (string)reader["Indirizzo"],
                            Città = (string)reader["Città"],
                            CodiceFiscale = (string)reader["CodiceFiscale"]
                        });
                    }
                }
            }
            return View(trasgressori);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Trasgressore trasgressore)
        {
            if (!ModelState.IsValid)
            {
                return View(trasgressore);
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "INSERT INTO Trasgressori (Nome, Cognome, Indirizzo, Città, CodiceFiscale) VALUES (@Nome, @Cognome, @Indirizzo, @Città, @CodiceFiscale)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Nome", trasgressore.Nome);
                cmd.Parameters.AddWithValue("@Cognome", trasgressore.Cognome);
                cmd.Parameters.AddWithValue("@Indirizzo", trasgressore.Indirizzo);
                cmd.Parameters.AddWithValue("@Città", trasgressore.Città);
                cmd.Parameters.AddWithValue("@CodiceFiscale", trasgressore.CodiceFiscale);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
