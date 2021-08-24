using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fluxor
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class FeatureAttribute : Attribute
	{
		public string Name { get; set; }
		public string GetInitialStateMethodName { get; set; }
		public byte MaximumStateChangedNotificationsPerSecond { get; set; }
	}
}
