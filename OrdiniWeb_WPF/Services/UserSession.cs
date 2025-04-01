using OrdiniWeb_WPF.Models;

namespace OrdiniWeb_WPF.Services
{
    public static class UserSession
    {
        public static User CurrentUser { get; set; }

        public static bool IsLoggedIn => CurrentUser != null;
    }
}
