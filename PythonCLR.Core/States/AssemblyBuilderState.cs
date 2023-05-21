using System.Reflection.Emit;
using System.Reflection;

namespace PythonCLR.Core.States
{
    internal class AssemblyBuilderState
    {
        private const string EntryPointTypeName = "Program";
        private const string EntryPointMethodName = "Main";
        private const char NamesSeparator = '.';

        private AssemblyBuilder _assemblyBuilder;
        private ModuleBuilder _moduleBuilder;
        private LinkedList<TypeBuilder> _currentTypeBuilders;
        private LinkedList<MethodBuilder> _currentMethodBuilders;

        private Dictionary<string, TypeBuilder> _typeBuilders;
        private Dictionary<string, MethodBuilder> _methodBuilders;

        public Assembly GeneratedAssembly => _assemblyBuilder;

        public AssemblyBuilderState(string assemblyName)
        {
            _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.RunAndCollect);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(assemblyName);
            _currentTypeBuilders = new();
            _currentMethodBuilders = new();
            _typeBuilders = new();
            _methodBuilders = new();

            StartClassDefinition(EntryPointTypeName, parent: null);
            StartMethodDefinition(EntryPointMethodName, MethodAttributes.Static, argTypes: null, returnType: null);
        }

        public void StartClassDefinition(string className, Type? parent)
        {
            TypeBuilder typeBuilder = _moduleBuilder.DefineType(className, TypeAttributes.Class | TypeAttributes.Public, parent);
            _typeBuilders.Add(className, typeBuilder);
            _currentTypeBuilders.AddLast(typeBuilder);
        }

        public void AddField(string name, Type type)
        {
            // Мы всегда внутри типа EntryPointTypeName, поэтому последний элемент списка никогда не null.
            TypeBuilder typeBuilder = _currentTypeBuilders.Last!.Value;
            typeBuilder.DefineField(name, type, FieldAttributes.Public);
        }

        public void StartMethodDefinition(string methodName, MethodAttributes methodAttributes, Type[]? argTypes, Type? returnType)
        {
            // Мы всегда внутри типа EntryPointTypeName, поэтому последний элемент списка никогда не null.
            TypeBuilder currentTypeBuilder = _currentTypeBuilders.Last!.Value;
            MethodBuilder methodBuilder = currentTypeBuilder.DefineMethod(methodName, methodAttributes | MethodAttributes.Public, returnType, argTypes);
            _methodBuilders.Add($"{currentTypeBuilder.Name}{NamesSeparator}{methodName}", methodBuilder);
            _currentMethodBuilders.AddLast(methodBuilder);
        }

        public void EndClassDefinition()
        {
            TypeBuilder currentTypeBuilder = _currentTypeBuilders.Last!.Value;
            currentTypeBuilder.CreateType();
            _currentTypeBuilders.RemoveLast();
        }

        public void EndMethodDefinition()
        {
            // Добавим ret на случай void-метода.
            ILGenerator generator = GetCurrentMethodGenerator();
            generator.Emit(OpCodes.Ret);
            _currentMethodBuilders.RemoveLast();
        }

        public Type GetTypeByName(string typeName) => _typeBuilders[typeName];

        public ILGenerator GetCurrentMethodGenerator()
        {
            MethodBuilder currentMethodBuilder = _currentMethodBuilders.Last?.Value
                ?? throw new InvalidOperationException("Не внутри метода.");
            return currentMethodBuilder.GetILGenerator();
        }
    }
}
