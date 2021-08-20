using System;

namespace Fluxor
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class FeatureAttribute : Attribute
	{
		public string Name { get; }
		public string GetInitialStateMethodName { get; }
		public byte MaximumStateChangedNotificationsPerSecond { get; set; }

		public FeatureAttribute(string name, string getInitialStateMethodName)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
			GetInitialStateMethodName = getInitialStateMethodName ?? throw new ArgumentNullException(nameof(getInitialStateMethodName));
		}
	}
}
