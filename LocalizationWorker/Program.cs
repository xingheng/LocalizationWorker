using System;
using System.IO;

namespace LocalizationWorker
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Localizatino worker is running....");

			WorkerAction wAction = WorkerAction.SearchOnly;
			WorkerFormat wFormat = WorkerFormat.FormatKV;
			bool fVerbose = false;
			string workingPath = "", outputPath = "";

			bool hasWrongFormat = false;
			for(int i = 0; i < args.Length; i++) {
				string arg = args [i];

				switch (arg) {
				case "-a":
				case "--action":
					{
						int index = i + 1;
						if (index < args.Length) {
							if (args [index].ToLower () == "search")
								wAction = WorkerAction.SearchOnly;
							else if (args [index].ToLower () == "replace")
								wAction = WorkerAction.SearchReplace;
							else {
								Console.WriteLine ("Wrong action parameter, try 'search' or 'replace' again.");
								hasWrongFormat = true;
								break;
							}
						} else {
							Console.WriteLine ("Lack of action parameter, try 'search' or 'replace' again.");
							hasWrongFormat = true;
							break;
						}
					}
					break;
				case "-o":
				case "--output":
					{
						int index = i + 1;
						if (index < args.Length) {
							outputPath = args [index];
						} else {
							Console.WriteLine ("Lack of output filepath parameter, give me an valid path and try again.");
							hasWrongFormat = true;
							break;
						}
					}
					break;
				case "-v":
				case "--verbose":
					{
						fVerbose = true;
					}
					break;
				case "-f":
				case "--format":
					{
						int index = i + 1;
						if (index < args.Length) {
							if (args [index].ToLower () == "kkv")
								wFormat = WorkerFormat.FormatKKV;
							else if (args [index].ToLower () == "kv")
								wFormat = WorkerFormat.FormatKV;
							else {
								Console.WriteLine ("Wrong output format parameter, try 'kv' or 'kkv' again.");
								hasWrongFormat = true;
								break;
							}
						} else {
							Console.WriteLine ("Lack of output format parameter, try 'kv' or 'kkv' again.");
							hasWrongFormat = true;
							break;
						}
					}
					break;
				case "-d":
				case "--dir":
				default:
					{
						if (!string.IsNullOrEmpty(workingPath))
							continue;

						int index = i + 1;
						if (index < args.Length) {
							workingPath = args [index];
							if (!Directory.Exists (workingPath)) {
								Console.WriteLine ("Wrong working directory path parameter, correct it and try again.");
								hasWrongFormat = true;
								break;
							}
						} else {
							Console.WriteLine ("Lack of working directory path parameter, give me one and try again.");
							hasWrongFormat = true;
							break;
						}
					}
					break;
				}
			}

			Console.WriteLine ("workingPath: " + workingPath);

			if (hasWrongFormat)
				return;


			Worker wk = new Worker (workingPath);
			Worker.fVerbose = fVerbose;
			wk.Action = wAction;
			wk.OutputPath = outputPath;
			wk.OutputFormat = wFormat;
			wk.StartWorking ();

			Console.WriteLine ("\nThanks for using this localization tool, any feedbacks please send a message to me<xinghenghan@163.com>.");
			Console.WriteLine ("Press any keys to exit...");
			Console.ReadKey ();
		}
	}
}
