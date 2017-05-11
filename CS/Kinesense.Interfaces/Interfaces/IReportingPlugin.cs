using Kinesense.Interfaces.Classes;
using Kinesense.Interfaces.EventArguments;
using System;
using System.Drawing;

namespace Kinesense.Interfaces
{
    public interface IReportingPlugin
    {
        void Open(string options);
        void MakeHeader(string userID, string caseID, string caseNotes, string caseTitle, int numbEvents);
        void AddTitlePageEntry(string userID, string caseID, string caseNotes, string caseTitle, int numbEvents, System.Drawing.Bitmap titleImage);
        void AddNotesPageEntry(string title, string notes);

        void AddDisclosurePage(
            string reportImageName, 
            string imageName, 
            string videoSource, 
            DateTime startTime, 
            DateTime startTimeDatabase, 
            string privateNotes, 
            string reportNotes, 
            DateTime endTime, 
            DateTime endTimeDatabase, 
            TimeSpan duration, 
            TimeSpan titleDuration, 
            TimeZoneInfo databaseTimeZone);

        void AddTROEntry(TagsReportOutputObject data);
        void AddTROHeader(TagsReportOutputObject data);
        event EventHandler<BasicPercentCompleteEventArgs> _TROEntryReportProgress;

        void AddImageEntry(Bitmap entryImage, string notes);
        void AddImageEntry(Bitmap entryImage, string notes, string title);

        void AddImageEntry(System.Drawing.Bitmap footerBackground, string caseID, string hash, string time, int eventNumber, System.Drawing.Bitmap entryImage, string notes, string title, bool allowUpsize, string name, bool uncompressed, bool hasBeenClarified, bool hasBeenAnnotated);

        void AddWatermark(Bitmap watermark, Bitmap watermarkMini);
        void AddSingleImageWithFullTitle(System.Drawing.Bitmap titleImage, System.Drawing.Bitmap reportImage, System.Drawing.Bitmap footerImage, string userID, string caseID, string reportTitle, string imageTitle, string notes, bool allowUpsize, bool uncompressed);
        void AddEntry(string text);
        void AddEntry(string text, string title);
        void Save(string filename);
        void CloseFile(bool savechanges);
        void OpenFileInExternalApp();
        bool CanAddSecurityCode { get; }
        bool MakeReportAnonomous { get; set; }

        string FileExtension { get; }

        string ReportType { get; }
        void SetFont(string name, int size);

        //IDebugMessageManager DebugMessageManager { get; set; }
    }
}
