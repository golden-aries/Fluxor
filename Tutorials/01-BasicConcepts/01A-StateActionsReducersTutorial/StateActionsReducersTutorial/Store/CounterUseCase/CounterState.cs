using Fluxor;

namespace BasicConcepts.StateActionsReducersTutorial.Store.CounterUseCase
{
	[Feature(name: "Counter", getInitialStateMethodName: nameof(GetInitialState))]
	public class CounterState
	{
		public int ClickCount { get; }

		private static CounterState GetInitialState() => new CounterState(0);

		public CounterState(int clickCount)
		{
			ClickCount = clickCount;
		}
	}
}
