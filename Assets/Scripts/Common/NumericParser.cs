using UnityEngine;

namespace Game.Common
{
    public class NumericParser
    {
        public static float GetFloat(string inValue)
        {
            var outValue = 0.0f;

            if (!string.IsNullOrEmpty(inValue))
            {
                if (float.TryParse(inValue, out float value))
                {
                    outValue = value;
                }
                else
                {
                    Debug.LogError("Failed to parse value string to float");
                }
            }
            else
            {
                Debug.LogError("value data is empty or null");
            }
            return outValue;
        }
    }
}