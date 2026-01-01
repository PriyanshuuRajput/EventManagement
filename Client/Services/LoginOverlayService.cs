namespace EventBooking.Client.Services
{
    public class LoginOverlayService
    {
        public event Action? OnShowLogin;
        public event Action? OnShowManagerSignup;
        public string? ReturnUrl { get; private set;  }

        public void ShowLogin(string? returnUrl = null) {
            ReturnUrl = returnUrl;
            OnShowLogin?.Invoke(); 
        }
        public void ClearReturnUrl()
        {
            ReturnUrl = null;
        }
        public void ShowManagerSignup() => OnShowManagerSignup?.Invoke();
    }

}
