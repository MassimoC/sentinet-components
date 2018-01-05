using System;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Canopus.Sentinet.Routers.Context
{
    public sealed class RoutingRule : IXmlSerializable
    {
        #region Properties

        public string Name { get; set; }
        public string Value { get; set; }

        public Collection<string> Endpoints { get; private set; }

        #endregion

        #region Constructors

        public RoutingRule()
        {
            Endpoints = new Collection<string>();
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        ///     Initializes RoutingRule instance from its XML representation.
        /// </summary>
        /// <param name="reader">
        ///     The Xml reader stream from where Region instance is intialized.
        /// </param>
        public void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            Endpoints = new Collection<string>();

            if (reader.NodeType == XmlNodeType.Element &&
                reader.LocalName == "RoutingRule")
            {
                string attr = reader.GetAttribute("name");
                if (attr != null)
                {
                    Name = attr;
                }
                attr = reader.GetAttribute("value");
                if (attr != null)
                {
                    Value = attr;
                }

                bool isEmpty = reader.IsEmptyElement;
                reader.Read();

                if (!isEmpty)
                {
                    while (reader.NodeType != XmlNodeType.EndElement && !reader.EOF)
                    {
                        if (reader.LocalName == "Endpoint")
                        {
                            Endpoints.Add(reader.ReadElementContentAsString());
                        }
                        else
                        {
                            reader.ReadToNextSibling("Endpoint");
                        }
                    }

                    reader.Read();
                }
            }

        }

        /// <summary>
        ///     Seralizes RoutingRule instance in XML representation.
        /// </summary>
        /// <param name="writer">
        ///     The Xml writer stream where Route instance is serialized.
        /// </param>
        public void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            writer.WriteAttributeString("name", Name);
            writer.WriteAttributeString("value", Value);

            foreach (string endpoint in Endpoints)
            {
                writer.WriteElementString("Endpoint", endpoint);
            }
        }

        /// <summary>
        ///     Not implemented.
        /// </summary>
        public XmlSchema GetSchema()
        {
            return null;
        }

        #endregion
    }
}
