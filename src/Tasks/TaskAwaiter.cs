using UnityEngine;

namespace Izhitsa.Tasks {
	/**
	 <summary>
	 YieldInstruction class which can be used in IEnumerators
	 to wait for a <see cref="Task"/> to stop running.
	 </summary>
	 */
	public class TaskAwaiter : CustomYieldInstruction {
		/// <summary>Used by Unity to continue waiting.</summary>
		public override bool keepWaiting
			=> task.IsRunning || task.IsWaiting;

		private Task task;

		
		/**
		 <summary>
		 Creates a TaskAwaiter which will wait for a Task to stop running.
		 </summary>
		 <param name="task">The Task this TaskAwaiter should wait for.
		 </param>
		 */
		internal TaskAwaiter(Task task){
			this.task = task;
		}
	}
}