namespace NoesisGUI.MonoGameWrapper
{
	using System;
	using System.Reflection;

	using Microsoft.Xna.Framework;
	using Microsoft.Xna.Framework.Graphics;

	using Helpers;
	using Input;

	using SharpDX.Direct3D;
	using SharpDX.DXGI;

	using DX11 = SharpDX.Direct3D11;

	using System.IO;

	using Path = System.IO.Path;

	public interface IWrappedTexture 
	{
		Noesis.TextureSource ImageSource { get; }
		void GenerateMipMaps();
	}

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
		public const string DefaultFileFilters = "*.xaml";

		#region Fields

		private readonly DX11.Device dx11Device;

		private readonly DeviceDX11StateHelper deviceState = new DeviceDX11StateHelper();

		private readonly Game game;

		private readonly GraphicsDeviceManager graphics;

		private readonly GraphicsDevice graphicsDevice;

		private readonly InputManager inputManager;

		private ContentReloader contentReloader;
		#endregion

		#region Constructors and Destructors

		/// <summary>
		///     Initializes a new instance of the <see cref="NoesisWrapper" /> class.
		/// </summary>
		/// <param name="game">The MonoGame game instance.</param>
		/// <param name="graphics">Graphics device manager of the game instance.</param>
		/// <param name="dataPath">(optional) Local path to the folder which will be used as root for other paths</param>
		/// <param name="rootXamlRelativePath">Local XAML file path - will be used as the UI root element</param>
		/// <param name="themeResourcesRelativePath">(optional) Local XAML file path - will be used as global ResourceDictionary (UI style)</param>
		/// <param name="autoReloadFileFilter">Re-loads the UI when any data file that matches the filter changed</param>
		public NoesisWrapper(
			Game game,
			GraphicsDeviceManager graphics,
			string rootXamlRelativePath,
			string dataPath,
			string themeResourcesRelativePath,
			string autoReloadFileFilter = null)
		{
			this.game = game;
			this.graphics = graphics;

			this.graphicsDevice = this.graphics.GraphicsDevice;

			// Initialize DX11 renderer
			this.dx11Device = (DX11.Device)this.graphicsDevice.Handle;

			this.game.Window.TextInput += this.HandleWindowTextInput;
			this.game.Window.ClientSizeChanged += this.HandleWindowClientSizeChange;

			var fullDataPath = Path.GetFullPath(dataPath);

			Noesis.GUI.Init();
			Noesis.GUI.SetResourceProvider(fullDataPath);

			if (!string.IsNullOrWhiteSpace(autoReloadFileFilter))
			{
				contentReloader = new ContentReloader(fullDataPath, rootXamlRelativePath, themeResourcesRelativePath, autoReloadFileFilter);
				this.View = contentReloader.View;
			}
			else
			{
				var theme = (Noesis.ResourceDictionary)Noesis.GUI.LoadXaml(themeResourcesRelativePath);
				var content = (Noesis.FrameworkElement)Noesis.GUI.LoadXaml(rootXamlRelativePath);
				this.View = Noesis.GUI.CreateView(content, theme);
			}

			// Use Noesis.View.AntialiasingMode.PPAA if you want less jagged edges.
			this.View.SetAntialiasingMode(Noesis.View.AntialiasingMode.MSAA);
			this.View.SetTessellationQuality(Noesis.View.TessellationQuality.TessellationQuality_Medium);

			this.inputManager = new InputManager(this.View);

			this.UpdateSize();

			this.View.Renderer.InitD3D11(
				this.dx11Device.ImmediateContext.NativePointer,
				new Noesis.VGOptions
				{
					// TODO: Set vector graphics options here.
				});
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="NoesisWrapper" /> class.
		/// </summary>
		/// <param name="game">The MonoGame game instance.</param>
		/// <param name="graphics">Graphics device manager of the game instance.</param>
		/// <param name="dataPath">(optional) Local path to the folder which will be used as root for other paths</param>
		/// <param name="rootXamlRelativePath">Local XAML file path - will be used as the UI root element</param>
		/// <param name="themeResourcesRelativePath">(optional) Local XAML file path - will be used as global ResourceDictionary (UI style)</param>
		/// <param name="shouldAutoReloadChangedFiles">Re-loads the UI when any data file that matches the default filter is changed</param>
		public NoesisWrapper(
			Game game,
			GraphicsDeviceManager graphics,
			string rootXamlRelativePath,
			string dataPath,
			string themeResourcesRelativePath,
			bool shouldAutoReloadChangedFiles)
			: this(
				game,
				graphics,
				rootXamlRelativePath,
				dataPath,
				themeResourcesRelativePath,
				shouldAutoReloadChangedFiles ? DefaultFileFilters : null)
		{
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
			this.View.Renderer.Render();
			this.deviceState.Restore(this.dx11Device.ImmediateContext);
		}

		public void PreRender(GameTime gameTime)
		{
			this.deviceState.Save(this.dx11Device.ImmediateContext);

			var renderer = this.View.Renderer;

			renderer.UpdateRenderTree();

			if (renderer.NeedsOffscreen())
			{
				renderer.RenderOffscreen();
			}

			this.deviceState.Restore(this.dx11Device.ImmediateContext);
		}

		public void Update(GameTime gameTime)
		{
			this.View.Update(gameTime.TotalGameTime.TotalSeconds);
			this.inputManager.Update(gameTime);
			this.contentReloader?.Update(gameTime);
		}

		#endregion

		#region Methods

		private void UpdateSize()
		{
			var viewport = this.graphicsDevice.Viewport;
			this.View.SetSize(viewport.Width, viewport.Height);
		}

		private void HandleWindowClientSizeChange(object sender, System.EventArgs e)
		{
			this.UpdateSize();
		}

		#endregion

		public void Dispose()
		{
			this.View = null;

			this.contentReloader?.Dispose();
			this.contentReloader = null;

			this.game.Window.TextInput -= this.HandleWindowTextInput;
			this.game.Window.ClientSizeChanged -= this.HandleWindowClientSizeChange;

			// TODO: How do we dispose just this Noesis UI view?
			Noesis.GUI.Shutdown();
		}

		public Noesis.View View { get; private set; }

		public IWrappedTexture Wrap(Texture2D texture)
		{
			return new WrappedTexture(texture);
		}

		private sealed class WrappedTexture : IWrappedTexture
		{
			private readonly DX11.ShaderResourceView shaderResourceView;

			public WrappedTexture(Texture2D texture)
			{
				if (texture.IsDisposed)
					throw new ObjectDisposedException("NoesisWrapper: texture was disposed");

				// TODO: Currently we assume DX11 here.
				var method = texture.GetType().GetMethod("GetShaderResourceView", BindingFlags.Instance | BindingFlags.NonPublic);
				if (method == null)
					throw new NotSupportedException("NoesisWrapper: only Monogame DX11 is supported");

				var srv = (DX11.ShaderResourceView)method.Invoke(texture, null);
				if (srv == null)
					throw new NotSupportedException("NoesisWrapper: Monogame texture did not provide a shader resource view");

				var desc = srv.Description;

				Noesis.TextureFormat format;
				switch (desc.Format)
				{
					case Format.R8G8B8A8_UNorm:
						format = Noesis.TextureFormat.BGRA8;
						break;
					case Format.R8_UNorm:
						format = Noesis.TextureFormat.R8;
						break;
					default:
						throw new NotSupportedException($"NoesisWrapper: Texture format {desc.Format} is not supported");
				}

				if (desc.Dimension != ShaderResourceViewDimension.Texture2D)
					throw new NotSupportedException($"NoesisWrapper: Shader resource view dimension {desc.Dimension} is not supported");

				this.ImageSource = Noesis.TextureSource.CreateD3D11(srv.NativePointer,
					texture.Width, texture.Height, desc.Texture2D.MipLevels, format, false);

				this.shaderResourceView = srv;
			}

			public Noesis.TextureSource ImageSource { get; }

			public void GenerateMipMaps()
			{
				this.shaderResourceView.Device.ImmediateContext.GenerateMips(this.shaderResourceView);
			}
		}

		private class ContentReloader : IDisposable
		{
			private readonly string fullDataPath;
			private readonly string rootXamlRelativePath;
			private readonly string themeResourcesRelativePath;

			private readonly Noesis.ContentControl viewContentControl;
			private readonly Noesis.ResourceDictionary viewThemeResources;

			private FileSystemWatcher dataWatcher;
			private TimeSpan dataFileChangeDelay;

			public ContentReloader(string fullDataPath, string rootXamlRelativePath, string themeResourcesRelativePath, string fileFilter)
			{
				this.fullDataPath = fullDataPath;
				this.rootXamlRelativePath = rootXamlRelativePath;
				this.themeResourcesRelativePath = themeResourcesRelativePath;

				this.viewContentControl = new Noesis.ContentControl();
				this.viewThemeResources = new Noesis.ResourceDictionary();
				this.View = this.View = Noesis.GUI.CreateView(this.viewContentControl, this.viewThemeResources);

				this.LoadUI();

				this.dataWatcher = new FileSystemWatcher(fullDataPath)
				{
					IncludeSubdirectories = true,
					Filter = fileFilter,
					NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.DirectoryName | NotifyFilters.FileName,
				};

				this.dataWatcher.Changed += this.HandleDataFileChange;
				this.dataWatcher.Created += this.HandleDataFileChange;
				this.dataWatcher.Deleted += this.HandleDataFileChange;
				this.dataWatcher.Renamed += this.HandleDataFileChange;

				this.dataWatcher.EnableRaisingEvents = true;
			}

			public Noesis.View View { get; private set; }

			private void LoadUI()
			{
				try
				{
					// If something is wrong with the XAML, we keep the previous UI
					var theme = (Noesis.ResourceDictionary)Noesis.GUI.LoadXaml(this.themeResourcesRelativePath);
					var content = (Noesis.FrameworkElement)Noesis.GUI.LoadXaml(this.rootXamlRelativePath);
					var style = theme.FindName("RootContainerStyle") as Noesis.Style;

					// Replace content and theme.
					this.viewContentControl.Content = null;
					this.viewThemeResources.MergedDictionaries.Clear();
					this.viewThemeResources.MergedDictionaries.Add(theme);
					this.viewContentControl.Style = style;
					this.viewContentControl.Content = content;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
			}

			private void HandleDataFileChange(object sender, FileSystemEventArgs e)
			{
				var relativePath = e.FullPath.StartsWith(this.fullDataPath, StringComparison.OrdinalIgnoreCase)
				? e.FullPath.Substring(this.fullDataPath.Length).TrimStart(Path.DirectorySeparatorChar)
				: e.FullPath;

				Console.WriteLine($"{e.FullPath} was changed, re-creating UI");

				// Throttle changes, to deal with multiple files being saved 
				this.dataFileChangeDelay = TimeSpan.FromSeconds(0.5);
			}

			public void Dispose()
			{
				this.View = null;

				this.dataWatcher?.Dispose();
				this.dataWatcher = null;
			}

			public void Update(GameTime gameTime)
			{
				if (this.dataFileChangeDelay > TimeSpan.Zero)
				{
					this.dataFileChangeDelay -= gameTime.ElapsedGameTime;

					if (this.dataFileChangeDelay <= TimeSpan.Zero)
					{
						this.LoadUI();
					}
				}
			}
		}
	}
}