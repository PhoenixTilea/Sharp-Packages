// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Reflection.Behavior
{
    public static class ComponentCache
    {
        private static readonly Dictionary<UInt32, ComponentContainer> components;
        private static readonly ReadWriteLock componentLock;

        static ComponentCache()
        {
            components = new Dictionary<UInt32, ComponentContainer>();
            componentLock = new ReadWriteLock();
        }

        public static bool AddComponent(InstanceId id, object instance)
        {
            using (ThreadContext.WriteLock(componentLock))
            {
                ComponentContainer instances; if (!components.TryGetValue(id.ComponentId, out instances))
                {
                    instances = new ComponentContainer();
                    components.Add(id.ComponentId, instances);
                }
                return instances.Add(id.ObjectId, instance);
            }
        }

        public static bool HasComponents(InstanceId id)
        {
            using (ThreadContext.ReadLock(componentLock))
                foreach (ComponentContainer instances in components.Values)
                {
                    if (instances.Contains(id.ObjectId))
                        return true;
                }
            return false;
        }

        public static void GetComponentIds(ICollection<UInt32> ids)
        {
            using (ThreadContext.ReadLock(componentLock))
                foreach (UInt32 id in components.Keys)
                {
                    ids.Add(id);
                }
        }

        public static bool GetComponent(InstanceId id, out object instance)
        {
            using (ThreadContext.ReadLock(componentLock))
            {
                ComponentContainer instances; if (components.TryGetValue(id.ComponentId, out instances))
                    return instances.TryGetValue(id.ObjectId, out instance);

                instance = null;
                return false;
            }
        }

        public static ComponentContainer GetComponents(InstanceId id)
        {
            ComponentContainer instances;
            components.TryGetValue(id.ComponentId, out instances);

            return instances;
        }

        public static ReadScope BeginRead()
        {
            return ThreadContext.ReadLock(componentLock);
        }

        public static bool RemoveComponent(InstanceId id)
        {
            using (ThreadContext.WriteLock(componentLock))
            {
                ComponentContainer instances; if (components.TryGetValue(id.ComponentId, out instances))
                {
                    if (instances.Remove(id.ObjectId))
                    {
                        if(instances.IsEmpty)
                            components.Remove(id.ComponentId);

                        return true;
                    }
                }
                return false;
            }
        }

        public static int RemoveComponents(UInt32 owner)
        {
            int count = 0;
            using (ThreadContext.WriteLock(componentLock))
            {
                foreach (ComponentContainer container in components.Values)
                    if (container.Remove(owner))
                    {
                        count++;
                    }
            }
            return count;
        }
    }
}
