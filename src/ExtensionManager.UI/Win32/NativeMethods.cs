using System;
using System.Windows;
using System.Windows.Interop;

namespace ExtensionManager.UI.Win32;

internal static class NativeMethods
{
    /// <summary>
    /// Associates a new large or small icon with a window. The system displays the large icon in the ALT+TAB dialog box, and the small icon in the window caption.
    /// </summary>
    private const int WM_SETICON = 0x0080;

    // from winuser.h
    private const int GWL_STYLE = -16;
    private const int WS_DLGFRAME = 0x00400000;
    private const int WS_MAXIMIZEBOX = 0x10000;
    private const int WS_MINIMIZEBOX = 0x20000;

    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_DLGMODALFRAME = 0x0001;
    private const int SWP_NOSIZE = 0x0001;
    private const int SWP_NOMOVE = 0x0002;
    private const int SWP_NOZORDER = 0x0004;
    private const int SWP_FRAMECHANGED = 0x0020;

    /// <summary>
    /// Changes the border of the window to the style commonly utilized for dialog boxes.
    /// </summary>
    /// <remarks>
    /// thanks stack overflow <https://stackoverflow.com/questions/339620/how-do-i-remove-minimize-and-maximize-from-a-resizable-window-in-wpf>
    /// </remarks>
    public static void StyleWindowAsDialogBox(this Window window)
    {
        if (window is null)
            return;

        var hwnd = new WindowInteropHelper(window).Handle;

        if (!User32.IsWindow(hwnd))
            return;

        SetDialogWindowFrame(hwnd);
        HideMaximizeButton(hwnd);
        HideMinimizeButton(hwnd);
        RemoveIcon(hwnd);
    }

    /// <summary>
    /// Removes the Maximize button from a <see cref="T:System.Windows.Window"/>'s title bar.
    /// </summary>
    /// <remarks>
    /// thanks stack overflow <https://stackoverflow.com/questions/339620/how-do-i-remove-minimize-and-maximize-from-a-resizable-window-in-wpf>
    /// </remarks>
    private static void HideMaximizeButton(IntPtr hwnd)
    {
        var currentStyle = User32.GetWindowLong(hwnd, GWL_STYLE);

        User32.SetWindowLong(hwnd, GWL_STYLE, currentStyle & ~WS_MAXIMIZEBOX);
    }

    /// <summary>
    /// Removes the Maximize button from a <see cref="T:System.Windows.Window"/>'s title bar.
    /// </summary>
    /// <remarks>
    /// thanks stack overflow <https://stackoverflow.com/questions/339620/how-do-i-remove-minimize-and-maximize-from-a-resizable-window-in-wpf>
    /// </remarks>
    private static void HideMinimizeButton(IntPtr hwnd)
    {
        var currentStyle = User32.GetWindowLong(hwnd, GWL_STYLE);

        User32.SetWindowLong(hwnd, GWL_STYLE, currentStyle & ~WS_MINIMIZEBOX);
    }

    /// <summary>
    /// Removes the icon from the title bar of the <see cref="T:System.Windows.Window"/> referred to by the <paramref name="window"/> parameter.
    /// </summary>
    /// <param name="window">Reference to an instance of a <see cref="T:System.Windows.Window"/> from which the icon is to be removed.</param>
    /// <remarks>Thank you <a href="https://stackoverflow.com/questions/18580430/hide-the-icon-from-a-wpf-window">Stack Overflow.</a></remarks>
    private static void RemoveIcon(IntPtr hwnd)
    {
        // Change the extended window style to not show a window icon
        var extendedStyle = User32.GetWindowLong(hwnd, GWL_EXSTYLE);

        User32.SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_DLGMODALFRAME);

        User32.SendMessage(hwnd, WM_SETICON, 1, IntPtr.Zero);
        User32.SendMessage(hwnd, WM_SETICON, 0, IntPtr.Zero);

        // Update the window's non-client area to reflect the changes
        User32.SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
    }

    /// <summary>
    /// Changes the border of the window to the style commonly utilized for dialog boxes.
    /// </summary>
    /// <remarks>
    /// thanks stack overflow <https://stackoverflow.com/questions/339620/how-do-i-remove-minimize-and-maximize-from-a-resizable-window-in-wpf>
    /// </remarks>
    private static void SetDialogWindowFrame(IntPtr hwnd)
    {
        var currentStyle = User32.GetWindowLong(hwnd, GWL_STYLE);

        User32.SetWindowLong(hwnd, GWL_STYLE, currentStyle | WS_DLGFRAME);
    }
}
