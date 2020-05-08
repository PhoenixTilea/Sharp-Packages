// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SE;

namespace SE.App
{
    public partial class Application
    {
        private readonly static PlatformID WindowsMask = (PlatformID.Win32NT | PlatformID.Win32S | PlatformID.Win32Windows | PlatformID.WinCE);
        private static PlatformFlags currentPlatform = PlatformFlags.Undefined;

        public static PlatformFlags Platform
        {
            get
            {
                if (currentPlatform == PlatformFlags.Undefined)
                {
                    if (Path.DirectorySeparatorChar == '\\' && (WindowsMask & Environment.OSVersion.Platform) == Environment.OSVersion.Platform) currentPlatform = PlatformFlags.Windows;
                    else
                    {
                        string platformName = string.Empty;
                        try
                        {
                            Process p = new Process();
                            p.StartInfo.UseShellExecute = false;
                            p.StartInfo.FileName = "uname";
                            p.Start();

                            platformName = p.StandardOutput.ReadToEnd();
                            p.WaitForExit();

                            if (platformName == null)
                                platformName = string.Empty;
                            platformName = platformName.Trim();
                        }
                        catch
                        { }

                        if (platformName.Contains("Darwin"))
                        {
                            currentPlatform = PlatformFlags.Unix;
                            currentPlatform |= PlatformFlags.Mac;
                        }
                        else if (platformName.Contains("Linux"))
                        {
                            currentPlatform = PlatformFlags.Unix;
                            currentPlatform |= PlatformFlags.Linux;
                        }
                        else if (!string.IsNullOrWhiteSpace(platformName))
                            currentPlatform = PlatformFlags.Unix;
                    }
                    if (currentPlatform != PlatformFlags.Undefined)
                    {
                        if (IntPtr.Size == 8) currentPlatform |= PlatformFlags.x64;
                        else if (IntPtr.Size == 4) currentPlatform |= PlatformFlags.x86;
                    }
                }
                return currentPlatform;
            }
        }
    }
}
