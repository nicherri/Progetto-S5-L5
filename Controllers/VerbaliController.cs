using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ServerPoliziaApp.Models;

namespace ServerPoliziaApp.Controllers
{
    public class VerbaliController : Controller
    {
        private readonly string _connectionString;

        public VerbaliController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Index()
        {
            var verbali = new List<Verbale>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Verbali";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        verbali.Add(new Verbale
                        {
                            NumeroVerbale = (string)reader["NumeroVerbale"],
                            TrasgressoreId = (int)reader["TrasgressoreId"],
                            ViolazioneId = (int)reader["ViolazioneId"],
                            DataViolazione = (DateTime)reader["DataViolazione"],
                            Importo = (decimal)reader["Importo"],
                            PuntiDecurtati = (int)reader["PuntiDecurtati"]
                        });
                    }
                }
            }
            return View(verbali);
        }

        public IActionResult Create()
        {
            ViewBag.Trasgressori = GetTrasgressori();
            ViewBag.Violazioni = GetViolazioni();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Verbale verbale)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Trasgressori = GetTrasgressori();
                ViewBag.Violazioni = GetViolazioni();
                return View(verbale);
            }

            verbale.NumeroVerbale = GenerateNumeroVerbale();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "INSERT INTO Verbali (NumeroVerbale, TrasgressoreId, ViolazioneId, DataViolazione, Importo, PuntiDecurtati) VALUES (@NumeroVerbale, @TrasgressoreId, @ViolazioneId, @DataViolazione, @Importo, @PuntiDecurtati)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@NumeroVerbale", verbale.NumeroVerbale);
                cmd.Parameters.AddWithValue("@TrasgressoreId", verbale.TrasgressoreId);
                cmd.Parameters.AddWithValue("@ViolazioneId", verbale.ViolazioneId);
                cmd.Parameters.AddWithValue("@DataViolazione", verbale.DataViolazione.ToString("yyyy-MM-ddTHH:mm:ss"));
                cmd.Parameters.AddWithValue("@Importo", verbale.Importo);
                cmd.Parameters.AddWithValue("@PuntiDecurtati", verbale.PuntiDecurtati);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction(nameof(Index));
        }

        private string GenerateNumeroVerbale()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new char[10];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = chars[random.Next(chars.Length)];
            }
            return new string(result);
        }

        private List<Trasgressore> GetTrasgressori()
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
            return trasgressori;
        }

        private List<Violazione> GetViolazioni()
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
            return violazioni;
        }
    }
}
