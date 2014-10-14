using System.Collections.Generic;
using System;

namespace WFS210.Services
{
	/// <summary>
	/// The service manager is a state object used to switch between different
	/// types of services.
	/// </summary>
	public class ServiceManager
	{
		/// <summary>
		/// Mapping between a service type and its object.
		/// </summary>
		protected Dictionary<ServiceType, Service> Services = new Dictionary<ServiceType, Service> ();

		/// <summary>
		/// Gets or sets the type of the active service.
		/// </summary>
		/// <value>The type of the service.</value>
		public ServiceType ServiceType { get; set; }

		/// <summary>
		/// Gets the active service.
		/// </summary>
		/// <value>The active service.</value>
		public Service ActiveService {
			get { return GetService (ServiceType); }
		}

		/// <summary>
		/// Occurs when settings have changed.
		/// </summary>
		public event EventHandler SettingsChanged;

		/// <summary>
		/// Occurs when settings have changed.
		/// </summary>
		public event EventHandler<EventArgs> FullFrame;

		/// <summary>
		/// Gets the service object for the specified service type.
		/// </summary>
		/// <returns>The service.</returns>
		/// <param name="serviceType">Service type.</param>
		public Service GetService (ServiceType serviceType)
		{
			return Services [serviceType];
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WFS210.Services.ServiceManager"/> class.
		/// </summary>
		/// <param name="oscilloscope">Oscilloscope used to instantiate each service.</param>
		/// <param name="defaultType">The default service type.</param>
		public ServiceManager (Oscilloscope oscilloscope, ServiceType defaultType)
		{

			AddService(ServiceType.Demo, new DemoService(oscilloscope));
			AddService(ServiceType.Live, new LiveService(oscilloscope));

			this.ServiceType = defaultType;
		}

		public void AddService (ServiceType type, Service service)
		{
			this.Services.Add (type, service);
			GetService (type).SettingsChanged += HandleSettingsChanged;
			GetService (type).FullFrame += HandleFullFrame;
		}

		void HandleSettingsChanged (object sender, System.EventArgs e)
		{
			if (SettingsChanged != null)
				SettingsChanged (this, null);
		}

		void HandleFullFrame (object sender, System.EventArgs e)
		{
			if (FullFrame != null)
				FullFrame (this, null);
		}
	}
}

