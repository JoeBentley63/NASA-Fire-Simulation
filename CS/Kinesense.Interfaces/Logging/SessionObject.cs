using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.Xml.Serialization;

namespace Kinesense.Interfaces
{
    public class SessionObject
    {
        public string Header { get; set; }
        private List<DataTable> _tableList = new List<DataTable>();
        public List<DataTable> TableList
        {
            get { return _tableList; }
            set { _tableList = value; }
        }
        //public DataTable ImportTable { get; set; }
        //public DataTable ViewerTable { get; set; }
        //public DataTable ReportTable { get; set; }
        //public DataTable VideoReportTable { get; set; }
        //public DataTable TimeZoneTable { get; set; }
        //public DataTable UserEventsTable { get; set; }
        //public DataTable PDFReportTable { get; set; }
        //public DataTable MultiImportTable { get; set; }

        [XmlIgnoreAttribute]
        public List<Tuple<string, string, ByteArrayBitmap>> FilterList { get; set; }
        public List<string> ExcludedList { get; set; }
    }

    //public class SessionLogSerializer
    //{
    //    public static void EncodeToXml(string path, SessionObject sessionObj)
    //    {
    //        try
    //        {
    //            XmlDocument document = new XmlDocument();
    //            document.AppendChild(document.CreateXmlDeclaration("1.0", null, null));

    //            var node = document.CreateNode(XmlNodeType.Element, "SessionLog", null);

    //            XmlDocumentHelper doc = new XmlDocumentHelper(document, node);
    //            doc.AddAttribute("Header", sessionObj.Header);
    //            //doc.AddAttribute("ImportTable", 
    //        }
    //        catch (Exception ee)
    //        {
    //            DebugMessageLogger.LogError(ee);
    //        }
    //    }
    //}
}
