using OpenTK.Graphics.OpenGL4;

public sealed class Mesh : IDisposable
{
    private readonly int _vao, _vbo, _ebo;
    private readonly int _indexCount;

    private Mesh(float[] vertices, uint[] indices)
    {
        _indexCount = indices.Length;

        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();

        GL.BindVertexArray(_vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        int stride = 5 * sizeof(float);

        // posición (vec3)
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
        GL.EnableVertexAttribArray(0);

        // UV (vec2)
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.BindVertexArray(0);
    }

    // Factory Method → crea el cubo
    public static Mesh CreateCube()
    {
        float[] vertices =
        {
            // posiciones         // UV
            -0.5f,-0.5f, 0.5f, 0f,0f,
             0.5f,-0.5f, 0.5f, 1f,0f,
             0.5f, 0.5f, 0.5f, 1f,1f,
            -0.5f, 0.5f, 0.5f, 0f,1f,

            -0.5f,-0.5f,-0.5f, 1f,0f,
             0.5f,-0.5f,-0.5f, 0f,0f,
             0.5f, 0.5f,-0.5f, 0f,1f,
            -0.5f, 0.5f,-0.5f, 1f,1f,
        };

        uint[] indices =
        {
            0,1,2, 2,3,0,
            1,5,6, 6,2,1,
            5,4,7, 7,6,5,
            4,0,3, 3,7,4,
            3,2,6, 6,7,3,
            4,5,1, 1,0,4
        };

        return new Mesh(vertices, indices);
    }

    public void Draw()
    {
        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, 0);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(_ebo);
        GL.DeleteBuffer(_vbo);
        GL.DeleteVertexArray(_vao);
    }
}