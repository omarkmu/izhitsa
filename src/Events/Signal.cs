using System;

namespace Izhitsa.Events {
	/**
	 <summary>
	 Contains a callback for use in <see cref="Events.Broadcast"/>.
	 </summary>
	 <seealso cref="Events.Broadcast"/>
	 */
	public class Signal {
		/**
		 <summary>
		 The Broadcast that this Signal is connected to. (Read Only)
		 </summary>
		 <exception cref="MemberAccessException">
		 Thrown if an attempt to access the member is made
		 after the signal has been disconnected.
		 </exception>
		 */
		public Broadcast Broadcast {
			get {
				if (Disconnected)
					throw new MemberAccessException("Cannot access member; " +
					"the signal is disconnected.");
				return broadcast;
			}
		}
		/// <summary>Is this signal disconnected? (Read Only)</summary>
		public bool Disconnected { get; private set; }

		/// <summary>The signal's callback function.</summary>
		internal Action<object[]> callback { get; set; }


		/// <summary>The Broadcast this Signal is connected to.</summary>
		private Broadcast broadcast;
		
		
		/**
		 <summary>
		 Creates a Signal.
		 </summary>
		 <param name="func">A callback function to connect to the Signal.
		 </param>
		 <param name="bc">The Broadcast
		 that the Signal is being added to.</param>
		 */
		internal Signal(Action<object[]> func, Broadcast bc){
			callback = func;
			broadcast = bc;
		}


		/**
		 <summary>
		 Disconnects the Signal from its Broadcast.
		 </summary>
		 <exception cref="MethodAccessException">
		 Thrown if the Signal is already disconnected.
		 </exception>
		 */
		public void Disconnect(){
			if (Disconnected)
				throw new MethodAccessException("The signal is already disconnected.");
			broadcast?.disconnect(this);
			Disconnected = true;
			broadcast = null;
			callback = null;
		}
		
		/**
		 <summary>
		 Calls the callback function.
		 </summary>
		 <param name="args">Arguments to fire the Signal with.</param>
		 */
		internal void call(params object[] args) {
			if (callback == null) return;
			callback(args);
		}
	}
}