using System.Collections.Generic;

namespace WFS210.UI
{
	public class DisplaySettings
	{
		/// <summary>
		/// The marker units.
		/// </summary>
		public List<MarkerUnit> MarkerUnits = new List<MarkerUnit> ();

		/// <summary>
		/// The signal units.
		/// </summary>
		public List<SignalUnit> SignalUnits = new List<SignalUnit> ();

		/// <summary>
		/// Initializes a new instance of the <see cref="WFS210.UI.DisplaySettings"/> class.
		/// </summary>
		/// <param name="defaultMarkerUnit">Default marker unit.</param>
		/// <param name="defaultSignalUnit">Default signal unit.</param>
		public DisplaySettings (MarkerUnit defaultMarkerUnit, SignalUnit defaultSignalUnit)
		{
			for (int i = 0; i < 2; i++) {
				MarkerUnits.Add (defaultMarkerUnit);
			}

			for (int i = 0; i < 2; i++) {
				SignalUnits.Add (defaultSignalUnit);
			}
		}
	}
}

