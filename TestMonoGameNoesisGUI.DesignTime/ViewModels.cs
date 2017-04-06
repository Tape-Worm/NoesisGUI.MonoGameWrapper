namespace TestMonoGameNoesisGUI.DesignTime
{
    using System;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public class ViewModel
    {
        #region Static Fields

        public static ViewModel Instance = new ViewModel();

        #endregion

        #region Constructors and Destructors

        public ViewModel()
        {
            MonogameTextureSource = CreateImageSource("https://upload.wikimedia.org/wikipedia/commons/thumb/e/e6/MonoGame_Logo.svg/512px-MonoGame_Logo.svg.png");
            MonogameRenderTargetSource = CreateImageSource("https://pbs.twimg.com/media/CnBt-QNWcAE2tnd.jpg");
        }

        private static ImageSource CreateImageSource(string url)
        {
            return new BitmapImage(new Uri(url));
        }

        #endregion

        #region Public Properties

        public ImageSource MonogameRenderTargetSource { get; }

        public ImageSource MonogameTextureSource { get; }

        #endregion
    }
}