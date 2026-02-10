using OpenTK.Mathematics;

public sealed class Camera
{
    public Matrix4 View { get; private set; } = Matrix4.Identity;
    public Matrix4 Projection { get; private set; } = Matrix4.Identity;

    // Cámara en perspectiva mirando a un punto
    public void SetPerspectiveLookAt(
        Vector3 position,
        Vector3 target,
        Vector3 up,
        float fovDegrees,
        float aspect,
        float near,
        float far)
    {
        View = Matrix4.LookAt(position, target, up);

        Projection = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(fovDegrees),
            aspect,
            near,
            far
        );
    }
}