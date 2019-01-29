using System;
using System.Windows.Input;

namespace ModelConverter.WPFApp {
    public class BasicCommand : ICommand
    {
        private readonly Action _action;
        private readonly Func<bool> _canExecuteActionFunc;

        public BasicCommand(Action action, Func<bool> canExecuteActionFunc = null)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _canExecuteActionFunc = canExecuteActionFunc;
        }

        public bool CanExecute(object parameter) => _canExecuteActionFunc?.Invoke() ?? true;

        public void Execute(object parameter) => _action?.Invoke();

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    }

}