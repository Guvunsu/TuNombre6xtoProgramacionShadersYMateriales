using OpenTK.Graphics.OpenGL4;

Public sealed class Renderable
{
    public Mesh Mesh { get; }
    public Material Material { get; }
    
    public vector3 Position = vector3.Zero;
    public float rotation = 0f;
    public vector3 Scale = vector3.One;

    public Renderable(Mesh mesh, Material material)
    {
        Mesh = mesh;
        Material = material;
    }
    public Matrix4 ModelMatrix =>
    Matrix4.CreateScale(Scale) *
    Matrix4.CreateRotationZ(rotation) *
    Matrix4.CreateTranslation(Position);
}