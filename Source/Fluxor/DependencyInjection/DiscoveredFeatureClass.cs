using System;
using System.Reflection;

namespace Fluxor.DependencyInjection
{
	internal class DiscoveredFeatureClass
	{
		public readonly Type FeatureInterfaceGenericType;
		public readonly Type ImplementingType;
		public readonly Type StateType;
		public bool HasInstance => Instance != null;
		private readonly IFeature Instance;

		public DiscoveredFeatureClass(Type implementingType, Type stateType)
		{
			FeatureInterfaceGenericType = typeof(IFeature<>).MakeGenericType(stateType);
			ImplementingType = implementingType;
			StateType = stateType;
		}

		public DiscoveredFeatureClass(IFeature instance)
		{
			Instance = instance;
			ImplementingType = instance.GetType();
			StateType = instance.GetStateType();
			FeatureInterfaceGenericType = typeof(IFeature<>).MakeGenericType(StateType);
		}

		private static T CreateStateUsingConstructor<T>()
			where T : new()
			=> new T();


		public DiscoveredFeatureClass(Type stateType, FeatureAttribute featureAttribute)
		{
			StateType = stateType ?? throw new ArgumentNullException(nameof(stateType));
			FeatureInterfaceGenericType = typeof(IFeature<>).MakeGenericType(StateType);
			
			MethodInfo getInitialStateMethodInfo;

			if (featureAttribute.GetInitialStateMethodName != null)
			{
				getInitialStateMethodInfo =
					FindGetInitialStateMethodInfo(stateType, featureAttribute.GetInitialStateMethodName);
			}
			else
			{
				ConstructorInfo stateTypeParameterlessConstructor = FindStateTypeParameterlessConstructor(stateType);
				getInitialStateMethodInfo =
					typeof(DiscoveredFeatureClass)
						.GetMethod(nameof(CreateStateUsingConstructor), BindingFlags.NonPublic | BindingFlags.Static)
						.MakeGenericMethod(stateType);
			}

			Type getInitialStateFunctionType = typeof(Func<>).MakeGenericType(stateType);
			Type featureInstanceType = typeof(AutoDiscoveredFeature<>).MakeGenericType(stateType);
			ImplementingType = featureInstanceType;
			ConstructorInfo constructor = featureInstanceType.GetConstructor(
				new Type[]
				{
					typeof(string), getInitialStateFunctionType, typeof(byte)
				});

			Instance = (IFeature)constructor.Invoke(
				new object[]
				{
					featureAttribute.Name ?? stateType.Name,
					getInitialStateMethodInfo.CreateDelegate(getInitialStateFunctionType),
					featureAttribute.MaximumStateChangedNotificationsPerSecond
				});
		}

		public IFeature GetInstance(IServiceProvider serviceProvider) =>
			Instance ?? (IFeature)serviceProvider.GetService(ImplementingType);

		private static MethodInfo FindGetInitialStateMethodInfo(Type stateType, string methodName)
		{
			MethodInfo result =
				stateType.GetMethod(
					name: methodName,
					bindingAttr: BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (result?.ReturnType != stateType)
				result = null;
			if ((result?.GetParameters()?.Length ?? 0) != 0)
				result = null;
			if (result == null)
				throw new ArgumentException(
					message: $"{stateType.Name} does not implement either public or non-public" +
					$" static {stateType.Name} {methodName}();",
					paramName: nameof(stateType));

			return result;
		}

		private static ConstructorInfo FindStateTypeParameterlessConstructor(Type stateType)
		{
			ConstructorInfo result = stateType.GetConstructor(Array.Empty<Type>());
			if (result == null)
				throw new ArgumentException(
					message: $"{stateType.Name} must implement a public parameterless constructor if" +
					$" {nameof(FeatureAttribute)}.{nameof(FeatureAttribute.GetInitialStateMethodName)} is null");
			return result;
		}
	}
}
