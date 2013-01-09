//-----------------------------------------------------------------------
// <copyright file="Manager.cs" company="Visemet">
//     Copyright (c) Visemet. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Visemet.Environment
{
    using System;
    using System.Collections.ObjectModel;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// Manages the collection of environment variables.
    /// </summary>
    public class Manager : IXmlSerializable
    {
        /// <summary>
        /// The collection of environment variables.
        /// </summary>
        private ObservableCollection<Variable> variables = new ObservableCollection<Variable>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Manager" /> class.
        /// </summary>
        public Manager()
        {
        }

        /// <summary>
        /// Gets the collection of environment variables.
        /// </summary>
        public ObservableCollection<Variable> Variables
        {
            get
            {
                return this.variables;
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
            if (!reader.IsStartElement("Manager"))
            {
                throw new XmlException("Expected <Manager> element");
            }

            bool isEmptyElement = reader.IsEmptyElement;
            reader.ReadStartElement("Manager");

            if (!isEmptyElement)
            {
                if (!reader.IsStartElement("Variables"))
                {
                    throw new XmlException("Expected <Variables> subelement in <Manager> element");
                }

                isEmptyElement = reader.IsEmptyElement;
                reader.ReadStartElement("Variables");

                if (!isEmptyElement)
                {
                    while (reader.IsStartElement("Variable"))
                    {
                        Variable variable = new Variable();
                        variable.ReadXml(reader);
                        this.Variables.Add(variable);
                    }

                    reader.ReadEndElement(); // Variables
                }

                reader.ReadEndElement(); // Manager
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
            writer.WriteStartElement("Variables");

            foreach (Variable variable in this.Variables)
            {
                writer.WriteStartElement("Variable");
                variable.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }
}
