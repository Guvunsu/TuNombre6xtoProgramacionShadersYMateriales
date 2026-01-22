using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;

class Game : GameWindow
{
    // ID used by OpenGL
    private int _vao; // vertex array object 
    private int _vbo; // vertex buffer object 
    private int _ebo; // element buffer object 
    private Shader _shader;
    private Texture _texture;

    public Game(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) { }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.1f, 0.1f, 0.15f, 1f);
        // 4 vertices para generar un Quad 
        //Un Quad no es un primitiva, un Quad son 2 triangulos 
        //-----------------
        //4 flotantes por vertice (x,y) & (u,v)
        float[] vertices =
        {               //color
           -0.5f, 0.5f,  0f,1f/*,0f*/,    // v0: izq, arriba
           0.5f, 0.5f,  1f,1f/*,0f*/,    // v1: der, arriba
           -0.5f, -0.5f,  0f,0f/*,0f*/,   // v2: izq, abajo
           0.5f, -0.5f,   1f,0f,/*,0f*/    // v3: der, abajo 
        };

        uint[] indices = {
            // son los 2 triangulos del quad
            //indicando sus posiciones y su frontera 
            0,2,1, //Triangulo 1
            2,3,1  //Triangulo 2
         };

        // Creamos/Generamos los objectos de OpenGL
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();

        //Activamos el VAO 
        GL.BindVertexArray(_vao);
        
        //Para el VBO, hacemos los "Blind"/ enlaces de datos con la GOU y los shaders.
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        //Iinicializamos el EBO
        //Contiene los indices de como leer los vertices. El EBO se guarda en el VAO
        // VAO es donde el arrayObject guarda e inicializa los vertices
        //se hace espacio de meomoria de la gpu el cual tiene la cantidad de indices 
        //Contiene los indices de como leer los vertices. El EBO se guarda en el VAO
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer,indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        // VAO sabe como leer el VBO
        // los blindiamos 
        //Posiciones 1: Colores 
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        //Atributo 2: Colores 
         GL.VertexAttribPointer(1,2 , VertexAttribPointerType.Float, false, 4* sizeof(float),2* sizeof(float));
        GL.EnableVertexAttribArray(1); 

        _texture = new Texture("Textures/LenaForsen.png");
        _shader = new Shader("Shaders/textured.vert", "Shaders/textured.frag");
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit);

        _shader.Use();
        _shader.SetInt("uTex",0);
        _texture.Use(TextureUnit.Texture0);
        //ya que sabe leer el vao le pasamos los datos 
        // Contiene VAO ya trae el VBO + EBO = al formato de como se tiene que leer
        GL.BindVertexArray(_vao);
        //utilizando el shader dibujame los tringulos
                                                   //aqui recibe un ebo porque ya 
                                                   //tiene mas de 3 triangulos
                                            //recibir
                                            //+6 vertices 
                                                                                //donde inicia el
                                                                                //indice el ebo 
        GL.DrawElements(PrimitiveType.Triangles, 6 , DrawElementsType.UnsignedInt,0);

        SwapBuffers();
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        GL.DeleteBuffer(_ebo);
        GL.DeleteBuffer(_vbo);
        GL.DeleteVertexArray(_vao);
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
            Title = "E02 - Triangle",
            Size = new Vector2i(800, 600),
            API = ContextAPI.OpenGL,
            APIVersion = new Version(3, 3),
            Profile = ContextProfile.Core,
            Flags = ContextFlags.ForwardCompatible
        };

        using var game = new Game(gws, nws);
        game.Run();
    }
}