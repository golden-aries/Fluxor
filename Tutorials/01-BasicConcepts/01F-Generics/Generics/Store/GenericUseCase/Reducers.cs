using Fluxor;

namespace BasicConcepts.Generics.Store.GenericUseCase
{
	public static class Reducers<TInnerState>
	{
		[ReducerMethod]
		public static GenericState<TInnerState> ReduceStartBusyOperationAction(
			GenericState<TInnerState> state,
			StartBusyOperationAction<TInnerState> _)
		=>
			new GenericState<TInnerState>(state.IsBusyCount + 1, state.InnerState);

		[ReducerMethod]
		public static GenericState<TInnerState> ReduceEndBusyOperationAction(
			GenericState<TInnerState> state,
			EndBusyStateAction<TInnerState> _)
		=>
			new GenericState<TInnerState>(state.IsBusyCount - 1, state.InnerState);
	}
}
