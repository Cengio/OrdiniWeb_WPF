using System.Windows.Controls;
using System.Windows;
using OrdiniWeb_WPF.ViewModels;
using OrdiniWeb_WPF.Properties;

namespace OrdiniWeb_WPF.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();

            DataContext ??= new LoginViewModel();

            var viewModel = DataContext as LoginViewModel;
            if (viewModel != null && Settings.Default.RicordaUtente)
            {
                viewModel.NomeUtente = Settings.Default.UltimoUtente;
                viewModel.RicordaUtente = true;
            }
        }
    }
}
