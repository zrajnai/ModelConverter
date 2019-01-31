using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModelConverter.WPFApp.Commands
{
    public class AsyncCommand : ICommand
    {

        #region Member Variables

        private readonly Func<bool> _canExecuteFunc;
        private readonly Func<Task> _command;

        #endregion

        #region Constructors

        public AsyncCommand(Func<Task> command, Func<bool> canExecuteFunc)
        {
            _command = command;
            _canExecuteFunc = canExecuteFunc;
        }

        #endregion

        #region Events

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        #endregion

        #region Public Methods

        public bool CanExecute(object parameter) => _canExecuteFunc?.Invoke() ?? true;

        public async void Execute(object parameter)
        {
            await _command();
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        #endregion

    }
}