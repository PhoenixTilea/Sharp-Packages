// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;
using SE.Reflection;

namespace SE.Reflection.Behavior
{
    public static partial class EntityExtension
    {
        public static bool AddComponent(this IEntity entity, UInt32 id, object instance)
        {
            if (instance == null)
                return false;

            if (!ComponentCache.AddComponent(entity.Id | id, instance))
            {
                IDisposable disposer = instance as IDisposable;
                if (disposer != null)
                    disposer.Dispose();

                return false;
            }
            else return true;
        }
        public static bool AddComponent(this IEntity entity, UInt32 id)
        {
            object instance; if (ComponentFactory.TryCreateComponent(entity, id, out instance))
            {
                return AddComponent(entity, id, null);
            }
            else return false;
        }

        public static bool AddComponent<T>(this IEntity entity, T instance) where T : class
        {
            return AddComponent(entity, typeof(T).GetComponentId(), instance);
        }
        public static bool AddComponent<T>(this IEntity entity) where T : class, new()
        {
            Type component = typeof(T);
            UInt32 id = component.GetComponentId();

            object instance; if (!ComponentFactory.TryCreateComponent(entity, id, out instance))
                instance = component.CreateInstance<T>();

            return AddComponent(entity, id, instance);
        }
    }
}