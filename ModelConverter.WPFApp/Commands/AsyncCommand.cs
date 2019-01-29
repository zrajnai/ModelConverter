using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModelConverter.WPFApp.Commands
{
    public class AsyncCommand : ICommand
    {
        private readonly Func<Task> _command;
        private readonly Func<bool> _canExecuteFunc;

        public AsyncCommand(Func<Task> command, Func<bool> canExecuteFunc)
        {
            _command = command;
            _canExecuteFunc = canExecuteFunc;
        }

        public bool CanExecute(object parameter) => _canExecuteFunc?.Invoke() ?? true;

        public async void Execute(object parameter)
        {
            await _command();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}