using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;

class Game : GameWindow
{
    private Mesh _mesh;
    private Shader _shader;
    private Texture _texture;
    private Material _material;
    private Renderable _obj;
    private Renderer _renderer;

    private float _time;

    public Game(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) { }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.08f, 0.08f, 0.10f, 1f);

        // ========= 1) Quad (pos vec2 + uv vec2) =========
        float[] vertices =
        {
            // x,     y,     u,   v
            -0.6f,  0.6f,  0f,  1f,
             0.6f,  0.6f,  1f,  1f,
            -0.6f, -0.6f,  0f,  0f,
             0.6f, -0.6f,  1f,  0f
        };

        uint[] indices =
        {
            0, 2, 1,
            2, 3, 1
        };

        int stride = 4 * sizeof(float);

        _mesh = new Mesh(vertices, indices, stride, () =>
        {
            // location 0: aPos
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(0);

            // location 1: aUV
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 2 * sizeof(float));
            GL.EnableVertexAttribArray(1);
        });

        // ========= 2) Shader de luz básica =========
        _shader = new Shader("Shaders/lit_flat.vert", "Shaders/lit_flat.frag");

        // ========= 3) Textura y material =========
        _texture = new Texture("Textures/Lenna.png"); // cambia al nombre real
        _material = new Material(_shader, _texture);

        // ========= 4) Un solo objeto =========
        _obj = new Renderable(_mesh, _material)
        {
            Position = Vector3.Zero,
            Scale = Vector3.One,
            RotationZ = 0f
        };

        // ========= 5) Renderer (sin cámara complicada) =========
        _renderer = new Renderer
        {
            View = Matrix4.Identity,
            Projection = Matrix4.CreateOrthographicOffCenter(-1, 1, -1, 1, -1, 1)
        };

        // ========= 6) Uniforms fijos del material/luz =========
        // (Puedes dejarlos aquí porque casi no cambian)
        _shader.Use();
        _shader.SetInt("uTex", 0); // TextureUnit 0
        _shader.SetVector3("uLightColor", new Vector3(1f, 1f, 0f));
        _shader.SetVector3("uBaseColor", new Vector3(1f, 1f, 1f)); // tinte (cámbialo para ver efecto)
        _shader.SetFloat("uAmbient", 1.0f); // 20% de luz ambiente
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        _time += (float)e.Time;

        // (Opcional) rota el objeto para ver cambios (ojo: como la normal es fija, esto no cambia la luz “real” aún)
        // _obj.RotationZ = _time * 0.5f;
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit);

        // ========= Luz girando (para ver el efecto sin cámara) =========
        // Como la normal del plano es (0,0,1), necesitamos una luz con componente Z positiva.
        float a = _time * 0.8f;
        Vector3 lightDir = Vector3.Normalize(new Vector3(MathF.Cos(a), MathF.Sin(a), 1.0f));

        _shader.Use();
        _shader.SetVector3("uLightDir", lightDir);

        // Dibujar
        _renderer.Draw(_obj);

        SwapBuffers();
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        _mesh.Dispose();
        _texture.Dispose();
        _shader.Dispose();
    }
}

class Program
{
    static void Main()
    {
        var gws = GameWindowSettings.Default;

        var nws = new NativeWindowSettings
        {
            Title = "Clase - Luz basica (color * intensidad)",
            Size = new Vector2i(900, 600),
            API = ContextAPI.OpenGL,
            APIVersion = new Version(3, 3),
            Profile = ContextProfile.Core,
            Flags = ContextFlags.ForwardCompatible
        };

        using var game = new Game(gws, nws);
        game.Run();
    }
}