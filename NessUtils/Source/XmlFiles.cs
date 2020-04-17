using System.Xml.Serialization;
using System.IO;


namespace Ness.Utils
{
    /// <summary>
    /// Save / load XML files.
    /// </summary>
    public static class XmlFiles
    {
        /// <summary>
        /// Show last XML file read. For debug purposes.
        /// </summary>
        public static string LastReadFile { get; private set; }

        /// <summary>
        /// Save XML file.
        /// </summary>
        /// <param name="obj">Object to save.</param>
        /// <param name="filename">Output file name.</param>
        public static void SaveXml(object obj, string filename)
        {
            using (var writer = new StreamWriter(filename))
            {
                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(writer, obj);
                writer.Flush();
            }
        }

        /// <summary>
        /// Read an XML file.
        /// </summary>
        /// <param name="filename">Input file name.</param>
        public static T LoadXml<T>(string filename)
        {
            LastReadFile = filename;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (var reader = File.OpenText(filename))
            {
                XmlDeserializationEvents eventsHandler = new XmlDeserializationEvents()
                {
                    OnUnknownAttribute = (object sender, XmlAttributeEventArgs e) => { throw new System.Exception("Error parsing file '" + filename + "': invalid attribute '" + e.Attr.Name + "' at line " + e.LineNumber); },
                    OnUnknownElement = (object sender, XmlElementEventArgs e) => { throw new System.Exception("Error parsing file '" + filename + "': invalid element '" + e.Element.Name + "' at line " + e.LineNumber); },
                    OnUnknownNode = (object sender, XmlNodeEventArgs e) => { throw new System.Exception("Error parsing file '" + filename + "': invalid element '" + e.Name + "' at line " + e.LineNumber); },
                    OnUnreferencedObject = (object sender, UnreferencedObjectEventArgs e) => { throw new System.Exception("Error parsing file '" + filename + "': unreferenced object '" + e.UnreferencedObject.ToString() + "'"); },
                };
                return (T)serializer.Deserialize(System.Xml.XmlReader.Create(reader), eventsHandler);
            }
        }

        /// <summary>
        /// Get an array with xml file names for a given folder.
        /// Return just file name, without the extension or path.
        /// </summary>
        public static string[] GetXmlFileNames(string folder)
        {
            var files = Directory.GetFiles(folder, "*.xml", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; ++i)
            {
                files[i] = Path.GetFileNameWithoutExtension(files[i]);
            }
            return files;
        }
    }
}
