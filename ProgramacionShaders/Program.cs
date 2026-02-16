using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.IO;
class Game : GameWindow
{
Mesh _mesh = null!;
Shader _shader = null!;
Texture _texture = null!;

    Matrix4 _model;
    Matrix4 _view;
    Matrix4 _projection;

    public Game(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) {}

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.Viewport(0, 0, Size.X, Size.Y);
        GL.ClearColor(0.1f, 0.1f, 0.1f, 1f);

        // Ahora sí necesitamos profundidad (porque es un cubo 3D)
        GL.Enable(EnableCap.DepthTest);

        // Game ya NO conoce vértices → responsabilidad de Mesh
        _mesh = Mesh.CreateCube();

        // Shader leído desde archivos externos
        _shader = new Shader("Shaders/textured_mvp.vert", "Shaders/textured.frag");

        // Textura externa
_texture = new Texture(Path.Combine("Textures", "Pikachu.png"));

        _model = Matrix4.Identity;

        _view = Matrix4.LookAt(
            new Vector3(2, 2, 3),
            Vector3.Zero,
            Vector3.UnitY
        );

        _projection = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(60f),
            Size.X / (float)Size.Y,
            0.1f,
            100f
        );
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // Rotación automática para apreciar el cubo
        _model *= Matrix4.CreateRotationY((float)e.Time);

        Matrix4 mvp = _model * _view * _projection;

        _shader.Use();

        _texture.Use(TextureUnit.Texture0);
        _shader.SetInt("uTex", 0);

        _shader.SetMatrix4("uMVP", mvp);

        _mesh.Draw();

        SwapBuffers();
    }

    static void Main()
    {
        using var g = new Game(
            GameWindowSettings.Default,
            new NativeWindowSettings()
            {
              ClientSize = new Vector2i(800, 600),
                Title = "Cubo Texturizado"
            }
        );

        g.Run();
    }
}
