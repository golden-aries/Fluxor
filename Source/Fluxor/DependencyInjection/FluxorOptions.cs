using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fluxor.DependencyInjection
{
	/// <summary>
	/// An options class for configuring Fluxor
	/// </summary>
	public class FluxorOptions
	{
		internal List<AssemblyScanSettings> AssembliesToScan { get; } = new List<AssemblyScanSettings>();
		internal List<Type> MiddlewareTypes { get; }  = new List<Type>();
		internal List<IFeature> RegisteredGenericFeatures { get; } = new List<IFeature>();
		internal List<Type> AdditionalTypesToScan { get; } = new List<Type>();
		/// <summary>
		/// Service collection for registering services
		/// </summary>
		public readonly IServiceCollection Services;

		/// <summary>
		/// Creates a new instance
		/// </summary>
		/// <param name="services"></param>
		public FluxorOptions(IServiceCollection services)
		{
			Services = services;
		}


		/// <summary>
		/// Enables the developer to specify a class that implements <see cref="IMiddleware"/>
		/// which should be injected into the <see cref="IStore.AddMiddleware(IMiddleware)"/> method
		/// after dependency injection has completed.
		/// </summary>
		/// <typeparam name="TMiddleware">The Middleware type</typeparam>
		/// <returns>Options</returns>
		public FluxorOptions AddMiddleware<TMiddleware>()
			where TMiddleware : IMiddleware
		{
			if (MiddlewareTypes.IndexOf(typeof(TMiddleware)) > -1)
				return this;

			Services.AddScoped(typeof(TMiddleware));
			Assembly assembly = typeof(TMiddleware).Assembly;
			string @namespace = typeof(TMiddleware).Namespace;

			var assemblyScanSettings = new AssemblyScanSettings(assembly, @namespace);
			if (!AssembliesToScan.Contains(assemblyScanSettings))
				AssembliesToScan.Add(assemblyScanSettings);

			MiddlewareTypes.Add(typeof(TMiddleware));
			return this;
		}

		/// <summary>
		/// Registers a state without having to create a Feature class
		/// </summary>
		/// <typeparam name="TState">The type of the state to add to the store</typeparam>
		/// <param name="name"><see cref="IFeature.MaximumStateChangedNotificationsPerSecond"/><see cref="IFeature.GetName"/></param>
		/// <param name="initialState">The initial state for the feature</param>
		/// <param name="maximumStateChangedNotificationsPerSecond"></param>
		/// <returns></returns>
		public FluxorOptions RegisterGenericFeature<TState>(
			string name,
			TState initialState,
			byte maximumStateChangedNotificationsPerSecond = 0)
		{
			var feature = new GenericFeature<TState>(name, initialState, maximumStateChangedNotificationsPerSecond);
			RegisteredGenericFeatures.Add(feature);
			return this;
		}

		/// <summary>
		/// Enables automatic discovery of features/effects/reducers
		/// </summary>
		/// <param name="additionalAssembliesToScan">A collection of assemblies to scan</param>
		/// <returns>Options</returns>
		public FluxorOptions ScanAssemblies(
			Assembly assemblyToScan,
			params Assembly[] additionalAssembliesToScan)
		{
			if (assemblyToScan == null)
				throw new ArgumentNullException(nameof(assemblyToScan));

			var allAssemblies = new List<Assembly> { assemblyToScan };
			if (additionalAssembliesToScan != null)
				allAssemblies.AddRange(additionalAssembliesToScan);

			var newAssembliesToScan = allAssemblies.Select(x => new AssemblyScanSettings(x)).ToList();
			AssembliesToScan.AddRange(newAssembliesToScan);

			return this;
		}

		public FluxorOptions ScanTypes(Type type, params Type[] additionalTypes)
		{
			if (type is null)
				throw new ArgumentNullException(nameof(type));

			if (additionalTypes == null)
				additionalTypes = Array.Empty<Type>();

			AdditionalTypesToScan.AddRange(additionalTypes.Append(type));

			return this;
		}
	}
}
