using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Kinesense.Interfaces.Clarification
{
    //public class ClarificationSetting
    //{
    //    public ClarificationSetting(string name, object value)
    //    {            
    //        this.Name = name;
    //        this.Value = value;

    //    }
    //    /// <summary>
    //    /// This Name must be unique to this settings group or bad things will happen
    //    /// </summary>
    //    public string Name { get; private set; }
    //    public object Value { get; set; }
    //}


    public class ClarificationSettings
    {
        public string ClarificationName;
        public int ClarificationSettingsVersion;
        public SerializableDictionary<string, Object> Settings = new SerializableDictionary<string, object>();

        
        public string ToXML()
        {
            try
            {
                var serializer = new XmlSerializer(typeof(ClarificationSettings));
                var sb = new StringBuilder();

                using (TextWriter writer = new StringWriter(sb))
                {
                    serializer.Serialize(writer, this);
                }

                return sb.ToString();
            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
            }
            return "ERROR";
        }
        
        public static ClarificationSettings NewFromXML(string data)
        {
            if (data == "ERROR")
                return null;

            XmlSerializer deserializer = new XmlSerializer(typeof(ClarificationSettings));

            ClarificationSettings cSet = null;
            try
            {
                using (TextReader reader = new StringReader(data))
                {
                    cSet = (ClarificationSettings)deserializer.Deserialize(reader);
                }
            }
            catch (Exception er)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(er);
            }
            return cSet;
        }
    }
}
