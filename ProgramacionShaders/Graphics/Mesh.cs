using System;
using OpenTK.Graphics.OpenGL4;

public sealed class Mesh : IDisposable
{
    private readonly int _vao;
    private readonly int _vbo;
    private readonly int _ebo;
    private readonly int _indexCount;

    public Mesh(float[] vertices, uint[] indices)
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

        // pos(3), normal(3), tangent(3), uv(2) = 11 floats
        int stride = 11 * sizeof(float);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, stride, 6 * sizeof(float));
        GL.EnableVertexAttribArray(2);

        GL.VertexAttribPointer(3, 2, VertexAttribPointerType.Float, false, stride, 9 * sizeof(float));
        GL.EnableVertexAttribArray(3);

        GL.BindVertexArray(0);
    }


    public void Draw()
    {
        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(_ebo);
        GL.DeleteBuffer(_vbo);
        GL.DeleteVertexArray(_vao);
        GC.SuppressFinalize(this);
    }
}