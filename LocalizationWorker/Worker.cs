using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace LocalizationWorker
{
	public enum WorkerAction {
		SearchOnly,
		SearchReplace
	};

	public enum WorkerFormat {
		FormatKV,
		FormatKKV
	};

	public class Worker
	{
		private WorkerAction _action;
		public WorkerAction Action {
			get { return _action; }
			set { _action = value; }
		}

		private string _outputPath;
		public string OutputPath{
			get { return _outputPath; }
			set { _outputPath = value; }
		}

		private WorkerFormat _outputFormat;
		public WorkerFormat OutputFormat
		{
			get { return _outputFormat; }
			set { _outputFormat = value; }
		}

		public static bool fVerbose = false;

		string workingPath = "";
		List<string> allWorkingFiles = new List<string> ();

		public Worker (string workingPath)
		{
			this.workingPath = workingPath;
		}

		public void StartWorking()
		{
			if (!Directory.Exists (workingPath)) {
				Console.WriteLine ("working path is invalid." + workingPath);
				return;
			}

			Console.WriteLine ("Starting scaning directory " + workingPath);

			GetDestinationFiles (workingPath);

			if (fVerbose) {
				foreach (string item in allWorkingFiles)
					Console.WriteLine (item);
			}

			Console.WriteLine ("Finished scanning all files.");

			List<MatchItem> allUnits = new List<MatchItem> ();
			for (int i = 0; i < allWorkingFiles.Count; i++) {
				string file = allWorkingFiles [i];

				SearchFile (file, allUnits);
			}

			string outputFile = "localization_output.txt";
			if (!string.IsNullOrEmpty(this.OutputPath))
				outputFile = this.OutputPath;

			File.Delete (outputFile);

			for (int i = 0; i < allUnits.Count; i++) {
				MatchItem imatch = allUnits [i];
//				string output = string.Format("{0} \t= {1}: {2}; NEW: {3}",
//					imatch.keyString, i + 1, imatch.value, imatch.newValue);
				string output = "";

				switch (this.OutputFormat)
				{
					case WorkerFormat.FormatKKV:
						output = string.Format("{0} \t= {1} \t= {2};",
							imatch.keyString, imatch.keyString, imatch.value);
						break;
					case WorkerFormat.FormatKV:
					default:
						output = string.Format("{0} \t= {1};",
							imatch.keyString, imatch.value);
						break;
				}

				Console.WriteLine(output);

				output += "\r\n";
				File.AppendAllText(outputFile, output);
			}

			Console.WriteLine (string.Format("Finished searching all files, totalCount: {0}.", 
				allUnits.Count));
		}

		#region Scan files
		void GetDestinationFiles(string destDirPath)
		{
			if (fVerbose) 
				Console.WriteLine ("Found directory " + destDirPath);

			DirectoryInfo dirInfo = new DirectoryInfo (destDirPath);
			if (!dirInfo.Exists)
				return;

			FileInfo[] allfiles = dirInfo.GetFiles ();
			foreach (FileInfo fileInfo in allfiles) {
				string fullPath = fileInfo.FullName;
				if (FValidFile (fullPath))
					allWorkingFiles.Add (fullPath);
			}

			DirectoryInfo[] allDirs = dirInfo.GetDirectories ();
			foreach( DirectoryInfo item in allDirs) {
				GetDestinationFiles(item.FullName);
			}
		}

		bool FValidFile(string fullFilePath)
		{
			string[] extensions = new string[]{ ".m", ".mm", ".h" };
			foreach (string item in extensions) {
				if (fullFilePath.EndsWith (item))
					return true;
			}
			return false;
		}
		#endregion

		#region Searching 
		int SearchFile(string filePath, List<MatchItem> existingMatches)
		{
			int count = 0;

			Regex regex = new Regex ("@\"\\S{0,}\\p{IsCJKUnifiedIdeographs}{1,}\\S{0,}\"");

			string allText = File.ReadAllText (filePath);

			Match match = regex.Match(allText);
			while (match.Success) {
				// Handle match here...
				count++;
				string mValue = match.Value;

				MatchItem iMatch = null;
				foreach (MatchItem item in existingMatches) 
				{
					if (item.value == mValue) {
						iMatch = item;
						break;
					}
				}

				if (iMatch == null) {
					iMatch = new MatchItem ();
					iMatch.keyString = string.Format("\"kLocKey_{0}_{1}\"", Utilities.GetFileName(filePath), count);
					iMatch.countNO = count;
					iMatch.value = mValue;

					// #define  LocalizedString(__keyString)	NSLocalizedString((__keyString), nil)

					iMatch.newValue = string.Format ("LocalizedString(@{0})", iMatch.keyString);
					existingMatches.Add (iMatch);
				}

				if (this.Action == WorkerAction.SearchReplace) {

					// starting write operation on code files.
					// replace matched value with key string.

					string fileContent = File.ReadAllText (filePath);
					string newFileContent = fileContent.Replace (mValue, iMatch.newValue);

					// THERE SHOULD BE A BUG ON MONO...
					// After replacing the matche string, there is a newline character at the end of file content.
					// 	so, drop it explicitly here!
					newFileContent = newFileContent.Substring (0, newFileContent.Length - 1);

					// Replace...
					Utilities.ForceWriteFile (filePath, newFileContent);
				}

				match = match.NextMatch();
			}

			return count;
		}

		#endregion
	}

	class MatchItem
	{
		public string keyString {
			get;
			set;
		}

		public int countNO {
			get;
			set;
		}

		public string value {
			get;
			set;
		}

		public string newValue {
			get;
			set;
		}
	}
}

