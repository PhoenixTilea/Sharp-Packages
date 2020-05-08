// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Reflection.Behavior
{
    public static partial class EntityExtension
    {
        public static bool RemoveComponent(this IEntity entity, UInt32 id)
        {
            return ComponentCache.RemoveComponent(entity.Id | id);
        }
        public static bool RemoveComponent<T>(this IEntity entity) where T : class, new()
        {
            return RemoveComponent(entity, typeof(T).GetComponentId());
        }
    }
}