using System;
using System.Collections.Generic;

namespace Izhitsa {
	/**
	 * <summary>Class which facilitates event creation &amp; usage.</summary>
	 */
    public static partial class EventManager {
		/// <summary>A container for all broadcasts.</summary>
        private static Dictionary<string, Broadcast> broadcasts = new Dictionary<string, Broadcast>();
		
		
		/// <summary>Delegate for handling event connections.</summary>
		public delegate void Handler(object[] args);


		/**
		 * <summary>
		 * Connects a function to a `<see cref="Broadcast"/>` and returns a `<see cref="Signal"/>`.
		 * </summary>
		 * <param name="name">The name of the `<see cref="Broadcast"/>` to connect to. 
		 * </param>
		 * <param name="func">An Action to connect.
		 * </param>
		 */
		public static Signal Connect(string name, Action func)
			=> getBroadcast(name).Connect((args) => func());
		/**
		 * <summary>
		 * Connects a function to a `<see cref="Broadcast"/>`, and returns a `<see cref="Signal"/>`.
		 * </summary>
		 * <param name="name">The name of the `<see cref="Broadcast"/>` to connect to.
		 * </param>
		 * <param name="func">An Action to connect.
		 * </param>
		 */
		public static Signal Connect(string name, Action<object[]> func)
			=> getBroadcast(name).Connect((args) => func(args));
		/**
		 * <summary>
		 * Fires the `<see cref="Broadcast"/>` which matches the `<paramref name="name"/>`.
		 * </summary>
		 * <param name="name">The name of the `<see cref="Broadcast"/>` to fire.
		 * </param>
		 * <param name="args">The arguments to fire the `<see cref="Broadcast"/>` with.
		 * </param>
		 */
        public static void Fire(string name, params object[] args){
            if (!broadcasts.ContainsKey(name)) return;
            broadcasts[name].Fire(args);
        }
		/**
		 * <summary>
		 * Returns the `<see cref="Broadcast"/>` with the name
		 * `<paramref name="name"/>`, or `null` if none is found.
		 * </summary>
		 * <param name="name">The name of the `<see cref="Broadcast"/>` to get.
		 * </param>
		 */
		public static Broadcast GetBroadcast(string name)
			=> (broadcasts.ContainsKey(name)) ? broadcasts[name] : null;
		
		/**
		 * <summary>
		 * Registers a `<see cref="Broadcast"/>` (adds it to the dictionary).
		 * </summary>
		 * <param name="bc">The `<see cref="Broadcast"/>` to add.</param>
		 */
		internal static void register(Broadcast bc) => broadcasts.Add(bc.Name, bc);
		
		/**
		 * <summary>
		 * Returns a broadcast associated with `<paramref name="name"/>`, creating it if it does not exist.
		 * </summary>
		 * <param name="name">The name of the `<see cref="Broadcast"/>` to get.</param>
		 */
		private static Broadcast getBroadcast(string name){
			Broadcast bc = GetBroadcast(name);
			if (bc == null) bc = new Broadcast(name);
			return bc;
		}
	}
}