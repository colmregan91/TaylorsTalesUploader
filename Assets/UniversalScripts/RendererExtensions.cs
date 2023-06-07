using UnityEngine;

public static class RendererExtensions
{
    static Plane[] planes;
    public static void Init(this Renderer renderer, Camera cam)
    {
        planes = GeometryUtility.CalculateFrustumPlanes(cam);
    }
    public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
    {

        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
}