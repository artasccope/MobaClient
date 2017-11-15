using UnityEngine;

namespace GameFW.Ultility
{
    /// <summary>
    /// Unity渲染器的扩展
    /// </summary>
    public static class RendererExtensions
    {
        /// <summary>
        /// 测试这个渲染器是否在视锥范围内
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
        public static bool IsVisibleFrom(this Renderer renderer, Camera camera) {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);

            return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
        }
    }
}
