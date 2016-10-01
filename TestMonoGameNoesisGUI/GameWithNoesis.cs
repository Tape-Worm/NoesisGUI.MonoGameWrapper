using System;

namespace TestMonoGameNoesisGUI
{
	#region

	using Microsoft.Xna.Framework;
	using Microsoft.Xna.Framework.Graphics;
	using Microsoft.Xna.Framework.Input;

	using NoesisGUI.MonoGameWrapper;

	#endregion

	/// <summary>
	///     This is an example MonoGame game using NoesisGUI
	/// </summary>
	/// </summary>
	public class GameWithNoesis : Game
	{
		#region Fields

		private GraphicsDeviceManager graphics;

		private NoesisWrapper noesisWrapper;

		private SpriteBatch spriteBatch;

		// NOTE: In a real application, one would use a texture atlas, instead of two different textures ;-)
		private Texture2D monogameLogo;
		private Texture2D noesisGuiLogo;

		#endregion

		#region Constructors and Destructors

		public GameWithNoesis()
		{
			graphics = new GraphicsDeviceManager(this)
			{
				// TODO: Disable/enable MSAA here
				PreferMultiSampling = true
			};

			graphics.PreparingDeviceSettings += delegate(object sender, PreparingDeviceSettingsEventArgs args)
				{
					// TODO: Set MSAA levels here
					var presentationParameters = args.GraphicsDeviceInformation.PresentationParameters;
					presentationParameters.MultiSampleCount = 8;
				};

			Content.RootDirectory = "Content";
		}

		#endregion

		#region Methods

		/// <summary>
		///     This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			this.noesisWrapper.PreRender(gameTime);
			this.Render(gameTime);
			this.noesisWrapper.PostRender();
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
			this.IsMouseVisible = true;

			this.noesisWrapper = new NoesisWrapper(
				this,
				this.graphics,
				rootXamlPath: "NoesisRoot.xaml",
				stylePath: "NoesisStyle.xaml",
				dataLocalPath: "Data");

			// TODO: Add your initialization logic here

			base.Initialize();
		}

		/// <summary>
		///     LoadContent will be called once per game and is the place to load
		///     all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here
			this.monogameLogo = this.Content.Load<Texture2D>("MonogameLogo");
			this.noesisGuiLogo = this.Content.Load<Texture2D>("NoesisGuiLogo");

			var textureSource = this.noesisWrapper.ConvertTextureToNoesis(this.monogameLogo);
			this.noesisWrapper.View.Content.DataContext = textureSource;
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

			this.noesisWrapper.Update(gameTime);
			
			Tick(gameTime);

			base.Update(gameTime);
		}

		protected virtual void Tick(GameTime gameTime)
		{
			// TODO: Add your update logic here
		}

		protected virtual void Render(GameTime gameTime)
		{
			// TODO: Add your drawing code here
			this.GraphicsDevice.Clear(
				ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil,
				Color.CornflowerBlue,
				1,
				0);

			// NOTE: Ignore any Commodore 64/Amiga demo-style code here ;-)
			var backBufferWidth = this.GraphicsDevice.PresentationParameters.BackBufferWidth;
			var backBufferHeight = this.GraphicsDevice.PresentationParameters.BackBufferHeight;
			float logoPosX = (float)(gameTime.TotalGameTime.TotalSeconds * 200 % backBufferWidth);
			float logoPosY = (float)(Math.Abs(50 * Math.Sin(gameTime.TotalGameTime.TotalSeconds * 5)));
			var scale = new Vector2(0.1f);

			this.spriteBatch.Begin();
			this.spriteBatch.Draw(monogameLogo, new Vector2(logoPosX, logoPosY), scale: scale);
			this.spriteBatch.Draw(noesisGuiLogo, new Vector2(backBufferWidth - logoPosX, backBufferHeight - noesisGuiLogo.Height * scale.Y - logoPosY), scale: scale);
			this.spriteBatch.End();
		}

		#endregion
	}
}