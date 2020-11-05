using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace InfoInkasService.Core.Tools
{
    public class XmlSerializerHelper
    {

        /// <summary>
        /// Deserializes an xml document back into an object
        /// </summary>
        /// <param name="xml">The xml data to deserialize</param>
        /// <param name="type">The type of the object being deserialized</param>
        /// <returns>A deserialized object</returns>
        public static bool TryDeserialize<T>(XmlDocument xml, out T result) where T : new()
        {
            result = new T();
            XmlSerializer s = new XmlSerializer(result.GetType());
            string xmlString = xml.OuterXml.ToString();
            byte[] buffer = ASCIIEncoding.UTF8.GetBytes(xmlString);
            MemoryStream ms = new MemoryStream(buffer);
            XmlReader reader = new XmlTextReader(ms);
            Exception caught = null;

            try
            {
                object o = s.Deserialize(reader);
                result = (T)o;
                return true;
            }

            catch (Exception e)
            {
                caught = e;
            }
            finally
            {
                reader.Close();

                if (caught != null)
                    throw caught;
            }
            return false;
        }
    }
}
