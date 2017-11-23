using Izhitsa.Events;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Izhitsa {
	namespace InputManagement {
		public static partial class InputManager {
			/**
			 * <summary>
			 * An event which fires when a key is bound.
			 * </summary>
			 * <para>arg0: `{string}` The name of the key that was bound.
			 * </para>
			 * <para>arg1: `{KeyCode}` The KeyCode which was bound.
			 * </para>
			 */
			public static Broadcast KeyBound => keyBound;
			/**
			 * <summary>
			 * An event which fires when a key is pressed.
			 * </summary>
			 * <para>arg0: `{InputEvent}` The event that occurred.
			 * </para>
			 */
			public static Broadcast KeyEvent => keyEvent;
			/**
			 * <summary>
			 * An event which fires when a key is unbound.
			 * </summary>
			 * <para>arg0: `{string}` The name of the key that was unbound.
			 * </para>
			 * <para>arg1: `{KeyCode}` The previous KeyCode value of the binding.
			 * </para>
			 */
			public static Broadcast KeyUnbound => keyUnbound;
			/**
			 * <summary>
			 * An event which fires when a sequence is bound.
			 * </summary>
			 * <para>arg0: `{string}` The name of the key that was bound.
			 * </para>
			 * <para>arg1: `{Sequence}` The Sequence which was bound.
			 * </para>
			 */
			public static Broadcast SequenceBound => seqBound;
			/**
			 * <summary>
			 * An event which fires when a sequence is unbound.
			 * </summary>
			 * <para>arg0: `{string}` The name of the sequence that was unbound.
			 * </para>
			 * <para>arg1: `{Sequence}` The previous Sequence of the binding.
			 * </para>
			 */
			public static Broadcast SequenceUnbound => seqUnbound;

			/// <summary>Primary KeyBound event.</summary>
			private static Broadcast keyBound = new Broadcast();
			/// <summary>Contains KeyBound events.</summary>
			private static Dictionary<string, Broadcast> keyBoundEvents { get; }
				= new Dictionary<string, Broadcast>();
			/// <summary>Primary Key event.</summary>
			private static Broadcast keyEvent { get; } = new Broadcast();
			/// <summary>Contains KeyPressed events.</summary>
			private static Dictionary<KeyCode, Broadcast> keyEvents { get; }
				= new Dictionary<KeyCode, Broadcast>();
			/// <summary>Primary KeyUnbound event.</summary>
			private static Broadcast keyUnbound = new Broadcast();
			/// <summary>Contains KeyUnbound events.</summary>
			private static Dictionary<string, Broadcast> keyUnboundEvents { get; }
				= new Dictionary<string, Broadcast>();
			/// <summary>Contains mouse events.</summary>
			private static Dictionary<int, Broadcast> mouseEvents { get; }
				= new Dictionary<int, Broadcast>();
			/// <summary>Primary SequenceBound event.</summary>
			private static Broadcast seqBound { get; } = new Broadcast();
			/// <summary>Contains KeyBound events.</summary>
			private static Dictionary<string, Broadcast> seqBoundEvents { get; }
				= new Dictionary<string, Broadcast>();
			/// <summary>Contains sequence completion events.</summary>
			private static Dictionary<string, Broadcast> seqEvents { get; }
				= new Dictionary<string, Broadcast>();
			/// <summary>Primary SequenceUnbound event.</summary>
			private static Broadcast seqUnbound { get; } = new Broadcast();
			/// <summary>Contains SequenceUnbound events.</summary>
			private static Dictionary<string, Broadcast> seqUnboundEvents { get; }
				= new Dictionary<string, Broadcast>();
			///


			/**
			 * <summary>
			 * Connects a function to a Broadcast which fires when `<paramref name="action"/>` is bound
			 * to a key, and returns a Signal.
			 * </summary>
			 * <param name="action">The name of the bound key to connect to.
			 * </param>
			 * <param name="func">The function to connect.
			 * </param>
			 */
			public static Signal OnKeyBound(string action, Action func){
				if (!keyBoundEvents.ContainsKey(action))
					keyBoundEvents.Add(action, new Broadcast());
				Broadcast bc = keyBoundEvents[action];
				return bc.Connect(func);
			}
			/**
			 * <summary>
			 * Connects a function to a Broadcast which fires when `<paramref name="key"/>` is pressed,
			 * and returns a Signal.
			 * </summary>
			 * <param name="key">The key to listen for.
			 * </param>
			 * <param name="func">The function to connect.
			 * </param>
			 */
			public static Signal OnKeyEvent(KeyCode key, Action<InputEvent> func){
				if (!keyEvents.ContainsKey(key))
					keyEvents.Add(key, new Broadcast());
				Broadcast bc = keyEvents[key];
				return bc.Connect((args) => func((InputEvent)args[0]));
			}
			/**
			 * <summary>
			 * Connects a function to a Broadcast which fires when `<paramref name="action"/>` is bound
			 * to a key, and returns a Signal.
			 * </summary>
			 * <param name="action">The name of the bound key to connect to.
			 * </param>
			 * <param name="func">
			 * The function to connect.
			 * </param>
			 */
			public static Signal OnKeyUnbound(string action, Action func){
				if (!keyUnboundEvents.ContainsKey(action))
					keyUnboundEvents.Add(action, new Broadcast());
				Broadcast bc = keyUnboundEvents[action];
				return bc.Connect(func);
			}
			/**
			 * <summary>
			 * Connects a function to a Broadcast which fires when mouse button
			 * `<paramref name="button"/>` is interacted with, and returns a Signal.
			 * </summary>
			 * <param name="button">The mouse button.
			 * </param>
			 * <param name="func">
			 * The function to connect.
			 * </param>
			 */
			public static Signal OnMouse(int button, Action<InputEvent> func){
				if (!mouseEvents.ContainsKey(button))
					mouseEvents.Add(button, new Broadcast());
				Broadcast bc = mouseEvents[button];
				return bc.Connect((args) => func((InputEvent)args[0]));
			}
			/**
			 * <summary>
			 * Connects a function to a Broadcast which fires when a `Sequence`
			 * bound to the name `seqName` is completed successfully, and returns
			 * a `Signal`.
			 * </summary>
			 * <param name="seqName">The name of the sequence.
			 * </param>
			 * <param name="func">
			 * The function to connect.
			 * </param>
			 */
			public static Signal OnSequence(string seqName, Action func){
				if (!seqEvents.ContainsKey(seqName))
					seqEvents.Add(seqName, new Broadcast());
				Broadcast bc = seqEvents[seqName];
				return bc.Connect(func);
			}
			/**
			 * <summary>
			 * Connects a function to a Broadcast which fires when a `Sequence`
			 * bound to the name `seqName` is completed successfully, and returns
			 * a `Signal`.
			 * </summary>
			 * <param name="keyName">The name of the sequence.
			 * </param>
			 * <param name="func">
			 * The function to connect.
			 * </param>
			 */
			public static Signal OnSequence(string seqName, Action<object[]> func){
				if (!(seqEvents.ContainsKey(seqName)))
					seqEvents.Add(seqName, new Broadcast());
				Broadcast bc = seqEvents[seqName];
				return bc.Connect((args)=> func(args));
			}
		}
	}
}