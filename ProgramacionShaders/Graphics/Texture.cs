using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

public sealed class Texture : IDisposable
{
    public int Handle { get; }

    public Texture(string path)
    {
        Handle = GL.GenTexture();
        Use(TextureUnit.Texture0);

        // Voltear imagen (OpenGL usa UV invertidas)
        StbImage.stbi_set_flip_vertically_on_load(1);

        using var stream = File.OpenRead(path);
        var img = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

        GL.TexImage2D(TextureTarget.Texture2D, 0,
            PixelInternalFormat.Rgba,
            img.Width, img.Height, 0,
            PixelFormat.Rgba,
            PixelType.UnsignedByte,
            img.Data);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
    }

    public void Use(TextureUnit unit)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, Handle);
    }

    public void Dispose()
    {
        GL.DeleteTexture(Handle);
    }
}