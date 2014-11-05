using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace LocalizationWorker
{
	public class Worker
	{
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

//			foreach (string item in allWorkingFiles)
//				Console.WriteLine (item);

			Console.WriteLine ("Finished scanning all files.");
			Console.WriteLine ("\n\n\n");

			int totalCount = 0;
			List<MatchItem> allUnits = new List<MatchItem> ();
			for (int i = 0; i < allWorkingFiles.Count; i++) {
				string file = allWorkingFiles [i];

				List<MatchItem> units = new List<MatchItem> ();
				totalCount += SearchFile (file, out units);
				allUnits.AddRange (units);
			}

			string outputFile = @"/Users/hanwei/Desktop/sample/localization_output.txt";
			File.Delete (outputFile);

			int multiItemCount = 0;
			List<string> multiNames = new List<string> ();

			for (int i = 0; i < allUnits.Count; i++) {
				MatchItem imatch = allUnits [i];
				string output = string.Format("kLocKey_{0}_LINE{1}_{2} \t= {3};",
					imatch.fileName, imatch.lineNO, i + 1, imatch.value);
				Console.WriteLine(output);


				foreach (MatchItem a in allUnits) {
					if (a != imatch && a.value == imatch.value) {
						multiItemCount++;
						multiNames.Add (a.value);
						break;
					}
				}

				output += "\r\n";
				File.AppendAllText(outputFile, output);
			}

			Console.WriteLine (string.Format("Finished searching all files, totalCount: {0}, multi: {1}.", 
				totalCount.ToString(), multiItemCount.ToString()));

			foreach (var str in multiNames)
				Console.WriteLine (str);
		}

		#region Scan files
		void GetDestinationFiles(string destDirPath)
		{
			//Console.WriteLine ("Found directory " + destDirPath);

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
		int SearchFile(string filePath, out List<MatchItem> matchStrings)
		{
			int count = 0;
			matchStrings = new List<MatchItem> ();

			Regex regex = new Regex ("@\"\\S{0,}\\p{IsCJKUnifiedIdeographs}{1,}[0-9]{0,}\"");

			FileInfo curFileInfo = new FileInfo (filePath);
			string[] allLines = File.ReadAllLines (filePath);
			for (int i = 0; i < allLines.Length; i++) {
				string curLine = allLines [i];

				Match match = regex.Match(curLine);
				while (match.Success) {
					// Handle match here...
					count++;

					//Console.WriteLine (string.Format ("{0} : {1}", count, match.Value));

					MatchItem iMatch = new MatchItem ();
					iMatch.fullPath = curFileInfo.FullName;
					iMatch.fileName = curFileInfo.Name;
					iMatch.lineNO = i;
					iMatch.value = match.Value;

					matchStrings.Add (iMatch);

					match = match.NextMatch();
				}
			}

			return count;
		}


		// Deprecated.
		public static List<string> GetComparedUnit(string sourceText, string startUnit, string endUnit)
		{
			List<string> result = new List<string> ();

			int firstIndex = sourceText.IndexOf (startUnit);
			while (firstIndex != -1) {
				string newContent = sourceText.Substring (firstIndex + startUnit.Length,
					sourceText.Length - firstIndex - startUnit.Length);

				int endIndex = newContent.IndexOf (endUnit) + firstIndex + startUnit.Length;
				if (endIndex != -1) {
					string validUnit = sourceText.Substring (firstIndex, endIndex + endUnit.Length - firstIndex);
					result.Add (validUnit);

					sourceText = sourceText.Substring (endIndex + endUnit.Length, sourceText.Length - endIndex - endUnit.Length);
					firstIndex = endIndex + endUnit.Length;

				} else {
					Console.WriteLine (string.Format("GetComparedUnit: invalid source text: [{0}]", sourceText));
					firstIndex = -1;
				}
			}

			return result;
		}

		#endregion
	}

	class MatchItem
	{
		public string fullPath {
			get;
			set;
		}

		public string fileName {
			get;
			set;
		}

		public int lineNO {
			get;
			set;
		}

		public string value {
			get;
			set;
		}
	}
}

