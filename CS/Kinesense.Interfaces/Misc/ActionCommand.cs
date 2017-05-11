using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Kinesense.Interfaces
{
    public class ActionCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action _action;
        private bool _canExecute;

        public ActionCommand(Action action, bool canExecute = true)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public void Execute(object parameter)
        {
            _action();
        }

        public void SetCanExecute(bool value)
        {
            if(_canExecute != value)
            {
                _canExecute = value;

                if (CanExecuteChanged != null)
                    CanExecuteChanged(this, new EventArgs());
            }
        }
    }
}
