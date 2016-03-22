using System;
using System.Windows.Input;

namespace DummyImageViewer
{
    /// <summary>
    /// SimpleDelegateCommand
    /// </summary>
    public class SimpleDelegateCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        /// <summary>
        /// Si verifica quando vi sono delle modifiche che influiscono sull'esecuzione del comando.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleDelegateCommand"/> class.
        /// </summary>
        /// <param name="execute">The execute.</param>
        public SimpleDelegateCommand(Action<object> execute)
            : this(execute, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleDelegateCommand"/> class.
        /// </summary>
        /// <param name="execute">The execute.</param>
        /// <param name="canExecute">The can execute.</param>
        public SimpleDelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Definisce il metodo che determina se il comando può essere eseguito nello stato corrente.
        /// </summary>
        /// <param name="parameter">Dati utilizzati dal comando.Se il comando non richiede dati da passare, questo oggetto può essere impostato su null.</param>
        /// <returns>
        /// true se questo comando può essere eseguito. In caso contrario false.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
                return true;

            return _canExecute(parameter);
        }

        /// <summary>
        /// Definisce il metodo da chiamare quando viene richiamato il comando.
        /// </summary>
        /// <param name="parameter">Dati utilizzati dal comando.Se il comando non richiede dati da passare, questo oggetto può essere impostato su null.</param>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// Raises the can execute changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged == null)
                return;

            CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}
