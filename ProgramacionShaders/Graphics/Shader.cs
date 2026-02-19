using System;
using System.IO;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

public sealed class Shader : IDisposable
{
    public int Handle { get; }

    public Shader(string vertexPath, string fragmentPath)
    {
        if (!File.Exists(vertexPath))
            throw new FileNotFoundException($"No se encontró el vertex shader: {vertexPath}");

        if (!File.Exists(fragmentPath))
            throw new FileNotFoundException($"No se encontró el fragment shader: {fragmentPath}");

        string vertexSrc = File.ReadAllText(vertexPath);
        string fragmentSrc = File.ReadAllText(fragmentPath);

        int vs = CompileShader(ShaderType.VertexShader, vertexSrc);
        int fs = CompileShader(ShaderType.FragmentShader, fragmentSrc);

        Handle = GL.CreateProgram();
        GL.AttachShader(Handle, vs);
        GL.AttachShader(Handle, fs);
        GL.LinkProgram(Handle);

        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int status);
        if (status == 0)
        {
            string log = GL.GetProgramInfoLog(Handle);
            throw new Exception($"Error linkeando shader program:\n{log}");
        }

        GL.DetachShader(Handle, vs);
        GL.DetachShader(Handle, fs);
        GL.DeleteShader(vs);
        GL.DeleteShader(fs);
    }

    public void Use() => GL.UseProgram(Handle);

    public void SetMatrix4(string name, Matrix4 value, bool transpose = false)
    {
        int loc = GL.GetUniformLocation(Handle, name);
        GL.UniformMatrix4(loc, transpose, ref value);
    }

    public void SetVector3(string name, OpenTK.Mathematics.Vector3 value)
    {
        int loc = GL.GetUniformLocation(Handle, name);
        GL.Uniform3(loc, value);
    }

    public void SetInt(string name, int value)
    {
        int loc = GL.GetUniformLocation(Handle, name);
        GL.Uniform1(loc, value);
    }

    public void SetFloat(string name, float value)
    {
        int loc = GL.GetUniformLocation(Handle, name);
        GL.Uniform1(loc, value);
    }   

    public void Dispose()
    {
        GL.DeleteProgram(Handle);
    }

    private static int CompileShader(ShaderType type, string src)
    {
        int shader = GL.CreateShader(type);
        GL.ShaderSource(shader, src);
        GL.CompileShader(shader);

        GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);
        if (status == 0)
        {
            string log = GL.GetShaderInfoLog(shader);
            throw new Exception($"Error compilando {type}:\n{log}");
        }

        return shader;
    }
}
