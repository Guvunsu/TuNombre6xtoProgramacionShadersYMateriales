using System;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;


// =========================
// ENTRY POINT
// =========================
internal static class Program
{
    private static void Main()
    {
        var gws = GameWindowSettings.Default;

        var nws = new NativeWindowSettings
        {
            Size = new Vector2i(900, 650),
            Title = "Cubo 3D (Shader + Mesh)",
            APIVersion = new Version(3, 3),
            Profile = ContextProfile.Core,
            Flags = ContextFlags.ForwardCompatible
        };

        using var game = new Game(gws, nws);
        game.Run();
    }
}


public sealed class Game : GameWindow
{
    // Layout por vértice: pos(3), normal(3), tangent(3), uv(2) = 11 floats

    private static readonly float[] CubeVertices =
    {
        // +Z (Front) normal (0,0,1), tangent (1,0,0)
        -1, -1,  1,   0, 0, 1,   1, 0, 0,   0, 0,
         1, -1,  1,   0, 0, 1,   1, 0, 0,   1, 0,
         1,  1,  1,   0, 0, 1,   1, 0, 0,   1, 1,
        -1,  1,  1,   0, 0, 1,   1, 0, 0,   0, 1,

        // +X (Right) normal (1,0,0), tangent (0,0,-1)
         1, -1,  1,   1, 0, 0,   0, 0,-1,   0, 0,
         1, -1, -1,   1, 0, 0,   0, 0,-1,   1, 0,
         1,  1, -1,   1, 0, 0,   0, 0,-1,   1, 1,
         1,  1,  1,   1, 0, 0,   0, 0,-1,   0, 1,

        // -Z (Back) normal (0,0,-1), tangent (-1,0,0)
         1, -1, -1,   0, 0,-1,  -1, 0, 0,   0, 0,
        -1, -1, -1,   0, 0,-1,  -1, 0, 0,   1, 0,
        -1,  1, -1,   0, 0,-1,  -1, 0, 0,   1, 1,
         1,  1, -1,   0, 0,-1,  -1, 0, 0,   0, 1,

        // -X (Left) normal (-1,0,0), tangent (0,0,1)
        -1, -1, -1,  -1, 0, 0,   0, 0, 1,   0, 0,
        -1, -1,  1,  -1, 0, 0,   0, 0, 1,   1, 0,
        -1,  1,  1,  -1, 0, 0,   0, 0, 1,   1, 1,
        -1,  1, -1,  -1, 0, 0,   0, 0, 1,   0, 1,

        // +Y (Top) normal (0,1,0), tangent (1,0,0)
        -1,  1,  1,   0, 1, 0,   1, 0, 0,   0, 0,
         1,  1,  1,   0, 1, 0,   1, 0, 0,   1, 0,
         1,  1, -1,   0, 1, 0,   1, 0, 0,   1, 1,
        -1,  1, -1,   0, 1, 0,   1, 0, 0,   0, 1,

        // -Y (Bottom) normal (0,-1,0), tangent (1,0,0)
        -1, -1, -1,   0,-1, 0,   1, 0, 0,   0, 0,
         1, -1, -1,   0,-1, 0,   1, 0, 0,   1, 0,
         1, -1,  1,   0,-1, 0,   1, 0, 0,   1, 1,
        -1, -1,  1,   0,-1, 0,   1, 0, 0,   0, 1,
    };

    private static readonly uint[] CubeIndices =
    {
        0, 1, 2,  2, 3, 0,        // front
        4, 5, 6,  6, 7, 4,        // right
        8, 9,10, 10,11, 8,        // back
       12,13,14, 14,15,12,        // left
       16,17,18, 18,19,16,        // top
       20,21,22, 22,23,20         // bottom
    };

    private Mesh _cube;
    private Shader _shader;

    private int _texDiffuse;
    private int _texNormal;

    private float _angleDeg;
    private Matrix4 _projection;

    public Game(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) { }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.08f, 0.09f, 0.12f, 1f);
        GL.Enable(EnableCap.DepthTest);
        GL.Viewport(0, 0, Size.X, Size.Y);

        _cube = new Mesh(CubeVertices, CubeIndices);

        string baseDir = AppContext.BaseDirectory;

        _shader = new Shader(
            Path.Combine(baseDir, "Shaders", "lit_normal.vert"),
            Path.Combine(baseDir, "Shaders", "lit_normal.frag")
        );

        _texDiffuse = Texture.Load2D(Path.Combine(baseDir, "Textures", "Pikachu.jpg"), srgb: true);
        _texNormal  = Texture.Load2D(Path.Combine(baseDir, "Textures", "Lenna.png"),  srgb: false);

        _projection = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(45f),
            Size.X / (float)Size.Y,
            0.1f, 100f
        );
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, Size.X, Size.Y);

        _projection = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(45f),
            Size.X / (float)Size.Y,
            0.1f, 100f
        );
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        _angleDeg += 60f * (float)e.Time;

        if (KeyboardState.IsKeyDown(Keys.Escape))
            Close();
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        var model =
            Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_angleDeg)) *
            Matrix4.CreateRotationX(MathHelper.DegreesToRadians(_angleDeg * 0.6f));

        var view = Matrix4.LookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.UnitY);

        _shader.Use();

        // Matrices (shader: uProj * uView * uModel)
        _shader.SetMatrix4("uModel", model);
        _shader.SetMatrix4("uView", view);
        _shader.SetMatrix4("uProj", _projection);

        // Luz direccional
        _shader.SetVector3("uLightDir", Vector3.Normalize(new Vector3(0.9f, 0.9f, 0.9f)));
        _shader.SetVector3("uLightColor", new Vector3(0.1f, 0.5f, 0.6f));
        _shader.SetVector3("uBaseColor", new Vector3(0.4f, 0.9f, 0.2f));
        _shader.SetFloat("uAmbient", 0.50f);

        // Texturas
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, _texDiffuse);
        _shader.SetInt("uDiffuse", 0);

        GL.ActiveTexture(TextureUnit.Texture1);
        GL.BindTexture(TextureTarget.Texture2D, _texNormal);
        _shader.SetInt("uNormalMap", 1);

        _cube.Draw();

        SwapBuffers();
    }

    protected override void OnUnload()
    {
        base.OnUnload();

        if (_texDiffuse != 0) GL.DeleteTexture(_texDiffuse);
        if (_texNormal  != 0) GL.DeleteTexture(_texNormal);

        _cube?.Dispose();
        _shader?.Dispose();
    }
}
