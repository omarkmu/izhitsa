using System;
using System.Collections;
using UnityEngine;
using Izhitsa.Events;
using Izhitsa.Events.Generic;
using static Izhitsa.Tasks.TaskUtils;

namespace Izhitsa {
	namespace Tasks {
		/**
		 <summary>
		 An IEnumerator task, for handling and controlling Coroutines.
		 </summary>
		 */
		public class Task {
			/// <summary>Was <see cref="Cancel"/> called?</summary>
			public bool CancelRequested { get; protected set; }
			/// <summary>The Exception that was thrown.</summary>
			public Exception Exception { get; protected set; }
			/// <summary>Is the current status <see cref="TaskStatus.Canceled"/>?</summary>
			public bool IsCanceled => Status == TaskStatus.Canceled;
			/// <summary>Is the current status <see cref="TaskStatus.Completed"/>?</summary>
			public bool IsCompleted => Status == TaskStatus.Completed;
			/// <summary>Is the current status <see cref="TaskStatus.Faulted"/>?</summary>
			public bool IsFaulted => Status == TaskStatus.Faulted;
			/// <summary>Is the value of <see cref="Result"/> null?</summary>
			public bool IsNull { get; protected set; } = true;
			/// <summary>Is the current status <see cref="TaskStatus.Running"/>?</summary>
			public bool IsRunning => Status == TaskStatus.Running;
			/// <summary>Is the current status <see cref="TaskStatus.WaitingToRun"/>?</summary>
			public bool IsWaiting => Status == TaskStatus.WaitingToRun;
			/// <summary>The return value of the IEnumerator.</summary>
			public virtual object Result {
				get {
					return result;
				}
				protected set {
					IsNull = (value == null);
					result = value;
				}
			}
			/// <summary>The current status of the Task.</summary>
			public TaskStatus Status { get; protected set; } = TaskStatus.Created;
			/// <summary>If exceptions are suppressed, <see cref="Run"/> will never throw exceptions.</summary>
			public bool SuppressExceptions { get; set; } = false;
			/// <summary>The type of the Task.</summary>
			public virtual Type Type => null;
			/// <summary>Was the Task force canceled?</summary>
			public bool WasForceCanceled { get; protected set; }

			/// <summary>True if <see cref="ForceCancel"/> was called.</summary>
			protected bool forceCancel = false;
			/// <summary>Event which fires when the Task is canceled.</summary>
			protected Broadcast onCancel = new Broadcast();
			/// <summary>Event which fires when the Task is completed.</summary>
			protected Broadcast<bool> onComplete = new Broadcast<bool>();
			/// <summary>Event which fires when the Task errors.</summary>
			protected Broadcast onError = new Broadcast();
			/// <summary>Event which fires on every iteration after the first in the Task.</summary>
			protected Broadcast onIteration = new Broadcast();
			/// <summary>Event which fires when the Task is ran.</summary>
			protected Broadcast onRun = new Broadcast();

			private object result;
			private object _lock = new object();


			/// <summary>Creates an empty Task which can be run later.</summary>
			public Task(){}
			/**
			 <summary>
			 Creates and runs a Task.
			 </summary>
			 <param name="enumerator">The IEnumerator to run the Task with.
			 </param>
			 <exception cref="System.Exception">Thrown if the task is already running or waiting.
			 Exceptions can also be thrown from running the Task, depending on the value of
			 <see cref="SuppressExceptions"/>.
			 </exception>
			 */
			public Task(IEnumerator enumerator){ Run(enumerator); }


			/**
			 <summary>
			 Requests task cancellation in the running IEnumerator.
			 The IEnumerator is not guaranteed to respect a cancel request.
			 </summary>
			 */
			public void Cancel(){
				CancelRequested = true;
			}
			/**
			 <summary>
			 Runs the Task after <paramref name="seconds"/> seconds have passed.
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
					throw new Exception("The Task is currently waiting.");
				StartCoroutine(delayedRun(enumerator, seconds, false));
			}
			/**
			 <summary>
			 Runs the Task after <paramref name="seconds"/> seconds have passed.
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
					throw new Exception("The Task is currently waiting.");
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
			public Signal OnCancel(Action<bool> func)
				=> onCancel.Connect(() => func(WasForceCanceled));
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
			public Signal OnError(Action<Exception> func)
				=> onError.Connect(() => func(Exception));
			/**
			 <summary>
			 Connects an Action to run on every Task iteration after the first.
			 </summary>
			 <param name="func">An Action to run on each iteration.
			 </param>
			 */
			public virtual Signal OnIteration(Action func)
				=> onIteration.Connect(() => func());
			/**
			 <summary>
			 Connects an Action to run on every Task iteration after the first.
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
				=> onRun.Connect(() => func());
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
					throw new Exception("The Task is currently waiting.");
				return StartCoroutine(run(enumerator));
			}
			/**
			 <summary>
			 Returns a TaskAwaiter which will wait until the task is not running.
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

					while (true){
						if (forceCancel){
							Status = TaskStatus.Canceled;
							WasForceCanceled = true;
							onCancel.Fire();
							onComplete.Fire(false);
							yield break;
						}
						try {
							if (!enumerator.MoveNext()) break;
							Result = enumerator.Current;
							onIteration.Fire();
						} catch (OperationCanceledException){
							Status = TaskStatus.Canceled;
							onCancel.Fire();
							onComplete.Fire(false);
							yield break;
						} catch (Exception e){
							Status = TaskStatus.Faulted;
							Exception = e;
							onError.Fire();
							onComplete.Fire(false);
							if (SuppressExceptions) yield break;
							throw Exception;
						}
						yield return Result;
					}

					Status = TaskStatus.Completed;
					onComplete.Fire(true);
				}
			}
		}
	}
}