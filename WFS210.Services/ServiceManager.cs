using System.Collections.Generic;

namespace WFS210.Services
{
	public class ServiceManager
	{
		protected Dictionary<ServiceType, Service> Services = new Dictionary<ServiceType, Service> ();

		public ServiceType ServiceType { get; set; }

		public Service ActiveService {
			get { return GetService(ServiceType); }
		}

		public Service GetService(ServiceType serviceType)
		{
			return Services [serviceType];
		}

		public ServiceManager (Oscilloscope oscilloscope, ServiceType defaultType)
		{
			this.Services.Add (ServiceType.Demo, new DemoService (oscilloscope));
			this.Services.Add (ServiceType.Live, new LiveService (oscilloscope));

			this.ServiceType = defaultType;
		}
	}
}

