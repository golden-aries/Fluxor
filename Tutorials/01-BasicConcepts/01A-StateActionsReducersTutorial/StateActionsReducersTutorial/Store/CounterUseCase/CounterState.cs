using Fluxor;

namespace BasicConcepts.StateActionsReducersTutorial.Store.CounterUseCase
{
	[Feature]
	//[Feature(GetInitialStateMethodName = nameof(GetInitialState))]
	public class CounterState: Temp<CounterState>
	{
		public int ClickCount { get; }

		private static CounterState GetInitialState() => new CounterState(0);

		public CounterState(int clickCount)
		{
			ClickCount = clickCount;
		}

		public CounterState() => new CounterState(0);
	}

	public class Temp<T>
	{
		public string Hello { get; set; }
	}
}
