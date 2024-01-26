using System;

namespace KBot.Util
{
    public static class UMath
    {
        public static float Deg2Rad(float deg)
        {
            return (float)((deg * Math.PI) / 180);
        }

        public static float Rad2Deg(float rad)
        {
            return (float)((rad * 180f) / Math.PI);
        }

        public static float Constain(float value, float min, float max)
        {
            if (value <= min) return min;
            if (value >= max) return max;
            return value;
        }
    }
}
