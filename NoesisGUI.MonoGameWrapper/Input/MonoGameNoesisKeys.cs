namespace NoesisGUI.MonoGameWrapper.Input
{
    using System.Collections.Generic;

    using Microsoft.Xna.Framework.Input;

    internal static class MonoGameNoesisKeys
    {
        #region Static Fields

        private static readonly Dictionary<Keys, Noesis.Key> NoesisKeys =
            new Dictionary<Keys, Noesis.Key>
            {
                { Keys.Back, Noesis.Key.Back },
                { Keys.Tab, Noesis.Key.Tab },
                { Keys.OemClear, Noesis.Key.Clear },
                { Keys.Enter, Noesis.Key.Return },
                { Keys.Pause, Noesis.Key.Pause },
                { Keys.Escape, Noesis.Key.Escape },
                { Keys.Space, Noesis.Key.Space },
                { Keys.PageUp, Noesis.Key.Prior },
                { Keys.PageDown, Noesis.Key.Next },
                { Keys.End, Noesis.Key.End },
                { Keys.Home, Noesis.Key.Home },
                { Keys.Left, Noesis.Key.Left },
                { Keys.Up, Noesis.Key.Up },
                { Keys.Right, Noesis.Key.Right },
                { Keys.Down, Noesis.Key.Down },
                { Keys.Select, Noesis.Key.Select },
                { Keys.Print, Noesis.Key.Print },
                { Keys.Execute, Noesis.Key.Execute },
                { Keys.PrintScreen, Noesis.Key.Print },
                { Keys.Insert, Noesis.Key.Insert },
                { Keys.Delete, Noesis.Key.Delete },
                { Keys.Help, Noesis.Key.Help },
                { Keys.D0, Noesis.Key.D0 },
                { Keys.D1, Noesis.Key.D1 },
                { Keys.D2, Noesis.Key.D2 },
                { Keys.D3, Noesis.Key.D3 },
                { Keys.D4, Noesis.Key.D4 },
                { Keys.D5, Noesis.Key.D5 },
                { Keys.D6, Noesis.Key.D6 },
                { Keys.D7, Noesis.Key.D7 },
                { Keys.D8, Noesis.Key.D8 },
                { Keys.D9, Noesis.Key.D9 },
                { Keys.NumPad0, Noesis.Key.NumPad0 },
                { Keys.NumPad1, Noesis.Key.NumPad1 },
                { Keys.NumPad2, Noesis.Key.NumPad2 },
                { Keys.NumPad3, Noesis.Key.NumPad3 },
                { Keys.NumPad4, Noesis.Key.NumPad4 },
                { Keys.NumPad5, Noesis.Key.NumPad5 },
                { Keys.NumPad6, Noesis.Key.NumPad6 },
                { Keys.NumPad7, Noesis.Key.NumPad7 },
                { Keys.NumPad8, Noesis.Key.NumPad8 },
                { Keys.NumPad9, Noesis.Key.NumPad9 },
                { Keys.Multiply, Noesis.Key.Multiply },
                { Keys.OemPlus, Noesis.Key.Add },
                { Keys.Separator, Noesis.Key.Separator },
                { Keys.OemMinus, Noesis.Key.Subtract },
                { Keys.OemPeriod, Noesis.Key.Decimal },
                { Keys.Divide, Noesis.Key.Divide },
                { Keys.A, Noesis.Key.A },
                { Keys.B, Noesis.Key.B },
                { Keys.C, Noesis.Key.C },
                { Keys.D, Noesis.Key.D },
                { Keys.E, Noesis.Key.E },
                { Keys.F, Noesis.Key.F },
                { Keys.G, Noesis.Key.G },
                { Keys.H, Noesis.Key.H },
                { Keys.I, Noesis.Key.I },
                { Keys.J, Noesis.Key.J },
                { Keys.K, Noesis.Key.K },
                { Keys.L, Noesis.Key.L },
                { Keys.M, Noesis.Key.M },
                { Keys.N, Noesis.Key.N },
                { Keys.O, Noesis.Key.O },
                { Keys.P, Noesis.Key.P },
                { Keys.Q, Noesis.Key.Q },
                { Keys.R, Noesis.Key.R },
                { Keys.S, Noesis.Key.S },
                { Keys.T, Noesis.Key.T },
                { Keys.U, Noesis.Key.U },
                { Keys.V, Noesis.Key.V },
                { Keys.W, Noesis.Key.W },
                { Keys.X, Noesis.Key.X },
                { Keys.Y, Noesis.Key.Y },
                { Keys.Z, Noesis.Key.Z },
                { Keys.F1, Noesis.Key.F1 },
                { Keys.F2, Noesis.Key.F2 },
                { Keys.F3, Noesis.Key.F3 },
                { Keys.F4, Noesis.Key.F4 },
                { Keys.F5, Noesis.Key.F5 },
                { Keys.F6, Noesis.Key.F6 },
                { Keys.F7, Noesis.Key.F7 },
                { Keys.F8, Noesis.Key.F8 },
                { Keys.F9, Noesis.Key.F9 },
                { Keys.F10, Noesis.Key.F10 },
                { Keys.F11, Noesis.Key.F11 },
                { Keys.F12, Noesis.Key.F12 },
                { Keys.F13, Noesis.Key.F13 },
                { Keys.F14, Noesis.Key.F14 },
                { Keys.F15, Noesis.Key.F15 },
                { Keys.NumLock, Noesis.Key.NumLock },
                { Keys.Scroll, Noesis.Key.Scroll },
                { Keys.LeftShift, Noesis.Key.LeftShift },
                { Keys.RightShift, Noesis.Key.RightShift },
                { Keys.LeftControl, Noesis.Key.LeftCtrl },
                { Keys.RightControl, Noesis.Key.RightCtrl },
                { Keys.LeftAlt, Noesis.Key.LeftAlt },
                { Keys.RightAlt, Noesis.Key.RightAlt }
            };

        #endregion

        #region Public Methods and Operators

        public static Noesis.Key Convert(Keys key)
        {
            Noesis.Key noesisKey;
            return NoesisKeys.TryGetValue(key, out noesisKey) ? noesisKey : Noesis.Key.None;
        }

        #endregion
    }
}