/*using System;
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
}*/

//----------------------------------------------------------------------------------------------------------------

﻿using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

class Game : GameWindow
{
    // Mesh: encapsula VAO/VBO/EBO y el DrawElements.
    Mesh _mesh;

    // Shader: programa de GPU (vertex + fragment).
    Shader _shader;

    // Matrices del pipeline:
    // Model: transforma el objeto (mover/rotar/escalar).
    // View: transforma la escena según la cámara (LookAt).
    // Projection: define la lente (perspectiva: FOV, aspect, near/far).
    Matrix4 _model;
    Matrix4 _view;
    Matrix4 _projection;

    public Game(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) {}

    protected override void OnLoad()
    {
        base.OnLoad();

        // 1) Viewport: define el área donde OpenGL va a dibujar dentro de la ventana.
        // Si no lo configuras, puedes terminar rasterizando en un área incorrecta.
        GL.Viewport(0, 0, Size.X, Size.Y);

        // 2) Color de fondo (se usa al limpiar el framebuffer).
        GL.ClearColor(0.1f, 0.1f, 0.1f, 1f);

        // 3) Estado OpenGL: para este ejemplo simple desactivamos:
        // - CullFace: evita confusiones de “cara frontal/trasera”. Vemos ambas.
        // - DepthTest: como solo dibujamos 1 quad plano, no es necesario.
        GL.Disable(EnableCap.CullFace);
        GL.Disable(EnableCap.DepthTest);

        // 4) Geometría: un quad en el plano XY, centrado en el origen.
        // Cada vértice tiene 2 floats: (x, y).
        float[] verts =
        {
            // x,    y
            -0.5f,  0.5f,  // 0: esquina superior izquierda
             0.5f,  0.5f,  // 1: esquina superior derecha
            -0.5f, -0.5f,  // 2: esquina inferior izquierda
             0.5f, -0.5f   // 3: esquina inferior derecha
        };

        // 5) Índices: dos triángulos que forman el quad:
        // Triángulo 1: (0, 2, 1)
        // Triángulo 2: (2, 3, 1)
        uint[] idx = { 0, 2, 1, 2, 3, 1 };

        // 6) Crear Mesh:
        // stride = 2 floats por vértice (x,y) => 2 * sizeof(float)
        // setupAttribs define cómo se leen los atributos desde el VBO.
        _mesh = new Mesh(verts, idx, 2 * sizeof(float), () =>
        {
            // Atributo 0 en el vertex shader:
            // - size = 2 (vec2)
            // - tipo = float
            // - stride = 2 floats
            // - offset = 0
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        });

        // 7) Cargar shaders desde archivos.
        // basic.vert debe declarar: layout(location=0) in vec2 aPos; uniform mat4 uMVP;
        // basic.frag pinta un color sólido.
        _shader = new Shader("Shaders/basic.vert", "Shaders/basic.frag");

        // 8) Matriz Model: identidad (no transformamos el quad).
        _model = Matrix4.Identity;

        // 9) Matriz View: cámara en (0,0,5) mirando al origen.
        // Esto crea una “cámara clásica” viendo el quad desde el eje Z positivo.
        _view = Matrix4.LookAt(
            new Vector3(8, 0, 5),
            Vector3.Zero,
            Vector3.UnitY
        );

        // 10) Matriz Projection: perspectiva.
        // FOV: 60 grados, aspect: ancho/alto, near: 0.1, far: 100.
        // (Near debe ser > 0 en perspectiva.)
        _projection = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(60f),
            Size.X / (float)Size.Y,
            0.1f,
            100f
        );
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        // 11) Limpiar el framebuffer (pinta el fondo con el ClearColor).
        GL.Clear(ClearBufferMask.ColorBufferBit);

        // 12) Construir la matriz final que mandamos al shader como uMVP.
        // Importante: aquí estás usando una convención donde el vector se multiplica
        // a la izquierda en el vertex shader (p * uMVP).
        // Por eso el orden en CPU es: Model * View * Projection.
        Matrix4 mvp = _model * _view * _projection;

        // 13) Activar el shader y subir el uniform uMVP.
        _shader.Use();
        _shader.SetMatrix4("uMVP", mvp);

        // 14) Dibujar el mesh (usa su VAO y DrawElements).
        _mesh.Draw();

        // 15) Presentar el frame en pantalla.
        SwapBuffers();
    }

    static void Main()
    {
        using var g = new Game(
            GameWindowSettings.Default,
            new NativeWindowSettings
            {
                Size = new Vector2i(800, 600),
                Title = "Quad en perspectiva (sin textura)"
            }
        );
        g.Run();
    }
}