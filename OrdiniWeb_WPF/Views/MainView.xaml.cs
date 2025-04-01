using System.Windows.Controls;
using System.Windows;
using OrdiniWeb_WPF.Views.Pages;

namespace OrdiniWeb_WPF.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void BtnClienti_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ClientiPage());
        }
    }
}