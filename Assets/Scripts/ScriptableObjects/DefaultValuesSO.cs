using UnityEngine;

namespace Game.ScriptableObjects
{
    [CreateAssetMenu(fileName = "DefaultValues", menuName = "Custom/DefaultValues")]
    public class DefaultValuesSO : ScriptableObject
    {
        public float piastresDefault = 0;
        public float doubloonsDefault = 100;

        public float _timeToHint=10;

        public LevelConfigSO levelConfig;
        public LevelConfigRepositorySO levelConfigRepository;
       
        
    }
}
