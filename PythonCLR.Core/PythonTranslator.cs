using Antlr4.Runtime;
using Lokad.ILPack;
using PythonCLR.Core.Contexts;
using PythonCLR.Core.Listeners;
using PythonCLR.Core.States;
using PythonCLR.Core.Visitors;
using System.Reflection.Metadata.Ecma335;

namespace PythonCLR.Core
{
    public static class PythonTranslator
    {
        public static void TranslateCodeToAssembly(string code, string assemblyName)
        {
            TranslateToAssembly(new AntlrInputStream(code), assemblyName);
        }

        public static void TranslateFileToAssembly(string filePath, string assemblyName)
        {
            using StreamReader reader = new(filePath);
            TranslateToAssembly(new AntlrInputStream(reader), assemblyName);
        }

        private static void TranslateToAssembly(AntlrInputStream stream, string assemblyName)
        {
            using AssemblyBuilderStateContext assemblyContext = AssemblyBuilderStateContext.Create(assemblyName);

            PythonParser parser = CreateParser(stream);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(new ParseErrorListener());

            PythonParser.ProgramContext context = parser.program();
            CLRPythonVisitor visitor = new();

            visitor.Visit(context);

            AssemblyBuilderState assemblyBuilderState = AssemblyBuilderStateContext.GetCurrentState();
            // Материализуем внешний класс. Некрасиво, но - как есть.
            assemblyBuilderState.EndMethodDefinition();
            assemblyBuilderState.EndClassDefinition();

            AssemblyGenerator assemblyGenerator = new();
            assemblyGenerator.GenerateAssembly(assemblyBuilderState.GeneratedAssembly, $"./{assemblyName}.dll");
        }

        private static PythonParser CreateParser(AntlrInputStream stream)
        {
            PythonLexer lexer = new(stream);
            CommonTokenStream tokenStream = new(lexer);
            return new PythonParser(tokenStream);
        }
    }
}
