using System;
using System.Windows.Forms;

namespace FindInFiles.Controls
{
	class LinkedCheckBox : CheckBox
	{
		private LinkedCheckBox target;

		public LinkedCheckBox Target
		{
			set
			{
				if( target != null )
					return;

				target = value;
				target.Target = this; // 2 way link
			}
		}

		protected override void OnCheckedChanged( EventArgs e )
		{
			base.OnCheckedChanged( e );
			target.Checked = Checked;
		}
	}
}
