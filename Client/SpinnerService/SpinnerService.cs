namespace EventBooking.Client.SpinnerService
{
    public class SpinnerService
    {
        public event Action? OnShow;
        public event Action? OnHide;
        public bool IsEnabled { get; private set; } = false;

        // Call this when Blazor is ready
        public void Enable()
        {
            IsEnabled = true;
        }

        public void Show()
        {
            if (!IsEnabled) return;
            OnShow?.Invoke();
        }

        public void Hide()
        {
            if (!IsEnabled) return;
            OnHide?.Invoke();
        }
    }
}
