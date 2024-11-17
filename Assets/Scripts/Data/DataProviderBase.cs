#nullable enable
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using UnityEngine;

namespace Core.Data
{
    public class DataProviderBase : MonoBehaviour
    {
        public Dictionary<Enum, BehaviorSubject<object?>> _observables = new();

        public void Publish(Enum controlName, object value)
        {
            EnsureCreated(controlName);
            _observables[controlName].OnNext(value);
        }

        public object? GetValue(Enum controlName)
        {
            return _observables.ContainsKey(controlName)
                ? _observables[controlName].Value
                : null;

        }

        public IObservable<TValue> Receive<TValue>(Enum controlName)
        {
            EnsureCreated(controlName);

            return _observables[controlName]
                .Where(value => value is TValue)!
                .Cast<TValue>();
        }

        private void EnsureCreated(Enum controlName)
        {
            lock (_observables)
            {
                if (_observables.ContainsKey(controlName) == false)
                    _observables[controlName] = new BehaviorSubject<object?>(null);
            }
        }

        public void Clear()
        {
            _observables.Clear();
        }
    }
}