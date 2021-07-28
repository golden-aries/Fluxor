using Fluxor.DependencyInjection;

namespace BasicConcepts.Generics.Store.GenericUseCase
{
	public static class GenericUseCaseRegistration
	{
		public static FluxorOptions RegisterGenericUseCase<TState>(this FluxorOptions options, string name, TState initialState)
		{
			options
				.RegisterGenericFeature(name, new GenericState<TState>(
					isBusyCount: 0,
					innerState: initialState))
				.ScanType(typeof(Reducers<TState>))
				.ScanType(typeof(Reducers<TState>));
			return options;
		}
	}
}
