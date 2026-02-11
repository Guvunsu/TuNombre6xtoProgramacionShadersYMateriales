using OpenTK.Windowing.Desktop;
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