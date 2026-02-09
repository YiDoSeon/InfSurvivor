using System;

namespace Shared.Utils
{
    public static class CMath
    {
        public static int FloorToInt(float value) => (int)MathF.Floor(value);
        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
            {
                return min;
            }
            if (value > max)
            {
                return max;
            }
            return value;
        }
    }
}
