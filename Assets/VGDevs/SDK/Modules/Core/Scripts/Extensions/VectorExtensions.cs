using UnityEngine;

namespace VGDevs
{
    public static class VectorExtensions
    {
        public static Vector3 X0Y(this Vector2 vector)
        {
            return new Vector3(vector.x, 0f, vector.y);
        }

        public static Vector2 XY(this Vector3 vector)
        {
            return vector;
        }

        public static Vector2 XZ(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        public static Vector2 ZX(this Vector3 vector)
        {
            return new Vector2(vector.z, vector.x);
        }
    }
}
