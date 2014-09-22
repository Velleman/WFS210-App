using System;

namespace WFS210.Services
{
	public class ServiceManager
	{
		private Service activeService;

		public Service ActiveService {
			get {
				return activeService;
			}
			set {
				activeService = value;
				activeService.Activate ();
			}
		}

		public ServiceManager (Service activeService)
		{
			ActiveService = activeService;
		}
	}
}

