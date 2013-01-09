//-----------------------------------------------------------------------
// <copyright file="DelegateCommand.cs" company="Visemet">
//     Copyright (c) Visemet. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Visemet.Environment
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// A command that delegates its <see cref="CanExecute(object)"/> and
    /// its <see cref="Execute(object)"/> methods.
    /// </summary>
    /// <typeparam name="T">The type of parameters passed.</typeparam>
    public class DelegateCommand<T> : ICommand
    {
        /// <summary>
        /// The method that executes the command.
        /// </summary>
        private Action<T> executeMethod;

        /// <summary>
        /// The method that indicates whether the command can execute.
        /// </summary>
        private Func<T, bool> canExecuteMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand{T}" /> class.
        /// </summary>
        /// <param name="executeMethod">The method to execute.</param>
        /// <param name="canExecuteMethod">
        /// The method to indicate whether the command can execute.
        /// </param>
        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
        {
            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the
        /// command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Defines the method that determines whether the command can
        /// execute in its current state.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. If the command does not require
        /// data to be passed, this object can be set to null.
        /// </param>
        /// <returns>
        /// <code>true</code> if this command can be executed; otherwise
        /// <code>false</code>.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return this.canExecuteMethod((T)parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. If the command does not require
        /// data to be passed, this object can be set to null.
        /// </param>
        public void Execute(object parameter)
        {
            this.executeMethod((T)parameter);
        }

        /// <summary>
        /// Raises <see cref="CanExecuteChange"/>.
        /// </summary>
        protected void OnCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
            {
                this.CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
