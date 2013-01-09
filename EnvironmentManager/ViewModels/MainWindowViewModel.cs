//-----------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="Visemet">
//     Copyright (c) Visemet. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Visemet.Environment
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Xml;
    using Microsoft.Win32;

    /// <summary>
    /// The view model for the main window.
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The manager for environment variables used by this view
        /// model.
        /// </summary>
        private Manager manager;

        /// <summary>
        /// A filtered view of only system variables on the collection of
        /// environment variables.
        /// </summary>
        private ListCollectionView systemVariables;

        /// <summary>
        /// A filtered view of only user variables on the collection of
        /// environment variables.
        /// </summary>
        private ListCollectionView userVariables;

        /// <summary>
        /// The currently selected system variable.
        /// </summary>
        private Variable selectedSystemVariable;

        /// <summary>
        /// The currently selected user variable.
        /// </summary>
        private Variable selectedUserVariable;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
        /// </summary>
        public MainWindowViewModel()
        {
            this.PropertyChanged += this.PropertyChangedListener;

            this.OnRevert(null);
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Specifies the option of a browse command
        /// </summary>
        public enum Browse
        {
            /// <summary>
            /// The browse command seeks to open a file.
            /// </summary>
            Open,

            /// <summary>
            /// The browse command seeks to save a file.
            /// </summary>
            Save
        }

        /// <summary>
        /// Gets the filtered view of only system variables.
        /// </summary>
        public ListCollectionView SystemVariables
        {
            get
            {
                return this.systemVariables;
            }

            private set
            {
                this.systemVariables = value;
                this.OnPropertyChanged("SystemVariables");
            }
        }

        /// <summary>
        /// Gets the filtered view of only user variables.
        /// </summary>
        public ListCollectionView UserVariables
        {
            get
            {
                return this.userVariables;
            }

            private set
            {
                this.userVariables = value;
                this.OnPropertyChanged("UserVariables");
            }
        }

        /// <summary>
        /// Gets or sets the currently selected system variable.
        /// </summary>
        public Variable SelectedSystemVariable
        {
            get
            {
                return this.selectedSystemVariable;
            }

            set
            {
                this.selectedSystemVariable = value;
                this.OnPropertyChanged("SelectedSystemVariable");
            }
        }

        /// <summary>
        /// Gets or sets the currently selected user variable.
        /// </summary>
        public Variable SelectedUserVariable
        {
            get
            {
                return this.selectedUserVariable;
            }

            set
            {
                this.selectedUserVariable = value;
                this.OnPropertyChanged("SelectedUserVariable");
            }
        }

        /// <summary>
        /// Gets the collection of values associated with the currently
        /// selected system variable.
        /// </summary>
        public ObservableCollection<Value> SystemValues
        {
            get
            {
                return this.SelectedSystemVariable == null ? null : this.SelectedSystemVariable.Values;
            }
        }

        /// <summary>
        /// Gets the collection of values associated with the currently
        /// selected user variable.
        /// </summary>
        public ObservableCollection<Value> UserValues
        {
            get
            {
                return this.SelectedUserVariable == null ? null : this.SelectedUserVariable.Values;
            }
        }

        /// <summary>
        /// Gets the selected value converted.
        /// </summary>
        public SelectedValueConverter SelectedValueConverter
        {
            get
            {
                return SelectedValueConverter.Instance;
            }
        }

        /// <summary>
        /// Gets the browse command.
        /// </summary>
        public ICommand BrowseCommand
        {
            get
            {
                return new DelegateCommand<Browse>(this.OnBrowse, this.CanBrowse);
            }
        }

        /// <summary>
        /// Gets the import command.
        /// </summary>
        public ICommand ImportCommand
        {
            get
            {
                return new DelegateCommand<string>(this.OnImport, this.CanImport);
            }
        }

        /// <summary>
        /// Gets the export command.
        /// </summary>
        public ICommand ExportCommand
        {
            get
            {
                return new DelegateCommand<string>(this.OnExport, this.CanExport);
            }
        }

        /// <summary>
        /// Gets the revert command.
        /// </summary>
        public ICommand RevertCommand
        {
            get
            {
                return new DelegateCommand<object>(this.OnRevert, this.CanRevert);
            }
        }

        /// <summary>
        /// Gets the apply command.
        /// </summary>
        public ICommand ApplyCommand
        {
            get
            {
                return new DelegateCommand<object>(this.OnApply, this.CanApply);
            }
        }

        /// <summary>
        /// Gets or sets the manager for environment variables used by
        /// this view model.
        /// </summary>
        private Manager Manager
        {
            get
            {
                return this.manager;
            }

            set
            {
                this.manager = value;
                this.OnPropertyChanged("Manager");
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event with the provided property name.
        /// </summary>
        /// <param name="name">The property name.</param>
        protected void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        /// <summary>
        /// Initializes the manager.
        /// </summary>
        private void InitializeManager()
        {
            this.InitializeVariables(EnvironmentVariableTarget.Machine);
            this.InitializeVariables(EnvironmentVariableTarget.User);
        }

        /// <summary>
        /// Initializes the environment variables for the current user of
        /// the local machine.
        /// </summary>
        /// <param name="target">
        /// One of the <see cref="System.EnvironmentVariableTarget" /> values.
        /// </param>
        private void InitializeVariables(EnvironmentVariableTarget target)
        {
            Variable.VariableType type;

            switch (target)
            {
                case EnvironmentVariableTarget.Machine:
                    type = Variable.VariableType.System;
                    break;
                case EnvironmentVariableTarget.User:
                    type = Variable.VariableType.User;
                    break;
                default:
                    throw new ArgumentException("Invalid environment variable target", "target");
            }

            IDictionary<string, string> variables = Proxy.GetEnvironmentVariables(target);

            foreach (string name in variables.Keys)
            {
                Variable variable = new Variable();
                variable.Name = name;
                variable.State = true;
                variable.Type = type;

                foreach (string content in variables[name].Split(';'))
                {
                    Value value = new Value();
                    value.Content = content;
                    value.State = true;

                    variable.Values.Add(value);
                }

                Manager.Variables.Add(variable);
            }
        }

        /// <summary>
        /// Modifies the environment variables for the current user of
        /// the local machine.
        /// </summary>
        /// <param name="target">
        /// One of the <see cref="System.EnvironmentVariableTarget" /> values.
        /// </param>
        private void ModifyVariables(EnvironmentVariableTarget target)
        {
            Func<Variable, bool> isBlankVariable;

            switch (target)
            {
                case EnvironmentVariableTarget.Machine:
                    isBlankVariable = this.IsSystemVariable;
                    break;
                case EnvironmentVariableTarget.User:
                    isBlankVariable = this.IsUserVariable;
                    break;
                default:
                    throw new ArgumentException("Invalid environment variable target", "target");
            }

            IEnumerable<Variable> variables;
            IEnumerable<Value> values;

            variables =
                from variable in this.Manager.Variables
                where variable.State && isBlankVariable(variable)
                select variable;

            foreach (Variable variable in variables)
            {
                values =
                    from value in variable.Values
                    where value.State
                    select value;

                string variableName = variable.Name;
                string valueContent = string.Join<Value>(";", values);

                Proxy.SetEnvironmentVariable(variableName, valueContent, target);
            }
        }

        /// <summary>
        /// Deletes the environment variables for the current user of the
        /// local machine.
        /// </summary>
        /// <param name="target">
        /// One of the <see cref="System.EnvironmentVariableTarget" /> values.
        /// </param>
        private void DeleteVariables(EnvironmentVariableTarget target)
        {
            IDictionary<string, string> variables = Proxy.GetEnvironmentVariables(target);

            foreach (string name in variables.Keys)
            {
                Proxy.SetEnvironmentVariable(name, null, target);
            }
        }

        /// <summary>
        /// Returns whether the browse command can execute.
        /// </summary>
        /// <param name="parameter">
        /// One of the <see cref="Browse" /> values.
        /// </param>
        /// <returns>
        /// <code>true</code> if the browse command can execute,
        /// otherwise <code>false</code>.
        /// </returns>
        private bool CanBrowse(Browse parameter)
        {
            ICommand command;

            switch (parameter)
            {
                case Browse.Open:
                    command = this.ImportCommand;
                    break;
                case Browse.Save:
                    command = this.ExportCommand;
                    break;
                default:
                    throw new ArgumentException("Invalid browse option", "parameter");
            }

            return command.CanExecute(null);
        }

        /// <summary>
        /// Executes the browse command by creating either an open or
        /// save file dialog and subsequently executing the corresponding
        /// command.
        /// </summary>
        /// <param name="parameter">
        /// One of the <see cref="Browse" /> values.
        /// </param>
        private void OnBrowse(Browse parameter)
        {
            FileDialog dialog;
            ICommand command;

            switch (parameter)
            {
                case Browse.Open:
                    dialog = new OpenFileDialog();
                    command = this.ImportCommand;
                    break;
                case Browse.Save:
                    dialog = new SaveFileDialog();
                    command = this.ExportCommand;
                    break;
                default:
                    throw new ArgumentException("Invalid browse option", "parameter");
            }

            dialog.DefaultExt = ".xml";
            dialog.Filter = "eXtensible Markup Language file (.xml)|*.xml";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string filename = dialog.FileName;
                command.Execute(filename);
            }
        }

        /// <summary>
        /// Returns whether the import command can execute.
        /// </summary>
        /// <param name="parameter">
        /// The name of the file to open.
        /// </param>
        /// <returns><code>true</code>.</returns>
        private bool CanImport(string parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the import command.
        /// </summary>
        /// <param name="parameter">The file to read from.</param>
        private void OnImport(string parameter)
        {
            XmlReader reader = XmlReader.Create(parameter);

            this.Manager.Variables.Clear();
            this.Manager.ReadXml(reader);
        }

        /// <summary>
        /// Returns whether the export command can execute.
        /// </summary>
        /// <param name="parameter">
        /// The name of the file to save.
        /// </param>
        /// <returns><code>true</code>.</returns>
        private bool CanExport(string parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the export command.
        /// </summary>
        /// <param name="parameter">The file to write to.</param>
        private void OnExport(string parameter)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            using (XmlWriter writer = XmlWriter.Create(parameter, settings))
            {
                writer.WriteStartElement("Manager");
                this.Manager.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Returns whether the revert command can execute.
        /// </summary>
        /// <param name="parameter">An unused parameter.</param>
        /// <returns><code>true</code>.</returns>
        private bool CanRevert(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the revert command.
        /// </summary>
        /// <param name="parameter">An unused parameter.</param>
        private void OnRevert(object parameter)
        {
            this.Manager = Properties.Settings.Default.EnvironmentManager;

            if (this.Manager == null)
            {
                this.Manager = new Manager();

                this.InitializeManager();
            }
        }

        /// <summary>
        /// Returns whether the apply command can execute.
        /// </summary>
        /// <param name="parameter">An unused parameter.</param>
        /// <returns><code>true</code>.</returns>
        private bool CanApply(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the apply command.
        /// </summary>
        /// <param name="parameter">An unused parameter.</param>
        private void OnApply(object parameter)
        {
            this.DeleteVariables(EnvironmentVariableTarget.Machine);
            this.ModifyVariables(EnvironmentVariableTarget.Machine);

            this.DeleteVariables(EnvironmentVariableTarget.User);
            this.ModifyVariables(EnvironmentVariableTarget.User);

            Properties.Settings.Default.EnvironmentManager = this.Manager;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Listens for property changes and fires dependencies.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Provides the data for the event.</param>
        private void PropertyChangedListener(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Manager":
                    if (Manager != null)
                    {
                        this.SystemVariables = new ListCollectionView(this.Manager.Variables);
                        this.SystemVariables.Filter += new Predicate<object>(this.IsSystemVariable);

                        this.UserVariables = new ListCollectionView(this.Manager.Variables);
                        this.UserVariables.Filter += new Predicate<object>(this.IsUserVariable);
                    }

                    break;
                case "SelectedSystemVariable":
                    this.OnPropertyChanged("SystemValues");
                    break;
                case "SelectedUserVariable":
                    this.OnPropertyChanged("UserValues");
                    break;
            }
        }

        /// <summary>
        /// Returns whether the specified variable is a system variable.
        /// </summary>
        /// <param name="parameter">A variable.</param>
        /// <returns>
        /// <code>true</code> if the specified variable is a system
        /// variable, and <code>false</code> otherwise.
        /// </returns>
        private bool IsSystemVariable(object parameter)
        {
            Variable variable = (Variable)parameter;
            return variable.Type == Variable.VariableType.System;
        }

        /// <summary>
        /// Returns whether the specified variable is a user variable.
        /// </summary>
        /// <param name="parameter">A variable.</param>
        /// <returns>
        /// <code>true</code> if the specified variable is a user
        /// variable, and <code>false</code> otherwise.
        /// </returns>
        private bool IsUserVariable(object parameter)
        {
            Variable variable = (Variable)parameter;
            return variable.Type == Variable.VariableType.User;
        }
    }
}
