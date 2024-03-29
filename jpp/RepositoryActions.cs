﻿using System;
using JPPSVN.tasks;
using System.Windows.Forms;
using SharpSvn;

namespace JPPSVN.jpp {
	internal class RepositoryActions : IDisposable {
		public PathBuilder PathBuilder { get; set; }

		public ToolStripStatusLabel Label { get; }

		public SvnClient Client { get; }

		public RepositoryActions(ToolStripStatusLabel label) {
			Client = new SvnClient();
			Label = label;
		}

		public Tasks.StartupUpdateTask StartupUpdate(string RepositoryURL, bool urlChanged) {
			var task = new Tasks.StartupUpdateTask(Client, Label, RepositoryURL, PathBuilder, urlChanged);
			task.DoWork += (sender, e) => task.Execute();
			return task;
		}

		private StatusBackgroundWorker CreateWorker() {
			return new StatusBackgroundWorker(Label);
		}

		private CopyProjectTask.CopyProjectArgs CreateCopyProjectArgs(Data data, string destination, bool onlySrcFolderFromProject) {
			return new CopyProjectTask.CopyProjectArgs(
				destination,
				data.Revision,
				PathBuilder.GetUserProjects(data.User),
				data.Project,
				onlySrcFolderFromProject
         );
		}

      public StatusBackgroundWorker CreateCopyAllTask(Data data, string destination, bool onlySrcFolderFromProject) {
			StatusBackgroundWorker worker = CreateWorker();
			CopyProjectAndTestsTask task = new CopyProjectAndTestsTask(
				worker, 
				Client,
				CreateCopyProjectArgs(data, destination, onlySrcFolderFromProject), 
				PathBuilder.GetProjectTests(data.Project));
			worker.DoWork += task.DoWork;
	      return worker;
      }

		public StatusBackgroundWorker CreateCopyProjectTask(Data data, string destination, bool onlySrcFolderFromProject) {
			StatusBackgroundWorker worker = CreateWorker();
			CopyProjectTask task = new CopyProjectTask(
				worker,
				Client,
				CreateCopyProjectArgs(data, destination, onlySrcFolderFromProject));
			worker.DoWork += task.DoWork;
			return worker;
		}

		public void Dispose() {
			Client.Dispose();
		}
	}
}
