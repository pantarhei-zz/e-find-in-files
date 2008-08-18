using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace FindInFiles.Controls
{
	class LinkedComboBox : ComboBox
	{
		private LinkedComboBox target;

		public LinkedComboBox Target
		{
			set
			{
				if( value == null )
					throw new ArgumentNullException( "Target" );

				if( target != null )
					return;

				target = value;
				target.Target = this; // 2 way link
			}
		}

		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;

				if( target != null )
					target.SetText( value ); // prevent infinite loops
			}
		}

		protected void SetText( string value )
		{
			base.Text = value;
		}
		
		// you'd think this would infinite loop, but apparently not
		protected override void OnLostFocus( EventArgs e )
		{
			base.OnLostFocus( e );

			// set the text items of the linked box
			if( target != null )
			{
				target.Text = this.Text;

				target.Items.Clear();
				var newItems = new object[Items.Count];
				Items.CopyTo( newItems, 0 );

				target.Items.AddRange( newItems );
			}
		}
	}
}
