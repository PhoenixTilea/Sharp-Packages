// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Npm
{
    public static partial class ResponseCodes
    {
        public static class PackageInfo
        {
            public const string InvalidPackageName = "Invalid package name '{0}'";
            public const string InvalidPackageVersion = "Invalid package version '{0}'";
            public const string InvalidPackageReference = "Could not resolve a package reference";
        }
    }
}