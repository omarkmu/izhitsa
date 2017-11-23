using System;
using System.Collections;
using static Izhitsa.EventManagement.EventManager;

namespace Izhitsa {
	namespace Tasks {
		namespace Generic {
			/**
			 * <summary>
			 * An IEnumerator task, for handling and controlling Coroutines.
			 * The provided type is used for the Result type, and throws an exception if
			 * the IEnumerator's return type doesn't match.
			 * </summary>
			 */
			public class Task<TResult> : Task {
				/// <summary>The return value of the IEnumerator.</summary>
				public new TResult Result { get; protected set; }
				/// <summary>The type of the Task.</summary>
				public override Type Type => typeof(TResult);

				/// <summary>Creates an empty Task which can be run later.</summary>
				public Task(){}
				/**
				 * <summary>
				 * Creates and runs a Task.
				 * </summary>
				 * <param name="enumerator">The IEnumerator to run the Task with.
				 * </param>
				 * <exception cref="System.Exception">Thrown if the task is already running or waiting.
				 * Exceptions can also be thrown from running the Task, depending on the value of
				 * `<see cref="Izhitsa.Tasks.Task.SuppressExceptions"/>`.
				 * </exception>
				 */
				public Task(IEnumerator enumerator) : base(enumerator) {}

				/**
				 * <summary>
				 * Connects an Action to run when the Task is completed.
				 * </summary>
				 * <param name="func">An Action to run when the task is completed.
				 * Gets called with the result of the Task.
				 * </param>
				 */
				public Signal OnComplete(Action<TResult> func)
					=> onComplete.Connect(() => func(Result));
				/**
				 * <summary>
				 * Connects an Action to run on every Task iteration after the first.
				 * </summary>
				 * <param name="func">An Action to run on each iteration.
				 * Gets called with the current result of the Task.
				 * </param>
				 */
				public Signal OnIteration(Action<TResult> func)
					=> onIteration.Connect(() => func(Result));

				/**
				 * <summary>
				 * The internal IEnumerator handler.
				 * </summary>
				 * <param name="enumerator">The IEnumerator to run.
				 * </param>
				 */
				protected override IEnumerator run(IEnumerator enumerator){
					Status = TaskStatus.Running;
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
							yield break;
						} catch (InvalidCastException){
							Status = TaskStatus.Faulted;
							string message = (enumerator.Current == null) ?
								$"Failed cast from null, type {Type.Name} is not nullable; consider using the Nullable type." :
								$"Failed cast from {enumerator.Current.GetType().Name} to {Type.Name}.";
							Exception = new InvalidCastException(message);
							onError.Fire();
							if (SuppressExceptions) yield break;
							throw Exception;
						} catch (Exception e){
							Status = TaskStatus.Faulted;
							Exception = e;
							onError.Fire();
							if (SuppressExceptions) yield break;
							throw Exception;
						}
						yield return Result;
					}

					Status = TaskStatus.Completed;
					onComplete.Fire();
				}
			}
		}
	}
}