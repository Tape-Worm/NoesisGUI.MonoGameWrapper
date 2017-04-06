namespace NoesisGUI.MonoGameWrapper
{
    public interface IWrappedTexture
    {
        #region Public Properties

        Noesis.Texture NoesisTexture { get; }

        #endregion

        #region Public Methods and Operators

        void GenerateMipMaps();

        #endregion
    }
}