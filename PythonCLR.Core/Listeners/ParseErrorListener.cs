using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace PythonCLR.Core.Listeners
{
    internal class ParseErrorListener : BaseErrorListener
    {
        public override void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
        {
            throw new Exception($"line {line} : {charPositionInLine} {msg}");
        }
    }
}
