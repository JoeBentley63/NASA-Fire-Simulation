using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Kinesense.Interfaces.Clarification
{
    public interface IClarification
    {
        /// <summary>
        /// Publicly readable name for the Clarification
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A name for the operation formed. This must be consistent over different versions
        /// and unique to this operation. i.e. BrightnessChange would be an operation and could
        /// be this value. The "Name" files can be language specific, this can't.
        /// </summary>
        string OperationName { get; }

        /// <summary>
        /// A short description of what the clarification does
        /// </summary>
        string Description { get; }

        /// <summary>
        /// If we were to be organising the clarification by group, what is its major heading
        /// </summary>
        MajorClarificationCategory MajorCategory { get; }

        /// <summary>
        /// If we were to be organising the clarification by group, what is its minor heading
        /// </summary>
        MinorClarificationCategory MinorCategory { get; }


        /// <summary>
        /// The image to apply the enhancement too
        /// Runs NewSourceImageSet when set
        /// </summary>
        ByteArrayBitmap SourceImage { get; set; }

        /// <summary>
        /// The result image of the enhancement
        /// Fires NewResultImageSet when set
        /// </summary>
        ByteArrayBitmap ResultImage { get; }

        /// <summary>
        /// Event to be fired when a new Result image is ready
        /// </summary>
        event EventHandler NewResultImageSet;

        /// <summary>
        /// UI element available for altering the settings of the Clarification
        /// </summary>
        UIElement EnhancementUIElement { get; }

        /// <summary>
        /// to be polled when changing to ensure changes are not left behind without being applied
        /// Only needs to be used if the UI has an "Apply Changes" button or similar
        /// </summary>
        bool HasHadChanges { get; }

        /// <summary>
        /// If there have been changes that have not been applied, this allows the calling interface to force their application
        /// Only needs to have function for those plug-ins with apply buttons of course.
        /// </summary>
        void ApplyChanges();

        /// <summary>
        /// For Errors in the UI version only, the non-UI version should just return a Null.
        /// </summary>
        bool HasEncouteredError { get; }

        /// <summary>
        /// A flag indicating if the Clarification may change the dimensions of the source image
        /// </summary>
        bool ChangesImageDimensions { get; }

        /// <summary>
        /// Settings for the clarification
        /// </summary>
        ClarificationSettings Settings { get; set; }

        /// <summary>
        /// Human readable settings for the log
        /// </summary>
        string SettingHumanReadable { get; }

        /// <summary>
        /// A route for the clarification to be called without requiring a UI
        /// </summary>
        /// <param name="source">Source Image</param>
        /// <param name="settings">Settings for clarification</param>
        /// <param name="settingsHumanReadable">Human readable interpretation of the clarification++</param>
        /// <returns></returns>
        ByteArrayBitmap Process(ByteArrayBitmap source, ClarificationSettings settings, out string settingsHumanReadable);

    }
}
