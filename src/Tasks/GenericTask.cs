using System;
using System.Collections;
using Izhitsa.Events;
using Izhitsa.Events.Generic;

namespace Izhitsa {
	namespace Tasks {
		namespace Generic {
			/**
			 <summary>
			 The generic form of <see cref="Task"/>, for handling and controlling Coroutines.
			 The provided type is used for the Result type, and throws an exception while
			 running if the IEnumerator's return type is incompatible.
			 </summary>
			 */
			public class Task<TResult> : Task {
				/// <summary>Is the value of <see cref="Result"/> null?</summary>
				public new bool IsNull { get; protected set; }
				/// <summary>The return value of the IEnumerator.</summary>
				public new TResult Result { get; protected set; }
				/// <summary>The type of the Task.</summary>
				public override Type Type => typeof(TResult);

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
				 <see cref="Izhitsa.Tasks.Task.SuppressExceptions"/>.
				 </exception>
				 */
				public Task(IEnumerator enumerator) : base(enumerator) {}


				/**
				 <summary>
				 Connects an Action to run when the Task is completed.
				 </summary>
				 <param name="func">An Action to run when the task is completed.
				 Gets called with a boolean representing whether or not the task completed successfully,
				 and the result of the Task.
				 </param>
				 */
				public Signal<bool> OnComplete(Action<bool, TResult> func)
					=> onComplete.Connect(success => func(success, Result));
				/**
				 <summary>
				 Connects an Action to run on every Task iteration after the first.
				 </summary>
				 <param name="func">An Action to run on each iteration.
				 Gets called with the current result of the Task.
				 </param>
				 */
				public Signal OnIteration(Action<TResult> func)
					=> onIteration.Connect(() => func(Result));

				/**
				 <summary>
				Creates, runs, and returns a Task.
				</summary>
				<param name="enumerator">The IEnumerator to run.
				</param>
				*/
				public static new Task<TResult> Start(IEnumerator enumerator)
					=> new Task<TResult>(enumerator);

				/**
				 <summary>
				 The internal IEnumerator handler.
				 </summary>
				 <param name="enumerator">The IEnumerator to run.
				 </param>
				 */
				protected override IEnumerator run(IEnumerator enumerator){
					lock (_lock){
						Status = TaskStatus.Running;
						Result = default(TResult);
						IsNull = true;
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
								object current = enumerator.Current;
								IsNull = (current == null);
								Result = (TResult)current;
								onIteration.Fire();
							} catch (OperationCanceledException){
								Status = TaskStatus.Canceled;
								onCancel.Fire();
								onComplete.Fire(false);
								yield break;
							} catch (InvalidCastException){
								Status = TaskStatus.Faulted;
								string message = (enumerator.Current == null) ?
									$"Failed cast from null, type {Type.Name} is not nullable; consider using the Nullable type." :
									$"Failed cast from {enumerator.Current.GetType().Name} to {Type.Name}.";
								Exception = new InvalidCastException(message);
								onError.Fire();
								onComplete.Fire(false);
								if (SuppressExceptions) yield break;
								throw Exception;
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
}