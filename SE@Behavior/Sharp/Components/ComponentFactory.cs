// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Reflection.Behavior
{
    public static class ComponentFactory
    {
        private readonly static Type EntityType;

        private static readonly Dictionary<UInt64, Func<object>> componentCreator;
        private static readonly ReadWriteLock componentLock;

        static ComponentFactory()
        {
            EntityType = typeof(IEntity);

            componentCreator = new Dictionary<UInt64, Func<object>>();
            componentLock = new ReadWriteLock();
        }

        public static bool Register<EntityType>(UInt32 id, Func<object> creator)
        {
            UInt64 instanceId = (new InstanceId((UInt32)typeof(EntityType).GetHashCode()) | id);
            using (ThreadContext.WriteLock(componentLock))
            {
                if (componentCreator.ContainsKey(instanceId)) return false;
                else
                {
                    componentCreator.Add(instanceId, creator);
                    return true;
                }
            }
        }
        public static bool Register<EntityType, ComponentType>(Func<object> creator)
        {
            return Register<EntityType>((UInt32)typeof(ComponentType).GetHashCode(), creator);
        }
        public static bool Register<EntityType, ComponentType, FactoryType>()
        {
            return Register<EntityType>
            (
                (UInt32)typeof(ComponentType).GetHashCode(),
                typeof(FactoryType).GetCreator<Func<object>>()
            );
        }

        public static bool TryCreateComponent(IEntity entity, UInt32 id, out object instance)
        {
            UInt64 instanceId = (new InstanceId((UInt32)entity.GetType().GetType(EntityType).GetHashCode()) | id);
            instance = null;

            using (ThreadContext.ReadLock(componentLock))
            {
                Func<object> creator; if (!componentCreator.TryGetValue(instanceId, out creator))
                    return false;

                if (creator == null)
                    return false;

                instance = creator();
                return (instance != null);
            }
        }
    }
}
