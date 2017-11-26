using Izhitsa.Tasks;
using Izhitsa.Tasks.Generic;
using System;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace Izhitsa {
	namespace Tasks {
		/**
		 <summary>
		 Provides concurrency-related utilities.
		 </summary>
		 */
		public static class TaskUtils {
			/// <summary> A reference of the main thread.</summary>
			internal static Thread main { get; set; }
			
			/**
			 <summary>
			 Returns true if the calling method is running
			 on the main thread.
			 </summary>
			 */
			public static bool IsMainThread(){
				if (main == null){
					Proxy.Activate();
					return true;
				}
				return main == Thread.CurrentThread;
			}
			/**
			 <summary>
			 Starts a Coroutine.
			 </summary>
			 <param name="routine">An IEnumerator to start the Coroutine with.
			 </param>
			 */
			public static Coroutine StartCoroutine(IEnumerator routine) => Proxy.startCoroutine(routine);
		}
	}
}