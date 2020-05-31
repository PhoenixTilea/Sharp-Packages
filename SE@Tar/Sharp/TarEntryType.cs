// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Storage.Tar
{
    public enum TarEntryType : byte
    {
        File = 0,
        HardLink,
        SymbolicLink,
        CharacterSpecial,
        BlockSpecial,
        Directory,
        Fifo,
        ContiguousFile
    }
}
