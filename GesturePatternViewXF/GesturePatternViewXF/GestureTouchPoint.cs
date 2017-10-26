using Xamarin.Forms;

namespace GesturePatternViewXF
{
	public class GestureTouchPoint
	{
		#region properties

		public bool IsTouched { get; set; }

		public string Value { get; set; }

        public Point Location { get; set; }

        public Point Center => new Point(this.Location.X + (this.Width/2), this.Location.Y + (this.Height/2));

        public double Width { get; set; }

        public double Height { get; set; }
        
		#endregion


		#region public methods

		public void Touch()
		{
			this.IsTouched = true;
		}

		public void Reset()
		{
			if (this.IsTouched)
			{
				this.IsTouched = false;
			}
		}

		#endregion
	}
}
