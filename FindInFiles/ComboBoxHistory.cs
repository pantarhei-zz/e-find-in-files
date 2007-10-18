using System;
using System.Diagnostics	;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace FindInFiles
{
	class ComboBoxHistory
	{
		public const int HISTORY_LENGTH = 10;

		private readonly string Name;
		private readonly ComboBox Box;
		private readonly List<string> Values = new List<string>();

		public ComboBoxHistory( string name, ComboBox box )
		{
			Debug.Assert( name != null );
			Debug.Assert( box != null );

			Name = name;
			Box = box;
		}

		public void Load( RegistryKey key )
		{
			Values.Clear();
			PushBack( key.GetValue( Name ) as string );

			for( int i = 1; i < HISTORY_LENGTH; i++ )
			{
				PushBack( key.GetValue( Name + i.ToString() ) as string );
			}

			SetComboBox( Box );
		}

		public void Grab()
		{
			PushFront( Box.Text );
		}

		public void PushFront( string value )
		{
			if( value == null || value.Length < 1 )
				return;

			Values.Remove( value );
			Values.Insert( 0, value );

			Trim();
		}

		public void PushBack( string value )
		{
			if( value == null || value.Length < 1 )
				return;

			Values.Add( value );

			Trim();
		}

		public void Trim()
		{
			while( Values.Count > HISTORY_LENGTH )
				Values.RemoveAt( HISTORY_LENGTH );
		}

		public void Save( RegistryKey key )
		{
			Trim();

			if( Values.Count < 1 )
				return;

			key.SetValue( Name, Values[0] );
			for( int i = 1; i < Values.Count; i++ )
			{
				key.SetValue( Name + i.ToString(), Values[i] );
			}
		}

		public string[] ToArray()
		{
			return Values.ToArray();
		}

		public void SetComboBox( System.Windows.Forms.ComboBox box )
		{
			box.Items.Clear();
			if( Values.Count < 1 )
				return;

			box.Items.AddRange( ToArray() );
			box.Text = Values[0];
		}
	}
}
