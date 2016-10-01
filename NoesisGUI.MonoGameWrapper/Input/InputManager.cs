using System;
using System.Diagnostics;

namespace NoesisGUI.MonoGameWrapper.Input
{
	#region

	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Runtime.InteropServices;

	using Microsoft.Xna.Framework;
	using Microsoft.Xna.Framework.Input;

	using MouseState = Microsoft.Xna.Framework.Input.MouseState;

	#endregion

	internal class InputManager
	{
		#region Fields

		private readonly Keyboard keyboard;

		private readonly Mouse mouse;

		#endregion

		#region Constructors and Destructors

		public InputManager(Noesis.View rootView)
		{
			this.mouse = new Mouse(rootView);
			this.keyboard = new Keyboard(rootView);
		}

		#endregion

		#region Public Methods and Operators

		public void OnTextInput(TextInputEventArgs args)
		{
			// HACK: This seems to fix a bug: after pressing backspace, 
			// a textbox would ignore the first incoming char.
			// Passing a 0 char seems a workaround
			this.keyboard.OnChar('\0');
			this.keyboard.OnChar(args.Character);

			//var keyDown = (Keys)args.Character;
			//if (keyDown != Keys.None)
			//{
			//	var noesisKey = MonoGameNoesisKeys.Convert(keyDown);
			//	if (noesisKey != Key.None)
			//	{
			//		this.keyboard.OnKeyDown(noesisKey);
			//	}
			//}
		}

		public void Update(GameTime gameTime)
		{
			this.keyboard.UpdateKeyboard(gameTime);
			this.mouse.UpdateMouse(gameTime);
		}

		#endregion

		internal class Keyboard
		{
			#region Fields

			[Flags]
			private enum ModifierKeyFlags
			{
				Shift = 1,
				Control = 2,
				Alt = 4,
				Command = 8
			}

			private struct RepeatingKey
			{
				public readonly Keys Key;
				public readonly TimeSpan RepeatTime;
				public readonly int RepeatCount;

				public RepeatingKey(Keys key, TimeSpan currentTime, int currentCount)
				{
					Key = key;
					RepeatTime = currentTime + TimeSpan.FromMilliseconds(currentCount == 0 ? 200 : 20);
					RepeatCount = currentCount + 1;
				}
			}

			private readonly Keys[] modifierKeys =
			{
				Keys.LeftShift, Keys.RightShift,
				Keys.LeftControl, Keys.RightControl,
				Keys.LeftAlt, Keys.RightAlt,
				Keys.LeftWindows, Keys.RightWindows,
				Keys.CapsLock,
			};

			private readonly ModifierKeyFlags[] modifierKeyFlags =
			{
				ModifierKeyFlags.Shift, ModifierKeyFlags.Shift,
				ModifierKeyFlags.Control, ModifierKeyFlags.Control,
				ModifierKeyFlags.Alt, ModifierKeyFlags.Alt,
				ModifierKeyFlags.Command, ModifierKeyFlags.Command,
				ModifierKeyFlags.Shift,
			};

			private readonly List<Keys> pressedKeys = new List<Keys>();
			private readonly List<Keys> releasedKeys = new List<Keys>();
			private readonly List<Keys> repeatedKeys = new List<Keys>();
			private RepeatingKey repeatingKey;

			private readonly Noesis.View rootView;

			private Keys[] previousKeys = new Keys[0];

			[DllImport("user32.dll")]
			static extern uint MapVirtualKey(uint uCode, uint uMapType);

			#endregion

			#region Constructors and Destructors

			public Keyboard(Noesis.View rootView)
			{
				this.rootView = rootView;
			}

			#endregion

			#region Public Methods and Operators

			public void OnChar(char character)
			{
				this.rootView.Char(character);
			}

			public void OnKeyDown(Noesis.Key noesisKey)
			{
				this.rootView.KeyDown(noesisKey);
			}

			public void UpdateKeyboard(GameTime gameTime)
			{
				var state = Microsoft.Xna.Framework.Input.Keyboard.GetState();

				Keys[] currentKeys = state.GetPressedKeys();

				var currentTime = gameTime.TotalGameTime;

				// determine pressed since last update keys
				this.pressedKeys.Clear();
				this.repeatedKeys.Clear();

				ModifierKeyFlags modifierKeysFlags = 0;

				foreach (var key in currentKeys)
				{
					var modifierKeyIndex = Array.IndexOf(this.modifierKeys, key);
					if (modifierKeyIndex >= 0)
						modifierKeysFlags |= this.modifierKeyFlags[modifierKeyIndex];

					if (!this.previousKeys.Contains(key))
					{
						this.pressedKeys.Add(key);
						repeatingKey = default(RepeatingKey);
					}
					else if (modifierKeyIndex < 0)
					{
						if (repeatingKey.Key == key)
						{
							if (currentTime >= repeatingKey.RepeatTime)
							{
								this.repeatedKeys.Add(key);
								repeatingKey = new RepeatingKey(key, currentTime, repeatingKey.RepeatCount);
							}
						}
						else
						{
							repeatingKey = new RepeatingKey(key, currentTime, 0);
						}
					}
				}

				// determine release since last update keys
				if (this.releasedKeys.Count > 0)
				{
					this.releasedKeys.Clear();
				}
				foreach (var key in this.previousKeys)
				{
					if (!currentKeys.Contains(key))
					{
						this.releasedKeys.Add(key);
					}
				}

				if (this.pressedKeys.Count > 0)
				{
					Debug.WriteLine(string.Join(" ", this.pressedKeys));
				}

				// for each pressed key - KeyDown
				foreach (var keyDown in this.pressedKeys.Concat(this.repeatedKeys))
				{
					//// Convert to char
					//var localKey = (char)MapVirtualKey((uint)keyDown, 2);

					//// If any other modifier keys than SHIFT are press, ignore the mapping to char
					//if (localKey != 0 && !char.IsControl(localKey) && (modifierKeysFlags & ~ModifierKeyFlags.Shift) == 0)
					//{
					//	var localChar = modifierKeysFlags.HasFlag(ModifierKeyFlags.Shift)
					//		? char.ToUpper(localKey)
					//		: char.ToLower(localKey);

					//	this.rootView.Char(localChar);
					//}
					//else
					//{
						var noesisKey = MonoGameNoesisKeys.Convert(keyDown);
						if (noesisKey != Noesis.Key.None)
						{
							this.rootView.KeyDown(noesisKey);
						}
					//}
				}

				// for each released key - KeyUp
				foreach (var keyUp in this.releasedKeys)
				{
					var noesisKey = MonoGameNoesisKeys.Convert(keyUp);
					if (noesisKey != Noesis.Key.None)
					{
						this.rootView.KeyUp(noesisKey);
					}
				}

				this.previousKeys = currentKeys;
			}

			#endregion
		}

		internal class Mouse
		{
			#region Fields

			private readonly Noesis.View rootView;

			private int mouseLastX;

			private int mouseLastY;

			private MouseState previousMouseState;

			#endregion

			#region Constructors and Destructors

			public Mouse(Noesis.View rootView)
			{
				this.rootView = rootView;
			}

			#endregion

			#region Public Methods and Operators

			public void UpdateMouse(GameTime gameTime)
			{
				var mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
				var x = mouseState.X;
				var y = mouseState.Y;

				if (this.mouseLastX != x
					|| this.mouseLastY != y)
				{
					this.rootView.MouseMove(x, y);
					this.mouseLastX = x;
					this.mouseLastY = y;
				}

				this.ProcessMouseButtonDown(x, y, mouseState.LeftButton, this.previousMouseState.LeftButton, Noesis.MouseButton.Left);
				this.ProcessMouseButtonDown(x, y, mouseState.RightButton, this.previousMouseState.RightButton, Noesis.MouseButton.Right);
				this.ProcessMouseButtonDown(x, y, mouseState.MiddleButton, this.previousMouseState.MiddleButton, Noesis.MouseButton.Middle);

				this.ProcessMouseButtonUp(x, y, mouseState.LeftButton, this.previousMouseState.LeftButton, Noesis.MouseButton.Left);
				this.ProcessMouseButtonUp(x, y, mouseState.RightButton, this.previousMouseState.RightButton, Noesis.MouseButton.Right);
				this.ProcessMouseButtonUp(x, y, mouseState.MiddleButton, this.previousMouseState.MiddleButton, Noesis.MouseButton.Middle);

				this.previousMouseState = mouseState;
			}

			#endregion

			#region Methods

			private void ProcessMouseButtonDown(
				int x,
				int y,
				ButtonState xnaButtonCurrent,
				ButtonState xnaButtonPrevious,
				Noesis.MouseButton noesisButton)
			{
				if (xnaButtonCurrent == ButtonState.Pressed
					&& xnaButtonCurrent != xnaButtonPrevious)
				{
					this.rootView.MouseDown(x, y, noesisButton);
				}
			}

			private void ProcessMouseButtonUp(
				int x,
				int y,
				ButtonState xnaButtonCurrent,
				ButtonState xnaButtonPrevious,
				Noesis.MouseButton noesisButton)
			{
				if (xnaButtonCurrent == ButtonState.Released
					&& xnaButtonCurrent != xnaButtonPrevious)
				{
					this.rootView.MouseUp(x, y, noesisButton);
				}
			}

			#endregion
		}
	}
}