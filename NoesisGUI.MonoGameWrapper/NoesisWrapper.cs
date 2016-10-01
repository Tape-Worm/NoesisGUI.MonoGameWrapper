namespace NoesisGUI.MonoGameWrapper
{
	#region

	using System;
	using System.Reflection;

	using Microsoft.Xna.Framework;
	using Microsoft.Xna.Framework.Graphics;

	using NoesisGUI.MonoGameWrapper.Helpers;
	using NoesisGUI.MonoGameWrapper.Input;

	using DX11 = SharpDX.Direct3D11;

	#endregion

	/// <summary>
	///     Noesis Monogame UI wrapper usage:
	///     1. at game Initialize() create wrapper instance (see this class constructor)
	///     2. at game Update() invoke wrapper.Update(gameTime)
	///     3. at game Draw() invoke:
	///     - 3.1. wrapper.PreRender(gameTime)
	///     - 3.2. clear graphics device (including stencil buffer)
	///     - 3.3. your game drawing code
	///     - 3.4. wrapper.PostRender()
	///     Please be sure you have IsMouseVisible=true at the MonoGame instance
	/// </summary>
	public class NoesisWrapper : IDisposable
	{
		#region Fields

		private readonly DX11.Device dx11Device;

		private readonly DeviceDX11StateHelper deviceState = new DeviceDX11StateHelper();

		private readonly Game game;

		private readonly GraphicsDeviceManager graphics;

		private readonly GraphicsDevice graphicsDevice;

		private readonly InputManager inputManager;

		private readonly Noesis.View view;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		///     Initializes a new instance of the <see cref="NoesisWrapper" /> class.
		/// </summary>
		/// <param name="game">The MonoGame game instance.</param>
		/// <param name="graphics">Graphics device manager of the game instance.</param>
		/// <param name="rootXamlPath">Local XAML file path - will be used as the UI root element</param>
		/// <param name="stylePath">(optional) Local XAML file path - will be used as global ResourceDictionary (UI style)</param>
		/// <param name="dataLocalPath">(optional) Local path to the folder which will be used as root for other paths</param>
		/// <remarks>
		///     PLEASE NOTE: .XAML-files should be prebuilt to .NSB-files by NoesisGUI Build Tool).
		/// </remarks>
		public NoesisWrapper(
			Game game,
			GraphicsDeviceManager graphics,
			string rootXamlPath,
			string stylePath = null,
			string dataLocalPath = "Data")
		{
			this.game = game;
			this.graphics = graphics;

			this.graphicsDevice = this.graphics.GraphicsDevice;

			Noesis.GUI.Init();
			Noesis.GUI.SetResourceProvider(dataLocalPath);

			this.view = this.CreateRootView(rootXamlPath, stylePath);

			// Use Noesis.View.AntialiasingMode.PSAA if you want less jagged edges.
			this.view.SetAntialiasingMode(Noesis.View.AntialiasingMode.MSAA);
			this.view.SetTessellationQuality(Noesis.View.TessellationQuality.TessellationQuality_Medium);

			this.inputManager = new InputManager(this.view);

			this.game.Window.TextInput += this.HandleWindowTextInput;
			this.game.Window.ClientSizeChanged += this.HandleWindowClientSizeChange;

			this.UpdateSize();

			// Initialize DX11 renderer
			var device = ((DX11.Device)this.graphicsDevice.Handle);
			this.dx11Device = device;
			this.view.Renderer.InitD3D11(
				device.ImmediateContext.NativePointer,
				new Noesis.VGOptions
				{
					// TODO: Set vector graphics options here.
				});
		}

		private void HandleWindowTextInput(object sender, TextInputEventArgs args)
		{
			this.inputManager.OnTextInput(args);
		}

		#endregion

		#region Public Methods and Operators

		public void PostRender()
		{
			this.deviceState.Save(this.dx11Device.ImmediateContext);
			this.view.Renderer.Render();
			this.deviceState.Restore(this.dx11Device.ImmediateContext);
		}

		public void PreRender(GameTime gameTime)
		{
			this.deviceState.Save(this.dx11Device.ImmediateContext);

			var renderer = this.view.Renderer;

			renderer.UpdateRenderTree();

			if (renderer.NeedsOffscreen())
			{
				renderer.RenderOffscreen();
			}

			this.deviceState.Restore(this.dx11Device.ImmediateContext);
		}

		public void Update(GameTime gameTime)
		{
			this.view.Update(gameTime.TotalGameTime.TotalSeconds);
			this.inputManager.Update(gameTime);
		}

		#endregion

		#region Methods

		private Noesis.View CreateRootView(string rootXamlPath, string stylePath)
		{
			Noesis.ResourceDictionary theme = (Noesis.ResourceDictionary)Noesis.GUI.LoadXaml(stylePath ?? "NoesisStyle.xaml");
			Noesis.Grid content = (Noesis.Grid)Noesis.GUI.LoadXaml(rootXamlPath);
			var view = Noesis.GUI.CreateView(content, theme);
			return view;
		}

		private void UpdateSize()
		{
			var viewport = this.graphicsDevice.Viewport;
			this.view.SetSize(viewport.Width, viewport.Height);
		}

		private void HandleWindowClientSizeChange(object sender, System.EventArgs e)
		{
			this.UpdateSize();
		}

		#endregion

		public void Dispose()
		{
			this.game.Window.TextInput -= this.HandleWindowTextInput;
			this.game.Window.ClientSizeChanged -= this.HandleWindowClientSizeChange;

			// TODO: How do we dispose just this Noesis UI view?
			Noesis.GUI.Shutdown();
		}

		public Noesis.View View
		{
			get { return this.view; }
		}

		public Noesis.TextureSource ConvertTextureToNoesis(Texture2D texture)
		{
			var method = texture.GetType().GetMethod("GetTexture", BindingFlags.Instance | BindingFlags.NonPublic);
			
			var dx11Texture = (DX11.Texture2D)method.Invoke(texture, null);

			var noesisTexture = new Noesis.TextureSource(
				dx11Texture.NativePointer,
				texture.Width,
				texture.Height,
				texture.LevelCount);

			return noesisTexture;
		}
	}
}