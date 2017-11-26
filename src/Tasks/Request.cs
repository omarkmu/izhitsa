using System;

namespace Izhitsa {
	namespace Tasks {
		/**
		 <summary>
		 Used by <see cref="ThreadTask"/>s to receive and answer requests
		 from other threads on the main thread.
		 </summary>
		 */
		public class Request {
			/// <summary>Has the request been answered?</summary>
			public bool Answered { get; internal set; } = false;
			/// <summary>An exception that was thrown while calling Func.
			/// null if Func executed successfully.</summary>
			public Exception Exception { get; internal set; }
			/// <summary>Is the result of the request null?</summary>
			public bool IsNull { get; internal set; } = true;
			/// <summary>The Func to run.</summary>
			public Func<object> Func { get; private set; }
			/// <summary>The result of calling <see cref="Func"/>.</summary>
			public object Result {
				get {
					return result;
				}
				set {
					IsNull = (value == null);
					result = value;
				}
			}


			private object result = null;


			/**
			 <summary>
			 Creates a Request.
			 </summary>
			 */
			internal Request(Func<object> func){
				Func = func;
			}
		}
	}
}