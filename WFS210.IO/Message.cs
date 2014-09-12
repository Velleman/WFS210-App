using System;

namespace WFS210.IO
{
	/// <summary>
	/// Message used to communicate abstractly with the oscilloscope.
	/// </summary>
	public class Message
	{
		/// <summary>
		/// Gets or sets the command.
		/// </summary>
		/// <value>The command.</value>
		public byte Command { get; set; }

		/// <summary>
		/// Gets or sets the data.
		/// </summary>
		/// <value>The data.</value>
		public byte[] Payload { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="WFS210.IO.Message"/> class.
		/// </summary>
		/// <param name="command">Command.</param>
		public Message (byte command) : this (command, null)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WFS210.IO.Message"/> class.
		/// </summary>
		/// <param name="command">Command.</param>
		/// <param name="payload">Payload.</param>
		public Message (byte command, byte[] payload)
		{
			this.Command = command;
			this.Payload = payload;
		}
	}
}

