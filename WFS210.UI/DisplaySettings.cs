using System.Collections.Generic;

namespace WFS210.UI
{
	public class DisplaySettings
	{
		public List<MarkerUnit> MarkerUnits = new List<MarkerUnit> ();

		public List<SignalUnit> SignalUnits = new List<SignalUnit> ();

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

