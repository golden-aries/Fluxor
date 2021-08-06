using Fluxor;
using System.Threading.Tasks;

namespace BasicConcepts.Generics.Store.GenericUseCase
{
	public static class Effects<T>
	{
		[EffectMethod]
		public static async Task HandleStartBusyOperationActionAsync(
			StartBusyOperationAction<T> action,
			IDispatcher dispatcher)
		{
			System.Console.WriteLine($"\tEffect decrementing {typeof(T).Name}.IsBusyCount in 5 seconds");
			await Task.Delay(5000);
			dispatcher.Dispatch(new EndBusyStateAction<T>());
		}
	}
}
