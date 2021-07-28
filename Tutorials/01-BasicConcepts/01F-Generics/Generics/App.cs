using BasicConcepts.Generics.Store;
using BasicConcepts.Generics.Store.EditCustomerUseCase;
using BasicConcepts.Generics.Store.EditPurchaseOrderUseCase;
using Fluxor;
using System;

namespace BasicConcepts.Generics
{
	public class App
	{
		private readonly IStore Store;
		public readonly IDispatcher Dispatcher;

		public App(
			IStore store,
			IDispatcher dispatcher)
		{
			Store = store;
			Dispatcher = dispatcher;
		}

		public void Run()
		{
			Console.Clear();
			Console.WriteLine("Initializing store");
			Store.InitializeAsync().Wait();
			string input = "";
			do
			{
				Console.WriteLine("1: Make State1 busy");
				Console.WriteLine("2: Make State2 busy");
				Console.WriteLine("x: Exit");
				input = Console.ReadLine();

				switch (input.ToLowerInvariant())
				{
					case "1":
						var incrementCounterActionction = new StartBusyOperationAction<EditCustomerState>();
						Dispatcher.Dispatch(incrementCounterActionction);
						break;

					case "2":
						var fetchDataAction = new StartBusyOperationAction<EditPurchaseOrderState>();
						Dispatcher.Dispatch(fetchDataAction);
						break;

					case "x":
						Console.WriteLine("Program terminated");
						return;
				}

			} while (true);
		}
	}
}
