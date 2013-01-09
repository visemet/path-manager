//-----------------------------------------------------------------------
// <copyright file="Value.cs" company="Visemet">
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
    /// Model for environment values.
    /// </summary>
    public class Value : INotifyPropertyChanged, IXmlSerializable
    {
        /// <summary>
        /// The content of this environment value.
        /// </summary>
        private string content;

        /// <summary>
        /// The state of this environment value.
        /// </summary>
        private bool state;

        /// <summary>
        /// Initializes a new instance of the <see cref="Value" /> class.
        /// </summary>
        public Value()
        {
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the content of this environment value.
        /// </summary>
        public string Content
        {
            get
            {
                return this.content;
            }

            set
            {
                this.content = value;
                this.OnPropertyChanged("Content");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this environment
        /// value is included.
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
            if (!reader.IsStartElement("Value"))
            {
                throw new XmlException("Expected <Value> element");
            }

            bool isEmptyElement = reader.IsEmptyElement;
            reader.ReadStartElement("Value");

            if (!isEmptyElement)
            {
                if (!reader.IsStartElement("Content"))
                {
                    throw new XmlException("Expected <Content> subelement in <Value> element");
                }

                isEmptyElement = reader.IsEmptyElement;
                reader.ReadStartElement("Content");

                if (!isEmptyElement)
                {
                    this.Content = reader.ReadContentAsString();
                    reader.ReadEndElement(); // Content
                }

                if (!reader.IsStartElement("State"))
                {
                    throw new XmlException("Expected <State> subelement in <Value> element");
                }

                isEmptyElement = reader.IsEmptyElement;
                reader.ReadStartElement("State");

                if (!isEmptyElement)
                {
                    this.State = Convert.ToBoolean(reader.ReadContentAsString());
                    reader.ReadEndElement();
                }

                reader.ReadEndElement(); // State
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
            writer.WriteElementString("Content", this.Content);
            writer.WriteElementString("State", this.State.ToString());
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return this.Content;
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
