using System;

namespace Izhitsa.Tasks.Generic {
	public class Request<T> : Request {
		/// <summary>The Func to run. (Read Only)</summary>
		public new Func<T> Func { get; protected set; }
		/// <summary>Is the result of the request null or default? (Read Only)</summary>
		public new bool IsNull { get; protected set; }
		/// <summary>The result of calling <see cref="Func"/>. (Read Only)</summary>
		public new T Result { get; protected set; }


		/**
		 <summary>
		 Creates a Request.
		 </summary>
		 <param name="func">The Func to run when answering the request.
		 </param>
		 */
		public Request(Func<T> func){
			Func = func;
		}


		/**
		 <summary>
		 Answers the Request.
		 </summary>
		 */
		public override void Answer(){
			if (Answered) return;
			safeRun(this);
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
		private static void safeRun(Request<T> req){
			if (req.Func == null){
				req.IsNull = true;
				req.Result = default(T);
				req.Exception = null;
				return;
			}
			req.IsNull = true;
			req.Result = default(T);
			try {
				req.Result = req.Func();
				req.IsNull = req.Result == null;
			} catch (Exception e){
				req.Exception = e;
				return;
			}
		}
	}
}