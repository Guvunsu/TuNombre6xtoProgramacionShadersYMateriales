using OpenTK.Graphics.OpenGL4;

public sealed class Material
{
    public Shader Shader { get; }

    public int DiffuseTexture { get; }
    public int NormalTexture { get; }

    public Material(Shader shader, int diffuse, int normal)
    {
        Shader = shader;
        DiffuseTexture = diffuse;
        NormalTexture = normal;
    }

    public void Bind()
    {
        Shader.Use();

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, DiffuseTexture);
        Shader.SetInt("uDiffuse", 0);

        GL.ActiveTexture(TextureUnit.Texture1);
        GL.BindTexture(TextureTarget.Texture2D, NormalTexture);
        Shader.SetInt("uNormalMap", 1);
    }
}
