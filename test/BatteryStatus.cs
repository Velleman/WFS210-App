using System;

namespace WFS210
{
	/// <summary>
	/// Battery status.
	/// </summary>
	public enum BatteryStatus {

		/// <summary>
		/// The battery is low.
		/// </summary>
		BatteryLow,

		/// <summary>
		/// The USB cable is plugged in and the unit is charging.
		/// </summary>
		Charging,

		/// <summary>
		/// The battery is fully charged.
		/// </summary>
		Charged
	}
}

