using DevExpress.Mvvm.CodeGenerators;
using OrdiniWeb_WPF.Services;

namespace OrdiniWeb_WPF.ViewModels
{
    [GenerateViewModel]
    public partial class MainViewModel
    {
        public string UtenteCorrente => $"Utente: {UserSession.CurrentUser?.Nome ?? "Non loggato"}";
    }
}