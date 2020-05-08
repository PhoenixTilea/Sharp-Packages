// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Text.Cpp
{
    /// <summary>
    /// The list of valid preprocessor tokens
    /// </summary>
    public enum Token : ushort
    {
        Comment = 145,

        Identifier = 140,

        StringLiteral = 136,
        BogusStringLiteral = 135,

        CharacterLiteral = 131,
        BogusCharacterLiteral = 130,

        Numeric = 127,

        Character = 124,

        IfDirective = 121,
        IfdefDirective = 120,
        IfndefDirective = 119,
        ElifDirective = 118,
        ElseDirective = 117,
        EndifDirective = 116,

        DefineDirective = 113,
        UndefDirective = 112,

        Line = 110,
        Error = 109,
        Pragma = 108,

        IncludeDirective = 104,
        UnqoutedHeaderName = 103,
        BogusUnqoutedHeaderName = 102,

        BogusDirective = 98,

        Empty = 95,

        /// <summary>
        /// (##)
        /// </summary>
        Concat = 93,
        /// <summary>
        /// (#)
        /// </summary>
        Stringify = 92,

        New = 89,
        Delete = 88,

        /// <summary>
        /// (##)
        /// </summary>
        DoubleHash = 86,
        /// <summary>
        /// (#)
        /// </summary>
        Hash = 85,

        /// <summary>
        /// {
        /// </summary>
        LeftCurlyBracket = 82,
        /// <summary>
        /// {
        /// </summary>
        CurlyBracketOpen = LeftCurlyBracket,

        /// <summary>
        /// }
        /// </summary>
        RightCurlyBracket = 80,
        /// <summary>
        /// }
        /// </summary>
        CurlyBracketClose = RightCurlyBracket,

        /// <summary>
        /// (
        /// </summary>
        LeftRoundBracket = 78,
        /// <summary>
        /// (
        /// </summary>
        RoundBracketOpen = LeftRoundBracket,

        /// <summary>
        /// )
        /// </summary>
        RightRoundBracket = 76,
        /// <summary>
        /// )
        /// </summary>
        RoundBracketClose = RightRoundBracket,

        /// <summary>
        /// [
        /// </summary>
        LeftSquareBracket = 74,
        /// <summary>
        /// [
        /// </summary>
        SquareBracketOpen = LeftSquareBracket,

        /// <summary>
        /// ]
        /// </summary>
        RightSquareBracket = 72,
        /// <summary>
        /// ]
        /// </summary>
        SquareBracketClose = RightSquareBracket,

        /// <summary>
        /// <
        /// </summary>
        LeftAngleBracket = LessThan,
        /// <summary>
        /// <
        /// </summary>
        AngleBracketOpen = LessThan,

        /// <summary>
        /// >
        /// </summary>
        RightAngleBracket = GreaterThan,
        /// <summary>
        /// >
        /// </summary>
        AngleBracketClose = GreaterThan,

        /// <summary>
        /// (::)
        /// </summary>
        ScopeResolution = 70,

        /// <summary>
        /// (->*)
        /// </summary>
        PointerToMember = 68,
        /// <summary>
        /// (->)
        /// </summary>
        Pointer = 67,

        /// <summary>
        /// (.*)
        /// </summary>
        ReferenceToMember = 65,

        /// <summary>
        /// (...)
        /// </summary>
        VariableArgs = 63,

        /// <summary>
        /// (<=>)
        /// </summary>
        ThreeWayComparsion = 61,

        /// <summary>
        /// (&&)
        /// </summary>
        LogicalAnd = 59,

        /// <summary>
        /// (||)
        /// </summary>
        LogicalOr = 57,

        /// <summary>
        /// (<)
        /// </summary>
        LessThan = 55,
        /// <summary>
        /// (<=)
        /// </summary>
        LessEqual = 54,

        /// <summary>
        /// (>)
        /// </summary>
        GreaterEqual = 52,
        /// <summary>
        /// (>=)
        /// </summary>
        GreaterThan = 51,

        /// <summary>
        /// (!)
        /// </summary>
        LogicalNot = 49,
        /// <summary>
        /// (!=)
        /// </summary>
        NotEqual = 48,

        /// <summary>
        /// (=)
        /// </summary>
        Equal = 46,

        /// <summary>
        /// (&)
        /// </summary>
        BitwiseAnd = 44,
        /// <summary>
        /// (&=)
        /// </summary>
        BitwiseAndAssign = 43,

        /// <summary>
        /// (^)
        /// </summary>
        BitwiseXor = 41,
        /// <summary>
        /// (^=)
        /// </summary>
        BitwiseXorAssign = 40,

        /// <summary>
        /// (|)
        /// </summary>
        BitwiseOr = 38,
        /// <summary>
        /// (|=)
        /// </summary>
        BitwiseOrAssign = 37,

        /// <summary>
        /// (~)
        /// </summary>
        BitwiseNot = 35,

        /// <summary>
        /// (>>)
        /// </summary>
        RightShift = 33,
        /// <summary>
        /// (>>=)
        /// </summary>
        RightShiftAssign = 32,

        /// <summary>
        /// (<<)
        /// </summary>
        LeftShift = 30,
        /// <summary>
        /// (<<=)
        /// </summary>
        LeftShiftAssign = 29,

        /// <summary>
        /// (%)
        /// </summary>
        Mod = 27,
        /// <summary>
        /// (%=)
        /// </summary>
        ModAssign = 26,

        /// <summary>
        /// (*)
        /// </summary>
        Mult = 24,
        /// <summary>
        /// (*=)
        /// </summary>
        MultAssign = 23,

        /// <summary>
        /// (/)
        /// </summary>
        Div = 21,
        /// <summary>
        /// (/=)
        /// </summary>
        DivAssign = 20,

        /// <summary>
        /// (--)
        /// </summary>
        Decrement = 18,
        /// <summary>
        /// (-)
        /// </summary>
        Sub = 17,
        /// <summary>
        /// (-=)
        /// </summary>
        SubAssign = 16,

        /// <summary>
        /// (++)
        /// </summary>
        Increment = 14,
        /// <summary>
        /// (+)
        /// </summary>
        Add = 13,
        /// <summary>
        /// (+=)
        /// </summary>
        AddAssign = 12,

        /// <summary>
        /// (=)
        /// </summary>
        Assign = 10,

        /// <summary>
        /// (?)
        /// </summary>
        Ternary = 9,

        /// <summary>
        /// (;)
        /// </summary>
        Semicolon = 8,

        /// <summary>
        /// (:)
        /// </summary>
        Colon = 7,

        /// <summary>
        /// (.)
        /// </summary>
        Dot = 6,

        /// <summary>
        /// (,)
        /// </summary>
        Comma = 5,

        Whitespace = 2,
        NewLine = 1,

        Invalid = 0,
    }
}
