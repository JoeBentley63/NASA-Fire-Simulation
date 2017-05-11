using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Kinesense.Interfaces
{
    public struct SpriteData
    {
        // constructors
        public SpriteData(SpriteData s)
        {
            this._recognitionData = s.RecognitionData.Clone() as byte[];
            this._sourceDatabase = s.SourceDatabase;
            this._spriteColour = s.SpriteColour;
            this._spriteDirection = s._spriteDirection;
            this._spriteID = s.SpriteID;
            this._spriteType = s.SpriteType;
            this._videoSourceID = s.VideoSourceID;
        }

        // Required Data
        private int _spriteID;
		public int SpriteID { get{ return _spriteID;} set{ _spriteID = value;} }

        private int _spriteType;
        public int SpriteType { get { return _spriteType; } set{ _spriteType = value;} }

        private int _spriteColour;
        public int SpriteColour { get { return _spriteColour; } set{ _spriteColour = value;} }

        private int _spriteDirection;
        public int SpriteDirection { get { return _spriteColour; } set { _spriteColour = value; } }

        private byte[] _recognitionData;
        public byte[] RecognitionData { 
            get 
            {
                if (_recognitionData != null)
                {
                    return _recognitionData.Clone() as byte[];
                }
                else
                {
                    return new byte[1] {0};
                }
            } set { _recognitionData = value.Clone() as byte[]; } }

        /// <summary>
        /// Origin data for audit trail
        /// </summary>
        private int _videoSourceID;
        public int VideoSourceID { get { return _videoSourceID; } set{ _videoSourceID = value;} }

        /// <summary>
        /// Path string for project file
        /// </summary>
        private string _sourceDatabase;
        public String SourceDatabase { get { return _sourceDatabase; } set{ _sourceDatabase = value;} }
    }

    public struct SpritePositionData
    {
        // copy constructor
        public SpritePositionData(SpritePositionData s)
        {
            this._activity = s._activity;
            this._behaviour = s._behaviour;
            this._bottomRight = s._bottomRight;
            this._colour1 = s._colour1;
            this._colour2 = s._colour2;
            this._direction = s._direction;
            this._frameTime = s._frameTime;
            this._originVideoSourceID = s._originVideoSourceID;
            this._sourcedatabase = s._sourcedatabase;
            this._speed = s._speed;
            this._spriteID = s._spriteID;
            this._topLeft = s._topLeft;
        }

        //General Data
        private DateTime _frameTime;
        public DateTime FrameTime { get { return _frameTime; } set { _frameTime = value; } }

        private System.Drawing.Point _topLeft;
        public System.Drawing.Point TopLeft { get { return _topLeft; } set{ _topLeft = value;} }

        private System.Drawing.Point _bottomRight;
        public System.Drawing.Point BottomRight { get{ return _bottomRight;} set{ _bottomRight = value;} }

        private int _direction;
		public int Direction { get{ return _direction;} set{ _direction = value;} }

        private int _speed;
		public int Speed { get{ return _speed;} set{ _speed = value;} }

        private int _activity;
		public int Activity { get{ return _activity;} set{ _activity = value;} }

        private int _behaviour;
		public int Behaviour { get{ return _behaviour;} set{ _behaviour = value;} }

        private int _colour1;
		public int Colour1 { get{ return _colour1;} set{ _colour1 = value;} }

        private int _colour2;
        public int Colour2 { get { return _colour2; } set{ _colour2 = value;} }

        // Origin data for complete audit trail

        private int _originVideoSourceID;
		public int OriginVideoSourceID { get{ return _originVideoSourceID;} set{ _originVideoSourceID = value;} }

        /// <summary>
        /// Path string for project file
        /// </summary>
        private string _sourcedatabase;
        public String SourceDatabase { get { return _sourcedatabase; } set{ _sourcedatabase = value;} }

        private int _spriteID;
		public int SpriteID { get{ return _spriteID;} set{ _spriteID = value;} }
    }

    public interface ISimpleSpriteDataRecovery
    {
        /// <summary>
        /// connects to the database from the given project file
        /// </summary>
        /// <param name="projectFileLocation">Location of project file of DB to open</param>
        /// <returns>True if successful, False if fail</returns>
        bool OpenDatabase(string projectFileLocation);

		event EventHandler<UnhandledExceptionEventArgs> DatabaseOpened;

        /// <summary>
        /// Closes the DB connection
        /// </summary>
        /// <returns>True if successful, False if problem</returns>
        bool CloseDatabase();


        /// <summary>
        /// Returns a list of all videos included within the database
        /// </summary>
        /// <returns>ICollection of video numbers</returns>
        ICollection<int> GetListVideoSourceIDs();


        /// <summary>
        /// Returns a list of all sprites in the videoSourceID provided
        /// </summary>
        /// <param name="videoSourceID"></param>
        /// <returns>Icollection of sprite numbers</returns>
        ICollection<int> GetListSpriteIDsInVideoSource(int videoSourceID);



        /// <summary>
        /// Returns data about the given sprite No.
        /// </summary>
        /// <param name="videoSourceID">Video No. to look for sprite within</param>
        /// <param name="spriteID">Sprite No. within database to look at</param>
        /// <returns>ISpriteData object of sprite data</returns>
        SpriteData GetSpriteData(int videoSourceID, int spriteID);

		/// <summary>
		/// Returns list of spritepositiondata for given sprite id
		/// </summary>
		/// <param name="videoSourceID"></param>
		/// <param name="spriteID"></param>
		/// <returns></returns>
		ICollection<SpritePositionData> GetSpritePositionDataList(int videoSourceID, int spriteID);

        /// <summary>
        /// Returns data about the given frame in the given sprite.
        /// </summary>
        /// <param name="videoSourceID">Video No. to look for sprite within</param>
        /// <param name="spriteID">Sprite to look within</param>
		/// <param name="frameTime">Frame to look for</param>
        /// <returns>ISpritePositionData object of frame data</returns>
        SpritePositionData GetSpritePositionData(int videoSourceID, int spriteID, DateTime frameTime);

        /// <summary>
        /// Returns an image in the interfaces.ByteArrayBitmap format of the frame at the given timestamp in the given video.
        /// </summary>
        /// <param name="videoSourceID">Video No. to look for sprite within</param>
        /// <param name="frameTime">Time stamp of frame</param>
        /// <returns></returns>
		Kinesense.Interfaces.ByteArrayBitmap GetFrameByteArrayBitmap(int videoSourceID, DateTime frameTime);
    }
}
