namespace BasicConcepts.Generics.Store.GenericUseCase
{
	public class GenericState<TInnerState>
	{
		public uint IsBusyCount { get; }
		public TInnerState InnerState { get; }

		public GenericState(uint isBusyCount, TInnerState innerState)
		{
			IsBusyCount = isBusyCount;
			InnerState = innerState;
		}
	}
}
