using System.Runtime.InteropServices;

namespace GUI.Services;

/// <inheritdoc/>
public class ConsoleService : IConsoleService
{
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AllocConsole();

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

    [DllImport("user32.dll")]
    private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);


    private const int SW_HIDE = 0;
    private const int SW_SHOW = 5;

    private const uint SC_CLOSE = 0xF060;
    private const uint MF_GRAYED = 0x0001;

    private bool _isInitialized;
    private bool _isVisible;

    public ConsoleService()
    {
        if (!_isInitialized)
        {
            AllocConsole();
            _isInitialized = true;
            _isVisible = false; // Console starts as visible
            SetVisable(_isVisible);
        }
    }

    public void ToggleConsoleVisable()
    {
        // Ensure the console is allocated once for the lifetime of the app
        if (!_isInitialized)
        {
            AllocConsole();
            _isInitialized = true;
        }

        _isVisible = !_isVisible; // Toggle visibility
        SetVisable(_isVisible);
    }

    private void SetVisable(bool visible)
    {
        IntPtr handle = GetConsoleWindow();
        if (handle != IntPtr.Zero)
        {
            ShowWindow(handle, visible ? SW_SHOW : SW_HIDE);

            // Disable the close button on the console window. 
            // Closing the console will cause the entire application to exit, so force the user to toggle the console via the GUI.
            IntPtr hMenu = GetSystemMenu(handle, false);
            if (hMenu != IntPtr.Zero)
            {
                EnableMenuItem(hMenu, SC_CLOSE, MF_GRAYED);
            }
        }
    }
}
