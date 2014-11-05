using System;

namespace LocalizationWorker
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Localizatino worker is running....");

//			string str = "@\"abc\",.@\"defg\"";
//			var result = Worker.GetComparedUnit (str, "@\"", "\"");
//			foreach (var item in result)
//				Console.WriteLine (item);

			string workingPath = "";
			if (args.Length <= 1)
				workingPath = "/Users/hanwei/work/phantom-ios/SmartLightClient";
//				workingPath = "/Users/hanwei/Desktop/sample/loc";
			else
				workingPath = args [1];

			if (string.IsNullOrEmpty (workingPath))
				Console.WriteLine ("working path not found.");
			else
				(new Worker (workingPath)).StartWorking ();

			Console.ReadKey ();
		}
	}
}
