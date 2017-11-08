using System;
using System.Collections;
using System.Collections.Generic;
using static Izhitsa.Utility;

namespace Izhitsa {
	public static partial class EventManager {
		/**
		 * <summary>Class which represents an event.</summary>
		 */
		public class Broadcast {
			/// <summary>The name of the Broadcast. (Read Only)</summary>
			public string Name { get; private set; }

			private List<Signal> signals = new List<Signal>();


			/**
			 * <summary>
			 * Creates a nameless, empty Broadcast and does not
			 * register it with the EventManager.
			 * </summary>
			 */
			public Broadcast(){}
			/**
			 * <summary>
			 * Creates a new Broadcast.
			 * </summary>
			 * <param name="name">The name of the Broadcast, for `<see cref="EventManager"/>` registering.
			 * </param>
			 * <param name="unregistered">If this is true, the Broadcast will not be registered with
			 * `<see cref="EventManager"/>`.
			 * </param>
			 */
			public Broadcast(string name, bool unregistered = false){
				Name = name;
				if (!unregistered) register(this);
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
			 * Connects a `<see cref="Handler"/>`, and returns a `<see cref="Signal"/>`.
			 * </summary>
			 * <param name="func">The `<see cref="Handler"/>` to connect.
			 * </param>
			 */
			public Signal Connect(Handler func){
				Signal s = new Signal(func, this);
				signals.Add(s);
				return s;
			}
			/**
			 * <summary>
			 * Connects an Action, and returns a `<see cref="Signal"/>`.
			 * </summary>
			 * <param name="func">The Action to connect.
			 * </param>
			 */
			public Signal Connect(Action func) => Connect((args) => func());
			
			/**
			 * <summary>
			 * Connects a `<see cref="Handler"/>` to the Broadcast.
			 * </summary>
			 * <param name="bc">The Broadcast.
			 * </param>
			 * <param name="func">The Handler to connect.
			 * </param>
			 */
			public static Broadcast operator +(Broadcast bc, Handler func){
				bc.Connect(func);
				return bc;
			}
			/**
			 * <summary>
			 * Disconnects a Handler from the Broadcast.
			 * </summary>
			 * <param name="bc">The Broadcast.
			 * </param>
			 * <param name="func">The Handler to disconnect.
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
			 * Disconnects a `<see cref="Signal"/>`.
			 * </summary>
			 * <param name="sig">The Signal to disconnect.</param>
			 */
			internal void disconnect(Signal sig){
				if (signals.Contains(sig)) signals.Remove(sig);
			}
			
			/**
			 * <summary>
			 * Fires a `<see cref="Signal"/>`.
			 * </summary>
			 * <param name="sig">`<see cref="Signal"/>` to fire.
			 * </param>
			 * <param name="args">Arguments to fire the `<see cref="Signal"/>` with.</param>
			 */
			private IEnumerator fire(Signal sig, params object[] args){
				sig.call(args);
				yield return null;
			}
		}
	}
}