using UnityEngine;
using System.Collections.Generic;

namespace Core.Extensions
{
    public static class GameObjectExtensions
    {
        public static T GetComponent<T>(this IEnumerable<GameObject> gameObjects, bool findInChildren = false) where T : Behaviour
        {
            foreach (var obj in gameObjects)
            {
                var er = findInChildren ? obj.GetComponentInChildren<T>() : obj.GetComponent<T>();
                if (er != null)
                {
                    return er;
                }
            }

            return null;
        }

        public static TComponent GetOrAddComponent<TComponent>(this GameObject gameObject) where TComponent : Component
        {
            return gameObject.GetComponent<TComponent>() ?? gameObject.AddComponent<TComponent>();
        }

        public static bool HasComponent<TComponent>(this GameObject gameObject) where TComponent : Component
        {
            return gameObject.GetComponent<TComponent>() != null;
        }

        public static void RemoveAllChildren(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        public static Transform FindWithTag(this Transform transform, string tag, bool findInChildren = true)
        {
            foreach (Transform child in transform)
            {
                if (child.tag == tag)
                {
                    return child;
                }

                if (findInChildren)
                {
                    child.FindWithTag(tag, findInChildren);
                }
            }

            return null;
        }

        public static void SetStatic(this GameObject target, bool isStatic, bool forAllChildren)
        {
            target.isStatic = isStatic;
            if (forAllChildren)
            {
                foreach (Transform child in target.transform)
                {
                    child.gameObject.SetStatic(isStatic, forAllChildren);
                }
            }
        }
    }
}
