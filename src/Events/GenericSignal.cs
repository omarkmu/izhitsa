using System;

namespace Izhitsa {
	namespace Events {
		namespace Generic {
			/**
			 * <summary>
			 * Generic form of `<see cref="Signal"/>`.
			 * </summary>
			 */
			public class Signal<T> {
				/**
				 * <summary>
				 * The Broadcast that this Signal is connected to. (Read Only)
				 * </summary>
				 * <exception cref="MemberAccessException">
				 * Thrown if an attempt to access the member is made
				 * after the signal has been disconnected.
				 * </exception>
				 */
				public Broadcast<T> Broadcast {
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
				internal Action<T> callback { get; set; }

				/// <summary>The Broadcast this Signal is connected to.</summary>
				private Broadcast<T> broadcast;
				
				
				/**
				 * <summary>
				 * Creates a Signal.
				 * </summary>
				 * <param name="func">A callback function to connect to the Signal.
				 * </param>
				 * <param name="bc">The Broadcast
				 * that the Signal is being added to.</param>
				 */
				internal Signal(Action<T> func, Broadcast<T> bc){
					callback = func;
					broadcast = bc;
				}

				/**
				 * <summary>
				 * Disconnects the Signal from its Broadcast.
				 * </summary>
				 * <exception cref="MethodAccessException">
				 * Thrown if the Signal is already disconnected.
				 * </exception>
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
				 * <summary>
				 * Calls the callback function.
				 * </summary>
				 * <param name="args">Arguments to fire the Signal with.</param>
				 */
				internal void call(T arg) {
					if (callback == null) return;
					callback(arg);
				}
			}
			/**
			 * <summary>
			 * Generic form of `<see cref="Signal"/>`.
			 * </summary>
			 */
			public class Signal<T, T2> {
				/**
				 * <summary>
				 * The Broadcast that this Signal is connected to. (Read Only)
				 * </summary>
				 * <exception cref="MemberAccessException">
				 * Thrown if an attempt to access the member is made
				 * after the signal has been disconnected.
				 * </exception>
				 */
				public Broadcast<T, T2> Broadcast {
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
				internal Action<T, T2> callback { get; set; }

				/// <summary>The Broadcast this Signal is connected to.</summary>
				private Broadcast<T, T2> broadcast;
				
				
				/**
				 * <summary>
				 * Creates a Signal.
				 * </summary>
				 * <param name="func">A callback function to connect to the Signal.
				 * </param>
				 * <param name="bc">The Broadcast
				 * that the Signal is being added to.</param>
				 */
				internal Signal(Action<T, T2> func, Broadcast<T, T2> bc){
					callback = func;
					broadcast = bc;
				}

				/**
				 * <summary>
				 * Disconnects the Signal from its Broadcast.
				 * </summary>
				 * <exception cref="MethodAccessException">
				 * Thrown if the Signal is already disconnected.
				 * </exception>
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
				 * <summary>
				 * Calls the callback function.
				 * </summary>
				 * <param name="args">Arguments to fire the Signal with.</param>
				 */
				internal void call(T arg0, T2 arg1) {
					if (callback == null) return;
					callback(arg0, arg1);
				}
			}
			/**
			 * <summary>
			 * Generic form of `<see cref="Signal"/>`.
			 * </summary>
			 */
			public class Signal<T, T2, T3> {
				/**
				 * <summary>
				 * The Broadcast that this Signal is connected to. (Read Only)
				 * </summary>
				 * <exception cref="MemberAccessException">
				 * Thrown if an attempt to access the member is made
				 * after the signal has been disconnected.
				 * </exception>
				 */
				public Broadcast<T, T2, T3> Broadcast {
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
				internal Action<T, T2, T3> callback { get; set; }

				/// <summary>The Broadcast this Signal is connected to.</summary>
				private Broadcast<T, T2, T3> broadcast;
				
				
				/**
				 * <summary>
				 * Creates a Signal.
				 * </summary>
				 * <param name="func">A callback function to connect to the Signal.
				 * </param>
				 * <param name="bc">The Broadcast
				 * that the Signal is being added to.</param>
				 */
				internal Signal(Action<T, T2, T3> func, Broadcast<T, T2, T3> bc){
					callback = func;
					broadcast = bc;
				}

				/**
				 * <summary>
				 * Disconnects the Signal from its Broadcast.
				 * </summary>
				 * <exception cref="MethodAccessException">
				 * Thrown if the Signal is already disconnected.
				 * </exception>
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
				 * <summary>
				 * Calls the callback function.
				 * </summary>
				 * <param name="args">Arguments to fire the Signal with.</param>
				 */
				internal void call(T arg0, T2 arg1, T3 arg2) {
					if (callback == null) return;
					callback(arg0, arg1, arg2);
				}
			}
			/**
			 * <summary>
			 * Generic form of `<see cref="Signal"/>`.
			 * </summary>
			 */
			public class Signal<T, T2, T3, T4> {
				/**
				 * <summary>
				 * The Broadcast that this Signal is connected to. (Read Only)
				 * </summary>
				 * <exception cref="MemberAccessException">
				 * Thrown if an attempt to access the member is made
				 * after the signal has been disconnected.
				 * </exception>
				 */
				public Broadcast<T, T2, T3, T4> Broadcast {
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
				internal Action<T, T2, T3, T4> callback { get; set; }

				/// <summary>The Broadcast this Signal is connected to.</summary>
				private Broadcast<T, T2, T3, T4> broadcast;
				
				
				/**
				 * <summary>
				 * Creates a Signal.
				 * </summary>
				 * <param name="func">A callback function to connect to the Signal.
				 * </param>
				 * <param name="bc">The Broadcast
				 * that the Signal is being added to.</param>
				 */
				internal Signal(Action<T, T2, T3, T4> func, Broadcast<T, T2, T3, T4> bc){
					callback = func;
					broadcast = bc;
				}

				/**
				 * <summary>
				 * Disconnects the Signal from its Broadcast.
				 * </summary>
				 * <exception cref="MethodAccessException">
				 * Thrown if the Signal is already disconnected.
				 * </exception>
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
				 * <summary>
				 * Calls the callback function.
				 * </summary>
				 * <param name="args">Arguments to fire the Signal with.</param>
				 */
				internal void call(T arg0, T2 arg1, T3 arg2, T4 arg3) {
					if (callback == null) return;
					callback(arg0, arg1, arg2, arg3);
				}
			}
		}
	}
}
