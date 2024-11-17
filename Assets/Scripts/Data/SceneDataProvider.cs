#nullable enable

using System;

namespace Core.Data
{
    public class SceneDataProvider : DataProviderBase
    {
        public static SceneDataProvider? Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
