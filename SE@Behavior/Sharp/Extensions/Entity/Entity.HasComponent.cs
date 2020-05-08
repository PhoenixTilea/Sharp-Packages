// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Reflection.Behavior
{
    public static partial class EntityExtension
    {
        public static bool HasComponent<T>(this IEntity entity, UInt32 id) where T : class
        {
            object instance; if (ComponentCache.GetComponent(entity.Id | id, out instance))
            {
                return (instance is T);
            }
            else return false;
        }

        public static bool HasComponent<T>(this IEntity entity) where T : class
        {
            return HasComponent<T>(entity, typeof(T).GetComponentId());
        }
    }
}