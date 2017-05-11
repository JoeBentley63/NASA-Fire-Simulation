using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;

namespace Kinesense.Interfaces
{
    public interface ISessionLogger
    {
        //void MakeHeader(string dasd);
        void Open();
        bool Save(string filename);
        string[] FilesSaved { get; set; }
        void AddHeader(string header);
        void AddDataTable(DataTable table);
        string FileExtension { get; }
        void OpenFileInExternalApp();
        string ReportType { get; }
        void SetFont(string name, int size);
        void AddWatermark(Bitmap watermark, Bitmap watermarkMini);
        void AddTitleImageEntry(Bitmap entryImage, string notes);
        void AddList(List<string> excludedList);
        void AddFilterList(List<Tuple<string, string, ByteArrayBitmap>> filterList);
    }
}
