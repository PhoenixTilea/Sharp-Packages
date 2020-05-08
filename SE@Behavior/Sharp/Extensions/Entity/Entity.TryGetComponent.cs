// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Reflection.Behavior
{
    public static partial class EntityExtension
    {
        public static bool TryGetComponent<T>(this IEntity entity, UInt32 id, out T component) where T : class
        {
            object instance; if (ComponentCache.GetComponent(entity.Id | id, out instance))
            {
                component = instance as T;
            }
            else component = default(T);
            return (component != null);
        }
        public static bool TryGetComponent<TComponent, TAs>(this IEntity entity, out TAs component) where TAs : class
        {
            return TryGetComponent<TAs>(entity, typeof(TComponent).GetComponentId(), out component);
        }

        public static bool TryGetComponent<T>(this IEntity entity, out T component) where T : class
        {
            return TryGetComponent<T>(entity, typeof(T).GetComponentId(), out component);
        }
    }
}