using DevExpress.Xpf.Core;
using OrdiniWeb_WPF.Services;
using OrdiniWeb_WPF.Views;

namespace OrdiniWeb_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            if (!UserSession.IsLoggedIn)
            {
                Content = new LoginView();
            }
            else
            {
                Content = new MainView();
            }
        }
    }
}
