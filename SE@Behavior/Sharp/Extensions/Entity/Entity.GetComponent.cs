// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Reflection.Behavior
{
    public static partial class EntityExtension
    {
        public static T GetComponent<T>(this IEntity entity, UInt32 id)
        {
            object instance; if (ComponentCache.GetComponent(entity.Id | id, out instance))
            {
                return (T)instance;
            }
            else return default(T);
        }
        public static TAs GetComponent<TComponent, TAs>(this IEntity entity)
        {
            return GetComponent<TAs>(entity, (UInt32)typeof(TComponent).GetHashCode());
        }

        public static T GetComponent<T>(this IEntity entity)
        {
            return GetComponent<T>(entity, (UInt32)typeof(T).GetHashCode());
        }
    }
}