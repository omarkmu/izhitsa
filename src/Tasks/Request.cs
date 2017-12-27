using System;

namespace Izhitsa.Tasks {
	/**
	 <summary>
	 Used to create and answer requests.
	 </summary>
	 */
	public class Request {
		/// <summary>Has the request been answered? (Read Only)</summary>
		public bool Answered { get; protected set; } = false;
		/// <summary>An exception that was thrown while calling Func.
		/// null if Func executed successfully. (Read Only)</summary>
		public Exception Exception { get; protected set; }
		/// <summary>Is the result of the request null? (Read Only)</summary>
		public virtual bool IsNull => (Result == null);
		/// <summary>The Func to run. (Read Only)</summary>
		public virtual Func<object> Func { get; protected set; }
		/// <summary>The result of calling <see cref="Func"/>. (Read Only)</summary>
		public virtual object Result { get; protected set; } = null;

		/**
		 <summary>
		 Creates a Request.
		 </summary>
		 */
		protected Request(){}
		/**
		 <summary>
		 Creates a Request.
		 </summary>
		 <param name="func">The Func to run when answering the request.
		 </param>
		 */
		public Request(Func<object> func){
			Func = func;
		}


		/**
		 <summary>
		 Answers the Request.
		 </summary>
		 */
		public virtual void Answer(){
			if (Answered)
				return;
			Exception e;
			Result = safeRun(Func, out e);
			Exception = e;
			Answered = true;
		}

		/**
		 <summary>
		 Runs a Func safely and returns its result.
		 </summary>
		 <param name="func">The Func to run.
		 </param>
		 <param name="exc">An exception raised while running the Func,
		 if any.</param>
		 */
		private static object safeRun(Func<object> func, out Exception exc){
			if (func == null){
				exc = null;
				return null;
			}
			try {
				exc = null;
				return func();
			} catch (Exception e){
				exc = e;
				return null;
			}
		}
	}
}