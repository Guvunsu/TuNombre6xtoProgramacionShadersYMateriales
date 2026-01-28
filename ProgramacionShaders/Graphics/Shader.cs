using OpenTK.Graphics.OpenGL4;

public sealed class Shader : IDisposable
{
    public int Handle { get; }

    public Shader(string vertexPath, string fragmentPath)
    {
        string vertexSource = File.ReadAllText(vertexPath);
        string fragmentSource = File.ReadAllText(fragmentPath);

        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexSource);
        GL.CompileShader(vertexShader);
        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int vOk);
        if (vOk == 0) throw new Exception(GL.GetShaderInfoLog(vertexShader));

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentSource);
        GL.CompileShader(fragmentShader);
        GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out int fOk);
        if (fOk == 0) throw new Exception(GL.GetShaderInfoLog(fragmentShader));

        Handle = GL.CreateProgram();
        GL.AttachShader(Handle, vertexShader);
        GL.AttachShader(Handle, fragmentShader);
        GL.LinkProgram(Handle);
        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int linkOk);
        if (linkOk == 0) throw new Exception(GL.GetProgramInfoLog(Handle));

        GL.DetachShader(Handle, vertexShader);
        GL.DetachShader(Handle, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    public void  SetInt(string name, int value){
        int loc = GL.GetUniformLocation(Handle,name);
        GL.Uniform1(loc,value);
    }

    public void SetMatrix4(string name,OpenTK.Mathematics.Matrix4 value){
        int loc = OpenTK.Graphics.OpenGL4.GL.GetUniformLocation(Handle,name);
        OpenTK.Graphics.OpenGL4.GL.UniformMatrix4(loc,false,ref value);
    }

    public void Use() => GL.UseProgram(Handle);

    public void Dispose() => GL.DeleteProgram(Handle);
}