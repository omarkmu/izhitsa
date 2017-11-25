using System;
using System.Collections.Generic;

namespace Izhitsa {
	namespace Events {
		namespace Generic {
			/**
			 <summary>Generic form of <see cref="Broadcast"/>.</summary>
			 */
			public class Broadcast<T> {
				/// <summary>The name of the Broadcast. (Read Only)</summary>
				public string Name { get; private set; }


				/// <summary>The connected signals to this Broadcast.</summary>
				private List<Signal<T>> signals = new List<Signal<T>>();
			

				/**
				 <summary>
				 Creates a nameless, empty Broadcast.
				 </summary>
				 */
				public Broadcast(){}
				/**
				 <summary>
				 Creates a new Broadcast.
				 </summary>
				 <param name="name">The name of the Broadcast, for <see cref="EventManager"/> registering.
				 </param>
				 <param name="unregistered">If this is true, the Broadcast will not be registered with
				 EventManager.
				 </param>
				 */
				public Broadcast(string name, bool unregistered = false){
					Name = name;
					if (!unregistered){
						if (EventManager.BroadcastExists(name))
							throw new ArgumentException($"{name} is already a registered Broadcast.", "name");
						EventManager.register<T>(this);
					}
				}


				/**
				 <summary>
				 Fires the Broadcast.
				 </summary>
				 */
				public void Fire(T arg0){
					for (int i = 0; i < signals.Count; i++)
						signals[i].call(arg0);
				}
				/**
				 <summary>
				 Connects an Action and returns a Signal.
				 </summary>
				 <param name="func">The Action to connect.
				 </param>
				 */
				public Signal<T> Connect(Action<T> func){
					Signal<T> s = new Signal<T>(func, this);
					signals.Add(s);
					return s;
				}

				/**
				 <summary>
				 Disconnects a <see cref="Signal"/>.
				 </summary>
				 <param name="sig">The Signal to disconnect.</param>
				 */
				internal void disconnect(Signal<T> sig){
					if (signals.Contains(sig)) signals.Remove(sig);
				}
			}
			/**
			 <summary>Generic form of <see cref="Broadcast"/>.</summary>
			 */
			public class Broadcast<T, T2> {
				/// <summary>The name of the Broadcast. (Read Only)</summary>
				public string Name { get; private set; }


				/// <summary>The connected signals to this Broadcast.</summary>
				private List<Signal<T, T2>> signals = new List<Signal<T, T2>>();
			

				/**
				 <summary>
				 Creates a nameless, empty Broadcast.
				 </summary>
				 */
				public Broadcast(){}
				/**
				 <summary>
				 Creates a new Broadcast.
				 </summary>
				 <param name="name">The name of the Broadcast, for <see cref="EventManager"/> registering.
				 </param>
				 <param name="unregistered">If this is true, the Broadcast will not be registered with
				 EventManager.
				 </param>
				 */
				public Broadcast(string name, bool unregistered = false){
					Name = name;
					if (!unregistered){
						if (EventManager.BroadcastExists(name))
							throw new ArgumentException($"{name} is already a registered Broadcast.", "name");
						EventManager.register<T, T2>(this);
					}
				}


				/**
				 <summary>
				 Fires the Broadcast.
				 </summary>
				 */
				public void Fire(T arg0, T2 arg1){
					for (int i = 0; i < signals.Count; i++)
						signals[i].call(arg0, arg1);
				}
				/**
				 <summary>
				 Connects an Action and returns a Signal.
				 </summary>
				 <param name="func">The Action to connect.
				 </param>
				 */
				public Signal<T, T2> Connect(Action<T, T2> func){
					Signal<T, T2> s = new Signal<T, T2>(func, this);
					signals.Add(s);
					return s;
				}

				/**
				 <summary>
				 Disconnects a <see cref="Signal"/>.
				 </summary>
				 <param name="sig">The Signal to disconnect.</param>
				 */
				internal void disconnect(Signal<T, T2> sig){
					if (signals.Contains(sig)) signals.Remove(sig);
				}
			}
			/**
			 <summary>Generic form of <see cref="Broadcast"/>.</summary>
			 */
			public class Broadcast<T, T2, T3> {
				/// <summary>The name of the Broadcast. (Read Only)</summary>
				public string Name { get; private set; }

				/// <summary>The connected signals to this Broadcast.</summary>
				private List<Signal<T, T2, T3>> signals = new List<Signal<T, T2, T3>>();
			

				/**
				 <summary>
				 Creates a nameless, empty Broadcast.
				 </summary>
				 */
				public Broadcast(){}
				/**
				 <summary>
				 Creates a new Broadcast.
				 </summary>
				 <param name="name">The name of the Broadcast, for <see cref="EventManager"/> registering.
				 </param>
				 <param name="unregistered">If this is true, the Broadcast will not be registered with
				 EventManager.
				 </param>
				 */
				public Broadcast(string name, bool unregistered = false){
					Name = name;
					if (!unregistered){
						if (EventManager.BroadcastExists(name))
							throw new ArgumentException($"{name} is already a registered Broadcast.", "name");
						EventManager.register<T, T2, T3>(this);
					}
				}


				/**
				 <summary>
				 Fires the Broadcast.
				 </summary>
				 */
				public void Fire(T arg0, T2 arg1, T3 arg2){
					for (int i = 0; i < signals.Count; i++)
						signals[i].call(arg0, arg1, arg2);
				}
				/**
				 <summary>
				 Connects an Action and returns a Signal.
				 </summary>
				 <param name="func">The Action to connect.
				 </param>
				 */
				public Signal<T, T2, T3> Connect(Action<T, T2, T3> func){
					Signal<T, T2, T3> s = new Signal<T, T2, T3>(func, this);
					signals.Add(s);
					return s;
				}

				/**
				 <summary>
				 Disconnects a <see cref="Signal"/>.
				 </summary>
				 <param name="sig">The Signal to disconnect.</param>
				 */
				internal void disconnect(Signal<T, T2, T3> sig){
					if (signals.Contains(sig)) signals.Remove(sig);
				}
			}
			/**
			 <summary>Generic form of <see cref="Broadcast"/>.</summary>
			 */
			public class Broadcast<T, T2, T3, T4> {
				/// <summary>The name of the Broadcast. (Read Only)</summary>
				public string Name { get; private set; }


				/// <summary>The connected signals to this Broadcast.</summary>
				private List<Signal<T, T2, T3, T4>> signals = new List<Signal<T, T2, T3, T4>>();
			

				/**
				 <summary>
				 Creates a nameless, empty Broadcast.
				 </summary>
				 */
				public Broadcast(){}
				/**
				 <summary>
				 Creates a new Broadcast.
				 </summary>
				 <param name="name">The name of the Broadcast, for <see cref="EventManager"/> registering.
				 </param>
				 <param name="unregistered">If this is true, the Broadcast will not be registered with
				 EventManager.
				 </param>
				 */
				public Broadcast(string name, bool unregistered = false){
					Name = name;
					if (!unregistered){
						if (EventManager.BroadcastExists(name))
							throw new ArgumentException($"{name} is already a registered Broadcast.", "name");
						EventManager.register<T, T2, T3, T4>(this);
					}
				}


				/**
				 <summary>
				 Fires the Broadcast.
				 </summary>
				 */
				public void Fire(T arg0, T2 arg1, T3 arg2, T4 arg3){
					for (int i = 0; i < signals.Count; i++)
						signals[i].call(arg0, arg1, arg2, arg3);
				}
				/**
				 <summary>
				 Connects an Action and returns a Signal.
				 </summary>
				 <param name="func">The Action to connect.
				 </param>
				 */
				public Signal<T, T2, T3, T4> Connect(Action<T, T2, T3, T4> func){
					Signal<T, T2, T3, T4> s = new Signal<T, T2, T3, T4>(func, this);
					signals.Add(s);
					return s;
				}

				/**
				 <summary>
				 Disconnects a <see cref="Signal"/>.
				 </summary>
				 <param name="sig">The Signal to disconnect.</param>
				 */
				internal void disconnect(Signal<T, T2, T3, T4> sig){
					if (signals.Contains(sig)) signals.Remove(sig);
				}
			}
		}
	}
}