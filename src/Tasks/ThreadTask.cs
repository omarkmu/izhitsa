using Izhitsa.Events;
using Izhitsa.Tasks.Generic;
using static Izhitsa.Tasks.TaskUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Izhitsa.Tasks {
	/**
	 <summary>
	 A thread task which facilitates threading.
	 </summary>
	 */
	public class ThreadTask : Task {
		/// <summary>The return value of the task. (Read Only)</summary>
		public new object Result { get; protected set; }
		/// <summary>The amount of time, in milliseconds, to sleep while waiting
		/// for a Request.</summary>
		public int RequestSleepTime { get; set; } = 1;
		/// <summary>ThreadTasks always suppress exceptions. (Read Only)</summary>
		public new bool SuppressExceptions { get; } = true;

		private Thread thread;


		/// <summary>The maximum amount of <see cref="Tasks.Request"/>s to answer each frame.</summary>
		public static int MaxRequests { get; set; } = 15;
		/**
		 <summary>
		 The maximum amount of running threads. Does not include the main thread.
		 </summary>
		 <exception cref="ArgumentException">Thrown if value is less than one.</exception>
		 */
		public static int MaxThreads { 
			get { return maxThreads; }
			set {
				if (value < 1)
					throw new ArgumentException("MaxThreads must be at least 1.", "value");
				maxThreads = value;
			}
		}

		/// <summary>Contains all of the currently managed threads.</summary>
		private static Dictionary<Thread, ThreadTask> threads { get; }
			= new Dictionary<Thread, ThreadTask>();
		/// <summary>A queue of main thread requests.</summary>
		private static Queue<Request> requests { get; }
			= new Queue<Request>();
		///

		private static int maxThreads = 10;
		

		/// <summary>Creates an empty ThreadTask which can be run later.</summary>
		public ThreadTask(){}
		/// <summary>Creates a ThreadTask and runs <paramref name="func"/> on a new thread.</summary>
		public ThreadTask(Action<ThreadTask> func){ Run(func); }
		/// <summary>Creates a ThreadTask and runs <paramref name="func"/> on a new thread.</summary>
		public ThreadTask(Func<ThreadTask, object> func){ Run(func); }


		/**
		 <summary>
		 Runs <paramref name="func"/> on a new thread after a delay.
		 Uses scaled time.
		 </summary>
		 <param name="func">The Action to run.
		 </param>
		 <param name="seconds">The time to wait before running the ThreadTask.
		 </param>
		 <exception cref="System.Exception">Thrown if the task is already running or waiting, or if
		 Delay is called from a thread which isn't the main thread.
		 </exception>
		 */
		public void Delay(Action<ThreadTask> func, float seconds){
			if (!IsMainThread())
				throw new Exception("Delay can only be called from the main thread.");
			if (IsRunning)
				throw new Exception("The Task is already running.");
			if (IsWaiting)
				throw new Exception("The Task is currently waiting to run.");
			StartCoroutine(delayedRun(task => {
				func(task);
				return null;
			}, seconds, false));
		}
		/**
		 <summary>
		 Runs <paramref name="func"/> on a new thread after a delay.
		 Uses scaled time.
		 </summary>
		 <param name="func">The Func to run.
		 </param>
		 <param name="seconds">The time to wait before running the ThreadTask.
		 </param>
		 <remarks>
		 <see cref="Result"/> will take on the return value of the provided Func.
		 </remarks>
		 <exception cref="System.Exception">Thrown if the task is already running or waiting, or if
		 Delay is called from a thread which isn't the main thread.
		 </exception>
		 */
		public void Delay(Func<ThreadTask, object> func, float seconds){
			if (!IsMainThread())
				throw new Exception("Delay can only be called from the main thread.");
			if (IsRunning)
				throw new Exception("The Task is already running.");
			if (IsWaiting)
				throw new Exception("The Task is currently waiting to run.");
			StartCoroutine(delayedRun(func, seconds, false));
		}
		/**
		 <summary>
		 Runs <paramref name="func"/> on a new thread after a delay.
		 Uses unscaled time.
		 </summary>
		 <param name="func">The Action to run.
		 </param>
		 <param name="seconds">The time to wait before running the ThreadTask, in real time.
		 </param>
		 <exception cref="System.Exception">Thrown if the task is already running or waiting, or if
		 DelayRealtime is called from a thread which isn't the main thread.
		 </exception>
		 */
		public void DelayRealtime(Action<ThreadTask> func, float seconds){
			if (!IsMainThread())
				throw new Exception("DelayRealtime can only be called from the main thread.");
			if (IsRunning)
				throw new Exception("The Task is already running.");
			if (IsWaiting)
				throw new Exception("The Task is currently waiting to run.");
			StartCoroutine(delayedRun(task => {
				func(task);
				return null;
			}, seconds, true));
		}
		/**
		 <summary>
		 Runs <paramref name="func"/> on a new thread after a delay.
		 Uses unscaled time.
		 </summary>
		 <param name="func">The Func to run.
		 </param>
		 <param name="seconds">The time to wait before running the ThreadTask, in real time.
		 </param>
		 <remarks>
		 <see cref="Result"/> will take on the return value of the provided Func.
		 </remarks>
		 <exception cref="System.Exception">Thrown if the task is already running or waiting., or if
		 DelayRealtime is called from a thread which isn't the main thread.
		 </exception>
		 */
		public void DelayRealtime(Func<ThreadTask, object> func, float seconds){
			if (!IsMainThread())
				throw new Exception("DelayRealtime can only be called from the main thread.");
			if (IsRunning)
				throw new Exception("The Task is already running.");
			if (IsWaiting)
				throw new Exception("The Task is currently waiting to run.");
			StartCoroutine(delayedRun(func, seconds, true));
		}
		/**
		 <summary>
		 Attempts to force the ThreadTask to abort.
		 Not guaranteed to succeed.
		 </summary>
		 */
		public override void ForceCancel(){
			lock (threads){
				if (thread != null){
					if (threads.ContainsKey(thread)){
						threads.Remove(thread);
						thread.Abort();
					}
					thread = null;
				}
				Status = TaskStatus.Canceled;
				WasForceCanceled = true;
				if (IsMainThread()){
					onCancel.Fire(true);
					onComplete.Fire(false);	
				} else {
					Request(()=> onCancel.Fire(true), true);
					Request(()=> onComplete.Fire(false), true);
				}
			}
		}
		/**
		 <summary>
		 Makes a request for a Action to be called on the main thread.
		 Returns a <see cref="Tasks.Request"/>.
		 </summary>
		 <param name="action">The Action to be called.
		 </param>
		 <param name="async">If this is false, the thread will sleep until
		 the Request is answered. Otherwise, it will return the Request for the calling
		 thread to handle.
		 </param>
		 <exception cref="Exception">Thrown if an attempt to make a request from the
		 main thread is made.</exception>
		 */
		public Request Request(Action action, bool async = false){
			if (IsMainThread())
				throw new Exception("Cannot make a request from the main thread.");
			Request request = new Request(() => {
				action();
				return null;
			});
			lock (requests) requests.Enqueue(request);
			
			while (!request.Answered && !async) Sleep(RequestSleepTime);
			return request;
		}
		/**
		 <summary>
		 Makes a request for a Func to be called on the main thread.
		 Returns a <see cref="Tasks.Request"/>.
		 </summary>
		 <param name="func">The Func to be called.
		 The <see cref="Tasks.Request.Result"/> property
		 will be set to the return value of this Func.
		 </param>
		 <param name="async">If this is false, the thread will sleep until the request is answered.
		 </param>
		 <exception cref="Exception">Thrown if an attempt to make a request from the
		 main thread is made.</exception>
		 */
		public Request Request(Func<object> func, bool async = false){
			if (IsMainThread())
				throw new Exception("Cannot make a request from the main thread.");
			Request request = new Request(func);
			lock (requests) requests.Enqueue(request);
			
			while (!request.Answered && !async) Sleep(RequestSleepTime);
			return request;
		}
		/**
		 <summary>
		 Makes a request for a Func to be called on the main thread.
		 Returns a <see cref="Tasks.Request"/>.
		 </summary>
		 <param name="func">The Func to be called.
		 The <see cref="Tasks.Request.Result"/> property
		 will be set to the return value of this Func.
		 </param>
		 <param name="async">If this is false, the thread will sleep until the request is answered.
		 </param>
		 <exception cref="Exception">Thrown if an attempt to make a request from the
		 main thread is made.</exception>
		 */
		public Request<T> Request<T>(Func<T> func, bool async = false){
			if (IsMainThread())
				throw new Exception("Cannot make a request from the main thread.");
			Request<T> request = new Request<T>(func);
			lock (requests) requests.Enqueue(request);
			
			while (!request.Answered && !async) Sleep(RequestSleepTime);
			return request;
		}
		/**
		 <summary>
		 Runs <paramref name="func"/> on a new thread.
		 </summary>
		 <param name="func">The Action to run.
		 </param>
		 <exception cref="System.Exception">Thrown if the task is already running or waiting, or if
		 Run is called from a thread which isn't the main thread.
		 </exception>
		 */
		public void Run(Action<ThreadTask> func){
			if (!IsMainThread())
				throw new Exception("Run can only be called from the main thread.");
			if (IsRunning)
				throw new Exception("The Task is already running.");
			if (IsWaiting)
				throw new Exception("The Task is currently waiting to run.");
			StartCoroutine(run(task => {
				func(task);
				return null;
			}));
		}
		/**
		 <summary>
		 Runs <paramref name="func"/> on a new thread.
		 </summary>
		 <param name="func">The Func to run.
		 </param>
		 <remarks>
		 <see cref="Result"/> will take on the return value of the provided Func.
		 </remarks>
		 <exception cref="System.Exception">Thrown if the task is already running or waiting, or if
		 Run is called from a thread which isn't the main thread.
		 </exception>
		 */
		public void Run(Func<ThreadTask, object> func){
			if (!IsMainThread())
				throw new Exception("Run can only be called from the main thread.");
			if (IsRunning)
				throw new Exception("The Task is already running.");
			if (IsWaiting)
				throw new Exception("The Task is currently waiting to run.");
			
			StartCoroutine(run(func));
		}
		/**
		 <summary>
		 Puts the thread to sleep.
		 </summary>
		 <param name="milliseconds">The amount of milliseconds to sleep for.</param>
		 */
		public void Sleep(int milliseconds = 1) => Thread.Sleep(milliseconds);

		/**
		 <summary>
		 Creates, runs, and returns a ThreadTask.
		 </summary>
		 <param name="func">The Action to run.
		 </param>
		 */
		public static ThreadTask Start(Action<ThreadTask> func)
			=> new ThreadTask(func);

		/**
		 <summary>
		 Waits and then runs the Task.
		 </summary>
		 <param name="func">The Func to run.
		 </param>
		 <param name="seconds">The amount of seconds to wait before running.
		 </param>
		 <param name="realtime">Should the wait use realtime (as opposed to Time.time)?
		 </param>
		 */
		protected IEnumerator delayedRun(Func<ThreadTask, object> func, float seconds, bool realtime){
			Status = TaskStatus.WaitingToRun;
			if (realtime)
				yield return new WaitForSecondsRealtime(seconds);
			else
				yield return new WaitForSeconds(seconds);
			
			StartCoroutine(run(func));
		}
		/**
		 <summary>
		 The internal ThreadTask handler.
		 </summary>
		 <param name="func">The Func to run.
		 </param>
		 */
		protected IEnumerator run(Func<ThreadTask, object> func){
			Status = TaskStatus.WaitingToRun;
			Result = null;
			WasForceCanceled = false;
			CancelRequested = false;
			forceCancel = false;

			while (true){
				lock (threads){
					if (threads.Count >= MaxThreads){
						yield return null;
						continue;
					}
					ThreadStart threadStart = wrapper(func);
					thread = new Thread(threadStart);
					
					threads.Add(thread, this);
					thread.Start();
					break;
				}
			}
		}

		/**
		 <summary>Dequeues and answers thread requests.</summary>
		 */
		internal static void answerRequests(){
			int answered = 0;
			lock (requests){
				while (requests.Count > 0){
					requests.Dequeue().Answer();
					if (++answered >= MaxRequests) break;
				}
			}
		}
		/**
		 <summary>Attempts to force cancel all running (non-main) threads.</summary>
		 */
		internal static void terminate(){
			lock (threads){
				int i = 0;
				ThreadTask[] tasks = new ThreadTask[threads.Count];
				foreach (ThreadTask task in threads.Values)
					tasks[i++] = task;
				foreach (ThreadTask task in tasks)
					task.ForceCancel();
			}
		}
		
		/**
		 <summary>
		 Wraps a Func in a safe ThreadStart.
		 </summary>
		 <param name="func">The Func to wrap.</param>
		 */
		private ThreadStart wrapper(Func<ThreadTask, object> func){
			return () => {
				bool completed = false;
				try {
					object ret;
					Status = TaskStatus.Running;
					Request(()=> onRun.Fire(), true);
					
					ret = func(this);
					Result = ret;
					completed = true;
				} catch (OperationCanceledException){
					Status = TaskStatus.Canceled;
					Request(()=> onComplete.Fire(false), true);
					Request(()=> onCancel.Fire(false), true);
				} catch (Exception e){
					Status = TaskStatus.Faulted;
					Exception = e;
					Request(()=> onComplete.Fire(false), true);
					Request(()=> onError.Fire(e), true);
				}

				if (completed)
					Status = TaskStatus.Completed;
				Request(()=> onComplete.Fire(completed), true);
				
				lock (threads){
					if (thread != null){
						threads.Remove(thread);
						thread = null;
					}
				}
			};
		}


		#region NotImplemented
		/**
		 <summary>
		 Not implemented.
		 </summary>
		 <exception cref="NotImplementedException">Throws when called.
		 </exception>
		 */
		public override void Delay(IEnumerator enumerator, float seconds){
			throw new NotImplementedException();
		}
		/**
		 <summary>
		 Not implemented.
		 </summary>
		 <exception cref="NotImplementedException">Throws when called.
		 </exception>
		 */
		public override void DelayRealtime(IEnumerator enumerator, float seconds){
			throw new NotImplementedException();
		}
		/**
		 <summary>
		 Not implemented.
		 </summary>
		 <exception cref="NotImplementedException">Throws when called.
		 </exception>
		 */
		public override Signal OnIteration(Action func){
			throw new NotImplementedException();
		}
		/**
		 <summary>
		 Not implemented.
		 </summary>
		 <exception cref="NotImplementedException">Throws when called.
		 </exception>
		 */
		public override Signal OnIteration(Action<object> func){
			throw new NotImplementedException();
		}
		/**
		 <summary>
		 Not implemented.
		 </summary>
		 <exception cref="NotImplementedException">Throws when called.
		 </exception>
		 */
		public override Coroutine Run(IEnumerator enumerator){
			throw new NotImplementedException();
		}
		#endregion
	}
}