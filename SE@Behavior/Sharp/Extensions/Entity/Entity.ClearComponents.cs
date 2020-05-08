// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Reflection.Behavior
{
    public static partial class EntityExtension
    {
        public static int ClearComponents(this IEntity entity)
        {
            return ComponentCache.RemoveComponents(entity.Id.ObjectId);
        }
    }
}
