using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using SE;
using SE.Text.Cpp;

namespace Cutils.Test
{
    public class Tests
    {
        class TestPreprocessor : Preprocessor
        {
            protected override void OnNextCompilerToken(CompilerToken token)
            {
                switch (token.Type)
                {
                    case Token.Whitespace:
                        {
                            if (sb.Length > 0 && sb[sb.Length - 1] != ' ')
                            {
                                sb.Append(' ');
                            }
                        }
                        break;
                    default:
                        {
                            sb.Append(token.Buffer);
                        }
                        break;
                }
            }
        }

        private static StringBuilder sb = new StringBuilder();
        private static double total;
        private static int count;

        public static void Run()
        {
            total = 0;
            count = 0;

            Run("#include \"test.h\"", "");
            Run("#include <test.h> \n", "");

            Run("#define F(A) #A\n#include F(test.h)", "");
            Run("#define F(A) <A.h>\n#include F(test) \n", "");

            //Macros

            Run("token", "token");
            Run("#", "");
            Run("#define X Y\nX", "Y");
            Run("#define X (2 * Y)\n#define Y (4 + X)\nX", "(2 * (4 + X))");
            Run("#define X " + @"to\" + "\n" + "ken" + "\nX", "token");
            Run("#define X(x) token x other\nX(and one).", "token and one other.");
            Run("#define X(x,y) token x and y\nX(1, 2).", "token 1 and 2.");
            Run("#define INC(x) (x + 2)\nINC", "INC");
            Run("#define TEST(x) x\\\n?\nTEST(A)", "A?");

            Run("#define A B C D\nA", "B C D");
            Run("#define A B ## D\nA", "BD");
            Run("#define X # ## #\nX", "##");
            Run("#define F(A) A\n#define B 22\nF(B)", "22");
            Run("#define F(A) A ## 1\n#define B 22\n#define B1 C\nF(B)", "C");
            Run("#define F(A) #A\n#define B 22\nF(B)", "\"B\"");
            Run("#define F(A,B) in_between(A hash_hash B)\n#define hash_hash # ## #\n#define mkstr(a) # a\n#define in_between(a) mkstr(a)\nF(x, y)", "\"x ## y\"");
            Run("#define F(A) <A>\nF(x)", "<x>");
            Run("#define F(A,B) <A,B>\nF(x,y) + 1", "<x,y> + 1");
            Run("#define F(A,B) A ## B\n#define C D\nF(_,C)", "_C");
            Run("#define F(A,B,C) <A,B,C>\nF(x,y,z)", "<x,y,z>");

            Run("#define F(...) <__VA_ARGS__>\nF(x)", "<x>");
            Run("#define F(...) <#__VA_ARGS__>\nF(x)", "<\"x\">");
            Run("#define F(...) <__VA_ARGS__>\nF(x,y)", "<x,y>");
            Run("#define F(...) <#__VA_ARGS__>\nF(x,y)", "<\"x,y\">");
            Run("#define F(A,...) <A,__VA_ARGS__>\nF(x,y)", "<x,y>");
            Run("#define F(A,...) <A, __VA_ARGS__>\nF(x,y)", "<x, y>");
            Run("#define F(A,...) <A, __VA_ARGS__>\nF(x,y, z)", "<x, y, z>");
            Run("#define F(A,...) <A, #__VA_ARGS__>\nF(x,y, z)", "<x, \"y, z\">");

            Run("#define X list of tokens\nX", "list of tokens");
            Run("#define LST list\n#define TOKS tokens\n#define LOTS LST of TOKS\nLOTS LOTS", "list of tokens list of tokens");
            Run("#define LOTS LST of TOKS\n#define LST list\n#define TOKS tokens\nLOTS LOTS", "list of tokens list of tokens");
            Run("#define FUNC(x) arg=x.\nFUNC(var) FUNC(2)", "arg=var. arg=2.");
            Run("#define FUNC(x,y,z) int n = z+y*x;\nFUNC(1,2,3)", "int n = 3+2*1;");
            Run("#define X 20\n#define FUNC(x,y) x+y\nx=FUNC(X,Y);", "x=20+Y;");
            Run("#define FA(x,y) FB(x,y)\n#define FB(x,y) x + y\nFB(1,2);", "1 + 2;");
            Run("#define PRINTF(...) printf(__VA_ARGS__)\nPRINTF()", "printf()");
            Run("#define PRINTF(...) printf(__VA_ARGS__)\nPRINTF(\"hello\")", "printf(\"hello\")");
            Run("#define PRINTF(...) printf(__VA_ARGS__)\nPRINTF(\"%d\", 1)", "printf(\"%d\", 1)");
            Run("#define PRINTF(FORMAT, ...) printf(FORMAT, __VA_ARGS__)\nPRINTF(\"test\")", "printf(\"test\", )");
            Run("#define PRINTF(FORMAT, ...) printf(FORMAT, __VA_ARGS__)\nPRINTF(\"test %s\", \"hello\")", "printf(\"test %s\", \"hello\")");
            Run("#define PRINTF(FORMAT, ...) printf(FORMAT, __VA_ARGS__)\nPRINTF(\"test %s %d\", \"hello\", 1)", "printf(\"test %s %d\", \"hello\", 1)");

            //Compiler Extension (, ## __VA_ARGS__)
            Run("#define PRINTF(FORMAT, ...) printf(FORMAT, ## __VA_ARGS__)\nPRINTF(\"test\")", "printf(\"test\")");

            Run("#define INC(x) (x + 2)\nINC(\n1\n)", "(1 + 2)");

            Run("#define STRINGIZE(ARG) #ARG\nSTRINGIZE(+=)", "\"+=\"");
            Run("#define STRINGIZE(ARG) #ARG\nSTRINGIZE(:>)", "\":>\"");
            Run("#define STRINGIZE(ARG) #ARG\nSTRINGIZE(3.1415)", "\"3.1415\"");

            Run("#define CONCAT(X, Y) X ## Y\nCONCAT(+, =)", "+=");
            Run("#define CONCAT(X, Y) X ## Y\nCONCAT(3, .14159)", "3.14159");
            Run("#define CONCAT(X, Y) X ## Y\nCONCAT(3, hello)", "3hello");
            Run("#define CONCAT(X, Y) X ## #Y\nCONCAT(u, hello)", "u\"hello\"");
            Run("#define CONCAT(X, ...) X ## __VA_ARGS__\nCONCAT(hello) there", "hello there");
            Run("#define CONCAT(X, ...) X ## __VA_ARGS__\nCONCAT(hello, there)", "hellothere");
            Run("#define A _a_\n#define B _b_\n#define AB _ab_\n#define C(a,b) a##b\n#define D(a,b) C(a,b)\nC(A,B)\nD(A,B)", "_ab_ _a__b_");

            Run("#define A MACRO\nA", "MACRO");
            Run("#define A MACRO\n#undef A\nA", "A");
            Run("#define A(X) MACRO X\nA(x)", "MACRO x");
            Run("#define A(X) MACRO X\n#undef A\nA(x)", "A(x)");

            Run("#define REGISTER_NAME(num,name) NAME_##name = num,\nREGISTER_NAME(201, TRUE)", "NAME_TRUE = 201,");
            Run("#define FUNC_2(X) VALUE=X\n#define FUNC_N(X) FUNC_##X\n#define OBJECT FUNC_N(2)\nOBJECT(1234)", "VALUE=1234");

            //Compiler Extension (, ## __VA_ARGS__)
            Run("#define FUNC(fmt,...) (fmt, ## __VA_ARGS__)\nFUNC(a)", "(a)");
            Run("#define FUNC(fmt,...) (fmt, ## __VA_ARGS__)\nFUNC(a,b)", "(a,b)");
            Run("#define FUNC(fmt,...) (fmt, ## __VA_ARGS__)\nFUNC(a,b )", "(a,b)");
            Run("#define FUNC(fmt, ...) (fmt, ## __VA_ARGS__)\nFUNC(a)", "(a)");
            Run("#define FUNC(fmt, ...) (fmt, ## __VA_ARGS__)\nFUNC(a,b)", "(a,b)");
            Run("#define FUNC(fmt, ...) (fmt, ## __VA_ARGS__)\nFUNC(a,b )", "(a,b)");

            Run("#define EMPTY_TOKEN\n#define FUNC(_FuncName, _HType1, _HArg1) _FuncName(_HType1 _HArg1);\nFUNC(hello, EMPTY_TOKEN, int)", "hello( int);");
            Run("#define FUNC(x) Value = x\n#define PP_JOIN(x, y) x ## y\n#define RESULT(x) PP_JOIN(FU, NC)(x)\nRESULT(1234)", "Value = 1234");
            Run("#define VALUE 1234\n#define PP_JOIN(x, y) x ## y\n#define RESULT PP_JOIN(V, ALUE)\nRESULT", "1234");

            Run("#define EMPTY_TOKEN\n#define FUNC(x) (x)\nFUNC(EMPTY_TOKEN A)", "( A)");
            Run("#define EMPTY_TOKEN\n#define FUNC(x,y) (x y)\nFUNC(EMPTY_TOKEN,A)", "( A)");

            Run("#define EMPTY\n#define FUNC(x) (x)\nFUNC(x    y)", "(x y)");
            Run("#define EMPTY\n#define FUNC(x) (x)\nFUNC(x    EMPTY    y)", "(x y)");
            Run("#define EMPTY\n#define FUNC(x) (x)\nFUNC(x    EMPTY    y    EMPTY)", "(x y )");
            Run("#define EMPTY\n#define FUNC(x) (x)\nFUNC(EMPTY    x    EMPTY    y)", "( x y)");
            Run("#define EMPTY\n#define FUNC(x) (x)\nFUNC(    EMPTY    x    EMPTY    y)", "( x y)");
            Run("#define EMPTY\n#define FUNC(x) (x)\nFUNC(EMPTY    x    EMPTY    y    )", "( x y)");

            Run("#define EMPTY\n#define FUNC(x) (x)\n#define FUNC_2(x) FUNC(x)\nFUNC_2(x    y)", "(x y)");
            Run("#define EMPTY\n#define FUNC(x) (x)\n#define FUNC_2(x) FUNC(x)\nFUNC_2(x    EMPTY    y)", "(x y)");
            Run("#define EMPTY\n#define FUNC(x) (x)\n#define FUNC_2(x) FUNC(x)\nFUNC_2(x    EMPTY    y    EMPTY)", "(x y)");
            Run("#define EMPTY\n#define FUNC(x) (x)\n#define FUNC_2(x) FUNC(x)\nFUNC_2(EMPTY    x    EMPTY    y)", "(x y)");
            Run("#define EMPTY\n#define FUNC(x) (x)\n#define FUNC_2(x) FUNC(x)\nFUNC_2(    EMPTY    x    EMPTY    y)", "(x y)");
            Run("#define EMPTY\n#define FUNC(x) (x)\n#define FUNC_2(x) FUNC(x)\nFUNC_2(EMPTY    x    EMPTY    y    )", "(x y)");

            Run("#define EMPTY\n#define FUNC(x) ( x )\nFUNC(EMPTY EMPTY)", "( )");
            Run("#define EMPTY\n#define FUNC(x) ( x)\nFUNC(EMPTY EMPTY x)", "( x)");
            Run("#define EMPTY\n#define FUNC(x,y) ( x y)\n#define FUNC2(x,y) FUNC(x,y)\nFUNC2(EMPTY EMPTY EMPTY, x)", "( x)");
            Run("#define EMPTY\n#define DOUBLE_EMPTY EMPTY EMPTY\n#define FUNC(x) ( x)\nFUNC(DOUBLE_EMPTY x)", "( x)");
            Run("#define EMPTY\n#define DOUBLE_EMPTY EMPTY EMPTY\n#define FUNC(x,y) ( x y )\n#define FUNC2(x,y) FUNC(x,y)\nFUNC2(DOUBLE_EMPTY EMPTY, x)", "( x )");
            Run("#define EMPTY\n#define FUNC(x,y) ( x y )\nFUNC(EMPTY EMPTY EMPTY EMPTY EMPTY EMPTY, x)", "( x )");

            Run("#define _NON_MEMBER_CALL(FUNC, CV_REF_OPT) FUNC(X,CV_REF_OPT)\n#define _NON_MEMBER_CALL_CV(FUNC, REF_OPT) _NON_MEMBER_CALL(FUNC, REF_OPT) _NON_MEMBER_CALL(FUNC, const REF_OPT)\n#define _NON_MEMBER_CALL_CV_REF(FUNC) _NON_MEMBER_CALL_CV(FUNC, ) _NON_MEMBER_CALL_CV(FUNC, &)\n#define _IS_FUNCTION(X,CV_REF_OPT) (CV_REF_OPT)\n_NON_MEMBER_CALL_CV_REF(_IS_FUNCTION)", "() (const) (&) (const &)");

            //Compiler Extension (, ## __VA_ARGS__)
            Run("#define TEXT(x) L ## x\n#define checkf(expr, format, ...) AssertFailed(#expr, format, ##__VA_ARGS__)\ncheckf( true, TEXT( \"hello world\" ) );", "AssertFailed(\"true\", L\"hello world\");");

            //Logic

            Run("#if 1 + 2 > 3\nYes\n#else\nNo\n#endif", "No");
            Run("#if 1 + 2 >= 3\nYes\n#endif", "Yes");
            Run("#define ONE 1\n#define TWO 2\n#define PLUS(x, y) x + y\n#if PLUS(ONE, TWO) > 3\nYes\n#endif\nNo", "No");
            Run("#define ONE 1\n#define TWO 2\n#define PLUS(x, y) x + y\n#if PLUS(ONE, TWO) >= 3\nYes\n#endif", "Yes");
            Run("#define ONE 1\n#if defined ONE\nOne\n#elif defined TWO\nTwo\n#else\nThree\n#endif", "One");
            Run("#define TWO 1\n#if defined ONE\nOne\n#elif defined TWO\nTwo\n#else\nThree\n#endif", "Two");
            Run("#define ONE 0\n#if defined(ONE) + defined(TWO) >= 1\nYes\n#else\nNo\n#endif", "Yes");
            Run("#define ONE 0\n#if defined(ONE) + defined(TWO) >= 2\nYes\n#else\nNo\n#endif", "No");
            Run("#define ONE 0\n#define TWO\n#if defined(ONE) + defined(TWO) >= 2\nYes\n#else\nNo\n#endif", "Yes");
            Run("#define ONE 0\n#if defined(ONE) + defined(TWO) >= 1\nYes\n#else\n#define TWO 1\nNo\n#endif\nTWO", "Yes TWO");
            Run("#define ONE 0\n#if defined(ONE) + defined(TWO) >= 1\nYes\n#else\n#define TWO 1\n\n#if defined TWO\nTWO\n#endif\nNo\n#endif", "Yes");

            Console.WriteLine("{0} tests completed in {1}ms (~{2}ms per test)", count, total, total / count);
            Console.Read();
        }
        private static void Run(string input, string expected)
        {
            sb.Clear();
            using (MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(input)))
            {
                TestPreprocessor pp = new TestPreprocessor();
                Stopwatch sw = new Stopwatch();
                sw.Start();
                {
                    Debug.Assert(pp.Parse(ms));
                }
                sw.Stop();
                total += sw.Elapsed.TotalMilliseconds;
                count++;

                Debug.Assert(pp.Errors.Count == 0);
            }
            string result = sb.ToString().Trim();
            Debug.Assert(result.Equals(expected));
        }
    }
}