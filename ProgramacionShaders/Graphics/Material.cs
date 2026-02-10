using OpenTK.Graphics.OpenGL4;

public sealed class Material
{
    public Shader Shader { get; }
    public Texture Texture { get; }

    public Material(Shader shader, Texture texture)
    {
        Shader = shader;
        Texture = texture;
    }

    public void Bind()
    {
        Shader.Use();
        Texture.Use(TextureUnit.Texture0);
        // Esto puedes hacerlo 1 vez al cargar si siempre usas Texture0,
        // pero no pasa nada por dejarlo aquí.
        Shader.SetInt("uTex", 0);
    }
}

