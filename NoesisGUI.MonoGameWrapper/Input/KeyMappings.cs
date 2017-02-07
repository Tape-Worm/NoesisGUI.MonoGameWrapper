namespace NoesisGUI.MonoGameWrapper.Input
{
    #region

    using System.Collections.Generic;

    using Microsoft.Xna.Framework.Input;

    using Noesis;

    #endregion

    internal static class MonoGameNoesisKeys
    {
        #region Static Fields

        private static readonly Dictionary<Keys, Key> noesisKeys =
            new Dictionary<Keys, Key>
        {
            { Keys.Back, Key.Back },
            { Keys.Tab, Key.Tab },
            { Keys.OemClear, Key.Clear },
            { Keys.Enter, Key.Return },
            { Keys.Pause, Key.Pause },
            { Keys.Escape, Key.Escape },
            { Keys.Space, Key.Space },
            { Keys.PageUp, Key.Prior },
            { Keys.PageDown, Key.Next },
            { Keys.End, Key.End },
            { Keys.Home, Key.Home },
            { Keys.Left, Key.Left },
            { Keys.Up, Key.Up },
            { Keys.Right, Key.Right },
            { Keys.Down, Key.Down },
            { Keys.Select, Key.Select },
            { Keys.Print, Key.Print },
            { Keys.Execute, Key.Execute },
            { Keys.PrintScreen, Key.Print },
            { Keys.Insert, Key.Insert },
            { Keys.Delete, Key.Delete },
            { Keys.Help, Key.Help },
            { Keys.D0, Key.D0 },
            { Keys.D1, Key.D1 },
            { Keys.D2, Key.D2 },
            { Keys.D3, Key.D3 },
            { Keys.D4, Key.D4 },
            { Keys.D5, Key.D5 },
            { Keys.D6, Key.D6 },
            { Keys.D7, Key.D7 },
            { Keys.D8, Key.D8 },
            { Keys.D9, Key.D9 },
            { Keys.NumPad0, Key.NumPad0 },
            { Keys.NumPad1, Key.NumPad1 },
            { Keys.NumPad2, Key.NumPad2 },
            { Keys.NumPad3, Key.NumPad3 },
            { Keys.NumPad4, Key.NumPad4 },
            { Keys.NumPad5, Key.NumPad5 },
            { Keys.NumPad6, Key.NumPad6 },
            { Keys.NumPad7, Key.NumPad7 },
            { Keys.NumPad8, Key.NumPad8 },
            { Keys.NumPad9, Key.NumPad9 },
            { Keys.Multiply, Key.Multiply },
            { Keys.OemPlus, Key.Add },
            { Keys.Separator, Key.Separator },
            { Keys.OemMinus, Key.Subtract },
            { Keys.OemPeriod, Key.Decimal },
            { Keys.Divide, Key.Divide },
            { Keys.A, Key.A },
            { Keys.B, Key.B },
            { Keys.C, Key.C },
            { Keys.D, Key.D },
            { Keys.E, Key.E },
            { Keys.F, Key.F },
            { Keys.G, Key.G },
            { Keys.H, Key.H },
            { Keys.I, Key.I },
            { Keys.J, Key.J },
            { Keys.K, Key.K },
            { Keys.L, Key.L },
            { Keys.M, Key.M },
            { Keys.N, Key.N },
            { Keys.O, Key.O },
            { Keys.P, Key.P },
            { Keys.Q, Key.Q },
            { Keys.R, Key.R },
            { Keys.S, Key.S },
            { Keys.T, Key.T },
            { Keys.U, Key.U },
            { Keys.V, Key.V },
            { Keys.W, Key.W },
            { Keys.X, Key.X },
            { Keys.Y, Key.Y },
            { Keys.Z, Key.Z },
            { Keys.F1, Key.F1 },
            { Keys.F2, Key.F2 },
            { Keys.F3, Key.F3 },
            { Keys.F4, Key.F4 },
            { Keys.F5, Key.F5 },
            { Keys.F6, Key.F6 },
            { Keys.F7, Key.F7 },
            { Keys.F8, Key.F8 },
            { Keys.F9, Key.F9 },
            { Keys.F10, Key.F10 },
            { Keys.F11, Key.F11 },
            { Keys.F12, Key.F12 },
            { Keys.F13, Key.F13 },
            { Keys.F14, Key.F14 },
            { Keys.F15, Key.F15 },
            { Keys.NumLock, Key.NumLock },
            { Keys.Scroll, Key.Scroll },
            { Keys.LeftShift, Key.LeftShift },
            { Keys.RightShift, Key.RightShift },
            { Keys.LeftControl, Key.LeftCtrl },
            { Keys.RightControl, Key.RightCtrl},
            { Keys.LeftAlt, Key.LeftAlt },
            { Keys.RightAlt, Key.RightAlt }
        };

        #endregion

        #region Public Methods and Operators

        public static Key Convert(Keys key)
        {
            Key noesisKey;
            return noesisKeys.TryGetValue(key, out noesisKey) ? noesisKey : Key.None;
        }

        #endregion
    }
}