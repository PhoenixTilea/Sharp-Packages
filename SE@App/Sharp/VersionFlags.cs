// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.App
{
    /// <summary>
    /// Defines the available .NET runtime versions
    /// </summary>
    public enum VersionFlags : byte
    {
        Undefined = 0,
        
        NetFramework = 0x1,

        NetFramework4_8 = 3,
        NetFramework4_7_2 = 4,
        NetFramework4_7_1 = 5,
        NetFramework4_7 = 6,
        NetFramework4_6_2 = 7,
        NetFramework4_6_1 = 8,
        NetFramework4_6 = 9,
        NetFramework4_5_2 = 10,
        NetFramework4_5_1 = 11,
        NetFramework4_5 = 12,
        NetFramework4_0 = 13,
    }
}
