﻿using System;
using System.IO;

namespace JPPSVN {
	public static class DirectoryUtil {
		public static void Copy(string sourceDirName, string destDirName, bool copySubDirs) {
			// Get the subdirectories for the specified directory.
			DirectoryInfo dir = new DirectoryInfo(sourceDirName);

			if(!dir.Exists) {
				throw new DirectoryNotFoundException(
					"Source directory does not exist or could not be found: "
					+ sourceDirName);
			}

			Copy(dir, destDirName, copySubDirs);
		}

		private static void Copy(DirectoryInfo dir, string destDirName, bool copySubDirs) {
			DirectoryInfo[] dirs = dir.GetDirectories();
			// If the destination directory doesn't exist, create it.
			if(!Directory.Exists(destDirName)) {
				Directory.CreateDirectory(destDirName);
			}

			// Get the files in the directory and copy them to the new location.
			FileInfo[] files = dir.GetFiles();
			foreach(FileInfo file in files) {
				string temppath = Path.Combine(destDirName, file.Name);
				file.CopyTo(temppath, true);
			}

			// If copying subdirectories, copy them and their contents to new location.
			if(copySubDirs) {
				foreach(DirectoryInfo subdir in dirs) {
					string temppath = Path.Combine(destDirName, subdir.Name);
					Copy(subdir, temppath, copySubDirs);
				}
			}
		}

		public static void CopyIgnoreNotExists(string sourceDirName, string destDirName, bool copySubDirs) {
			// Get the subdirectories for the specified directory.
			DirectoryInfo dir = new DirectoryInfo(sourceDirName);

			if(dir.Exists)
				Copy(dir, destDirName, copySubDirs);
		}

		public static void DeleteDirectory(string path) {
			foreach(string directory in Directory.GetDirectories(path)) {
				DeleteDirectory(directory);
			}

			try {
				Directory.Delete(path, true);
			} catch(IOException) {
				Directory.Delete(path, true);
			} catch(UnauthorizedAccessException) {
				Directory.Delete(path, true);
			}
		}
	}
}