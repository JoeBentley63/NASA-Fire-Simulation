using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinesense.Interfaces.Enum;
using System.IO;

namespace Kinesense.Interfaces.Classes
{
    public interface ISessionLogSystem
    {
        void AddExportedClip(Guid videoID, string videoName, DateTime stime, DateTime etime, string offset, string filename);
        void AddUserEvent(string username, string eventText);
        void LogFrameViewed(string videoName, Guid videoID, DateTime time, string viewingType, string offset);
        void AddStartTime(string videoName, Guid videoID, DateTime startTime, string viewingType, string offset);
        void AddEndTime(DateTime endTime);
        void AddImportStartTime(string videoName, Guid videoID, DateTime startTime, DateTime endTime, string extra);
        void AddTimeZoneInfo(Guid videoID, string videoName, TimeZoneInfo timeZoneInfo, string offset, string comments);
        void AddImportEndTime(Guid videoID, DateTime endTime);
        void AddVideoReport(string name, DateTime exportedAt, int numberofvideos);
        void AddMutliVideos(Guid videoID, string videoSource, string fileName);
        void AddMultiVideoEndTime(string fileName, Guid videoID);
        void UpdateVideoHash(string name, string hash);
        void AddPDFReport(string name, PDFSessionTypes type, int numberofImages, DateTime exportTime, string hash);
        void LoadXML(object filename);
        void SaveThread();
        bool SaveToPDF(List<string> paths, bool isNow, string identifier, object typeList);
        void SaveToXML(string path);
        void SaveToXML(Stream stream, bool maskdata, string inserttext);
        void SaveThreadDispose();
        void SaveViewingLogs();
        void AddVideoMetadata(Guid videoGuid,string videoName, Dictionary<string, string> metadata, MetadataAction action);
    } 

    
}
