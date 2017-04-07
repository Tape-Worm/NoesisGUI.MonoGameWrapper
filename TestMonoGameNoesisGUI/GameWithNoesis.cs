namespace TestMonoGameNoesisGUI
{
    using System;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    using NoesisGUI.MonoGameWrapper;
    using NoesisGUI.MonoGameWrapper.Helpers;

    /// <summary>
    ///     This is an example MonoGame game using NoesisGUI 1.3 beta
    /// </summary>
    public class GameWithNoesis : Game
    {
        #region Fields

        private readonly GraphicsDeviceManager m_Graphics;

        // NOTE: In a real application, one would use a texture atlas, instead of two different textures ;-)
        private Texture2D m_MonogameLogoTexture;

        private Texture2D m_NoesisGuiLogoTexture;

        private NoesisWrapper m_NoesisWrapper;

        private RenderTarget2D m_RenderTarget;

        private SpriteBatch m_SpriteBatch;

        private ViewModel m_ViewModel;

        private IWrappedTexture m_WrappedRenderTarget;

        #endregion

        #region Constructors and Destructors

        public GameWithNoesis()
        {
            // TODO: Remove this code if you don't want the user to resize the window, and the backbuffer to adapt automatically.
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += HandleWindowClientSizeChanged;

#if DEBUG
            Window.ClientSizeChanged += PositionDevelopmentWindows;
#endif

            m_Graphics = new GraphicsDeviceManager(this)
                         {
                             // TODO: Disable/enable MSAA here
                             PreferMultiSampling = true
                         };

            m_Graphics.PreparingDeviceSettings += delegate(object sender, PreparingDeviceSettingsEventArgs args)
            {
                // TODO: Set MSAA levels here
                var presentationParameters = args.GraphicsDeviceInformation.PresentationParameters;
                presentationParameters.MultiSampleCount = 8;
            };

            Content.RootDirectory = "Content";
        }

        private void PositionDevelopmentWindows(object sender, EventArgs e)
        {
            Window.ClientSizeChanged -= PositionDevelopmentWindows;
            Development.PlaceWindowsOnOuterMonitors(Window);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            m_NoesisWrapper.PreRender(gameTime);
            Render(gameTime);
            m_NoesisWrapper.PostRender();
            base.Draw(gameTime);
        }

        /// <summary>
        ///     Allows the game to perform any initialization it needs to before starting to run.
        ///     This is where it can query for any required services and load any non-graphic
        ///     related content.  Calling base.Initialize will enumerate through any components
        ///     and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            IsMouseVisible = true;

            m_NoesisWrapper = new NoesisWrapper(
                this,
                m_Graphics,
                shouldAutoReloadChangedFiles: true,
                rootXamlRelativePath: "NoesisRoot.xaml",
                themeResourcesRelativePath: "NoesisStyle.xaml",
                dataPath: @"..\..\..\Data");

            base.Initialize();
        }

        /// <summary>
        ///     LoadContent will be called once per game and is the place to load
        ///     all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            m_SpriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            m_MonogameLogoTexture = Content.Load<Texture2D>("MonogameLogo");
            m_NoesisGuiLogoTexture = Content.Load<Texture2D>("NoesisGuiLogo");

            m_RenderTarget = new RenderTarget2D(
                GraphicsDevice,
                512,
                512,
                true,
                SurfaceFormat.Color,
                DepthFormat.None,
                0,
                RenderTargetUsage.PreserveContents);

            // An example of how to share a monogame texture with Noesis
            var textureSource = NoesisWrapper.Wrap(m_MonogameLogoTexture);
            m_WrappedRenderTarget = NoesisWrapper.Wrap(m_RenderTarget);
            m_ViewModel = new ViewModel(textureSource, m_WrappedRenderTarget);
            m_NoesisWrapper.View.Content.DataContext = m_ViewModel;
        }

        protected virtual void Render(GameTime gameTime)
        {
            // NOTE: Ignore any Commodore 64/Amiga demo-style code here ;-)
            var backBufferWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            var backBufferHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
            var logoPosX = (float)(gameTime.TotalGameTime.TotalSeconds * 200 % backBufferWidth);
            var logoPosY = (float)Math.Abs(50 * Math.Sin(gameTime.TotalGameTime.TotalSeconds * 5));
            var scale = new Vector2(0.1f);

            // Render to texture
            GraphicsDevice.SetRenderTarget(m_RenderTarget);
            GraphicsDevice.Clear(Color.SandyBrown);

            m_SpriteBatch.Begin();
            m_SpriteBatch.Draw(
                m_NoesisGuiLogoTexture,
                origin: new Vector2(m_NoesisGuiLogoTexture.Width / 2f, m_NoesisGuiLogoTexture.Height / 2f),
                rotation: (float)gameTime.TotalGameTime.TotalSeconds,
                position: new Vector2(m_RenderTarget.Width / 2f, m_RenderTarget.Width / 2f));
            m_SpriteBatch.End();

            m_WrappedRenderTarget.GenerateMipMaps();

            // Render to backbuffer 
            GraphicsDevice.SetRenderTarget(null);

            // TODO: Add your drawing code here
            GraphicsDevice.Clear(
                ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil,
                Color.CornflowerBlue,
                1,
                0);

            m_SpriteBatch.Begin();
            m_SpriteBatch.Draw(m_MonogameLogoTexture, new Vector2(logoPosX, logoPosY), scale: scale);
            m_SpriteBatch.Draw(
                m_NoesisGuiLogoTexture,
                new Vector2(
                    backBufferWidth - logoPosX,
                    backBufferHeight - m_NoesisGuiLogoTexture.Height * scale.Y - logoPosY),
                scale: scale);
            m_SpriteBatch.End();
        }

        protected virtual void Tick(GameTime gameTime)
        {
            // TODO: Add your update logic here
        }

        /// <summary>
        ///     UnloadContent will be called once per game and is the place to unload
        ///     game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        ///     Allows the game to run logic such as updating the world,
        ///     checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            m_NoesisWrapper.Update(gameTime);

            Tick(gameTime);

            base.Update(gameTime);
        }

        private void HandleWindowClientSizeChanged(object sender, EventArgs e)
        {
            var bounds = Window.ClientBounds;

            // When minimized, width and height will be 0
            bool isWindowMinimized = bounds.Width <= 0 || bounds.Height <= 0;

            // Avoid endless recursion bug in some versions of Monogame.
            bool isBufferAlreadyResized = m_Graphics.PreferredBackBufferWidth == bounds.Width
                                    && m_Graphics.PreferredBackBufferHeight == bounds.Height;

            if (isWindowMinimized || isBufferAlreadyResized)
                return;

            m_Graphics.PreferredBackBufferWidth = bounds.Width;
            m_Graphics.PreferredBackBufferHeight = bounds.Height;
            m_Graphics.ApplyChanges();
        }

        #endregion

        public class ViewModel
        {
            #region Constructors and Destructors

            public ViewModel(IWrappedTexture monogameTexture, IWrappedTexture monogameRenderTarget)
            {
                MonogameTextureSource = new Noesis.TextureSource(monogameTexture.NoesisTexture);
                MonogameRenderTargetSource = new Noesis.TextureSource(monogameRenderTarget.NoesisTexture);
            }

            #endregion

            #region Public Properties

            public Noesis.TextureSource MonogameRenderTargetSource { get; }

            public Noesis.TextureSource MonogameTextureSource { get; }

            #endregion
        }
    }
}