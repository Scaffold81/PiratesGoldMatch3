using UnityEngine;

namespace Game.ScriptableObjects
{
    [CreateAssetMenu(fileName = "DefaultValues", menuName = "Custom/DefaultValues")]
    public class DefaultValuesSO : ScriptableObject
    {
        public float currentPiastresDefault = 0;
        public float piastresDefault = 0;
        public float doubloonsDefault = 100;
        public float hintMarkDefault = 5;
        public float hintMarkCostInDoubloons = 50;
    }
}
