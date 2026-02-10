using OpenTK.Graphics.OpenGL4;

public sealed class Mesh : IDisposable
{
    private readonly int _vao, _vbo, _ebo;
    private readonly int _indexCount;

    public Mesh(float[] vertices, uint[] indices, int strideBytes, Action setupAttribs)
    {
        _indexCount = indices.Length;

        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();

        GL.BindVertexArray(_vao);

        // VBO
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(
            BufferTarget.ArrayBuffer,
            vertices.Length * sizeof(float),
            vertices,
            BufferUsageHint.StaticDraw
        );

        // EBO
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(
            BufferTarget.ElementArrayBuffer,
            indices.Length * sizeof(uint),
            indices,
            BufferUsageHint.StaticDraw
        );

        // 🔴 GARANTIZAR VBO ACTIVO para los atributos
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);

        // Atributos (capturan estado del VAO)
        setupAttribs.Invoke();

        // Limpieza
        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    public void Draw()
    {
        GL.BindVertexArray(_vao);
        GL.DrawElements(
            PrimitiveType.Triangles,
            _indexCount,
            DrawElementsType.UnsignedInt,
            0
        );
        GL.BindVertexArray(0);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(_ebo);
        GL.DeleteBuffer(_vbo);
        GL.DeleteVertexArray(_vao);
    }
}