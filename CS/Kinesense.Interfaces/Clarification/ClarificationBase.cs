using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Kinesense.Interfaces.Clarification
{
    public abstract class ClarificationBase
    {
        #region UI
        private object _UIEL;
        private System.Type _TheType;
        protected void SetBaseUIEL(object val, System.Type type)
        {            
            _UIEL = val;
            _TheType = type;
        }
        public UIElement EnhancementUIElement
        {
            get
            {
                if (_UIEL == null)
                    UIEL_Setup();
                return _UIEL as UIElement;
            }
        }

        #endregion


        #region Error State
        protected void UIEL_ErrorStateSet(bool state)
        {
            _HasEncounteredError = state;
        }
        protected bool _HasEncounteredError = false;
        public bool HasEncouteredError
        {
            get { return _HasEncounteredError; }
        }
        #endregion

        #region Settings
        protected ClarificationSettings _Settings = new ClarificationSettings();
        public ClarificationSettings SettingsBase
        {
            get { return _Settings; }
            set
            {
                _Settings = value;
                SetUIELFromSettings();
            }
        }
        #endregion

        #region SourceImage
        protected ByteArrayBitmap _SourceImage;
        protected ByteArrayBitmap SourceImageBase
        {
            get
            {
                return _SourceImage;
            }
            set
            {
                if (value == _SourceImage)
                    return;

                if (_UIEL == null)
                    UIEL_Setup();

                _SourceImage = value;
                SetResultImage(null);
                NewSourceImageSet();
            }
        }

        protected void NewSourceImageSet()
        {
            if (_UIEL != null)
                _TheType.InvokeMember("SetSourceImage", System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod, null, _UIEL, new object[1] { this._SourceImage });

                //UIEL.SourceImage = this._SourceImage;
        }
        #endregion

        #region ResultImage
        public event EventHandler NewResultImageSet;
        protected ByteArrayBitmap _ResultImage;
        protected ByteArrayBitmap ResultImageBase
        {
            get { return _ResultImage; }
            set { _ResultImage = value; }
        }

        protected void SetResultImage(ByteArrayBitmap im)
        {
            _ResultImage = im;
            if (NewResultImageSet != null)
                NewResultImageSet(this, null);
        }

        protected void UIEL_ResultImageSet(object sender, EventArgs e)
        {
            SetSettingsFromUIEL();           

            if (_TheType.InvokeMember("GetResultImage", 
                System.Reflection.BindingFlags.DeclaredOnly 
                | System.Reflection.BindingFlags.Public 
                | System.Reflection.BindingFlags.Instance 
                | System.Reflection.BindingFlags.InvokeMethod, null,_UIEL,null)  != null)
                SetResultImage(((ByteArrayBitmap)_TheType.InvokeMember("GetResultImage", 
                    System.Reflection.BindingFlags.DeclaredOnly 
                    | System.Reflection.BindingFlags.Public 
                    | System.Reflection.BindingFlags.Instance 
                    | System.Reflection.BindingFlags.InvokeMethod, null,_UIEL,null)).Clone());
            else
                SetResultImage(null);

        }
        #endregion


        protected abstract void SetUIELFromSettings();
        protected abstract void SetSettingsFromUIEL();
        protected abstract void UIEL_Setup();
        protected abstract string SettingsHR(ClarificationSettings settings);

        public void ApplyChanges()
        {
            try
            {
                if (_UIEL != null)
                    _TheType.InvokeMember("ApplyChanges", System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod, null, _UIEL, null);

                    //UIEL.ApplyChanges();
            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
            }
        }

        public bool HasHadChanges
        {
            get { return _HasHadChanges; }
        }
        protected bool _HasHadChanges = false;

        public bool ChangesImageDimensions
        {
            get { return _ChangesImageDimensions; }
        }
        protected bool _ChangesImageDimensions = false;

        protected void UIEL_ValuesChangedButNotApplied(bool val)
        {
            _HasHadChanges = val;
        }

        /// <summary>
        /// if time is null, get current or most recently accessed time
        /// </summary>
        /// <param name="VideoSourceID"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public delegate VideoFrame RequestFrameDelegate(/*int VideoSourceID, */DateTime? time);

        public static RequestFrameDelegate RequestFrame;


        public static DateTime ClipStartTime;
        public static DateTime ClipEndTime;
    }
}
