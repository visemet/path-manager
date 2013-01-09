//-----------------------------------------------------------------------
// <copyright file="Variable.cs" company="Visemet">
//     Copyright (c) Visemet. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Visemet.Environment
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// Model for environment variables.
    /// </summary>
    public class Variable : INotifyPropertyChanged, IXmlSerializable
    {
        /// <summary>
        /// The name of this environment variable.
        /// </summary>
        private string name;

        /// <summary>
        /// The state of this environment variable.
        /// </summary>
        private bool state;

        /// <summary>
        /// The type of this environment variable.
        /// </summary>
        private VariableType? type;

        /// <summary>
        /// The collection of values associated with this environment
        /// variable.
        /// </summary>
        private ObservableCollection<Value> values = new ObservableCollection<Value>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Variable" /> class.
        /// </summary>
        public Variable()
        {
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Specifies the type of an environment variable.
        /// </summary>
        public enum VariableType
        {
            /// <summary>
            /// The environment variable is a system variable.
            /// </summary>
            System,

            /// <summary>
            /// The environment variable is a user variable.
            /// </summary>
            User
        }

        /// <summary>
        /// Gets or sets the name of this environment variable.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
                this.OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this environment
        /// variable is included.
        /// </summary>
        public bool State
        {
            get
            {
                return this.state;
            }

            set
            {
                this.state = value;
                this.OnPropertyChanged("State");
            }
        }

        /// <summary>
        /// Gets or sets the type of this environment variable.
        /// </summary>
        public VariableType? Type
        {
            get
            {
                return this.type;
            }

            set
            {
                this.type = value;
                this.OnPropertyChanged("Type");
            }
        }

        /// <summary>
        /// Gets the collection of values associated with this
        /// environment variable.
        /// </summary>
        public ObservableCollection<Value> Values
        {
            get
            {
                return this.values;
            }
        }

        /// <summary>
        /// This method is reserved and should not be used.
        /// </summary>
        /// <returns>A null XmlSchema.</returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// The <see cref="System.Xml.XmlReader" /> stream from which the object is
        /// deserialized.
        /// </param>
        public void ReadXml(XmlReader reader)
        {
            if (!reader.IsStartElement("Variable"))
            {
                throw new XmlException("Expected <Variable> element");
            }

            bool isEmptyElement = reader.IsEmptyElement;
            reader.ReadStartElement("Variable");

            if (!isEmptyElement)
            {
                if (!reader.IsStartElement("Name"))
                {
                    throw new XmlException("Expected <Name> subelement in <Variable> element");
                }

                isEmptyElement = reader.IsEmptyElement;
                reader.ReadStartElement("Name");

                if (!isEmptyElement)
                {
                    this.Name = reader.ReadContentAsString();
                    reader.ReadEndElement(); // Name
                }

                if (!reader.IsStartElement("State"))
                {
                    throw new XmlException("Expected <State> subelement in <Variable> element");
                }

                isEmptyElement = reader.IsEmptyElement;
                reader.ReadStartElement("State");

                if (!isEmptyElement)
                {
                    this.State = Convert.ToBoolean(reader.ReadContentAsString());
                    reader.ReadEndElement(); // State
                }

                if (!reader.IsStartElement("Type"))
                {
                    throw new XmlException("Expected <Type> subelement in <Variable> element");
                }

                isEmptyElement = reader.IsEmptyElement;
                reader.ReadStartElement("Type");

                if (!isEmptyElement)
                {
                    this.Type = (VariableType)Enum.Parse(typeof(VariableType), reader.ReadContentAsString());
                    reader.ReadEndElement(); // Type
                }

                if (!reader.IsStartElement("Values"))
                {
                    throw new XmlException("Expected <Values> subelement in <Variable> element");
                }

                isEmptyElement = reader.IsEmptyElement;
                reader.ReadStartElement("Values");

                if (!isEmptyElement)
                {
                    while (reader.IsStartElement("Value"))
                    {
                        Value value = new Value();
                        value.ReadXml(reader);
                        this.Values.Add(value);
                    }

                    reader.ReadEndElement(); // Values
                }

                reader.ReadEndElement(); // Variable
            }
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// The <see cref="System.Xml.XmlWriter" /> stream to which the object is
        /// serialized.
        /// </param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Name", this.Name);

            writer.WriteElementString("State", this.State.ToString());

            writer.WriteElementString("Type", this.Type.ToString());

            writer.WriteStartElement("Values");

            foreach (Value value in this.Values)
            {
                writer.WriteStartElement("Value");
                value.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return this.Name;
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
    }
}
