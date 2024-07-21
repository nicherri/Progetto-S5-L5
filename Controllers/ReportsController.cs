using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data.SqlClient;
using ServerPoliziaApp.Models;

namespace ServerPoliziaApp.Controllers
{
    public class ReportsController : Controller
    {
        private readonly string _connectionString;

        public ReportsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult TotaleVerbaliPerTrasgressore()
        {
            var report = new List<TotalVerbalsPerTransgressorReport>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT Cognome, Nome, COUNT(*) AS TotaleVerbali FROM Trasgressori t JOIN Verbali v ON t.Id = v.TrasgressoreId GROUP BY Cognome, Nome";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        report.Add(new TotalVerbalsPerTransgressorReport
                        {
                            Cognome = (string)reader["Cognome"],
                            Nome = (string)reader["Nome"],
                            TotaleVerbali = (int)reader["TotaleVerbali"]
                        });
                    }
                }
            }
            return View(report);
        }

        public IActionResult TotalePuntiDecurtatiPerTrasgressore()
        {
            var report = new List<TotalPointsPerTransgressorReport>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT Cognome, Nome, SUM(PuntiDecurtati) AS TotalePunti FROM Trasgressori t JOIN Verbali v ON t.Id = v.TrasgressoreId GROUP BY Cognome, Nome";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        report.Add(new TotalPointsPerTransgressorReport
                        {
                            Cognome = (string)reader["Cognome"],
                            Nome = (string)reader["Nome"],
                            TotalePunti = (int)reader["TotalePunti"]
                        });
                    }
                }
            }
            return View(report);
        }

        public IActionResult ViolazioniOltre10Punti()
        {
            var report = new List<ViolationsOver10PointsReport>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT Importo, Cognome, Nome, DataViolazione, PuntiDecurtati FROM Trasgressori t JOIN Verbali v ON t.Id = v.TrasgressoreId WHERE PuntiDecurtati > 10";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        report.Add(new ViolationsOver10PointsReport
                        {
                            Importo = (decimal)reader["Importo"],
                            Cognome = (string)reader["Cognome"],
                            Nome = (string)reader["Nome"],
                            DataViolazione = (DateTime)reader["DataViolazione"],
                            PuntiDecurtati = (int)reader["PuntiDecurtati"]
                        });
                    }
                }
            }
            return View(report);
        }

        public IActionResult ViolazioniOltre400Euro()
        {
            var report = new List<ViolationsOver400EuroReport>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT Descrizione, ImportoMinimo FROM Violazioni WHERE ImportoMinimo > 400";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        report.Add(new ViolationsOver400EuroReport
                        {
                            Descrizione = (string)reader["Descrizione"],
                            ImportoMinimo = (decimal)reader["ImportoMinimo"]
                        });
                    }
                }
            }
            return View(report);
        }
    }
}
