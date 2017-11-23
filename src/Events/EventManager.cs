using System;
using System.Collections.Generic;
using Izhitsa.Events.Generic;
using UnityEngine;

namespace Izhitsa {
	namespace Events {
		/**
		 * <summary>Class which facilitates dynamic event creation and usage.</summary>
		 */
		public static class EventManager {
			/// <summary>A container for all broadcasts.</summary>
			private static Dictionary<string, object> broadcasts = new Dictionary<string, object>();

			/// <summary>Returns true if a Broadcast with the name `<paramref name="name"/>` exists.</summary>
			public static bool BroadcastExists(string name) => broadcasts.ContainsKey(name);

			/**
			 * <summary>
			 * Connects a function to a `<see cref="Broadcast"/>` and returns a `<see cref="Signal"/>`.
			 * </summary>
			 * <param name="name">The name of the Broadcast to connect to. 
			 * </param>
			 * <param name="func">An Action to connect.
			 * </param>
			 */
			public static Signal Connect(string name, Action func)
				=> getBroadcast(name).Connect(func);
			/**
			 * <summary>
			 * Connects a function to a `<see cref="Broadcast"/>` and returns a `<see cref="Signal"/>`.
			 * </summary>
			 * <param name="name">The name of the Broadcast to connect to.
			 * </param>
			 * <param name="func">An Action to connect.
			 * </param>
			 */
			public static Signal Connect(string name, Action<object[]> func)
				=> getBroadcast(name).Connect(func);
			/**
			 * <summary>
			 * Connects a function to a Broadcast and returns a Signal.
			 * </summary>
			 * <param name="name">The name of the Broadcast to connect to.
			 * </param>
			 * <param name="func">An Action to connect.
			 * </param>
			 */
			public static Signal<T> Connect<T>(string name, Action<T> func)
				=> getBroadcast<T>(name).Connect(func);
			/**
			 * <summary>
			 * Connects a function to a Broadcast and returns a Signal.
			 * </summary>
			 * <param name="name">The name of the Broadcast to connect to.
			 * </param>
			 * <param name="func">An Action to connect.
			 * </param>
			 */
			public static Signal<T, T2> Connect<T, T2>(string name, Action<T, T2> func)
				=> getBroadcast<T, T2>(name).Connect(func);
			/**
			 * <summary>
			 * Connects a function to a Broadcast and returns a Signal.
			 * </summary>
			 * <param name="name">The name of the Broadcast to connect to.
			 * </param>
			 * <param name="func">An Action to connect.
			 * </param>
			 */
			public static Signal<T, T2, T3> Connect<T, T2, T3>(string name, Action<T, T2, T3> func)
				=> getBroadcast<T, T2, T3>(name).Connect(func);
			/**
			 * <summary>
			 * Connects a function to a Broadcast and returns a Signal.
			 * </summary>
			 * <param name="name">The name of the Broadcast to connect to.
			 * </param>
			 * <param name="func">An Action to connect.
			 * </param>
			 */
			public static Signal<T, T2, T3, T4> Connect<T, T2, T3, T4>(string name, Action<T, T2, T3, T4> func)
				=> getBroadcast<T, T2, T3, T4>(name).Connect(func);
			///

			/**
			 * <summary>
			 * Fires the `<see cref="Broadcast"/>` which matches the `<paramref name="name"/>`.
			 * </summary>
			 * <param name="name">The name of the Broadcast to fire.
			 * </param>
			 * <param name="args">The arguments to fire the Broadcast with.
			 * </param>
			 */
			public static void Fire(string name, params object[] args){
				Broadcast bc = GetBroadcast(name);
				if (bc != null)
					bc.Fire(args);
			}
			/**
			 * <summary>
			 * Fires the Broadcast which matches the `<paramref name="name"/>`.
			 * </summary>
			 * <param name="name">The name of the Broadcast to fire.
			 * </param>
			 */
			public static void Fire<T>(string name, T arg){
				Broadcast<T> bc = GetBroadcast<T>(name);
				if (bc != null)
					bc.Fire(arg);
			}
			/**
			 * <summary>
			 * Fires the Broadcast which matches the `<paramref name="name"/>`.
			 * </summary>
			 * <param name="name">The name of the Broadcast to fire.
			 * </param>
			 */
			public static void Fire<T, T2>(string name, T arg0, T2 arg1){
				Broadcast<T, T2> bc = GetBroadcast<T, T2>(name);
				if (bc != null)
					bc.Fire(arg0, arg1);
			}
			/**
			 * <summary>
			 * Fires the Broadcast which matches the `<paramref name="name"/>`.
			 * </summary>
			 * <param name="name">The name of the Broadcast to fire.
			 * </param>
			 */
			public static void Fire<T, T2, T3>(string name, T arg0, T2 arg1, T3 arg2){
				Broadcast<T, T2, T3> bc = GetBroadcast<T, T2, T3>(name);
				if (bc != null)
					bc.Fire(arg0, arg1, arg2);
			}
			/**
			 * <summary>
			 * Fires the Broadcast which matches the `<paramref name="name"/>`.
			 * </summary>
			 * <param name="name">The name of the Broadcast to fire.
			 * </param>
			 */
			public static void Fire<T, T2, T3, T4>(string name, T arg0, T2 arg1, T3 arg2, T4 arg3){
				Broadcast<T, T2, T3, T4> bc = GetBroadcast<T, T2, T3, T4>(name);
				if (bc != null)
					bc.Fire(arg0, arg1, arg2, arg3);
			}

			/**
			 * <summary>
			 * Returns the `<see cref="Broadcast"/>` with the name
			 * `<paramref name="name"/>`, or null if it doesn't exist or it 
			 * is the wrong type.
			 * </summary>
			 * <param name="name">The name of the Broadcast to get.
			 * </param>
			 */
			public static Broadcast GetBroadcast(string name){
				if (broadcasts.ContainsKey(name)){
					object bc = broadcasts[name];
					if (bc is Broadcast) return (Broadcast)bc;
					Debug.LogWarning("GetBroadcast called with name pointing to incorrect type.");
				}
				return null;
			}
			/**
			 * <summary>
			 * Returns the Broadcast with the name
			 * `<paramref name="name"/>`, or null if it doesn't exist or it 
			 * is the wrong type.
			 * </summary>
			 * <param name="name">The name of the Broadcast to get.
			 * </param>
			 */
			public static Broadcast<T> GetBroadcast<T>(string name){
				if (broadcasts.ContainsKey(name)){
					object bc = broadcasts[name];
					if (bc is Broadcast<T>) return (Broadcast<T>)bc;
					Debug.LogWarning("GetBroadcast<T> called with name pointing to incorrect type.");
				}
				return null;
			}
			/**
			 * <summary>
			 * Returns the Broadcast with the name
			 * `<paramref name="name"/>`, or null if it doesn't exist or it 
			 * is the wrong type.
			 * </summary>
			 * <param name="name">The name of the Broadcast to get.
			 * </param>
			 */
			public static Broadcast<T, T2> GetBroadcast<T, T2>(string name){
				if (broadcasts.ContainsKey(name)){
					object bc = broadcasts[name];
					if (bc is Broadcast<T, T2>) return (Broadcast<T, T2>)bc;
					Debug.LogWarning("GetBroadcast<T, T2> called with name pointing to incorrect type.");
				}
				return null;
			}
			/**
			 * <summary>
			 * Returns the Broadcast with the name
			 * `<paramref name="name"/>`, or null if it doesn't exist or it 
			 * is the wrong type.
			 * </summary>
			 * <param name="name">The name of the Broadcast to get.
			 * </param>
			 */
			public static Broadcast<T, T2, T3> GetBroadcast<T, T2, T3>(string name){
				if (broadcasts.ContainsKey(name)){
					object bc = broadcasts[name];
					if (bc is Broadcast<T, T2, T3>) return (Broadcast<T, T2, T3>)bc;
					Debug.LogWarning("GetBroadcast<T, T2, T3> called with name pointing to incorrect type.");
				}
				return null;
			}
			/**
			 * <summary>
			 * Returns the Broadcast with the name
			 * `<paramref name="name"/>`, or null if it doesn't exist or it 
			 * is the wrong type.
			 * </summary>
			 * <param name="name">The name of the Broadcast to get.
			 * </param>
			 */
			public static Broadcast<T, T2, T3, T4> GetBroadcast<T, T2, T3, T4>(string name){
				if (broadcasts.ContainsKey(name)){
					object bc = broadcasts[name];
					if (bc is Broadcast<T, T2, T3, T4>) return (Broadcast<T, T2, T3, T4>)bc;
					Debug.LogWarning("GetBroadcast<T, T2, T3, T4> called with name pointing to incorrect type.");
				}
				return null;
			}

			/**
			 * <summary>
			 * Registers a `<see cref="Broadcast"/>` (adds it to the dictionary).
			 * </summary>
			 * <param name="bc">The Broadcast to add.</param>
			 */
			internal static void register(Broadcast bc) => broadcasts.Add(bc.Name, bc);
			/**
			 * <summary>
			 * Registers a Broadcast (adds it to the dictionary).
			 * </summary>
			 * <param name="bc">The Broadcast to add.</param>
			 */
			internal static void register<T>(Broadcast<T> bc) => broadcasts.Add(bc.Name, bc);
			/**
			 * <summary>
			 * Registers a Broadcast (adds it to the dictionary).
			 * </summary>
			 * <param name="bc">The Broadcast to add.</param>
			 */
			internal static void register<T, T2>(Broadcast<T, T2> bc) => broadcasts.Add(bc.Name, bc);
			/**
			 * <summary>
			 * Registers a Broadcast (adds it to the dictionary).
			 * </summary>
			 * <param name="bc">The Broadcast to add.</param>
			 */
			internal static void register<T, T2, T3>(Broadcast<T, T2, T3> bc) => broadcasts.Add(bc.Name, bc);
			/**
			 * <summary>
			 * Registers a Broadcast (adds it to the dictionary).
			 * </summary>
			 * <param name="bc">The Broadcast to add.</param>
			 */
			internal static void register<T, T2, T3,T4>(Broadcast<T, T2, T3,T4> bc) => broadcasts.Add(bc.Name, bc);

			/**
			 * <summary>
			 * Returns a `<see cref="Broadcast"/>` associated with `<paramref name="name"/>`, creating it if it does not exist.
			 * </summary>
			 * <param name="name">The name of the Broadcast to get.</param>
			 */
			private static Broadcast getBroadcast(string name){
				object bc = GetBroadcast(name);
				if (bc == null){
					 bc = new Broadcast(name);
					 return bc as Broadcast;
				} else {
					if (bc is Broadcast) return bc as Broadcast;
					throw new ArgumentException(
						"Attempt to connect Broadcast with non-matching type.");
				}
			}
			/**
			 * <summary>
			 * Returns a Broadcast associated with `<paramref name="name"/>`, creating it if it does not exist.
			 * </summary>
			 * <param name="name">The name of the Broadcast to get.</param>
			 */
			private static Broadcast<T> getBroadcast<T>(string name){
				object bc = GetBroadcast(name);
				if (bc == null){
					 bc = new Broadcast<T>(name);
					 return bc as Broadcast<T>;
				} else {
					if (bc is Broadcast<T>) return bc as Broadcast<T>;
					throw new ArgumentException(
						"Attempt to connect Broadcast<T> with non-matching type.");
				}
			}
			/**
			 * <summary>
			 * Returns a Broadcast associated with `<paramref name="name"/>`, creating it if it does not exist.
			 * </summary>
			 * <param name="name">The name of the Broadcast to get.</param>
			 */
			private static Broadcast<T, T2> getBroadcast<T, T2>(string name){
				object bc = GetBroadcast(name);
				if (bc == null){
					 bc = new Broadcast<T, T2>(name);
					 return bc as Broadcast<T, T2>;
				} else {
					if (bc is Broadcast<T, T2>) return bc as Broadcast<T, T2>;
					throw new ArgumentException(
						"Attempt to connect Broadcast<T, T2> with non-matching type.");
				}
			}
			/**
			 * <summary>
			 * Returns a Broadcast associated with `<paramref name="name"/>`, creating it if it does not exist.
			 * </summary>
			 * <param name="name">The name of the Broadcast to get.</param>
			 */
			private static Broadcast<T, T2, T3> getBroadcast<T, T2, T3>(string name){
				object bc = GetBroadcast(name);
				if (bc == null){
					 bc = new Broadcast<T, T2, T3>(name);
					 return bc as Broadcast<T, T2, T3>;
				} else {
					if (bc is Broadcast<T, T2, T3>) return bc as Broadcast<T, T2, T3>;
					throw new ArgumentException(
						"Attempt to connect Broadcast<T, T2, T3> with non-matching type.");
				}
			}
			/**
			 * <summary>
			 * Returns a Broadcast associated with `<paramref name="name"/>`, creating it if it does not exist.
			 * </summary>
			 * <param name="name">The name of the Broadcast to get.</param>
			 */
			private static Broadcast<T, T2, T3, T4> getBroadcast<T, T2, T3, T4>(string name){
				object bc = GetBroadcast(name);
				if (bc == null){
					 bc = new Broadcast<T, T2, T3, T4>(name);
					 return bc as Broadcast<T, T2, T3, T4>;
				} else {
					if (bc is Broadcast<T, T2, T3, T4>) return bc as Broadcast<T, T2, T3, T4>;
					throw new ArgumentException(
						"Attempt to connect Broadcast<T, T2, T3, T4> with non-matching type.");
				}
			}
		}
	}
}
