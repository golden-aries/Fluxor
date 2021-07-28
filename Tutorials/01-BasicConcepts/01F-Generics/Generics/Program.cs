using BasicConcepts.Generics.Store.Middlewares.Logging;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;
using System;
using BasicConcepts.Generics.Store.GenericUseCase;
using BasicConcepts.Generics.Store.EditCustomerUseCase;
using BasicConcepts.Generics.Store.EditPurchaseOrderUseCase;

namespace BasicConcepts.Generics
{
	class Program
	{
		static void Main(string[] args)
		{
			var services = new ServiceCollection();
			services.AddScoped<App>();
			services.AddFluxor(o => o
				.ScanAssemblies(typeof(Program).Assembly)
				.AddMiddleware<LoggingMiddleware>()
				.RegisterGenericUseCase("EditCustomer", new EditCustomerState())
				.RegisterGenericUseCase("EditPurchaseOrder", new EditPurchaseOrderState()));

			IServiceProvider serviceProvider = services.BuildServiceProvider();

			var app = serviceProvider.GetRequiredService<App>();
			app.Run();
		}
	}
}
