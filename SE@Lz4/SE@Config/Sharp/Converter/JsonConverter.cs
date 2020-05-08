// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using SE;
using SE.Json;

namespace SE.Config
{
    /// <summary>
    /// Converts incoming command line arguments into JSON data
    /// </summary>
    public class JsonConverter : ITypeConverter
    {
        public bool TryParseValue(Type memberType, string value, out object result)
        {
            result = new JsonDocument();

            MemoryStream ms = new MemoryStream();
            byte[] buffer = System.Text.Encoding.ASCII.GetBytes(value);

            ms.Seek(0, SeekOrigin.Begin);
            ms.SetLength(buffer.Length);
            ms.Write(buffer, 0, buffer.Length);
            ms.Seek(0, SeekOrigin.Begin);

            if ((result as JsonDocument).Load(ms)) return true;
            else return false;
        }
    }
}
