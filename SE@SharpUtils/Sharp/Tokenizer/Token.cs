using System;

namespace SE.Text.Sharp
{
    public enum Token : ushort
    {
        Invalid,
        SingleLineComment,
        MultiLineComment,
        Identifier,
        VerbatimIdentifier,
        StringLiteral,
        RawStringLiteral = VerbatimIdentifier,
        InterpolatedStringLiteral,
        BogusStringLiteral,
        CharacterLiteral,
        BogusCharacterLiteral,
        Numeric,
        Character,
        
        IfDirective,
        ElifDirective,
        ElseDirective,
        EndifDirective,
        DefineDirective,
        UndefDirective,
        Line,
        Warning,
        Error,
        Region,
        Endregion,
        Pragma,
        BogusDirective,
        
        Empty,
        
        New,
        
        Hash,
        
        LeftCurlyBracket,
        CurlyBracketOpen = LeftCurlyBracket,
        RightCurlyBracket,
        CurlyBracketClose = RightCurlyBracket,
        
        LeftRoundBracket,
        RoundBracketOpen = LeftRoundBracket,
        RightRoundBracket,
        RoundBracketClose = RightRoundBracket,
        
        LeftSquareBracket,
        SquareBracketOpen = LeftSquareBracket,
        RightSquareBracket,
        SquareBracketClose = RightSquareBracket,
        
        Lamda,
        
        RestParams,
        Range,
        
        LogicalAnd,
        LogicalOr,
        LessThan,
        LessEqual,
        GreaterEqual,
        GreaterThan,
        LogicalNot,
        NotEqual,
        Equal,

        LeftAngleBracket = LessThan,
        AngleBracketOpen = LessThan,
        RightAngleBracket = GreaterThan,
        AngleBracketClose = GreaterThan,

        BitwiseAnd,
        BitwiseAndAssign,
        BitwiseXor,
        BitwiseXorAssign,
        BitwiseOr,
        BitwiseOrAssign,
        BitwiseNot,
        RightShift,
        RightShiftAssign,
        LeftShift,
        LeftShiftAssign,
        
        Mod,
        ModAssign,
        Multiple,
        MultipleAssign,
        Div,
        DivAssign,
        Decrement,
        Sub,
        SubAssign,
        Increment,
        Add,
        AddAssign,
        
        Assign,
        NullCoalescing,
        NullCoalescingAssign,
        Ternary,
        
        Semicolon,
        Colon,
        Dot,
        Comma,
        Whitespace,
        NewLine
    }
}