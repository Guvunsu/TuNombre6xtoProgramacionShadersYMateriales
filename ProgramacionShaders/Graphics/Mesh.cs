using OpenTK.Graphics.OpenGL4;
//construye la geometria de un objeto, se encarga de crear 
//los buffers y configurar los atributos de vertice, ademas de 
//tener un metodo para dibujar el mesh y otro para eliminar los buffers cuando ya no se necesiten
public sealed class Mesh
{
    private readonly int Ebo ,Vao ,Vbo  ;
    private readonly int indexCount;

    public Mesh(float[] vertices, uint[] indices, int strideBytes, Action setupAttribs)
    {
        VertexCount = indices.Length;

        ebo = GL.GenBuffer();
        Vao = GL.GenVertexArray();
        Vbo = GL.GenBuffer();

        GL.BindVertexArray(Vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, Ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        //configurations de atributos (vao) de vertice, se hace en el constructor para que cada mesh tenga su propia configuracion de atributos
        setupAttribs.invoke();

        // Unbind VAO (optional)
        GL.BindVertexArray(0);
    }

    public void Draw()
    {
        GL.BindVertexArray(Vao);
        GL.DrawElements(PrimitiveType.Triangles, VertexCount, DrawElementsType.UnsignedInt, 0);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(Ebo);
        GL.DeleteBuffer(Vbo);
        GL.DeleteVertexArray(Vao);
    }
}