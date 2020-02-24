// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.App
{
    public partial class Application
    {
        private static VersionFlags targetVersion = VersionFlags.Undefined;

        public static VersionFlags TargetVersion
        {
            get
            {
                if (targetVersion == VersionFlags.Undefined)
                {
                    #if NETFRAMEWORK4_8
                    targetVersion = (VersionFlags)(VersionFlags.NetFramework4_8 << 2) | VersionFlags.NetFramework;
                    #elif NETFRAMEWORK4_7_2
                    targetVersion = (VersionFlags)((int)VersionFlags.NetFramework4_7_2 << 2) | VersionFlags.NetFramework;
                    #elif NETFRAMEWORK4_7_1
                    targetVersion = (VersionFlags)((int)VersionFlags.NetFramework4_7_1 << 2) | VersionFlags.NetFramework;
                    #elif NETFRAMEWORK4_7
                    targetVersion = (VersionFlags)((int)VersionFlags.NetFramework4_7 << 2) | VersionFlags.NetFramework;
                    #elif NETFRAMEWORK4_6_2
                    targetVersion = (VersionFlags)((int)VersionFlags.NetFramework4_6_2 << 2) | VersionFlags.NetFramework;
                    #elif NETFRAMEWORK4_6_1
                    targetVersion = (VersionFlags)((int)VersionFlags.NetFramework4_6_1 << 2) | VersionFlags.NetFramework;
                    #elif NETFRAMEWORK4_6
                    targetVersion = (VersionFlags)((int)VersionFlags.NetFramework4_6 << 2) | VersionFlags.NetFramework;
                    #elif NETFRAMEWORK4_5_2
                    targetVersion = (VersionFlags)((int)VersionFlags.NetFramework4_5_2 << 2) | VersionFlags.NetFramework;
                    #elif NETFRAMEWORK4_5_1
                    targetVersion = (VersionFlags)((int)VersionFlags.NetFramework4_5_1 << 2) | VersionFlags.NetFramework;
                    #elif NETFRAMEWORK4_5
                    targetVersion = (VersionFlags)((int)VersionFlags.NetFramework4_7 << 2) | VersionFlags.NetFramework;
                    #elif NETFRAMEWORK4_0
                    targetVersion = (VersionFlags)((int)VersionFlags.NetFramework4_0 << 2) | VersionFlags.NetFramework;
                    #endif
                }
                return targetVersion;
            }
        }
    }
}
