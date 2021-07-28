using Fluxor;
using System.Threading.Tasks;

namespace BasicConcepts.Generics.Store.GenericUseCase
{
	public static class Effects<TInnerState>
	{
		[EffectMethod]
		public static async Task HandleStartBusyOperationActionAsync(
			StartBusyOperationAction<TInnerState> action,
			IDispatcher dispatcher)
		{
			await Task.Delay(1000).ConfigureAwait(false);
			dispatcher.Dispatch(new EndBusyStateAction<TInnerState>());
		}
	}
}
