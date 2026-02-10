using OpenTK.Graphics.OpenGL4;
//Material.cs asocia un shader y una textura,
// y tiene un metodo para bindear ambos al mismo tiempo, 
//ademas de configurar el sampler del shader para que use la textura
public sealed class Material
{
    public Shader shader{get;};
    public Texture texture {get;};

    public Material(Shader shader, Texture texture)
    {
        Shader = shader;
        Texture = texture;
    }

    public void Bind()
    {
        Shader.Use();
        Texture.Use(TextureUnit.Texture0);
        shader.SetInt("uTex", 0); // Asegúrate de que el nombre del sampler en el shader coincida con este valor
    }
}