using PostSharp.Extensibility;
using PostSharp.Patterns.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MovieList.DB.File
{
    [Log(AttributeTargetMemberAttributes = MulticastAttributes.Private | MulticastAttributes.Protected | MulticastAttributes.Internal | MulticastAttributes.Public)]
    public class SerializeHelper
    {
        #region Methods

        /// <summary>
        /// Deserializes an xml file into an object list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public T DeSerializeObject<T>(string fileName) where T : new()
        {

            if (string.IsNullOrEmpty(fileName) || !System.IO.File.Exists(fileName)) { return new T(); }
            T objectOut = new T();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(fileName);
            string xmlString = xmlDocument.OuterXml;
            using (StringReader read = new StringReader(xmlString))
            {
                Type outType = typeof(T);
                XmlSerializer serializer = new XmlSerializer(outType);
                using (XmlReader reader = new XmlTextReader(read))
                {
                    objectOut = (T)serializer.Deserialize(reader);
                    reader.Close();
                }
                read.Close();
            }
            return objectOut;
        }

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializableObject"></param>
        /// <param name="fileName"></param>
        public void SerializeObject<T>(T serializableObject, string fileName)
        {
            if (serializableObject == null) { return; }
            XmlDocument xmlDocument = new XmlDocument();
            XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, serializableObject);
                stream.Position = 0;
                xmlDocument.Load(stream);
                xmlDocument.Save(fileName);
                stream.Close();
            }
        }

        #endregion Methods
    }
}
