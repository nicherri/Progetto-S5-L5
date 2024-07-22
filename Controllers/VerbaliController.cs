using Microsoft.AspNetCore.Mvc;
using ServerPoliziaApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

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
            var verbali = new List<VerbaleViewModel>();

            string query = @"
                SELECT 
                    Verbali.NumeroVerbale, 
                    Trasgressori.Nome AS NomeTrasgressore, 
                    Trasgressori.Cognome AS CognomeTrasgressore, 
                    Violazioni.Descrizione AS DescrizioneViolazione, 
                    Verbali.DataViolazione, 
                    Verbali.Importo, 
                    Verbali.PuntiDecurtati 
                FROM Verbali
                JOIN Trasgressori ON Verbali.TrasgressoreId = Trasgressori.Id
                JOIN Violazioni ON Verbali.ViolazioneId = Violazioni.Id";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        verbali.Add(new VerbaleViewModel
                        {
                            NumeroVerbale = reader.GetString(0),
                            NomeTrasgressore = reader.GetString(1),
                            CognomeTrasgressore = reader.GetString(2),
                            DescrizioneViolazione = reader.GetString(3),
                            DataViolazione = reader.GetDateTime(4),
                            Importo = reader.GetDecimal(5),
                            PuntiDecurtati = reader.GetInt32(6)
                        });
                    }
                }
                conn.Close();
            }

            return View(verbali);
        }

        public IActionResult Create()
        {
            ViewBag.Violazioni = GetViolazioni();
            ViewBag.Trasgressori = GetTrasgressori();
            return View(new Verbale { NumeroVerbale = GenerateUniqueVerbaleNumber() });
        }

        [HttpPost]
        public IActionResult Create(Verbale verbale)
        {
            Console.WriteLine("Inizio metodo Create [HttpPost]");
            Console.WriteLine($"NumeroVerbale: {verbale.NumeroVerbale}");
            Console.WriteLine($"TrasgressoreNome: {verbale.TrasgressoreNome}");
            Console.WriteLine($"TrasgressoreCognome: {verbale.TrasgressoreCognome}");
            Console.WriteLine($"ViolazioneId: {verbale.ViolazioneId}");
            Console.WriteLine($"DataViolazione: {verbale.DataViolazione}");
            Console.WriteLine($"Importo: {verbale.Importo}");
            Console.WriteLine($"PuntiDecurtati: {verbale.PuntiDecurtati}");

            if (ModelState.IsValid)
            {
                verbale.TrasgressoreId = GetTrasgressoreId(verbale.TrasgressoreNome, verbale.TrasgressoreCognome);
                Console.WriteLine($"TrasgressoreId dopo GetTrasgressoreId: {verbale.TrasgressoreId}");
                if (verbale.TrasgressoreId == 0)
                {
                    ModelState.AddModelError("", "Trasgressore non trovato.");
                    ViewBag.Violazioni = GetViolazioni();
                    ViewBag.Trasgressori = GetTrasgressori();
                    return View(verbale);
                }

                string query = "INSERT INTO Verbali (NumeroVerbale, TrasgressoreId, ViolazioneId, DataViolazione, Importo, PuntiDecurtati) VALUES (@NumeroVerbale, @TrasgressoreId, @ViolazioneId, @DataViolazione, @Importo, @PuntiDecurtati)";
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand command = new SqlCommand(query, conn);
                    command.Parameters.AddWithValue("@NumeroVerbale", verbale.NumeroVerbale);
                    command.Parameters.AddWithValue("@TrasgressoreId", verbale.TrasgressoreId);
                    command.Parameters.AddWithValue("@ViolazioneId", verbale.ViolazioneId);
                    command.Parameters.AddWithValue("@DataViolazione", verbale.DataViolazione);
                    command.Parameters.AddWithValue("@Importo", verbale.Importo);
                    command.Parameters.AddWithValue("@PuntiDecurtati", verbale.PuntiDecurtati);

                    Console.WriteLine($"Query: {command.CommandText}");
                    Console.WriteLine($"NumeroVerbale: {verbale.NumeroVerbale}");
                    Console.WriteLine($"TrasgressoreId: {verbale.TrasgressoreId}");
                    Console.WriteLine($"ViolazioneId: {verbale.ViolazioneId}");
                    Console.WriteLine($"DataViolazione: {verbale.DataViolazione}");
                    Console.WriteLine($"Importo: {verbale.Importo}");
                    Console.WriteLine($"PuntiDecurtati: {verbale.PuntiDecurtati}");

                    try
                    {
                        conn.Open();
                        command.ExecuteNonQuery();
                        Console.WriteLine("Verbale inserito con successo.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Errore durante l'inserimento del verbale: {ex.Message}");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }

                return RedirectToAction("Index");
            }

            ViewBag.Violazioni = GetViolazioni();
            ViewBag.Trasgressori = GetTrasgressori();
            return View(verbale);
        }

        private int GetTrasgressoreId(string nome, string cognome)
        {
            int trasgressoreId = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT Id FROM Trasgressori WHERE Nome = @Nome AND Cognome = @Cognome";
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@Nome", nome);
                command.Parameters.AddWithValue("@Cognome", cognome);
                conn.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        trasgressoreId = reader.GetInt32(0);
                    }
                }
                conn.Close();
            }
            return trasgressoreId;
        }

        private List<Violazione> GetViolazioni()
        {
            var violazioni = new List<Violazione>();

            string query = "SELECT Id, Descrizione FROM Violazioni";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        violazioni.Add(new Violazione
                        {
                            Id = reader.GetInt32(0),
                            Descrizione = reader.GetString(1)
                        });
                    }
                }
                conn.Close();
            }

            return violazioni;
        }

        private List<Trasgressore> GetTrasgressori()
        {
            var trasgressori = new List<Trasgressore>();

            string query = "SELECT Id, Nome, Cognome FROM Trasgressori";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        trasgressori.Add(new Trasgressore
                        {
                            Id = reader.GetInt32(0),
                            Nome = reader.GetString(1),
                            Cognome = reader.GetString(2)
                        });
                    }
                }
                conn.Close();
            }

            return trasgressori;
        }

        private string GenerateUniqueVerbaleNumber()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 10)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
