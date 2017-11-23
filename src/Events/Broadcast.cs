using static Izhitsa.Tasks.TaskUtils;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Izhitsa {
	namespace Events {
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
			 * EventManager.
			 * </param>
			 */
			public Broadcast(string name, bool unregistered = false){
				Name = name;
				if (!unregistered){
					if (EventManager.BroadcastExists(name))
						throw new ArgumentException($"{name} is already a registered Broadcast.", "name");
					EventManager.register(this);
				}
			}
			
			/**
			 * <summary>
			 * Fires the Broadcast.
			 * </summary>
			 * <param name="args">Arguments to fire with.</param>
			 */
			public void Fire(params object[] args){
				for (int i = 0; i < signals.Count; i++)
					signals[i].call(args);
			}
			/**
			 * <summary>
			 * Connects an Action and returns a `<see cref="Signal"/>`.
			 * </summary>
			 * <param name="func">The Action to connect.
			 * </param>
			 */
			public Signal Connect(Action<object[]> func){
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
			 * Connects an Action to the Broadcast.
			 * </summary>
			 * <param name="bc">The Broadcast.
			 * </param>
			 * <param name="func">The Action to connect.
			 * </param>
			 */
			public static Signal operator +(Broadcast bc, Action<object[]> func){
				return bc.Connect(func);
			}
			/**
			 * <summary>
			 * Disconnects an Action from the Broadcast.
			 * </summary>
			 * <param name="bc">The Broadcast.
			 * </param>
			 * <param name="func">The Action to disconnect.
			 * </param>
			 */
			public static bool operator -(Broadcast bc, Action<object[]> func){
				bool found = false;
				List<Signal> toRemove = new List<Signal>();
				foreach (Signal s in bc.signals)
					if (s.callback == func) toRemove.Add(s);
				foreach (Signal s in toRemove){
					found = true;
					bc.disconnect(s);
				}

				return found;
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
		}
	}
}