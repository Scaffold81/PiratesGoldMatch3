using System;
using System.Reactive.Disposables;
using UnityEngine;

namespace RxExtensions
{
    public static partial class DisposableExtensions
    {
        /// <summary>Dispose self on target gameObject has been destroyed. Return value is self disposable.</summary>
        public static T AddTo<T>(this T disposable, GameObject gameObject)
            where T : IDisposable
        {
            if (gameObject == null)
            {
                disposable.Dispose();
                return disposable;
            }

            var trigger = gameObject.GetComponent<DestroyTrigger>();
            if (trigger == null)
            {
                trigger = gameObject.AddComponent<DestroyTrigger>();
            }

            trigger.Destroyed.AddListener(disposable.Dispose);

            return disposable;
        }


        /// <summary>Dispose self on target gameObject has been destroyed. Return value is self disposable.</summary>
        public static T AddTo<T>(this T disposable, Component gameObjectComponent)
            where T : IDisposable
        {
            if (gameObjectComponent == null)
            {
                disposable.Dispose();
                return disposable;
            }

            return AddTo(disposable, gameObjectComponent.gameObject);
        }

        /// <summary>Add to disposable list.</summary>
        public static T AddTo<T>(this T disposable, CompositeDisposable gameObjectComponent)
            where T : IDisposable
        {
            gameObjectComponent.Add(disposable);
            return disposable;
        }
    }
}
