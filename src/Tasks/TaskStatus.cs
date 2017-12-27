namespace Izhitsa.Tasks {
	/**
	 <summary>
	 Represents the current stage in the lifecycle of a <see cref="Task"/>.
	 </summary>
	 */
	public enum TaskStatus {
		/// <summary>The Task acknowledged a cancellation request by throwing a <see cref="TaskCanceledException"/>.</summary>
		Canceled,
		/// <summary>The Task completed successfully.</summary>
		Completed,
		/// <summary>The Task has been created, but has not yet been run.</summary>
		Created,
		/// <summary>The Task completed due to an unhandled Exception.</summary>
		Faulted,
		/// <summary>The Task is currently running.</summary>
		Running,
		/// <summary>The Task is waiting to run.</summary>
		WaitingToRun
	}
}