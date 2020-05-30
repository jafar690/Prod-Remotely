using System;
using System.Windows.Input;

namespace Silgred.Win.Services
{
    public class Executor : ICommand
    {
        public Executor(Action<object> executeAction, Predicate<object> isExecutable = null)
        {
            ExecuteAction = executeAction;
            IsExecutable = isExecutable;
        }

        private Action<object> ExecuteAction { get; }


        private Predicate<object> IsExecutable { get; }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return IsExecutable == null || IsExecutable.Invoke(parameter);
        }

        public void Execute(object parameter)
        {
            ExecuteAction.Invoke(parameter);
        }
    }
}