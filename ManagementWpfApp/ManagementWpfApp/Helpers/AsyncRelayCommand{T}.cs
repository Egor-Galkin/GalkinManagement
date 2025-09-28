using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ManagementWpfApp.Helpers
{
    public class AsyncRelayCommand<T> : ICommand
    {
        private readonly Func<T, Task> _execute;
        private readonly Predicate<T> _canExecute;
        private bool _isExecuting;
        private readonly Action<Exception> _onException;
        private readonly Dispatcher _dispatcher;

        public AsyncRelayCommand(Func<T, Task> execute, Predicate<T> canExecute = null, Action<Exception> onException = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
            _onException = onException;
            _dispatcher = Application.Current.Dispatcher;
        }

        public bool CanExecute(object parameter)
        {
            if (_isExecuting)
                return false;

            if (_canExecute == null)
                return true;

            if (parameter == null && typeof(T).IsValueType)
                return _canExecute(default(T));

            return parameter is T t && _canExecute(t);
        }

        public event EventHandler CanExecuteChanged;

        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        public async Task ExecuteAsync(object parameter)
        {
            if (!CanExecute(parameter)) return;

            try
            {
                _isExecuting = true;
                RaiseCanExecuteChanged();

                T paramValue = parameter is T t ? t : default;

                await _execute(paramValue);
            }
            catch (Exception ex)
            {
                if (_onException != null)
                {
                    await _dispatcher.BeginInvoke(new Action(() => _onException(ex)));
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}