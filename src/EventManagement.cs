using System;
using System.Collections;
using System.Collections.Generic;
using static Izhitsa.Utility;

namespace Izhitsa {
	/// <summary>Delegate for handling event connections.</summary>
	public delegate void Handler(object[] args);
	/**
	 * <summary>Class which facilitates event usage.</summary>
	 */
    public static class EventManager {
		/// <summary>A container for all broadcasts.</summary>
        private static Dictionary<string, Broadcast> broadcasts = new Dictionary<string, Broadcast>();
		
		/**
		 * <summary>
		 * Connects a function to a `Broadcast`, and returns a `Signal`.
		 * </summary>
		 * <param name="name">The name of the `Broadcast` to connect to.
		 * </param>
		 * <param name="func">A function to connect.
		 * </param>
		 */
		public static Signal Connect(string name, Action func)
			=> getBroadcast(name).Connect((args) => func());
		/**
		 * <summary>
		 * Connects a function to a `Broadcast`, and returns a `Signal`.
		 * </summary>
		 * <param name="name">The name of the `Broadcast` to connect to.
		 * </param>
		 * <param name="func">A function to connect.
		 * </param>
		 */
		public static Signal Connect(string name, Action<object[]> func)
			=> getBroadcast(name).Connect((args) => func(args));
		/**
		 * <summary>
		 * Fires the `Broadcast` which matches the `name`.
		 * </summary>
		 * <param name="name">The name of the `Broadcast` to fire.
		 * </param>
		 * <param name="args">The arguments to fire the `Broadcast` with.
		 * </param>
		 */
        public static void Fire(string name, params object[] args){
            if (!broadcasts.ContainsKey(name)) return;
            broadcasts[name].Fire(args);
        }
		/**
		 * <summary>
		 * Returns the `Broadcast` which matches the name `name`, or null if none is found.
		 * </summary>
		 * <param name="name">The name of the `Broadcast` to get.
		 * </param>
		 */
		public static Broadcast GetBroadcast(string name)
			=> (broadcasts.ContainsKey(name)) ? broadcasts[name] : null;
		

		
		/**
		 * <summary>
		 * Registers a `Broadcast` (adds it to the dictionary).
		 * </summary>
		 * <param name="bc">The `Broadcast` to add.</param>
		 */
		internal static void register(Broadcast bc) => broadcasts.Add(bc.Name, bc);
		/**
		 * <summary>
		 * Returns a broadcast associated with `name`, creating it if it does not exist.
		 * </summary>
		 * <param name="name">The name of the `Broadcast`.</param>
		 */
		private static Broadcast getBroadcast(string name){
			Broadcast bc = GetBroadcast(name);
			if (bc == null) bc = new Broadcast(name);
			return bc;
		}
		
		/**
		 * <summary>Class which represents an event.</summary>
		 */
		public class Broadcast {
			/// <summary>The name of the Broadcast. (Read Only)</summary>
			public string Name { get; private set; }
			private List<Signal> signals;

			/**
			 * <summary>
			 * Creates a nameless, empty Broadcast and does not
			 * register it with the EventManager.
			 * </summary>
			 */
			public Broadcast(){
				signals = new List<Signal>();
			}
			/**
			 * <summary>
			 * Creates a new Broadcast.
			 * </summary>
			 * <param name="name">The name of the Broadcast, for EventManager registering.
			 * </param>
			 * <param name="unregistered">If this is true, the Broadcast will not be registered with
			 * the EventManager.
			 * </param>
			 */
			public Broadcast(string name, bool unregistered = false){
				Name = name;
				signals = new List<Signal>();
				if (!unregistered) EventManager.register(this);
			}
			/**
			 * <summary>
			 * Fires the Broadcast.
			 * </summary>
			 * <param name="args">Arguments to fire with.</param>
			 */
			public void Fire(params object[] args){
				for (int i = 0; i < signals.Count; i++)
					StartCoroutine(fire(signals[i], args));
			}
			/**
			 * <summary>
			 * Connects a function, and returns a `Signal`.
			 * </summary>
			 * <param name="func">A function to connect.
			 * </param>
			 */
			public Signal Connect(Handler func){
				Signal s = new Signal(func, this);
				signals.Add(s);
				return s;
			}
			/**
			 * <summary>
			 * Connects a function, and returns a `Signal`.
			 * </summary>
			 * <param name="func">A function to connect.
			 * </param>
			 */
			public Signal Connect(Action func){
				return Connect((args)=>func());
			}

			/**
			 * <summary>
			 * Connects a function to the Broadcast.
			 * </summary>
			 * <param name="bc">The Broadcast.
			 * </param>
			 * <param name="func">A function to connect.
			 * </param>
			 */
			public static Broadcast operator +(Broadcast bc, Handler func){
				bc.Connect(func);
				return bc;
			}
			/**
			 * <summary>
			 * Disconnects a function from the Broadcast.
			 * </summary>
			 * <param name="bc">The Broadcast.
			 * </param>
			 * <param name="func">The function to disconnect.
			 * </param>
			 */
			public static Broadcast operator -(Broadcast bc, Handler func){
				List<Signal> toRemove = new List<Signal>();
				foreach (Signal s in bc.signals)
					if (s.callback == func) toRemove.Add(s);
				foreach(Signal s in toRemove) bc.disconnect(s);
				
				return bc;
			}

			/**
			 * <summary>
			 * Disconnects a signal.
			 * </summary>
			 * <param name="s">The signal to disconnect.
			 * </param>
			 */
			internal void disconnect(Signal s){
				if (signals.Contains(s)) signals.Remove(s);
			}
			
			/**
			 * <summary>
			 * Fires a signal.
			 * </summary>
			 * <param name="sig">Signal to fire.
			 * </param>
			 * <param name="args">Arguments to fire with.</param>
			 */
			private IEnumerator fire(Signal sig, params object[] args){
				sig.call(args);
				yield return null;
			}
		}
		/**
		 * <summary>Class which contains callbacks.</summary>
		 */
		public class Signal {
			/// <summary>The Broadcast this Signal is connected to. (Read Only)</summary>
			public Broadcast Broadcast => broadcast;
			internal Handler callback {
				get {
					return cb;
				}
				set {}
			}

			private Broadcast broadcast;
			private Handler cb;
			
			/**
			 * <summary>
			 * Creates a Signal.
			 * </summary>
			 * <param name="func">A callback function to connect to the signal.
			 * </param>
			 * <param name="bc">The Broadcast the signal is being added to.</param>
			 */
			internal Signal(Handler func, Broadcast bc){
				cb = func;
				broadcast = bc;
			}
			/**
			 * <summary>Disconnects the Signal from its Broadcast.</summary>
			 */
			public void Disconnect() => broadcast?.disconnect(this);
			/**
			 * <summary>
			 * Calls the callback function.
			 * </summary>
			 * <param name="args">Arguments to fire the signal with.</param>
			 */
			internal void call(params object[] args) => callback(args);
		}
	}
}