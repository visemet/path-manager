//-----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Visemet">
//     Copyright (c) Visemet. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Visemet.Environment
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" />
        /// class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Sets the new variable as a system variable.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Provides the data for the event.</param>aram>
        private void OnInitializingSystemVariable(object sender, InitializingNewItemEventArgs e)
        {
            ((Variable)e.NewItem).Type = Variable.VariableType.System;
        }

        /// <summary>
        /// Sets the new variable as a user variable.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Provides the data for the event.</param>
        private void OnInitializingUserVariable(object sender, InitializingNewItemEventArgs e)
        {
            ((Variable)e.NewItem).Type = Variable.VariableType.User;
        }
    }
}
