using PythonCLR.Core.States;

namespace PythonCLR.Core.Contexts
{
    internal class AssemblyBuilderStateContext : IDisposable
    {
        #region Static
        // У Interlocked.CompareExchange внезапно нет перегрузки для bool, а мне хотелось его сюда вставить.
        private static int _contextCreated = 0;
        private static AssemblyBuilderStateContext? _context;

        public static AssemblyBuilderStateContext Create(string assemblyName)
        {
            // Какая-то защита от многопоточного использования.
            if (Interlocked.CompareExchange(ref _contextCreated, value: 1, comparand: 0) != 0)
                throw new InvalidOperationException("Контекст уже создан.");

            _context = new AssemblyBuilderStateContext(assemblyName);
            return _context;
        }

        public static AssemblyBuilderState GetCurrentState()
        {
            if (_contextCreated != 1)
                throw new InvalidOperationException("Контекст ещё не создан или уже уничтожен.");

            // Многопоточность тут в любом случае ломается, потому что уничтожение контекста не уничтожает хранимое значение.
            return _context!._state;
        }
        #endregion

        #region Instance
        private readonly AssemblyBuilderState _state;

        private AssemblyBuilderStateContext(string assemblyName)
        {
            _state = new AssemblyBuilderState(assemblyName);
        }

        public void Dispose() => Interlocked.CompareExchange(ref _contextCreated, value: 0, comparand: 1);
        #endregion
    }
}
