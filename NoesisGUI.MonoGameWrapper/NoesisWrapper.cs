namespace NoesisGUI.MonoGameWrapper
{
    using System;
    using System.IO;
    using System.Reflection;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    using NoesisGUI.MonoGameWrapper.Helpers;
    using NoesisGUI.MonoGameWrapper.Input;

    using SharpDX.Direct3D;
    using SharpDX.DXGI;

    using DX11 = SharpDX.Direct3D11;

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
        #region Constants

        public const string DefaultFileFilters = "*.xaml";

        #endregion

        #region Fields

        private readonly DeviceDx11StateHelper m_DeviceState = new DeviceDx11StateHelper();

        private readonly DX11.Device m_Dx11Device;

        private readonly Game m_Game;

        private readonly GraphicsDeviceManager m_Graphics;

        private readonly GraphicsDevice m_GraphicsDevice;

        private readonly InputManager m_InputManager;

        private ContentReloader m_ContentReloader;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="NoesisWrapper" /> class.
        /// </summary>
        /// <param name="game">The MonoGame game instance.</param>
        /// <param name="graphics">Graphics device manager of the game instance.</param>
        /// <param name="dataPath">(optional) Local path to the folder which will be used as root for other paths</param>
        /// <param name="rootXamlRelativePath">Local XAML file path - will be used as the UI root element</param>
        /// <param name="themeResourcesRelativePath">
        ///     (optional) Local XAML file path - will be used as global ResourceDictionary
        ///     (UI style)
        /// </param>
        /// <param name="autoReloadFileFilter">Re-loads the UI when any data file that matches the filter changed</param>
        public NoesisWrapper(
            Game game,
            GraphicsDeviceManager graphics,
            string rootXamlRelativePath,
            string dataPath,
            string themeResourcesRelativePath,
            string autoReloadFileFilter = null)
        {
            m_Game = game;
            m_Graphics = graphics;

            m_GraphicsDevice = m_Graphics.GraphicsDevice;

            // Initialize DX11 renderer
            m_Dx11Device = (DX11.Device)m_GraphicsDevice.Handle;

            m_Game.Window.TextInput += HandleWindowTextInput;
            m_Game.Window.ClientSizeChanged += HandleWindowClientSizeChange;

            var fullDataPath = Path.GetFullPath(dataPath);

            if (!Directory.Exists(fullDataPath))
            {
                throw new DirectoryNotFoundException(fullDataPath);
            }

            Noesis.GUI.Init();
            Noesis.GUI.SetResourceProvider(fullDataPath);

            if (!string.IsNullOrWhiteSpace(autoReloadFileFilter))
            {
                m_ContentReloader = new ContentReloader(
                    fullDataPath,
                    rootXamlRelativePath,
                    themeResourcesRelativePath,
                    autoReloadFileFilter);
                View = m_ContentReloader.View;
            }
            else
            {
                try
                {
                    var theme = (Noesis.ResourceDictionary)Noesis.GUI.LoadXaml(themeResourcesRelativePath);
                    Noesis.GUI.SetTheme(theme);

                    var content = (Noesis.FrameworkElement)Noesis.GUI.LoadXaml(rootXamlRelativePath);
                    View = Noesis.GUI.CreateView(content);
                }
                catch (Exception ex)
                {
                    ReportError(ex);
                    throw;
                }
            }

            // Use Noesis.View.AntialiasingMode.PPAA if you want less jagged edges.
            View.SetAntialiasingMode(Noesis.View.AntialiasingMode.MSAA);
            View.SetTessellationQuality(Noesis.View.TessellationQuality.Medium);

            m_InputManager = new InputManager(View);

            UpdateSize();

            View.Renderer.InitD3D11(m_Dx11Device.ImmediateContext.NativePointer, new Noesis.VGOptions());
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NoesisWrapper" /> class.
        /// </summary>
        /// <param name="game">The MonoGame game instance.</param>
        /// <param name="graphics">Graphics device manager of the game instance.</param>
        /// <param name="dataPath">(optional) Local path to the folder which will be used as root for other paths</param>
        /// <param name="rootXamlRelativePath">Local XAML file path - will be used as the UI root element</param>
        /// <param name="themeResourcesRelativePath">
        ///     (optional) Local XAML file path - will be used as global ResourceDictionary
        ///     (UI style)
        /// </param>
        /// <param name="shouldAutoReloadChangedFiles">
        ///     Re-loads the UI when any data file that matches the default filter is
        ///     changed
        /// </param>
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

        #endregion

        #region Public Properties

        public Noesis.View View { get; private set; }

        #endregion

        #region Public Methods and Operators

        public static Noesis.Texture LinkTexture(Texture2D texture)
        {
            DX11.ShaderResourceView shaderResourceView;
            return LinkTexture(texture, out shaderResourceView);
        }

        public static Noesis.Texture LinkTexture(Texture2D texture, out DX11.ShaderResourceView shaderResourceView)
        {
            if (texture.IsDisposed)
            {
                throw new ObjectDisposedException("NoesisWrapper: texture was disposed");
            }

            // TODO: Currently we assume DX11 here.
            var getTxt = texture.GetType().GetMethod("GetTexture", BindingFlags.Instance | BindingFlags.NonPublic);
            if (getTxt == null)
            {
                throw new NotSupportedException("NoesisWrapper: only Monogame DX11 is supported");
            }

            var getSrv = texture.GetType()
                .GetMethod("GetShaderResourceView", BindingFlags.Instance | BindingFlags.NonPublic);
            if (getSrv == null)
            {
                throw new NotSupportedException("NoesisWrapper: only Monogame DX11 is supported");
            }

            var txt = (DX11.Texture2D)getTxt.Invoke(texture, null);
            if (txt == null)
            {
                throw new NotSupportedException("NoesisWrapper: Monogame texture did not provide the native texture");
            }

            var srv = shaderResourceView = (DX11.ShaderResourceView)getSrv.Invoke(texture, null);
            if (srv == null)
            {
                throw new NotSupportedException(
                    "NoesisWrapper: Monogame texture did not provide a shader resource view");
            }

            var desc = srv.Description;

            Noesis.Texture.Format format;
            switch (desc.Format)
            {
                case Format.R8G8B8A8_UNorm:
                    format = Noesis.Texture.Format.BGRA8;
                    break;
                case Format.R8_UNorm:
                    format = Noesis.Texture.Format.R8;
                    break;
                default:
                    throw new NotSupportedException($"NoesisWrapper: Texture format {desc.Format} is not supported");
            }

            if (desc.Dimension != ShaderResourceViewDimension.Texture2D)
            {
                throw new NotSupportedException(
                    $"NoesisWrapper: Shader resource view dimension {desc.Dimension} is not supported");
            }

            return Noesis.Texture.WrapD3D11Texture(
                texture,
                txt.NativePointer,
                texture.Width,
                texture.Height,
                desc.Texture2D.MipLevels,
                format,
                false);
        }

        public static IWrappedTexture Wrap(Texture2D texture)
        {
            return texture == null ? null : new WrappedTexture(texture);
        }

        public void Dispose()
        {
            View = null;

            m_ContentReloader?.Dispose();
            m_ContentReloader = null;

            m_Game.Window.TextInput -= HandleWindowTextInput;
            m_Game.Window.ClientSizeChanged -= HandleWindowClientSizeChange;

            // TODO: How do we dispose just this Noesis UI view?
            Noesis.GUI.Shutdown();
        }

        public void Draw(GameTime gameTime)
        {
            m_DeviceState.Save(m_Dx11Device.ImmediateContext);

            var renderer = View.Renderer;

            renderer.UpdateRenderTree();

            if (renderer.NeedsOffscreen())
            {
                renderer.RenderOffscreen();
            }

            OnRenderView();

            m_DeviceState.Restore(m_Dx11Device.ImmediateContext);
        }

        public void Update(GameTime gameTime)
        {
            View.Update(gameTime.TotalGameTime.TotalSeconds);
            m_InputManager.Update(gameTime);
            m_ContentReloader?.Update(gameTime);
        }

        #endregion

        #region Methods

        protected virtual void OnRenderView()
        {
            View.Renderer.Render();
        }

        private static void ReportError(Exception ex)
        {
            var fg = Console.ForegroundColor;
            var bg = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(ex);
            Console.ForegroundColor = fg;
            Console.BackgroundColor = bg;
        }

        private void HandleWindowClientSizeChange(object sender, EventArgs e)
        {
            UpdateSize();
        }

        private void HandleWindowTextInput(object sender, TextInputEventArgs args)
        {
            m_InputManager.OnTextInput(args);
        }

        private void UpdateSize()
        {
            var viewport = m_GraphicsDevice.Viewport;
            View.SetSize(viewport.Width, viewport.Height);
        }

        #endregion

        private class ContentReloader : IDisposable
        {
            #region Fields

            private readonly string m_FullDataPath;

            private readonly string m_RootXamlRelativePath;

            private readonly string m_ThemeResourcesRelativePath;

            private readonly Noesis.ContentControl m_ViewContentControl;

            private readonly Noesis.ResourceDictionary m_ViewThemeResources;

            private TimeSpan m_DataFileChangeDelay;

            private FileSystemWatcher m_DataWatcher;

            #endregion

            #region Constructors and Destructors

            public ContentReloader(
                string fullDataPath,
                string rootXamlRelativePath,
                string themeResourcesRelativePath,
                string fileFilter)
            {
                m_FullDataPath = fullDataPath;
                m_RootXamlRelativePath = rootXamlRelativePath;
                m_ThemeResourcesRelativePath = themeResourcesRelativePath;

                m_ViewContentControl = new Noesis.ContentControl();
                m_ViewThemeResources = new Noesis.ResourceDictionary();

                Noesis.GUI.SetTheme(m_ViewThemeResources);

                View = Noesis.GUI.CreateView(m_ViewContentControl);

                LoadUI();

                m_DataWatcher =
                    new FileSystemWatcher(fullDataPath)
                    {
                        IncludeSubdirectories = true,
                        Filter = fileFilter,
                        NotifyFilter =
                            NotifyFilters.LastWrite | NotifyFilters.DirectoryName
                            | NotifyFilters.FileName
                    };

                m_DataWatcher.Changed += HandleDataFileChange;
                m_DataWatcher.Created += HandleDataFileChange;
                m_DataWatcher.Deleted += HandleDataFileChange;
                m_DataWatcher.Renamed += HandleDataFileChange;

                m_DataWatcher.EnableRaisingEvents = true;
            }

            #endregion

            #region Public Properties

            public Noesis.View View { get; private set; }

            #endregion

            #region Public Methods and Operators

            public void Dispose()
            {
                View = null;

                m_DataWatcher?.Dispose();
                m_DataWatcher = null;
            }

            public void Update(GameTime gameTime)
            {
                if (m_DataFileChangeDelay > TimeSpan.Zero)
                {
                    m_DataFileChangeDelay -= gameTime.ElapsedGameTime;

                    if (m_DataFileChangeDelay <= TimeSpan.Zero)
                    {
                        LoadUI();
                    }
                }
            }

            #endregion

            #region Methods

            private void HandleDataFileChange(object sender, FileSystemEventArgs e)
            {
                var relativePath = e.FullPath.StartsWith(m_FullDataPath, StringComparison.OrdinalIgnoreCase)
                                       ? e.FullPath.Substring(m_FullDataPath.Length)
                                           .TrimStart(Path.DirectorySeparatorChar)
                                       : e.FullPath;

                Console.WriteLine($"{relativePath} was changed, re-creating UI");

                // Throttle changes, to deal with multiple files being saved 
                m_DataFileChangeDelay = TimeSpan.FromSeconds(0.1);
            }

            private void LoadUI()
            {
                // If something is wrong with the XAML, we keep the previous UI
                try
                {
                    // Clear the content and theme
                    m_ViewContentControl.Content = null;
                    m_ViewThemeResources.MergedDictionaries.Clear();

                    // Load and replace theme
                    var theme = (Noesis.ResourceDictionary)Noesis.GUI.LoadXaml(m_ThemeResourcesRelativePath);
                    var style = theme.FindName("RootContainerStyle") as Noesis.Style;
                    m_ViewThemeResources.MergedDictionaries.Add(theme);

                    // Load and replace content using the new theme
                    var content = (Noesis.FrameworkElement)Noesis.GUI.LoadXaml(m_RootXamlRelativePath);
                    m_ViewContentControl.Style = style;
                    m_ViewContentControl.Content = content;
                }
                catch (Exception ex)
                {
                    ReportError(ex);
                }
            }

            #endregion
        }

        private sealed class WrappedTexture : IWrappedTexture
        {
            #region Fields

            private readonly DX11.ShaderResourceView m_ShaderResourceView;

            #endregion

            #region Constructors and Destructors

            public WrappedTexture(Texture2D texture)
            {
                NoesisTexture = LinkTexture(texture, out m_ShaderResourceView);
            }

            #endregion

            #region Public Properties

            public Noesis.Texture NoesisTexture { get; }

            #endregion

            #region Public Methods and Operators

            public void GenerateMipMaps()
            {
                m_ShaderResourceView.Device.ImmediateContext.GenerateMips(m_ShaderResourceView);
            }

            #endregion
        }
    }
}