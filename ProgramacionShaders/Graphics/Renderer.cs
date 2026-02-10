using OpenTk.Graphics.OpenGL4;
//Renderer.cs se encarga de renderizar los objetos en la escena,
//tomando el Mesh lo dibuja usando el Material para configurar el shader y la textura,
//ademas de configurar las matrices de transformacion en el shader para posicionar los objetos en la escena
public sealed class Renderer
{
    public Matrix4 view = matrix4.Identity;
    public Matrix4 projection = matrix4.Identity;

    public void Draw(Renderable obj)
    {
        obj.Material.Bind();

        //mvp por instancia
        matrix4 mvp = projection * view * obj.ModelMatrix;
        obj.Material.Shader.SetMatrix4("uMVP", mvp);
        obj.Mesh.Draw();
    }
}