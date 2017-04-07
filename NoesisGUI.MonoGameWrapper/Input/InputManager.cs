namespace NoesisGUI.MonoGameWrapper.Input
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Input.Touch;

    internal class InputManager
    {
        #region Fields

        private readonly Keys[] m_ModifierKeys =
        {
            Keys.LeftShift, Keys.RightShift, Keys.LeftControl, Keys.RightControl, Keys.LeftAlt, Keys.RightAlt,
            Keys.LeftWindows, Keys.RightWindows, Keys.CapsLock
        };

        private readonly List<Keys> m_PressedKeys = new List<Keys>();

        private readonly List<Keys> m_ReleasedKeys = new List<Keys>();

        private readonly Noesis.View m_RootView;

        private Keys[] m_PreviousKeys = new Keys[0];

        private RepeatingKey m_RepeatingKey;

        #endregion

        #region Constructors and Destructors

        public InputManager(Noesis.View rootView)
        {
            m_RootView = rootView;

            // Let Monogame emulate touch when just using the mouse.
            // NOTE: Has some issues related to mouse capture, even in Monogame 3.6
            TouchPanel.EnableMouseTouchPoint = true;
        }

        #endregion

        #region Public Methods and Operators

        public void OnTextInput(TextInputEventArgs args)
        {
            DebugPrintChar("text", args.Character);

            // HACK: This seems to fix a bug: after pressing some keys like backspace or cursor left, 
            // a Noesis textbox would ignore the next incoming char.
            // Passing None to Noesis seems a workaround. 
            // Need to figure out where the bug comes from, could be our code
            m_RootView.KeyDown(Noesis.Key.None);

            var newChar = args.Character;

            // HACK: Space seems to be handled both as a control and textual key by Noesis, so don't pass it as a text char.
            if (newChar != ' ')
            {
                OnChar(newChar);
            }
        }

        public void Update(GameTime gameTime)
        {
            UpdateKeyboard(gameTime);
            UpdateTouch(gameTime);
        }

        #endregion

        #region Methods

        [Conditional("DEBUG")]
        private void DebugPrint(string message)
        {
            //Console.WriteLine(message);
        }

        [Conditional("DEBUG")]
        private void DebugPrintChar(string header, char character)
        {
            DebugPrint($"{header}: 0x{(int)character:X02}");
        }

        [Conditional("DEBUG")]
        private void DebugPrintKey(string header, Noesis.Key key)
        {
            DebugPrint($"{header}: {key} 0x{(int)key:X04}");
        }

        private void OnChar(char character)
        {
            DebugPrintChar("char", character);
            m_RootView.Char(character);
        }

        private void UpdateKeyboard(GameTime gameTime)
        {
            var state = Keyboard.GetState();

            Keys[] currentKeys = state.GetPressedKeys();

            var currentTime = gameTime.TotalGameTime;

            // determine pressed since last update keys
            m_PressedKeys.Clear();

            foreach (var key in currentKeys)
            {
                var modifierKeyIndex = Array.IndexOf(m_ModifierKeys, key);

                if (!m_PreviousKeys.Contains(key))
                {
                    m_PressedKeys.Add(key);
                    m_RepeatingKey = default(RepeatingKey);
                }
                else if (modifierKeyIndex < 0)
                {
                    if (m_RepeatingKey.Key == key)
                    {
                        if (currentTime >= m_RepeatingKey.RepeatTime)
                        {
                            m_PressedKeys.Add(key);
                            m_RepeatingKey = new RepeatingKey(key, currentTime, m_RepeatingKey.RepeatCount);
                        }
                    }
                    else
                    {
                        m_RepeatingKey = new RepeatingKey(key, currentTime, 0);
                    }
                }
            }

            // determine release since last update keys
            if (m_ReleasedKeys.Count > 0)
            {
                m_ReleasedKeys.Clear();
            }

            foreach (var key in m_PreviousKeys)
            {
                if (!currentKeys.Contains(key))
                {
                    m_ReleasedKeys.Add(key);
                }
            }

            // for each pressed key - KeyDown
            foreach (var keyDown in m_PressedKeys)
            {
                var noesisKey = MonoGameNoesisKeys.Convert(keyDown);
                if (noesisKey != Noesis.Key.None)
                {
                    DebugPrintKey("keydown", noesisKey);
                    m_RootView.KeyDown(noesisKey);
                }
            }

            // for each released key - KeyUp
            foreach (var keyUp in m_ReleasedKeys)
            {
                var noesisKey = MonoGameNoesisKeys.Convert(keyUp);
                if (noesisKey != Noesis.Key.None)
                {
                    DebugPrintKey("keyup", noesisKey);
                    m_RootView.KeyUp(noesisKey);
                }
            }

            m_PreviousKeys = currentKeys;
        }

        private void UpdateTouch(GameTime gametime)
        {
            var touchState = TouchPanel.GetState();

            if (touchState.Count > 0)
            {
                for (var i = 0; i < touchState.Count; i++)
                {
                    var touchLocation = touchState[i];

                    var x = (int)touchLocation.Position.X;
                    var y = (int)touchLocation.Position.Y;
                    var id = (uint)touchLocation.Id;

                    switch (touchLocation.State)
                    {
                        case TouchLocationState.Invalid: break;
                        case TouchLocationState.Moved:
                            m_RootView.TouchMove(x, y, id);
                            break;
                        case TouchLocationState.Pressed:
                            m_RootView.TouchDown(x, y, id);
                            break;
                        case TouchLocationState.Released:
                            m_RootView.TouchUp(x, y, id);
                            break;
                        default: throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        #endregion

        private struct RepeatingKey
        {
            #region Fields

            public readonly Keys Key;

            public readonly int RepeatCount;

            public readonly TimeSpan RepeatTime;

            #endregion

            #region Constructors and Destructors

            public RepeatingKey(Keys key, TimeSpan currentTime, int currentCount)
            {
                Key = key;
                RepeatTime = currentTime + TimeSpan.FromMilliseconds(currentCount == 0 ? 200 : 20);
                RepeatCount = currentCount + 1;
            }

            #endregion
        }
    }
}