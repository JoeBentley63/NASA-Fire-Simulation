using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;

namespace Kinesense.Interfaces
{
	public interface IReportingFormatOptions : INotifyPropertyChanged
	{
		UIElement UserInterface { get; }
		string FileExtension { get; }
		string ReportType { get; }
		string OptionsString { get; set; }
	}
}
