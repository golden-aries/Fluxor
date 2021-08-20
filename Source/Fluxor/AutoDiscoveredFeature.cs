using System;
using System.Collections.Generic;
using System.Text;

namespace Fluxor
{
	internal class AutoDiscoveredFeature<TState> : Feature<TState>
	{
		private readonly string Name;
		private readonly Func<TState> GetInitialStateFunc;

		public AutoDiscoveredFeature(
			string name,
			Func<TState> getInitialStateFunc,
			byte maximumStateChangedNotificationsPerSecond)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
			GetInitialStateFunc = getInitialStateFunc ?? throw new ArgumentNullException(nameof(getInitialStateFunc));
			MaximumStateChangedNotificationsPerSecond = maximumStateChangedNotificationsPerSecond;
		}

		public override string GetName() => Name;
		protected override TState GetInitialState() => GetInitialStateFunc();
	}
}
