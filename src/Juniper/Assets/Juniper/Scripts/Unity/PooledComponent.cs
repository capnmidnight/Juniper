using System;

using UnityEngine;

namespace Juniper
{
    public struct PooledComponent<T> where T : Component
    {
        public readonly T Value;
        public readonly bool IsNew;

        public PooledComponent(GameObject obj, Predicate<T> predicate = null, Action<T> onCreate = null)
        {
            if (predicate == null)
            {
                predicate = (_) => true;
            }

            Value = Array.Find(obj.GetComponents<T>(), predicate);
            IsNew = Value == null;
            if (IsNew)
            {
                Value = obj.AddComponent<T>();
                onCreate?.Invoke(Value);
            }
        }

        public PooledComponent(T value, bool isNew)
        {
            Value = value;
            IsNew = isNew;
        }

        public static implicit operator T(PooledComponent<T> obj)
        {
            return obj.Value;
        }
    }
}
