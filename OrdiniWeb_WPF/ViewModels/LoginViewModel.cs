using DevExpress.Mvvm.CodeGenerators;
using OrdiniWeb_WPF.Models;
using OrdiniWeb_WPF.Services;
using System;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows;

namespace OrdiniWeb_WPF.ViewModels
{
    [GenerateViewModel]
    public partial class LoginViewModel
    {
        // Le proprietà vengono generate automaticamente come public NomeUtente { get; set; }
        [GenerateProperty]
        private string _NomeUtente = string.Empty;

        [GenerateProperty]
        private string _Password = string.Empty;

        [GenerateProperty]
        private bool _RicordaUtente;

        [GenerateProperty]
        private string _MessaggioErrore;

        // Usa GenerateCommand per generare automaticamente LoginCommand
        [GenerateCommand(CanExecuteMethod = nameof(CanLogin))]
        public void Login()
        {
            MessaggioErrore = string.Empty;

            if (string.IsNullOrWhiteSpace(NomeUtente))
            {
                MessaggioErrore = "Inserisci il nome utente.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                MessaggioErrore = "Inserisci la password.";
                return;
            }

            using SqlConnection connInterop = new SqlConnection(ConfigurationManager.ConnectionStrings["Conn_Interoperabilita"].ConnectionString);
            try
            {
                connInterop.Open();
            }
            catch (Exception ex)
            {
                MessaggioErrore = $"Errore di connessione: {ex.Message}";
                return;
            }

            long idUtente = 0;
            int livello = 0;
            string? userCode = null;
            string? userID = null;
            bool passwordCriptata = false;

            // Verifica se l'utente esiste
            using (SqlCommand cmd = new SqlCommand("SELECT ID, pwdCriptata, UtenteOW FROM LoginUtentiProgramma WHERE Nome = @NomeUtente", connInterop))
            {
                cmd.Parameters.AddWithValue("@NomeUtente", NomeUtente);
                using SqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    MessaggioErrore = "Utente non trovato.";
                    return;
                }

                idUtente = Convert.ToInt64(reader["ID"]);
                passwordCriptata = Convert.ToBoolean(reader["pwdCriptata"]);
                userCode = reader["UtenteOW"] != DBNull.Value ? reader["UtenteOW"].ToString() : null;
            }

            // Cripta la password se necessario
            string passwordDaVerificare = Password.Trim();
            if (passwordCriptata)
            {
                // Per il momento non gestiamo la crittografia
                // var ccrypta = new drGiorginiLib.Ccrypta();
                // passwordDaVerificare = ccrypta.Encrypt("chiave", passwordDaVerificare);
            }

            // Verifica credenziali tramite stored procedure
            using (SqlCommand cmd = new SqlCommand("loginUtentiNew", connInterop))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Parameter1", NomeUtente.Trim());
                cmd.Parameters.AddWithValue("@Parameter2", passwordDaVerificare);
                cmd.Parameters.AddWithValue("@Parameter3", "%CRMUO%");

                using SqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    MessaggioErrore = "Credenziali non valide.";
                    return;
                }

                livello = Convert.ToInt32(reader["Livello"]);
                idUtente = Convert.ToInt64(reader["ID"]);
                userCode = reader["UtenteOW"]?.ToString();
            }

            if (string.IsNullOrEmpty(userCode))
            {
                MessaggioErrore = "UserCode mancante. Contattare l'amministratore.";
                return;
            }

            // Recupera UserID dal DB OrdiniWeb
            using SqlConnection connOrdiniWeb = new SqlConnection(ConfigurationManager.ConnectionStrings["Conn_OrdiniWeb"].ConnectionString);
            connOrdiniWeb.Open();
            using SqlCommand cmdUser = new SqlCommand("SELECT [User ID] FROM WU_users WHERE [User Code] = @UserCode", connOrdiniWeb);
            cmdUser.Parameters.AddWithValue("@UserCode", userCode);

            using SqlDataReader rdrUser = cmdUser.ExecuteReader();
            if (rdrUser.Read())
            {
                userID = rdrUser["User ID"].ToString();
            }
            else
            {
                MessaggioErrore = $"UserID non trovato per UserCode: {userCode}";
                return;
            }

            // Salva l'utente nella sessione
            UserSession.CurrentUser = new User
            {
                ID = idUtente,
                Nome = NomeUtente,
                Livello = livello,
                UserCode = userCode,
                UserID = userID
            };

            Application.Current.MainWindow.Content = new Views.MainView();
            Application.Current.MainWindow.Width = 1200;
            Application.Current.MainWindow.Height = 800;
            Application.Current.MainWindow.Content = new Views.MainView();

        }

        public bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(NomeUtente) && !string.IsNullOrWhiteSpace(Password);
        }
    }
}