using UnityEngine;


namespace KFrame.Utilities
{
    /// <summary>
    /// 三角形
    /// </summary>
    public struct Triangle
    {
        /// <summary>
        /// 顶点1
        /// </summary>
        public Vector2 vertex1;

        /// <summary>
        /// 顶点2
        /// </summary>
        public Vector2 vertex2;

        /// <summary>
        /// 顶点3
        /// </summary>
        public Vector2 vertex3;

        public Triangle(Vector2 vertex1, Vector2 vertex2, Vector2 vertex3)
        {
            this.vertex1 = vertex1;
            this.vertex2 = vertex2;
            this.vertex3 = vertex3;
        }
    }

    /// <summary>
    /// 圆形
    /// </summary>
    public struct Circle
    {
        /// <summary>
        ///圆的中心点 
        /// </summary>
        public Vector2 Center;

        /// <summary>
        /// 圆的半径
        /// </summary>
        public float Radius;
    }


    /// <summary>
    /// Koo几何工具
    /// 整理了如多边形内随机坐标点 等API
    /// </summary>
    public static partial class MathTool
    {
        /// <summary>
        /// 获取矩形内随机的一个点
        /// </summary>
        /// <param name="rect">Unity矩形</param>
        /// <returns>随机Vector2坐标点</returns>
        public static Vector2 GetRandomPosInRect(Rect rect)
        {
            Vector2 pos = new Vector2();
            pos.x = Random.Range(0, rect.width) + rect.x;
            pos.y = Random.Range(0, rect.height) + rect.y;

            return pos;
        }

        /// <summary>
        /// 获取BoxCollider2D内随机一点
        /// </summary>
        /// <returns></returns>
        public static Vector2 GetRandomPosInBoxCollider2D(BoxCollider2D collider2D)
        {
            var bounds = collider2D.bounds;
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomY = Random.Range(bounds.min.y, bounds.max.y);
            return new Vector2(randomX, randomY);
        }

        /// <summary>
        /// 随机一点于三角形
        /// </summary>
        /// <param name="triangle"></param>
        /// <returns></returns>
        public static Vector2 RandomInTriangle(Triangle triangle)
        {
            Vector2 pos = new Vector2();
            float r1 = Random.Range(0f, 1f);
            float r2 = Random.Range(0f, 1f);
            pos = (1 - Mathf.Sqrt(r1)) * triangle.vertex1 +
                  Mathf.Sqrt(r1) * (1 - r2) * triangle.vertex2 +
                  Mathf.Sqrt(r1) * r2 * triangle.vertex3;
            return pos;
        }

        /// <summary>
        /// 随机一点于三角形
        /// </summary>
        /// <param name="v1">顶点1</param>
        /// <param name="v2">顶点2</param>
        /// <param name="v3">顶点3</param>
        /// <returns></returns>
        public static Vector2 RandomInTriangle(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            Vector2 pos = new Vector2();
            float r1 = Random.Range(0f, 1f);
            float r2 = Random.Range(0f, 1f);
            pos = (1 - Mathf.Sqrt(r1)) * v1 +
                  Mathf.Sqrt(r1) * (1 - r2) * v2 +
                  Mathf.Sqrt(r1) * r2 * v3;
            return pos;
        }

        /// <summary>
        /// 在圆形区域内随机一个点
        /// </summary>
        public static Vector2 GetRandomPosInCircle(Circle circle)
        {
            //通过极坐标来随机
            float r = Mathf.Sqrt(Random.Range(0, circle.Radius));
            float angle = Random.Range(0, Mathf.PI * 2);
            Vector2 pos = new Vector2(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r);
            pos += circle.Center;
            return pos;
        }
    }
}