using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

public static class Texture
{
    public static int Load2D(string path, bool srgb = false)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Textura no encontrada: {path}");

        ImageResult image;
        using (var stream = File.OpenRead(path))
        {
            image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
        }

        FlipVertical(image.Data, image.Width, image.Height);

        int handle = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, handle);

        PixelInternalFormat internalFormat =
            srgb ? PixelInternalFormat.Srgb8Alpha8
                 : PixelInternalFormat.Rgba8;

        GL.TexImage2D(
            TextureTarget.Texture2D,
            0,
            internalFormat,
            image.Width,
            image.Height,
            0,
            PixelFormat.Rgba,
            PixelType.UnsignedByte,
            image.Data
        );

        // Parámetros
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        GL.BindTexture(TextureTarget.Texture2D, 0);

        return handle;
    }

    private static void FlipVertical(byte[] data, int width, int height)
    {
    int stride = width * 4;
    byte[] row = new byte[stride];

    for (int y = 0; y < height / 2; y++)
    {
        int top = y * stride;
        int bottom = (height - 1 - y) * stride;

        System.Buffer.BlockCopy(data, top, row, 0, stride);
        System.Buffer.BlockCopy(data, bottom, data, top, stride);
        System.Buffer.BlockCopy(row, 0, data, bottom, stride);
    }
}

}
