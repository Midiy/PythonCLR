using PythonCLR.Core.Contexts;
using PythonCLR.Core.States;

namespace PythonCLR.Core.Helpers
{
    internal static class TypesHelper
    {
        private static readonly Dictionary<string, Type> _knownTypes = new()
        {
            ["str"] = typeof(string),
            ["int"] = typeof(int),
            ["bool"] = typeof(bool)
        };

        public static Type? GetCLRTypeByPythonName(string? name)
        {
            if (name is null)
                return null;

            if (_knownTypes.TryGetValue(name, out var type)) 
                return type;

            AssemblyBuilderState assemblyBuilderState = AssemblyBuilderStateContext.GetCurrentState();
            type = assemblyBuilderState.GetTypeByName(name);
            _knownTypes.Add(name, type);
            return type;
        }
    }
}
