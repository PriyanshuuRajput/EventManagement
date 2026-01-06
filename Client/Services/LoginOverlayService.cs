namespace EventBooking.Client.Services
{
    public class LoginOverlayService
    {
        public event Action? OnShowLogin;
        public event Action? OnShowManagerSignup;
        public event Action? OnShowForgetPassword;
        public string? ReturnUrl { get; private set;  }

        public void ShowLogin(string? returnUrl = null) {
            ReturnUrl = returnUrl;
            OnShowLogin?.Invoke(); 
        }
        public void ClearReturnUrl()
        {
            ReturnUrl = null;
        }
        public void ForgetPassowrd()
        {
            OnShowForgetPassword?.Invoke();
        }
        public void ShowManagerSignup() => OnShowManagerSignup?.Invoke();
    }

}
