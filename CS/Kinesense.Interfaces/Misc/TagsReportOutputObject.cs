using System;
using System.Collections.Generic;

namespace Kinesense.Interfaces.Classes
{
    public class TagsReportOutputObject
    {

        /// <summary>
        /// Document header data, if empty no space is reserved
        /// </summary>
        public List<object> docHeaderTextAndEmph = new List<object>();

        /// <summary>
        /// Text to be written at given position
        /// </summary>
        public string FooterText = "";

        /// <summary>
        /// Left, Centre, Right
        /// </summary>
        public TextPosition FooterTextPosition = TextPosition.left;

        public enum TextPosition { left, centre, right };

        /// <summary>
        /// On / off
        /// </summary>
        public bool ShowPageNumbers = true;

        /// <summary>
        /// Document margins in mm
        /// </summary>
        public int[] docMargins = null;

        /// <summary>
        /// Headers
        /// </summary>
        public List<TagsReportTableHeaderElement> TableHeaders = new List<TagsReportTableHeaderElement>();

        /// <summary>
        /// Data
        /// Not all fields need to be filled for each row.
        /// </summary>
        public List<List<TagsReportOutputElement>> TableData = new List<List<TagsReportOutputElement>>();

        /// <summary>
        /// The Image width for image entries into the table
        /// </summary>
        public int ImageWidth = 200;



    }
    public enum KnowColumnCodes
    {
        blank,
        custom,
        recordingdatetime,
        realdatetime,
        realandrecordingdatetime,
        videosourcename,
        notes,
        image,
        videosourcegroup
    };

    public class TagsReportOutputElement
    {
        //public KnowColumnCodes AttatchesToHeaderCodeName = KnowColumnCodes.blank;
        public int AttatchedToHeaderColumnNumber = -1;
        public string Data = "";
        public bool DataIsPathToImage = false;
        //public string DataFormatingString = "";

        public override string ToString()
        {
            return Data + " -- DataIsPathToImage=" + DataIsPathToImage.ToString() + " -- AttatchedToHeaderColumnNumber=" + AttatchedToHeaderColumnNumber.ToString();
        }
    }

    public class TagsReportTableHeaderElement
    {
        public KnowColumnCodes HeaderCodeName = KnowColumnCodes.blank;
        public string HeaderText = "";
        public string DataFormatingString = "";
        public string CustomDataSourceName = "";
        public int ColumnNumber = -1;

        public override string ToString()
        {
            return HeaderText + 
                " -- DataFormatingString=" + DataFormatingString.ToString() + 
                " -- ColumnNumber=" + ColumnNumber.ToString() + 
                " -- HeaderCodeName=" + HeaderCodeName.ToString() +
                " -- CustomDataSourceName=" + CustomDataSourceName.ToString();
        }
    }


}
