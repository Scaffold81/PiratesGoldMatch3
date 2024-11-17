using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using UnityEngine.Events;

namespace RxExtensions
{
    public static class UnityEventExtensions
    {
        public static IObservable<T> AsObservable<T>(this UnityEvent<T> e)
        {
            return Observable.Create<T>((observer) =>
            {
                e.AddListener(observer.OnNext);
                return Disposable.Create(() =>
                {
                    e.RemoveListener(observer.OnNext);
                });
            });
        }
    }
}
