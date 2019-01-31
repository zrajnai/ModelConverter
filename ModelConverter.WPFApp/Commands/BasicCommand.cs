using System;
using System.Windows.Input;

namespace ModelConverter.WPFApp.Commands
{
    public class BasicCommand : ICommand
    {

        #region Member Variables

        private readonly Action _action;
        private readonly Func<bool> _canExecuteActionFunc;

        #endregion

        #region Constructors

        public BasicCommand(Action action, Func<bool> canExecuteActionFunc = null)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _canExecuteActionFunc = canExecuteActionFunc;
        }

        #endregion

        #region Events

        public event EventHandler CanExecuteChanged;

        #endregion

        #region Public Methods

        public bool CanExecute(object parameter) => _canExecuteActionFunc?.Invoke() ?? true;

        public void Execute(object parameter) => _action?.Invoke();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        #endregion

    }

}