using Izhitsa.Events;
using Izhitsa.Events.Generic;
using static Izhitsa.Tasks.TaskUtils;
using System;
using System.Collections;
using UnityEngine;

namespace Izhitsa.Tasks {
	/**
	 <summary>
	 An IEnumerator task, for handling and controlling Coroutines.
	 </summary>
	 */
	public class Task {
		/// <summary>Was <see cref="Cancel"/> called? (Read Only)</summary>
		public bool CancelRequested { get; protected set; }
		/// <summary>The Exception that was thrown. (Read Only)</summary>
		public Exception Exception { get; protected set; }
		/// <summary>Is the current status <see cref="TaskStatus.Canceled"/>? (Read Only)</summary>
		public bool IsCanceled => Status == TaskStatus.Canceled;
		/// <summary>Is the current status <see cref="TaskStatus.Completed"/>? (Read Only)</summary>
		public bool IsCompleted => Status == TaskStatus.Completed;
		/// <summary>Is the current status <see cref="TaskStatus.Faulted"/>? (Read Only)</summary>
		public bool IsFaulted => Status == TaskStatus.Faulted;
		/// <summary>Is the value of <see cref="Result"/> null? (Read Only)</summary>
		public bool IsNull => Result == null;
		/// <summary>Is the current status <see cref="TaskStatus.Running"/>? (Read Only)</summary>
		public bool IsRunning => Status == TaskStatus.Running;
		/// <summary>Is the current status <see cref="TaskStatus.WaitingToRun"/>? (Read Only)</summary>
		public bool IsWaiting => Status == TaskStatus.WaitingToRun;
		/// <summary>The return value of the IEnumerator. (Read Only)</summary>
		public virtual object Result { get; protected set; }
		/// <summary>The current status of the Task. (Read Only)</summary>
		public TaskStatus Status { get; protected set; } = TaskStatus.Created;
		/// <summary>If exceptions are suppressed, <see cref="Run"/> will never throw exceptions.</summary>
		public bool SuppressExceptions { get; set; } = false;
		/// <summary>The type of the Task. (Read Only)</summary>
		public virtual Type Type => typeof(object);
		/// <summary>Was the Task force canceled? (Read Only)</summary>
		public bool WasForceCanceled { get; protected set; }

		/// <summary>True if <see cref="ForceCancel"/> was called.</summary>
		protected bool forceCancel = false;
		/// <summary>Event which fires when the Task is canceled. (Read Only)</summary>
		protected virtual Broadcast<bool> onCancel { get; } = new Broadcast<bool>();
		/// <summary>Event which fires when the Task is completed. (Read Only)</summary>
		protected virtual Broadcast<bool> onComplete { get; } = new Broadcast<bool>();
		/// <summary>Event which fires when the Task errors. (Read Only)</summary>
		protected virtual Broadcast<Exception> onError { get; } = new Broadcast<Exception>();
		/// <summary>Event which fires on every iteration after the first in the Task. (Read Only)</summary>
		protected virtual Broadcast onIteration { get; } = new Broadcast();
		/// <summary>Event which fires when the Task is run. (Read Only)</summary>
		protected virtual Broadcast onRun { get; } = new Broadcast();

		private object _lock = new object();


		/// <summary>Creates an empty Task which can be run later.</summary>
		public Task(){}
		/**
		 <summary>
		 Creates and runs a Task.
		 </summary>
		 <param name="enumerator">The IEnumerator to run the Task with.
		 </param>
		 <exception cref="System.Exception">Exceptions can be thrown from running the Task,
		 depending on the value of <see cref="SuppressExceptions"/>.
		 </exception>
		 */
		public Task(IEnumerator enumerator){ Run(enumerator); }


		/**
		 <summary>
		 Requests task cancellation for the Task.
		 The IEnumerator is not guaranteed to respect a cancel request.
		 </summary>
		 */
		public void Cancel(){
			CancelRequested = true;
		}
		/**
		 <summary>
		 Runs the Task after a delay.
		 Uses scaled time.
		 </summary>
		 <param name="enumerator">The IEnumerator to run the Task with.
		 </param>
		 <param name="seconds">The time to wait before running the Task.
		 </param>
		 <exception cref="System.Exception">Thrown if the task is already running or waiting.
		 </exception>
		 */
		public virtual void Delay(IEnumerator enumerator, float seconds){
			if (IsRunning)
				throw new Exception("The Task is already running.");
			if (IsWaiting)
				throw new Exception("The Task is currently waiting to run.");
			StartCoroutine(delayedRun(enumerator, seconds, false));
		}
		/**
		 <summary>
		 Runs the Task after a delay.
		 Uses unscaled time.
		 </summary>
		 <param name="enumerator">The IEnumerator to run the Task with.
		 </param>
		 <param name="seconds">The time to wait before running the Task.
		 </param>
		 <exception cref="System.Exception">Thrown if the task is already running or waiting.
		 </exception>
		 */
		public virtual void DelayRealtime(IEnumerator enumerator, float seconds){
			if (IsRunning)
				throw new Exception("The Task is already running.");
			if (IsWaiting)
				throw new Exception("The Task is currently waiting to run.");
			StartCoroutine(delayedRun(enumerator, seconds, true));
		}
		/**
		 <summary>
		 Forces the IEnumerator to cancel.
		 Usage of <see cref="Cancel"/> is preferred.
		 </summary>
		 */
		public virtual void ForceCancel(){
			forceCancel = true;
		}
		/**
		 <summary>
		 Connects an Action to run when the Task gets canceled.
		 </summary>
		 <param name="func">An Action to run when the Task gets canceled.
		 Gets called with a boolean which is true if the Task was force canceled.
		 </param>
		 */
		public Signal<bool> OnCancel(Action<bool> func)
			=> onCancel.Connect(func);
		/**
		 <summary>
		 Connects an Action to run when the Task is completed.
		 </summary>
		 <param name="func">An Action to run when the task is completed.
		 Gets called with a boolean representing if the 
		 task completed successfully.
		 </param>
		 */
		public Signal<bool> OnComplete(Action<bool> func)
			=> onComplete.Connect(func);
		/**
		 <summary>
		 Connects an Action to run when the Task is completed.
		 </summary>
		 <param name="func">An Action to run when the task is completed.
		 Gets called with a boolean representing if the 
		 task completed successfully, and the result of the Task.
		 </param>
		 */
		public Signal<bool> OnComplete(Action<bool, object> func)
			=> onComplete.Connect(success => func(success, Result));
		/**
		 <summary>
		 Connects an Action to run when an exception is thrown in the Task.
		 </summary>
		 <param name="func">An Action to run when an exception is thrown.
		 Gets called with the Exception.
		 </param>
		 */
		public Signal<Exception> OnError(Action<Exception> func)
			=> onError.Connect(func);
		/**
		 <summary>
		 Connects an Action to run on every Task iteration.
		 </summary>
		 <param name="func">An Action to run on each iteration.
		 </param>
		 */
		public virtual Signal OnIteration(Action func)
			=> onIteration.Connect(func);
		/**
		 <summary>
		 Connects an Action to run on every Task iteration.
		 </summary>
		 <param name="func">An Action to run on each iteration.
		 Gets called with the current result of the Task.
		 </param>
		 */
		public virtual Signal OnIteration(Action<object> func)
			=> onIteration.Connect(() => func(Result));
		/**
		 <summary>
		 Connects an Action to run when the Task is ran.
		 </summary>
		 <param name="func">An Action to run when the task is ran.
		 </param>
		 */
		public Signal OnRun(Action func)
			=> onRun.Connect(func);
		/**
		 <summary>
		 Runs the Task.
		 </summary>
		 <param name="enumerator">The IEnumerator to run the Task with.
		 </param>
		 <exception cref="System.Exception">Thrown if the task is already running or waiting.
		 Exceptions can also be thrown from running the Task, depending on the value of
		 <see cref="SuppressExceptions"/>.
		 </exception>
		 */
		public virtual Coroutine Run(IEnumerator enumerator){
			if (IsRunning)
				throw new Exception("The Task is already running.");
			if (IsWaiting)
				throw new Exception("The Task is currently waiting to run.");
			return StartCoroutine(run(enumerator));
		}
		/**
		 <summary>
		 Returns a YieldInstruction which will wait until the task is not running.
		 </summary>
		 <example>
		 <code>
		 public IEnumerator DoAfterTask(Task task, Action action){
			yield return task.Wait();
			action();
		 }
		 </code>
		 </example>
		 */
		public TaskAwaiter Wait() => new TaskAwaiter(this);

		/**
		 <summary>
		 Creates, runs, and returns a Task.
		 </summary>
		 <param name="enumerator">The IEnumerator to run.
		 </param>
		 */
		public static Task Start(IEnumerator enumerator)
			=> new Task(enumerator);
		//
		
		/**
		 <summary>
		 Waits and then runs the Task.
		 </summary>
		 <param name="enumerator">The IEnumerator to run.
		 </param>
		 <param name="seconds">The amount of seconds to wait before running.
		 </param>
		 <param name="realtime">Should the wait use realtime (as opposed to Time.time)?
		 </param>
		 */
		protected IEnumerator delayedRun(IEnumerator enumerator, float seconds, bool realtime){
			Status = TaskStatus.WaitingToRun;
			if (realtime){
				yield return new WaitForSecondsRealtime(seconds);
			} else {
				yield return new WaitForSeconds(seconds);
			}
			StartCoroutine(run(enumerator));
		}
		/**
		 <summary>
		 The internal IEnumerator handler.
		 </summary>
		 <param name="enumerator">The IEnumerator to run.
		 </param>
		 */
		protected virtual IEnumerator run(IEnumerator enumerator){
			lock (_lock){
				Status = TaskStatus.Running;
				Result = null;
				WasForceCanceled = false;
				CancelRequested = false;
				forceCancel = false;
				onRun.Fire();
				bool hasLooped = false;

				while (true){
					if (forceCancel){
						Status = TaskStatus.Canceled;
						WasForceCanceled = true;
						onCancel.Fire(true);
						onComplete.Fire(false);
						yield break;
					}
					try {
						if (hasLooped) onIteration.Fire();
						if (!enumerator.MoveNext()) break;
						Result = enumerator.Current;
					} catch (OperationCanceledException){
						Status = TaskStatus.Canceled;
						onCancel.Fire(false);
						onComplete.Fire(false);
						yield break;
					} catch (Exception e){
						Status = TaskStatus.Faulted;
						Exception = e;
						onError.Fire(Exception);
						onComplete.Fire(false);
						if (SuppressExceptions) yield break;
						throw Exception;
					}

					hasLooped = true;
					yield return null;
				}

				Status = TaskStatus.Completed;
				onComplete.Fire(true);
			}
		}
	}
}