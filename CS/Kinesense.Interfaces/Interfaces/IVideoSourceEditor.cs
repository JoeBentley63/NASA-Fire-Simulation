using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace Kinesense.Interfaces
{
	public interface IVideoSourceEditor : INotifyPropertyChanged
	{
		UIElement UserInterface { get; }
        void Clear();
		void Open(string name, Uri uri);
		bool IsValid { get; }
        Uri GetUriSpecial(object obj);
		Uri Uri { get; }
        object GetSelection();
		string Name { get; }
		IEnumerable<KeyValuePair<string, string>> Properties { get; }
	}
}
