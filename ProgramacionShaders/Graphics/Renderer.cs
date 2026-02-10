using OpenTK.Mathematics;

public sealed class Renderer
{
    public Matrix4 View = Matrix4.Identity;
    public Matrix4 Projection = Matrix4.Identity;

    public void Draw(Renderable obj)
    {
        obj.Material.Bind();

        // MVP por instancia
        Matrix4 mvp = Projection * View * obj.ModelMatrix;
        obj.Material.Shader.SetMatrix4("uMVP", mvp);

        obj.Mesh.Draw();
    }
}