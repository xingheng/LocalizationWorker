using System;
using System.IO;

namespace LocalizationWorker
{
	public static class Utilities
	{
		public static string GetFileName(string fullPath)
		{
			FileInfo fileInfo = new FileInfo(fullPath);
			string fileName = fileInfo.Name;

			int index = fileName.IndexOf ('.');
			if (index >= 0) {
				fileName = fileName.Substring (0, index);
			}

			return fileName;
		}

		public static bool ForceWriteFile(string filePath, string content)
		{
			try
			{
				using (StreamWriter newTask = new StreamWriter (filePath, false/* overwrite*/)) { 
					newTask.WriteLine (content);
				}
			}
			catch (Exception ex) {
				Console.WriteLine (ex.ToString ());
				return false;
			}
			return true;
		}
	}
}

