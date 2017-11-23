using Izhitsa.Events;
using Izhitsa.Events.Generic;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Izhitsa {
	namespace InputManagement {
		public static partial class InputManager {
			/**
			 * <summary>
			 * An event which fires for any type of input.
			 * </summary>
			 * <param name="param0">The event that occurred.
			 * </param>
			 */
			private static Broadcast<InputEvent> input { get; } = new Broadcast<InputEvent>();
			/// <summary>Primary KeyBound event.</summary>
			private static Broadcast<string, KeyCode> keyBound { get; } = new Broadcast<string, KeyCode>();
			/// <summary>Contains KeyBound events.</summary>
			private static Dictionary<string, Broadcast<KeyCode>> keyBoundEvents { get; }
				= new Dictionary<string, Broadcast<KeyCode>>();
			/// <summary>Primary Key event.</summary>
			private static Broadcast<InputEvent> keyEvent { get; } = new Broadcast<InputEvent>();
			/// <summary>Contains Key events.</summary>
			private static Dictionary<KeyCode, Broadcast<InputEvent>> keyEvents { get; }
				= new Dictionary<KeyCode, Broadcast<InputEvent>>();
			/// <summary>Primary KeyUnbound event.</summary>
			private static Broadcast<string, KeyCode> keyUnbound { get; } = new Broadcast<string, KeyCode>();
			/// <summary>Contains KeyUnbound events.</summary>
			private static Dictionary<string, Broadcast<KeyCode>> keyUnboundEvents { get; }
				= new Dictionary<string, Broadcast<KeyCode>>();
			/// <summary>Primary mouse event.</summary>
			private static Broadcast<InputEvent> mouseEvent { get; }
				= new Broadcast<InputEvent>();
			/// <summary>Contains mouse events.</summary>
			private static Dictionary<int, Broadcast<InputEvent>> mouseEvents { get; }
				= new Dictionary<int, Broadcast<InputEvent>>();
			private static Broadcast<InputEvent> scrollEvent { get; }
				= new Broadcast<InputEvent>();
			/// <summary>Primary SequenceBound event.</summary>
			private static Broadcast<string, Sequence> seqBound { get; } = new Broadcast<string, Sequence>();
			/// <summary>Contains Sequence Bound events.</summary>
			private static Dictionary<string, Broadcast<Sequence>> seqBoundEvents { get; }
				= new Dictionary<string, Broadcast<Sequence>>();
			/// <summary>Contains sequence completion events.</summary>
			private static Dictionary<string, Broadcast> seqEvents { get; }
				= new Dictionary<string, Broadcast>();
			/// <summary>Primary SequenceUnbound event.</summary>
			private static Broadcast<string, Sequence> seqUnbound { get; } = new Broadcast<string, Sequence>();
			/// <summary>Contains SequenceUnbound events.</summary>
			private static Dictionary<string, Broadcast<Sequence>> seqUnboundEvents { get; }
				= new Dictionary<string, Broadcast<Sequence>>();
			///


			/**
			 * <summary>
			 * Connects `<paramref name="func"/>` to an event which fires on any input.
			 * </summary>
			 * <param name="param0">An InputEvent representing the input.
			 * </param>
			 */
			public static Signal<InputEvent> OnInput(Action<InputEvent> func)
				=> input.Connect(func);
			/**
			 * <summary>
			 * Connects an Action to a Broadcast which fires when `<paramref name="action"/>` is bound
			 * to a key, and returns a Signal.
			 * </summary>
			 * <param name="action">The name of the bound key to connect to.
			 * </param>
			 * <param name="func">The Action to connect.
			 * </param>
			 * <param name="param0">The KeyCode which was bound.
			 * </param>
			 */
			public static Signal<KeyCode> OnKeyBound(string action, Action<KeyCode> func){
				if (!keyBoundEvents.ContainsKey(action))
					keyBoundEvents.Add(action, new Broadcast<KeyCode>());
				Broadcast<KeyCode> bc = keyBoundEvents[action];
				return bc.Connect(func);
			}
			/**
			 * <summary>
			 * Connects `<paramref name="func"/>` to an event which fires when a key is bound.
			 * </summary>
			 * <param name="param0">The name of the key that was bound.
			 * </param>
			 * <param name="param1">The KeyCode which was bound.
			 * </param>
			 */
			public static Signal<string, KeyCode> OnKeyBound(Action<string, KeyCode> func)
				=> keyBound.Connect(func);
			/**
			 * <summary>
			 * Connects an Action to a Broadcast which fires when a key event happens on
			 * `<paramref name="key"/>`, and returns a Signal.
			 * </summary>
			 * <param name="key">The key to listen for.
			 * </param>
			 * <param name="func">The Action to connect.
			 * </param>
			 * <param name="param0">An InputEvent representing the input.
			 * </param>
			 */
			public static Signal<InputEvent> OnKeyEvent(KeyCode key, Action<InputEvent> func){
				if (!keyEvents.ContainsKey(key))
					keyEvents.Add(key, new Broadcast<InputEvent>());
				Broadcast<InputEvent> bc = keyEvents[key];
				return bc.Connect(func);
			}
			/**
			 * <summary>
			 * Connects `<paramref name="func"/>` to an event which fires on any key input.
			 * </summary>
			 * <param name="param0">An InputEvent representing the input.
			 * </param>
			 */
			public static Signal<InputEvent> OnKeyEvent(Action<InputEvent> func)
				=> keyEvent.Connect(func);
			/**
			 * <summary>
			 * Connects an Action to a Broadcast which fires when `<paramref name="action"/>` is bound
			 * to a key, and returns a Signal.
			 * </summary>
			 * <param name="action">The name of the bound key to connect to.
			 * </param>
			 * <param name="func">
			 * The Action to connect.
			 * </param>
			 * <param name="param0">The KeyCode that was bound previously.
			 * </param>
			 */
			public static Signal<KeyCode> OnKeyUnbound(string action, Action<KeyCode> func){
				if (!keyUnboundEvents.ContainsKey(action))
					keyUnboundEvents.Add(action, new Broadcast<KeyCode>());
				Broadcast<KeyCode> bc = keyUnboundEvents[action];
				return bc.Connect(func);
			}
			/**
			 * <summary>
			 * Connects an Action to a Broadcast which fires when an action is bound
			 * to a key, and returns a Signal.
			 * </summary>
			 * <param name="func">
			 * The Action to connect.
			 * </param>
			 * <param name="param0">The name of the key action which was unbound.
			 * </param>
			 * <param name="param1">The KeyCode that was bound previously.
			 * </param>
			 */
			public static Signal<string, KeyCode> OnKeyUnbound(Action<string, KeyCode> func)
				=> keyUnbound.Connect(func);
			/**
			 * <summary>
			 * Connects an Action to a Broadcast which fires when mouse button
			 * `<paramref name="button"/>` is interacted with, and returns a Signal.
			 * </summary>
			 * <param name="button">The mouse button.
			 * </param>
			 * <param name="func">
			 * The Action to connect.
			 * </param>
			 * <param name="param0">An InputEvent representing the input.
			 * </param>
			 */
			public static Signal<InputEvent> OnMouse(int button, Action<InputEvent> func){
				if (!mouseEvents.ContainsKey(button))
					mouseEvents.Add(button, new Broadcast<InputEvent>());
				Broadcast<InputEvent> bc = mouseEvents[button];
				return bc.Connect(func);
			}
			/**
			 * <summary>
			 * Connects an Action to a Broadcast which fires when a mouse button
			 * is interacted with, and returns a Signal.
			 * </summary>
			 * <param name="func">
			 * The Action to connect.
			 * </param>
			 * <param name="param0">An InputEvent representing the input.
			 * </param>
			 */
			public static Signal<InputEvent> OnMouse(Action<InputEvent> func)
				=> mouseEvent.Connect(func);
			/**
			 * <summary>
			 * Connects an Action to a Broadcast which fires when the mouse scroll wheel
			 * is interacted with, and returns a Signal.
			 * </summary>
			 * <param name="func">
			 * The Action to connect.
			 * </param>
			 * <param name="param0">An InputEvent representing the input.
			 * </param>
			 */
			public static Signal<InputEvent> OnScroll(Action<InputEvent> func)
				=> scrollEvent.Connect(func);
			/**
			 * <summary>
			 * Connects an Action to a Broadcast which fires when a `<see cref="InputManager.Sequence"/>`
			 * bound to the name `seqName` is completed successfully, and returns
			 * a `<see cref="Signal"/>`.
			 * </summary>
			 * <param name="seqName">The name of the sequence.
			 * </param>
			 * <param name="func">
			 * The Action to connect.
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
			 * Connects an Action to a Broadcast which fires when a `<see cref="InputManager.Sequence"/>`
			 * is bound to the name `seqName`, and returns a `<see cref="Signal"/>`.
			 * </summary>
			 * <param name="seqName">The name to listen for.
			 * </param>
			 * <param name="func">
			 * The Action to connect.
			 * </param>
			 */
			public static Signal<Sequence> OnSequenceBound(string seqName, Action<Sequence> func){
				if (!seqBoundEvents.ContainsKey(seqName))
					seqBoundEvents.Add(seqName, new Broadcast<Sequence>());
				Broadcast<Sequence> bc = seqBoundEvents[seqName];
				return bc.Connect(func);
			}
			/**
			 * <summary>
			 * Connects an Action to a Broadcast which fires when a `<see cref="InputManager.Sequence"/>`
			 * is bound, and returns a `<see cref="Signal"/>`.
			 * </summary>
			 * <param name="func">
			 * The Action to connect.
			 * </param>
			 * <param name="param0">The name of the sequence.
			 * </param>
			 * <param name="param1">The new Sequence.</param>
			 */
			public static Signal<string, Sequence> OnSequenceBound(Action<string, Sequence> func)
				=> seqBound.Connect(func);
			/**
			 * <summary>
			 * Connects an Action to a Broadcast which fires when a `<see cref="InputManager.Sequence"/>`
			 * is unbound from the name `seqName`, and returns a `<see cref="Signal"/>`.
			 * </summary>
			 * <param name="seqName">The name to listen for.
			 * </param>
			 * <param name="func">
			 * The Action to connect.
			 * </param>
			 * <param name="param0">The Sequence that was previously bound.</param>
			 */
			public static Signal<Sequence> OnSequenceUnbound(string seqName, Action<Sequence> func){
				if (!seqUnboundEvents.ContainsKey(seqName))
					seqUnboundEvents.Add(seqName, new Broadcast<Sequence>());
				Broadcast<Sequence> bc = seqUnboundEvents[seqName];
				return bc.Connect(func);
			}
			/**
			 * <summary>
			 * Connects an Action to a Broadcast which fires when a `<see cref="InputManager.Sequence"/>`
			 * is unbound, and returns a `<see cref="Signal"/>`.
			 * </summary>
			 * <param name="func">
			 * The Action to connect.
			 * </param>
			 * <param name="param0">The name of the sequence.
			 * </param>
			 * <param name="param1">TheSequence that was previously bound.</param>
			 */
			public static Signal<string, Sequence> OnSequenceUnbound(Action<string, Sequence> func)
				=> seqUnbound.Connect(func);
		}
	}
}