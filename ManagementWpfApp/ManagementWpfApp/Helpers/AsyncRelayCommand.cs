using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ManagementWpfApp.Helpers
{
    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;
        private bool _isExecuting;
        private readonly Action<Exception> _onException;
        private readonly Dispatcher _dispatcher;

        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute = null, Action<Exception> onException = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
            _onException = onException;
            _dispatcher = Application.Current.Dispatcher;
        }

        public bool CanExecute(object parameter) => !_isExecuting && (_canExecute?.Invoke() ?? true);

        public event EventHandler CanExecuteChanged;

        public async void Execute(object parameter)
        {
            await ExecuteAsync();
        }

        public async Task ExecuteAsync()
        {
            if (!CanExecute(null))
                return;

            try
            {
                _isExecuting = true;
                RaiseCanExecuteChanged();
                await _execute();
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

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}