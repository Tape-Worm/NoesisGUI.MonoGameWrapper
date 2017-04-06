namespace NoesisGUI.MonoGameWrapper.Helpers
{
    using SharpDX;
    using SharpDX.Direct3D;
    using SharpDX.Direct3D11;
    using SharpDX.DXGI;

    /// <summary>
    ///     This helper provide methods for saving and restoring DX11 graphics device state
    ///     with MonoGame. Provided by NoesisGUI team
    /// </summary>
    internal class DeviceDx11StateHelper
    {
        #region Fields

        private readonly Buffer[] m_Vb = new Buffer[1];

        private readonly int[] m_VbOffset = new int[1];

        private readonly int[] m_VbStride = new int[1];

        private Color4 m_BlendFactor;

        private BlendState m_BlendState;

        private DepthStencilState m_DepthState;

        private DepthStencilView m_DepthStencilView;

        private Buffer m_Ib;

        private Format m_IbFormat;

        private int m_IbOffset;

        private InputLayout m_Layout;

        private PixelShader m_Ps;

        private Buffer[] m_PsConstantBuffers;

        private ShaderResourceView[] m_PsResources;

        private SamplerState[] m_PsSamplers;

        private RasterizerState m_RasterizerState;

        private RenderTargetView[] m_RenderTargetView;

        private int m_SampleMaskRef;

        private Rectangle[] m_ScissorRectangles;

        private int m_StencilRefRef;

        private PrimitiveTopology m_Topology;

        private ViewportF[] m_Viewports;

        private VertexShader m_Vs;

        private Buffer[] m_VsConstantBuffers;

        private ShaderResourceView[] m_VsResources;

        private SamplerState[] m_VsSamplers;

        #endregion

        #region Public Methods and Operators

        public void Restore(DeviceContext context)
        {
            context.InputAssembler.PrimitiveTopology = m_Topology;
            context.InputAssembler.InputLayout = m_Layout;
            context.Rasterizer.SetViewports(m_Viewports);
            context.Rasterizer.SetScissorRectangles(m_ScissorRectangles);
            context.Rasterizer.State = m_RasterizerState;
            context.OutputMerger.SetBlendState(m_BlendState, m_BlendFactor, m_SampleMaskRef);
            context.OutputMerger.SetDepthStencilState(m_DepthState, m_StencilRefRef);
            context.OutputMerger.SetRenderTargets(m_DepthStencilView, m_RenderTargetView[0]);

            context.PixelShader.Set(m_Ps);
            context.PixelShader.SetConstantBuffers(0, m_PsConstantBuffers);
            context.PixelShader.SetSamplers(0, m_PsSamplers);
            context.PixelShader.SetShaderResources(0, m_PsResources);

            context.VertexShader.Set(m_Vs);
            context.VertexShader.SetConstantBuffers(0, m_VsConstantBuffers);
            context.VertexShader.SetSamplers(0, m_VsSamplers);
            context.VertexShader.SetShaderResources(0, m_VsResources);

            context.InputAssembler.SetIndexBuffer(m_Ib, m_IbFormat, m_IbOffset);
            context.InputAssembler.SetVertexBuffers(0, m_Vb, m_VbStride, m_VbOffset);

            m_RenderTargetView[0]?.Dispose();
            m_DepthStencilView?.Dispose();
        }

        public void Save(DeviceContext context)
        {
            m_Topology = context.InputAssembler.PrimitiveTopology;
            m_Layout = context.InputAssembler.InputLayout;
            m_Viewports = context.Rasterizer.GetViewports();
            m_ScissorRectangles = context.Rasterizer.GetScissorRectangles();
            m_RasterizerState = context.Rasterizer.State;
            m_BlendState = context.OutputMerger.GetBlendState(out m_BlendFactor, out m_SampleMaskRef);
            m_DepthState = context.OutputMerger.GetDepthStencilState(out m_StencilRefRef);
            m_RenderTargetView = context.OutputMerger.GetRenderTargets(1, out m_DepthStencilView);

            m_Ps = context.PixelShader.Get();
            m_PsConstantBuffers = context.PixelShader.GetConstantBuffers(0, 4);
            m_PsSamplers = context.PixelShader.GetSamplers(0, 4);
            m_PsResources = context.PixelShader.GetShaderResources(0, 4);

            m_Vs = context.VertexShader.Get();
            m_VsConstantBuffers = context.VertexShader.GetConstantBuffers(0, 4);
            m_VsSamplers = context.VertexShader.GetSamplers(0, 4);
            m_VsResources = context.VertexShader.GetShaderResources(0, 4);

            context.InputAssembler.GetIndexBuffer(out m_Ib, out m_IbFormat, out m_IbOffset);
            context.InputAssembler.GetVertexBuffers(0, 1, m_Vb, m_VbStride, m_VbOffset);
        }

        #endregion
    }
}