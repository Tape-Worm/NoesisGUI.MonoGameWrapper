using System.Windows.Media;

namespace TestMonoGameNoesisGUI.DesignTime
{
	using System;
	using System.Windows.Media.Imaging;

	public class WrappedTexture
	{
		public ImageSource ImageSource { get; }

		public WrappedTexture(string uri)
		{
			ImageSource = new BitmapImage(new Uri(uri));
		}
	}

	public class ViewModel
	{
		public WrappedTexture MonogameTexture { get; }
		public WrappedTexture MonogameRenderTarget { get; }

		public ViewModel()
		{
			MonogameTexture = new WrappedTexture("https://upload.wikimedia.org/wikipedia/commons/thumb/e/e6/MonoGame_Logo.svg/512px-MonoGame_Logo.svg.png");
			MonogameRenderTarget = new WrappedTexture("https://pbs.twimg.com/media/CnBt-QNWcAE2tnd.jpg");
		}

		public static ViewModel Instance = new ViewModel();
	}
}