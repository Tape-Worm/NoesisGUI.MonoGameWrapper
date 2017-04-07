namespace NoesisGUI.MonoGameWrapper.Helpers
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using Microsoft.Xna.Framework;

    /// <summary>
    /// Helper method during development
    /// </summary>
    public static class Development
    {
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        /// <summary>
        /// If you have a multi-monitor system, places the console window on the leftmost monitor, and the game window on the rightmost monitor.
        /// </summary>
        public static void PlaceWindowsOnOuterMonitors(GameWindow window, float consoleWidthScale = 0.5f, float consoleHeightScale = 1.0f)
        {
            Screen[] allScreens = Screen.AllScreens;
            if (allScreens.Length > 1)
            {
                var consoleWindow = GetConsoleWindow();
                if (consoleWindow != IntPtr.Zero)
                {
                    var debugScreen = allScreens.OrderBy(s => s.Bounds.Left).FirstOrDefault();
                    if (debugScreen != null)
                    {
                        var bounds = debugScreen.WorkingArea;
                        SetWindowPos(consoleWindow, 0, bounds.Left, bounds.Top, (int)(bounds.Width * consoleWidthScale), (int)(bounds.Height * consoleHeightScale), 0);
                    }

                    var gameScreen = allScreens.OrderByDescending(s => s.Bounds.Left).FirstOrDefault();
                    if (gameScreen != null)
                    {
                        var bounds = gameScreen.WorkingArea;
                        var windowWidth = window.ClientBounds.Width;
                        var windowHeight = window.ClientBounds.Height;

                        // Don't resize game window, center it.
                        SetWindowPos(window.Handle, 0, bounds.Left + (bounds.Width - windowWidth) / 2, bounds.Top + (bounds.Height - windowHeight) / 2, 0, 0, 1);
                    }
                }
            }
        }
    }
}