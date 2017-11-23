using System;

namespace Izhitsa {
	namespace Tasks {
		/**
		 * <summary>
		 * Exception which can be thrown by IEnumerators to signal acknowledgement
		 * of a cancellation request to the `<see cref="Izhitsa.Tasks.Task"/>`.
		 * </summary>
		 */
		public class TaskCanceledException : OperationCanceledException {
			/// <summary>The Task that was canceled.</summary>
			public Task Task { get; private set; }

			/// <summary>Creates an empty TaskCanceledException.</summary>
			public TaskCanceledException(){}
			/**
			 * <summary>
			 * Creates a TaskCanceledException.
			 * </summary>
			 * <param name="task">The Task to be canceled.
			 * </param>
			 */
			public TaskCanceledException(Task task){
				Task = task;
			}
		}
	}
}