using OpenTK.Mathematics;

public sealed class Renderable
{
    public Mesh Mesh { get; }
    public Material Material { get; }

    public Vector3 Position = Vector3.Zero;
    public float RotationZ = 0f;
    public Vector3 Scale = Vector3.One;

    public Renderable(Mesh mesh, Material material)
    {
        Mesh = mesh;
        Material = material;
    }

    public Matrix4 ModelMatrix =>
        Matrix4.CreateScale(Scale) *
        Matrix4.CreateRotationZ(RotationZ) *
        Matrix4.CreateTranslation(Position);
}