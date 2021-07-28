using Fluxor.DependencyInjection.DependencyScanners;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fluxor.DependencyInjection
{
	internal static class DependencyScanner
	{
		internal static void Scan(
			this IServiceCollection serviceCollection,
			FluxorOptions options)
		{
			if (options == null)
				throw new ArgumentNullException(nameof(options));

			GetCandidateTypes(
				options,
				allCandidateTypes: out Type[] allCandidateTypes,
				allNonAbstractCandidateTypes: out Type[] allNonAbstractCandidateTypes);

			// Classes must not be abstract
			DiscoveredReducerClass[] discoveredReducerClasses =
				ReducerClassessDiscovery.DiscoverReducerClasses(serviceCollection, allNonAbstractCandidateTypes);

			DiscoveredEffectClass[] discoveredEffectClasses =
				EffectClassessDiscovery.DiscoverEffectClasses(serviceCollection, allNonAbstractCandidateTypes);

			// Method reducer/effects may belong to abstract classes
			TypeAndMethodInfo[] allCandidateMethods = AssemblyScanSettings.FilterMethods(allCandidateTypes);

			DiscoveredReducerMethod[] discoveredReducerMethods =
				ReducerMethodsDiscovery.DiscoverReducerMethods(serviceCollection, allCandidateMethods);

			DiscoveredEffectMethod[] discoveredEffectMethods =
				EffectMethodsDiscovery.DiscoverEffectMethods(serviceCollection, allCandidateMethods);

			DiscoveredFeatureClass[] discoveredFeatureClasses =
				FeatureClassesDiscovery.DiscoverFeatureClasses(
					serviceCollection: serviceCollection,
					options: options,
					allCandidateTypes: allNonAbstractCandidateTypes,
					discoveredReducerClasses: discoveredReducerClasses,
					discoveredReducerMethods: discoveredReducerMethods);

			RegisterStore(
				serviceCollection: serviceCollection,
				options: options,
				discoveredFeatureClasses: discoveredFeatureClasses,
				discoveredEffectClasses: discoveredEffectClasses,
				discoveredEffectMethods: discoveredEffectMethods);
		}

		private static void GetCandidateTypes(
			FluxorOptions options,
			out Type[] allCandidateTypes,
			out Type[] allNonAbstractCandidateTypes)
		{
			if (options == null)
				throw new ArgumentNullException(nameof(options));

			IEnumerable<AssemblyScanSettings> assembliesToScan = options.AssembliesToScan;
			IEnumerable<AssemblyScanSettings> scanIncludeList =
				options
					.MiddlewareTypes
					.Select(t => new AssemblyScanSettings(t.Assembly, t.Namespace));

			Assembly[] allCandidateAssemblies =
				assembliesToScan
					.Select(x => x.Assembly)
					.Union(scanIncludeList.Select(x => x.Assembly))
					.Distinct()
					.ToArray();

			AssemblyScanSettings[] scanExcludeList =
				MiddlewareClassesDiscovery.FindMiddlewareLocations(allCandidateAssemblies);

			allCandidateTypes = allCandidateAssemblies
				.SelectMany(x => x.GetTypes())
				.Union(scanIncludeList.SelectMany(x => x.Assembly.GetTypes()))
				.Union(options.AdditionalTypesToScan)
				.Where(t => !t.IsGenericTypeDefinition)
				.Distinct()
				.ToArray();

			allCandidateTypes = AssemblyScanSettings.FilterClasses(
				scanExcludeList: scanExcludeList,
				scanIncludeList: scanIncludeList,
				types: allCandidateTypes);

			allNonAbstractCandidateTypes = allCandidateTypes
					.Where(t => !t.IsAbstract)
					.ToArray();
		}

		private static void RegisterStore(
			IServiceCollection serviceCollection,
			FluxorOptions options,
			IEnumerable<DiscoveredFeatureClass> discoveredFeatureClasses,
			IEnumerable<DiscoveredEffectClass> discoveredEffectClasses,
			IEnumerable<DiscoveredEffectMethod> discoveredEffectMethods)
		{
			// Register IDispatcher as an alias to Store
			serviceCollection.AddScoped<IDispatcher>(serviceProvider => serviceProvider.GetService<Store>());
			// Register IActionSubscriber as an alias to Store
			serviceCollection.AddScoped<IActionSubscriber>(serviceProvider => serviceProvider.GetService<Store>());
			// Register IStore as an alias to Store
			serviceCollection.AddScoped<IStore>(serviceProvider => serviceProvider.GetService<Store>());

			// Register a custom factory for building IStore that will inject all effects
			serviceCollection.AddScoped(typeof(Store), serviceProvider =>
			{
				var store = new Store();

				foreach (DiscoveredFeatureClass discoveredFeatureClass in discoveredFeatureClasses)
				{
					var feature = (IFeature)serviceProvider.GetService(discoveredFeatureClass.FeatureInterfaceGenericType);
					store.AddFeature(feature);
				}

				foreach (DiscoveredEffectClass discoveredEffectClass in discoveredEffectClasses)
				{
					var effect = (IEffect)serviceProvider.GetService(discoveredEffectClass.ImplementingType);
					store.AddEffect(effect);
				}

				foreach (DiscoveredEffectMethod discoveredEffectMethod in discoveredEffectMethods)
				{
					IEffect effect = EffectWrapperFactory.Create(serviceProvider, discoveredEffectMethod);
					store.AddEffect(effect);
				}

				foreach (Type middlewareType in options.MiddlewareTypes)
				{
					var middleware = (IMiddleware)serviceProvider.GetService(middlewareType);
					store.AddMiddleware(middleware);
				}

				return store;
			});
		}
	}
}
