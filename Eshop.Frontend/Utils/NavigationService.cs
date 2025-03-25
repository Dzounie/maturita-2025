using Microsoft.AspNetCore.Components;

namespace Eshop.Frontend.Components
{
    public class NavigationService
    {
        private readonly NavigationManager _navigationManager;

        public NavigationService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public void NavigateToHome()
        {
            _navigationManager.NavigateTo("/");
        }
        public void NavigateToLogin()
        {
            _navigationManager.NavigateTo("/login");
        }

        public void NavigateToAdminPanel()
        {
            _navigationManager.NavigateTo("/admin-panel");
        }

        public void NavigateTo(string url)
        {
            _navigationManager.NavigateTo(url);
        }
    }
}