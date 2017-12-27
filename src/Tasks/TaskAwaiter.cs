using UnityEngine;

namespace Izhitsa.Tasks {
	/**
	 <summary>
	 YieldInstruction class which can be used in IEnumerators
	 to wait for a <see cref="Task"/> to stop running.
	 </summary>
	 */
	public class TaskAwaiter : CustomYieldInstruction {
		public override bool keepWaiting
			=> task.IsRunning || task.IsWaiting;

		private Task task;

		/**
		 <summary>
		 Creates a TaskAwaiter which will wait for a Task to stop running.
		 </summary>
		 <param name="t">The Task this TaskAwaiter should wait for.
		 </param>
		 */
		public TaskAwaiter(Task t){
			task = t;
		}
	}
}