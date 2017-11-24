using System.Collections;
using UnityEngine;

namespace Izhitsa {
	namespace Tasks {
		/**
		 * <summary>
		 * Provides concurrency-related utilities.
		 * </summary>
		 */
		public static class TaskUtils {
			/**
			 * <summary>
			 * Starts a Coroutine.
			 * </summary>
			 * <param name="routine">An IEnumerator to start the Coroutine with.
			 * </param>
			 */
			public static Coroutine StartCoroutine(IEnumerator routine) => Proxy.startCoroutine(routine);
		}
	}
}