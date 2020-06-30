using System;

namespace SE.Text.Sharp
{
    public enum SharpPreprocessorState : byte
    {
        Master = 0,
        
        If,
        Elif,
        Else,
        Endif,
        Define,
        Undef,
        Error,
        Warning,
        Line,
        Region,
        Endregion,
        Pragma,
        
        Failure
    }
}