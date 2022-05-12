using System.IO;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Veldrid.SPIRV;
using System.Text;
using System.Runtime.InteropServices;

namespace cilspirv.test.veldrid
{
    class Program
    {
        private static GraphicsDevice _graphicsDevice;
        private static CommandList _commandList;
        private static DeviceBuffer _vertexBuffer;
        private static DeviceBuffer _indexBuffer;
        private static DeviceBuffer identityBuffer, uniformBuffer;
        private static ResourceSet resourceSet;
        private static Shader[] _shaders;
        private static Pipeline _pipeline;

        static void Main(string[] args)
        {
            WindowCreateInfo windowCI = new WindowCreateInfo()
            {
                X = 100,
                Y = 100,
                WindowWidth = 960,
                WindowHeight = 540,
                WindowTitle = "Veldrid Tutorial"
            };
            Sdl2Window window = VeldridStartup.CreateWindow(ref windowCI);

            GraphicsDeviceOptions options = new GraphicsDeviceOptions
            {
                PreferStandardClipSpaceYDirection = true,
                PreferDepthRangeZeroToOne = true
            };

            _graphicsDevice = VeldridStartup.CreateGraphicsDevice(window, options, GraphicsBackend.Vulkan);

            CreateResources();

            while (window.Exists)
            {
                window.PumpEvents();

                if (window.Exists)
                {
                    Draw();
                }
            }

            DisposeResources();
        }

        private static void CreateResources()
        {
            ResourceFactory factory = _graphicsDevice.ResourceFactory;

            VertexPositionColor[] quadVerticesOld =
            {
                new VertexPositionColor(new Vector2(-.75f, .75f), RgbaFloat.Red),
                new VertexPositionColor(new Vector2(.75f, .75f), RgbaFloat.Green),
                new VertexPositionColor(new Vector2(-.75f, -.75f), RgbaFloat.Blue),
                new VertexPositionColor(new Vector2(.75f, -.75f), RgbaFloat.Yellow)
            };
            SumVertex[] quadVertices =
            {
                new SumVertex(new Vector3(-.75f, .75f, 0), new(0f, 1f), RgbaFloat.Red),
                new SumVertex(new Vector3(.75f, .75f, 0), new(1f, 1f), RgbaFloat.Green),
                new SumVertex(new Vector3(-.75f, -.75f, 0), new(0f, 0f), RgbaFloat.Blue),
                new SumVertex(new Vector3(.75f, -.75f, 0), new(1f, 0f), RgbaFloat.Yellow)
            };
            BufferDescription vbDescription = new BufferDescription(
                4 * SumVertex.SizeInBytes,
                BufferUsage.VertexBuffer);
            _vertexBuffer = factory.CreateBuffer(vbDescription);
            _graphicsDevice.UpdateBuffer(_vertexBuffer, 0, quadVertices);

            ushort[] quadIndices = { 0, 1, 2, 3 };
            BufferDescription ibDescription = new BufferDescription(
                4 * sizeof(ushort),
                BufferUsage.IndexBuffer);
            _indexBuffer = factory.CreateBuffer(ibDescription);
            _graphicsDevice.UpdateBuffer(_indexBuffer, 0, quadIndices);

            VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
                new VertexElementDescription("UV", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4));

            ShaderDescription vertexShaderDesc = new ShaderDescription(
                ShaderStages.Vertex,
                File.ReadAllBytes("ExplicitUniform.spv"),
                "Vert");
            ShaderDescription fragmentShaderDesc = new ShaderDescription(
                ShaderStages.Fragment,
                File.ReadAllBytes("ExplicitUniform.spv"),
                "Frag");

            //_shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);
            _shaders = new[]
            {
                factory.CreateShader(vertexShaderDesc),
                factory.CreateShader(fragmentShaderDesc)
            };

            /*var uniforms = new test_shaders.Simple.Uniforms()
            {
                alphaReference = 0.3f,
                tint = Vector4.One,
                tintFactor = 0f,
                vectorColorFactor = 1f
            };*/
            var uniforms = new test_shaders.ExplicitUniform.Uniforms()
            {
                actualColor = new Vector4(1f, 1f, 1f, 1f)
            };
            uniformBuffer = factory.CreateBuffer(new BufferDescription((uint)test_shaders.ExplicitUniform.Uniforms.Size, BufferUsage.UniformBuffer));
            _graphicsDevice.UpdateBuffer(uniformBuffer, 0, ref uniforms);

            identityBuffer = factory.CreateBuffer(new BufferDescription((uint)(4 * 4 * sizeof(float)), BufferUsage.UniformBuffer));
            var identity = Matrix4x4.Identity;
            _graphicsDevice.UpdateBuffer(identityBuffer, 0, ref identity);

            /*var resourceLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("B0", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment),
                new ResourceLayoutElementDescription("B1", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment),
                new ResourceLayoutElementDescription("projection", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("view", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("world", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("uniforms", ResourceKind.UniformBuffer, ShaderStages.Fragment)));
            resourceSet = factory.CreateResourceSet(new ResourceSetDescription(
                resourceLayout, identityBuffer, identityBuffer, identityBuffer, identityBuffer, identityBuffer, uniformBuffer));*/

            var resourceLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("Uniforms", ResourceKind.UniformBuffer, ShaderStages.Fragment)));
            resourceSet = factory.CreateResourceSet(new ResourceSetDescription(
                resourceLayout, uniformBuffer));

            // Create pipeline
            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
            pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;
            pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
                depthTestEnabled: true,
                depthWriteEnabled: true,
                comparisonKind: ComparisonKind.LessEqual);
            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.Back,
                fillMode: PolygonFillMode.Solid,
                frontFace: FrontFace.Clockwise,
                depthClipEnabled: true,
                scissorTestEnabled: false);
            pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            pipelineDescription.ResourceLayouts = new[] { resourceLayout };
            //pipelineDescription.ResourceLayouts = System.Array.Empty<ResourceLayout>();
            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
                shaders: _shaders);
            pipelineDescription.Outputs = _graphicsDevice.SwapchainFramebuffer.OutputDescription;

            _pipeline = factory.CreateGraphicsPipeline(pipelineDescription);

            _commandList = factory.CreateCommandList();
        }

        private static void Draw()
        {
            // Begin() must be called before commands can be issued.
            _commandList.Begin();

            // We want to render directly to the output window.
            _commandList.SetFramebuffer(_graphicsDevice.SwapchainFramebuffer);
            _commandList.ClearColorTarget(0, RgbaFloat.Black);

            // Set all relevant state to draw our quad.
            _commandList.SetVertexBuffer(0, _vertexBuffer);
            _commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
            _commandList.SetPipeline(_pipeline);
            _commandList.SetGraphicsResourceSet(0, resourceSet);
            // Issue a Draw command for a single instance with 4 indices.
            _commandList.DrawIndexed(
                indexCount: 4,
                instanceCount: 1,
                indexStart: 0,
                vertexOffset: 0,
                instanceStart: 0);

            // End() must be called before commands can be submitted for execution.
            _commandList.End();
            _graphicsDevice.SubmitCommands(_commandList);

            // Once commands have been submitted, the rendered image can be presented to the application window.
            _graphicsDevice.SwapBuffers();
        }

        private static void DisposeResources()
        {
            _pipeline.Dispose();
            foreach (Shader shader in _shaders)
            {
                shader.Dispose();
            }
            _commandList.Dispose();
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
            _graphicsDevice.Dispose();
        }
    }

    struct VertexPositionColor
    {
        public const uint SizeInBytes = 24;
        public Vector2 Position;
        public RgbaFloat Color;
        public VertexPositionColor(Vector2 position, RgbaFloat color)
        {
            Position = position;
            Color = color;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = sizeof(float))]
    struct SumVertex
    {
        public const uint SizeInBytes = (3 + 2 + 4) * sizeof(float);
        public Vector3 Position;
        public Vector2 UV;
        public RgbaFloat Color;

        public SumVertex(Vector3 position, Vector2 uv, RgbaFloat color)
        {
            Position = position;
            UV = uv;
            Color = color;
        }
    }
}
