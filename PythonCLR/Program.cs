using PythonCLR.Core;
using System.Reflection;

namespace PythonCLR
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string code =
                """
                print("abc")

                """;

            PythonTranslator.TranslateCodeToAssembly(code, "Test");

            Assembly createdAssembly = Assembly.LoadFrom("./Test.dll");
            Type entryPointType = createdAssembly.GetType("Program")!;
            object instance = Activator.CreateInstance(entryPointType)!;
            entryPointType.GetMethod("Main")!.Invoke(instance, null);
        }
    }
}