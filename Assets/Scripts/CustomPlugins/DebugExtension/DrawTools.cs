using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DebugExtension
{
    public static class DrawTools
    {
        public static void DrawCircle(Vector2 center, float radius, Color color, float duration = 0f, int segments = 30)
        {
            // 圓形精度
            float angleStep = 360f / segments;
            
            for (int i = 0; i < segments; i++)
            {
                float angleA = angleStep * i * Mathf.Deg2Rad;
                float angleB = angleStep * (i + 1) * Mathf.Deg2Rad;

                Vector2 pointA = center + new Vector2(Mathf.Cos(angleA), Mathf.Sin(angleA)) * radius;
                Vector2 pointB = center + new Vector2(Mathf.Cos(angleB), Mathf.Sin(angleB)) * radius;

                if (duration == 0f) Debug.DrawLine(pointA, pointB, color);
                else Debug.DrawLine(pointA, pointB, color, duration);
            }
        }

        public static void DrawBounds(Vector2 center, Vector2 size, Vector2 offset, Color color, float duration = 0f)
        {
            center += offset;
            
            // 方框尺寸的一半
            Vector2 halfSize = size * 0.5f;

            // 四個角落的世界座標
            Vector2 topLeft     = center + new Vector2(-halfSize.x,  halfSize.y);
            Vector2 topRight    = center + new Vector2( halfSize.x,  halfSize.y);
            Vector2 bottomRight = center + new Vector2( halfSize.x, -halfSize.y);
            Vector2 bottomLeft  = center + new Vector2(-halfSize.x, -halfSize.y);

            // 使用 Debug.DrawLine 畫出方框（順時針)
            if (duration == 0f)
            {
                Debug.DrawLine(topLeft, topRight, color);
                Debug.DrawLine(topRight, bottomRight, color);
                Debug.DrawLine(bottomRight, bottomLeft, color);
                Debug.DrawLine(bottomLeft, topLeft, color);
            }
            else 
            {
                Debug.DrawLine(topLeft, topRight, color, duration);
                Debug.DrawLine(topRight, bottomRight, color, duration);
                Debug.DrawLine(bottomRight, bottomLeft, color, duration);
                Debug.DrawLine(bottomLeft, topLeft, color, duration);
            }
        }

        public static void DrawCone(Vector3 center, Vector3 forward, float radius, float angleRange, Color color, float duration = 0f, int segments = 30)
        {
            float halfAngle = angleRange / 2f;
            float startAngle = -halfAngle;
            float angleIncrement = angleRange / segments;
            Vector3 startDir = forward.normalized;

            for (int i = 0; i < segments; i++)
            {
                float angleA = startAngle + angleIncrement * i;
                float angleB = startAngle + angleIncrement * (i + 1);

                Vector3 dirA = Quaternion.Euler(0, 0, angleA) * startDir;
                Vector3 dirB = Quaternion.Euler(0, 0, angleB) * startDir;

                Vector3 pointA = center + dirA * radius;
                Vector3 pointB = center + dirB * radius;

                if (duration == 0f) Debug.DrawLine(pointA, pointB, color);
                else Debug.DrawLine(pointA, pointB, color, duration);
            }


            // 畫中心到左右邊界
            Vector3 left = center + Quaternion.Euler(0, 0, -halfAngle) * startDir * radius;
            Vector3 right = center + Quaternion.Euler(0, 0, halfAngle) * startDir * radius;
            if (duration == 0f)
            {
                Debug.DrawLine(center, left, color);
                Debug.DrawLine(center, right, color);
            }
            else
            {
                Debug.DrawLine(center, left, color, duration);
                Debug.DrawLine(center, right, color, duration);
            }
        }
    }

}
