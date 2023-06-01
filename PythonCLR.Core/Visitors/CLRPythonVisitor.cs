using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using PythonCLR.Core.Contexts;
using PythonCLR.Core.Helpers;
using PythonCLR.Core.States;
using System.Reflection;
using System.Reflection.Emit;

namespace PythonCLR.Core.Visitors
{
    internal class CLRPythonVisitor : PythonParserBaseVisitor<object>
    {
        private const string TrueLiteral = "True";

        public override object VisitClassDef([NotNull] PythonParser.ClassDefContext context)
        {
            string className = context.Identifier().ToString()!;
            ITerminalNode[]? parentClasses = context.identifierList()?.Identifier();
            if (parentClasses is not null && parentClasses.Length > 1)
                throw new NotSupportedException();
            Type? parent = TypesHelper.GetCLRTypeByPythonName(parentClasses?.Single().ToString());

            AssemblyBuilderState assemblyBuilderState = AssemblyBuilderStateContext.GetCurrentState();
            assemblyBuilderState.StartClassDefinition(className, parent);

            object result = base.VisitClassDef(context);

            assemblyBuilderState.EndClassDefinition();
            return result;
        }

        public override object VisitFuncDef([NotNull] PythonParser.FuncDefContext context)
        {
            string funcName = context.Identifier().ToString()!;
            Type? returnType = TypesHelper.GetCLRTypeByPythonName(context.type()?.ToString());
            Type[]? argTypes = context.argList()?
                                      .GetRuleContexts<PythonParser.TypedIdentifierContext>()
                                      .Select(typedIdentifier => TypesHelper.GetCLRTypeByPythonName(typedIdentifier.type().ToString())!)
                                      .ToArray();

            AssemblyBuilderState assemblyBuilderState = AssemblyBuilderStateContext.GetCurrentState();
            assemblyBuilderState.StartMethodDefinition(funcName, MethodAttributes.Public, argTypes, returnType);

            object result = base.VisitFuncDef(context);

            assemblyBuilderState.EndMethodDefinition();
            return result;
        }

        public override object VisitLiteral([NotNull] PythonParser.LiteralContext context)
        {
            AssemblyBuilderState assemblyBuilderState = AssemblyBuilderStateContext.GetCurrentState();
            ILGenerator generator = assemblyBuilderState.GetCurrentMethodGenerator();

            ITerminalNode literal;
            if ((literal = context.NumberLiteral()) is not null)
                generator.Emit(OpCodes.Ldc_R8, double.Parse(literal.ToString()!));
            else if ((literal = context.BoolLiteral()) is not null)
                generator.Emit(string.Equals(literal.ToString(), TrueLiteral, StringComparison.OrdinalIgnoreCase) ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
            else if ((literal = context.StringLiteral()) is not null)
                generator.Emit(OpCodes.Ldstr, literal.ToString()!.Trim('"', '\''));

            return base.VisitLiteral(context);
        }

        public override object VisitMulOrDivExpr([NotNull] PythonParser.MulOrDivExprContext context)
        {
            object result = base.VisitMulOrDivExpr(context);

            AssemblyBuilderState assemblyBuilderState = AssemblyBuilderStateContext.GetCurrentState();
            ILGenerator generator = assemblyBuilderState.GetCurrentMethodGenerator();

            foreach (PythonParser.MulOrDivOperatorContext operatorContext in context.mulOrDivOperator() ?? Array.Empty<PythonParser.MulOrDivOperatorContext>())
                generator.Emit(operatorContext.Mul() is not null ? OpCodes.Mul : OpCodes.Div);

            return result;
        }

        public override object VisitAddOrSubExpr([NotNull] PythonParser.AddOrSubExprContext context)
        {
            object result = base.VisitAddOrSubExpr(context);

            AssemblyBuilderState assemblyBuilderState = AssemblyBuilderStateContext.GetCurrentState();
            ILGenerator generator = assemblyBuilderState.GetCurrentMethodGenerator();

            foreach (PythonParser.AddOrSubOperatorContext operatorContext in context.addOrSubOperator() ?? Array.Empty<PythonParser.AddOrSubOperatorContext>())
                generator.Emit(operatorContext.Add() is not null ? OpCodes.Add : OpCodes.Sub);

            return result;
        }

        public override object VisitPrint([NotNull] PythonParser.PrintContext context)
        {
            object result = base.VisitPrint(context);

            AssemblyBuilderState assemblyBuilderState = AssemblyBuilderStateContext.GetCurrentState();
            ILGenerator generator = assemblyBuilderState.GetCurrentMethodGenerator();

            MethodInfo writeLineMethod = typeof(Console).GetMethod(nameof(Console.WriteLine), new[] { typeof(object) })!;
            generator.Emit(OpCodes.Call, writeLineMethod);

            return result;
        }
    }
}
