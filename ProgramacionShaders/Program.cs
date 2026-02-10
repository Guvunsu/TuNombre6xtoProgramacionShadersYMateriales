/*using System;
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
    private float _time; // acumulador de tiempo
    private Shader _shader;
    private Texture _texture;

      private int _vao2; // vertex array object 
    private int _vbo2; // vertex buffer object 
    private int _ebo2; // element buffer object 
    private Texture _texture2;

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
        //pikachu
        float[]vertices2={
         0.5f,0.5f,0f,1f,
          1.5f,0.5f,1f,1f,
          0.5f,-0.5f,0f,0f,
          1.5f,-0.5f,1f,0f,
          
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

        //Pikachu
         _vao2 = GL.GenVertexArray();
        _vbo2 = GL.GenBuffer();
        _ebo2 = GL.GenBuffer();

        //Activamos el VAO 
        GL.BindVertexArray(_vao2);
         //Para el VBO, hacemos los "Blind"/ enlaces de datos con la GOU y los shaders.
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo2);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices2.Length * sizeof(float), vertices2, BufferUsageHint.StaticDraw);

   
             //Iinicializamos el EBO
        //Contiene los indices de como leer los vertices. El EBO se guarda en el VAO
        // VAO es donde el arrayObject guarda e inicializa los vertices
        //se hace espacio de meomoria de la gpu el cual tiene la cantidad de indices 
        //Contiene los indices de como leer los vertices. El EBO se guarda en el VAO
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo2);
        GL.BufferData(BufferTarget.ElementArrayBuffer,indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

         GL.BindVertexArray(_vao2);

        //Atributos:
        //layout (0) -> aPos(vec2)
        //layout (1) -> aUV(vec2)
        int stride = 4 * sizeof(float);

        // VAO sabe como leer el VBO
        // los blindiamos 
        //Posiciones 1: Colores 
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false,stride, 0);
        GL.EnableVertexAttribArray(0);

        //Atributo 2: Colores 
         GL.VertexAttribPointer(1,2 , VertexAttribPointerType.Float, false,stride, 2* sizeof(float));
        GL.EnableVertexAttribArray(1); 

        //sHADERS (VERTEX CON MVP WHICH MEANS MODELOS VISTA Y PROYECCION)
         _shader = new Shader("Shaders/Textured_mvp.vert", "Shaders/textured.frag");

         //Textura
         _texture =new Texture("Textures/LenaForsen.png");
            //pikachu Textura
            _texture2 =new Texture("Textures/Pikachu.jpg");

         //conectar sampler con texture unit 0 
           _shader.Use();
           _shader.SetInt("uTex",0);
       /* _texture = new Texture("Textures/LenaForsen.png");*/
        /*_shader = new Shader("Shaders/textured.vert", "Shaders/textured.frag");*/
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit);
//1) construir matrices

// model: rotacion 2d + un poquito de escala opcional
var model = Matrix4.CreateRotationZ(_time)* Matrix4.CreateScale(1.0f);
var model2 = Matrix4.CreateRotationZ(-1*_time)* Matrix4.CreateScale(1.0f);
//view : identidad (no camara todavia)
var view = Matrix4.Identity;


//projection: Ortho para 2d (encaja bien con el quad en [-1,1])
//left,right,bottom,top,znear,zfar
var proj = Matrix4.CreateOrthographicOffCenter(-1f,1f,-1f,1f,-1f,1f);

//orden tipico para OPENGL: MVP =model *view *proj o proj * view * model segun convencion
//con esta configuracion (y el shader umpvp * vec4),suele ir bien con:
var mvp = model * view * proj;

//2) enviar uniforme al shader
        _shader.Use();
       /* _shader.SetInt("uTex",0);*/
       _shader.SetMatrix4("uMVP", mvp);
      
       //3) bind textura en texture0 y dibujar 
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
        //Pikachu
        var mvp2 = model2 * view * proj;
        _shader.Use();
       _shader.SetMatrix4("uMVP", mvp2);
       _texture2.Use(TextureUnit.Texture0);
        GL.BindVertexArray(_vao2);
        //utilizando el shader dibujame los tringulos
                                                   //aqui recibe un ebo porque ya 
                                                   //tiene mas de 3 triangulos
                                            //recibir
                                            //+6 vertices 
                                                                                //donde inicia el
                                                                                //indice el ebo 
        GL.DrawElements(PrimitiveType.Triangles, 6 , DrawElementsType.UnsignedInt,0);


        SwapBuffers();
    }protected override void OnUpdateFrame(FrameEventArgs e){
        base.OnUpdateFrame(e);
        //tiempo acumulado(segundos)
        _time+=(float)e.Time;
    }
    protected override void OnUnload()
    {
        base.OnUnload();
        GL.DeleteBuffer(_ebo);
        GL.DeleteBuffer(_vbo);
        GL.DeleteVertexArray(_vao);

        GL.DeleteBuffer(_ebo2);
        GL.DeleteBuffer(_vbo2);
        GL.DeleteVertexArray(_vao2);
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
*/

-------------------------------------------------------------------------------------------------------------------------


﻿using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;

class Game : GameWindow
{
    // =========================
    // 1) Recursos compartidos
    // =========================
    // La idea “motorcito” es:
    // - Mesh: geometría (quad) que se reutiliza
    // - Shader: programa GPU que se reutiliza
    // - Renderer: contiene View/Projection y sabe dibujar Renderables
    private Mesh _quadMesh;
    private Shader _shader;
    private Renderer _renderer;

    // =========================
    // 2) Recursos por material
    // =========================
    // Cada material = (shader + textura) en el caso simple
    private Texture _texA, _texB, _texC;
    private Material _matA, _matB, _matC;

    // =========================
    // 3) Lista de instancias (objetos en escena)
    // =========================
    // Cada Renderable tiene:
    // - Mesh (compartido)
    // - Material (puede ser distinto)
    // - Transform (Position/Rotation/Scale)
    private readonly List<Renderable> _objects = new();

    private float _time;

    public Game(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) { }

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(0.08f, 0.08f, 0.10f, 1f);

        // ============================================================
        // A) Crear la geometría UNA SOLA VEZ (Mesh compartido)
        // ============================================================
        // Quad con layout: (x, y, u, v) => 4 floats por vértice
        float[] vertices =
        {
            // x,     y,     u,   v
            -0.5f,  0.5f,  0f,  1f, // v0 (top-left)
             0.5f,  0.5f,  1f,  1f, // v1 (top-right)
            -0.5f, -0.5f,  0f,  0f, // v2 (bottom-left)
             0.5f, -0.5f,  1f,  0f  // v3 (bottom-right)
        };

        // 2 triángulos -> 6 índices
        uint[] indices =
        {
            0, 2, 1,
            2, 3, 1
        };

        int stride = 4 * sizeof(float);

        // Creamos el Mesh, y le decimos cómo configurar el VAO (atributos)
        // OJO: aquí es donde conectas el VBO con el shader:
        // location 0 -> aPos (vec2)
        // location 1 -> aUV  (vec2)
        _quadMesh = new Mesh(vertices, indices, stride, setupAttribs: () =>
        {
            // location 0 -> aPos (vec2)
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(0);

            // location 1 -> aUV (vec2)
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 2 * sizeof(float));
            GL.EnableVertexAttribArray(1);
        });

        // ============================================================
        // B) Shader compartido (uno para todos los objetos)
        // ============================================================
        _shader = new Shader("Shaders/lit_flat.vert", "Shaders/lit_flat.frag");

        // ============================================================
        // C) Texturas (una por “material”)
        // ============================================================
        // Cambia los nombres según tus archivos reales en Textures/
        _texA = new Texture("Textures/Lenna.png");
        _texB = new Texture("Textures/Lenna.png");
        _texC = new Texture("Textures/Lenna.png");

        // ============================================================
        // D) Material = Shader + Texture
        // ============================================================
        // Importante: el shader se comparte, lo que cambia es la textura
        _matA = new Material(_shader, _texA);
        _matB = new Material(_shader, _texB);
        _matC = new Material(_shader, _texC);

        // ============================================================
        // E) Renderer: define “cámara” (View) y Projection
        // ============================================================
        _renderer = new Renderer
        {
            View = Matrix4.Identity,
            Projection = Matrix4.CreateOrthographicOffCenter(-1f, 1f, -1f, 1f, -1f, 1f)
        };

        _shader.Use();
        _shader.SetVector3("uLightColor", new Vector3(1.0f, 0.0f, 0.0f));
         _shader.SetVector3("uBaseColor", new Vector3(1.0f, 1.0f, 1.0f));
         _shader.SetFloat("uAmbient", 0.5f); // conectar sampler2D uTex con texture unit 0


        // ============================================================
        // F) Crear instancias (Renderables)
        // ============================================================
        // MISMO mesh, diferente material (textura) y diferente transform
        _objects.Add(new Renderable(_quadMesh, _matA)
        {
            Position = new Vector3(-0.7f, 0.0f, 0f),
            Scale = new Vector3(0.5f, 0.5f, 1f)
        });

        _objects.Add(new Renderable(_quadMesh, _matB)
        {
            Position = new Vector3(0.0f, 0.0f, 0f),
            Scale = new Vector3(0.5f, 0.5f, 1f)
        });

        _objects.Add(new Renderable(_quadMesh, _matC)
        {
            Position = new Vector3(0.7f, 0.0f, 0f),
            Scale = new Vector3(0.5f, 0.5f, 1f)
        });

        // Nota didáctica:
        // - Mesh = geometría reutilizable
        // - Material = apariencia (textura)
        // - Renderable = “objeto en escena” (transform + mesh + material)
        // - Renderer = “dibujador” que aplica MVP + draw
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);
        _time += (float)e.Time;

        // Animación simple: cada objeto rota distinto
        if (_objects.Count >= 3)
        {
            _objects[0].RotationZ = _time;
            _objects[1].RotationZ = -_time * 0.7f;
            _objects[2].RotationZ = _time * 1.3f;
        }
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        // Dibujar todos los objetos:
        // Cada llamada hace:
        // - Bind material (shader + textura)
        // - Set uMVP (por objeto)
        // - Mesh.Draw()
        foreach (var obj in _objects)
            _renderer.Draw(obj);

        SwapBuffers();
    }

    protected override void OnUnload()
    {
        base.OnUnload();

        // Limpieza:
        // - Mesh (VAO/VBO/EBO)
        // - Texturas
        // - Shader program
        _quadMesh.Dispose();

        _texA.Dispose();
        _texB.Dispose();
        _texC.Dispose();

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
            Title = "Instancias: mismo mesh, diferente textura",
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

