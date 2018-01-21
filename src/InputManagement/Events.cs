using Izhitsa.Events;
using Izhitsa.Events.Generic;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Izhitsa.InputManagement {
	public static partial class InputManager {
		/// <summary>Main input event.</summary>
		private static Broadcast<InputEvent> input { get; } = new Broadcast<InputEvent>();
		/// <summary>Contains Key events tied to actions.</summary>
		private static Dictionary<string, Broadcast<InputEvent>> keyActionEvents { get; }
			= new Dictionary<string, Broadcast<InputEvent>>();
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
		/// <summary>Contains mouse events tied to actions.</summary>
		private static Dictionary<string, Broadcast<InputEvent>> mouseActionEvents { get; }
			= new Dictionary<string, Broadcast<InputEvent>>();
		/// <summary>Primary mouse bound event.</summary>
		private static Broadcast<string, int> mouseBound { get; } = new Broadcast<string, int>();
		/// <summary>Contains mouse bound events.</summary>
		private static Dictionary<string, Broadcast<int>> mouseBoundEvents { get; }
			= new Dictionary<string, Broadcast<int>>();
		/// <summary>Primary mouse event.</summary>
		private static Broadcast<InputEvent> mouseEvent { get; }
			= new Broadcast<InputEvent>();
		/// <summary>Contains mouse events.</summary>
		private static Dictionary<int, Broadcast<InputEvent>> mouseEvents { get; }
			= new Dictionary<int, Broadcast<InputEvent>>();
		/// <summary>Mouse move event.</summary>
		private static Broadcast<InputEvent> mouseMove { get; } = new Broadcast<InputEvent>();
		/// <summary>Primary mouse unbound event.</summary>
		private static Broadcast<string, int> mouseUnbound { get; } = new Broadcast<string, int>();
		/// <summary> Contains mouse unbound events.</summary>
		private static Dictionary<string, Broadcast<int>> mouseUnboundEvents { get; }
			= new Dictionary<string, Broadcast<int>>();
		/// <summary>Mouse scroll event.</summary>
		private static Broadcast<InputEvent> scrollEvent { get; }
			= new Broadcast<InputEvent>();
		/// <summary>Primary SequenceBound event.</summary>
		private static Broadcast<string, Sequence> seqBound { get; } = new Broadcast<string, Sequence>();
		/// <summary>Contains Sequence Bound events.</summary>
		private static Dictionary<string, Broadcast<Sequence>> seqBoundEvents { get; }
			= new Dictionary<string, Broadcast<Sequence>>();
		/// <summary>Contains sequence completion events.</summary>
		private static Dictionary<string, Broadcast<Sequence>> seqEvents { get; }
			= new Dictionary<string, Broadcast<Sequence>>();
		/// <summary>Primary SequenceUnbound event.</summary>
		private static Broadcast<string, Sequence> seqUnbound { get; } = new Broadcast<string, Sequence>();
		/// <summary>Contains SequenceUnbound events.</summary>
		private static Dictionary<string, Broadcast<Sequence>> seqUnboundEvents { get; }
			= new Dictionary<string, Broadcast<Sequence>>();
		/// <summary>Contains events that fire for all input types bound to an action.</summary>
		private static Dictionary<string, Broadcast<InputEvent, Sequence>> universalEvents { get; }
			= new Dictionary<string, Broadcast<InputEvent, Sequence>>();
		///


		/**
		 <summary>
		 Connects <paramref name="func"/> to an event which fires for all input types
		 bound to <paramref name="action"/>.
		 </summary>
		 <param name="action">The action string to listen for.
		 </param>
		 <param name="func">The Action to connect.
		 </param>
		 <para>Argument 0: An InputEvent representing the input. If the callback is called as a result of
		 sequence completion, this will be the last InputEvent in the sequence.
		 </para>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> or <paramref name="func"/> is null.
		 </exception>
		 */
		public static Signal<InputEvent, Sequence> OnAction(string action, Action<InputEvent> func){
			if (action == null)
				throw new ArgumentNullException("action");
			if (func == null)
				throw new ArgumentNullException("func");
			if (!universalEvents.ContainsKey(action))
				universalEvents.Add(action, new Broadcast<InputEvent, Sequence>());
			Broadcast<InputEvent, Sequence> bc = universalEvents[action];
			return bc.Connect((ev, _) => func(ev));
		}
		/**
		 <summary>
		 Connects <paramref name="func"/> to an event which fires for all input types
		 bound to <paramref name="action"/>.
		 </summary>
		 <param name="action">The action string to listen for.
		 </param>
		 <param name="func">The Action to connect.
		 </param>
		 <para>Argument 0: An InputEvent representing the input. If the callback is called as a result of
		 sequence completion, this will be the last InputEvent in the sequence.
		 </para>
		 <para>Argument 1: The completed Sequence, or null if the callback is not being called as a result of
		 sequence completion.
		 </para>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> or <paramref name="func"/> is null.
		 </exception>
		 */
		public static Signal<InputEvent, Sequence> OnAction(string action, Action<InputEvent, Sequence> func){
			if (action == null)
				throw new ArgumentNullException("action");
			if (func == null)
				throw new ArgumentNullException("func");
			if (!universalEvents.ContainsKey(action))
				universalEvents.Add(action, new Broadcast<InputEvent, Sequence>());
			Broadcast<InputEvent, Sequence> bc = universalEvents[action];
			return bc.Connect(func);
		}
		/**
		 <summary>
		 Connects <paramref name="func"/> to an event which fires on any input.
		 </summary>
		 <param name="func">The Action to connect.
		 </param>
		 <para>Argument 0: An InputEvent representing the input.
		 </para>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="func"/> is null.
		 </exception>
		 */
		public static Signal<InputEvent> OnInput(Action<InputEvent> func){
			if (func == null)
				throw new ArgumentNullException("func");
			return input.Connect(func);
		}
		/**
		 <summary>
		 Connects an Action to a Broadcast which fires when a key event happens on a key bound to
		 <paramref name="action"/>, and returns a Signal.
		 </summary>
		 <param name="action">The action string to listen for.
		 </param>
		 <param name="func">The Action to connect.
		 </param>
		 <para>Argument 0: An InputEvent representing the input.
		 </para>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> or <paramref name="func"/> is null.
		 </exception>
		 */
		public static Signal<InputEvent> OnKey(string action, Action<InputEvent> func){
			if (action == null)
				throw new ArgumentNullException("action");
			if (func == null)
				throw new ArgumentNullException("func");
			if (!keyActionEvents.ContainsKey(action))
				keyActionEvents.Add(action, new Broadcast<InputEvent>());
			Broadcast<InputEvent> bc = keyActionEvents[action];
			return bc.Connect(func);
		}
		/**
		 <summary>
		 Connects an Action to a Broadcast which fires when a key event happens on
		 <paramref name="key"/>, and returns a Signal.
		 </summary>
		 <param name="key">The key to listen for.
		 </param>
		 <param name="func">The Action to connect.
		 </param>
		 <para>Argument 0: An InputEvent representing the input.
		 </para>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="func"/> is null.
		 </exception>
		 */
		public static Signal<InputEvent> OnKey(KeyCode key, Action<InputEvent> func){
			if (func == null)
				throw new ArgumentNullException("func");
			if (!keyEvents.ContainsKey(key))
				keyEvents.Add(key, new Broadcast<InputEvent>());
			Broadcast<InputEvent> bc = keyEvents[key];
			return bc.Connect(func);
		}
		/**
		 <summary>
		 Connects <paramref name="func"/> to an event which fires on any key input.
		 </summary>
		 <param name="func">The Action to connect.
		 </param>
		 <para>Argument 0: An InputEvent representing the input.
		 </para>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="func"/> is null.
		 </exception>
		 */
		public static Signal<InputEvent> OnKey(Action<InputEvent> func){
			if (func == null)
				throw new ArgumentNullException("func");
			return keyEvent.Connect(func);
		}
		/**
		 <summary>
		 Connects an Action to a Broadcast which fires when <paramref name="action"/> is bound
		 to a key, and returns a Signal.
		 </summary>
		 <param name="action">The action string to listen for.
		 </param>
		 <param name="func">The Action to connect.
		 </param>
		 <para>Argument 0: The KeyCode which was bound.
		 </para>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> or <paramref name="func"/> is null.
		 </exception>
		 */
		public static Signal<KeyCode> OnKeyBound(string action, Action<KeyCode> func){
			if (action == null)
				throw new ArgumentNullException("action");
			if (func == null)
				throw new ArgumentNullException("func");
			if (!keyBoundEvents.ContainsKey(action))
				keyBoundEvents.Add(action, new Broadcast<KeyCode>());
			Broadcast<KeyCode> bc = keyBoundEvents[action];
			return bc.Connect(func);
		}
		/**
		 <summary>
		 Connects <paramref name="func"/> to an event which fires when a key is bound.
		 </summary>
		 <param name="func">The Action to connect.
		 </param>
		 <para>Argument 0: The action which the key was bound to.
		 </para>
		 <para>Argument 1: The KeyCode which was bound.
		 </para>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="func"/> is null.
		 </exception>
		 */
		public static Signal<string, KeyCode> OnKeyBound(Action<string, KeyCode> func){
			if (func == null)
				throw new ArgumentNullException("func");
			return keyBound.Connect(func);
		}
		/**
		 <summary>
		 Connects an Action to a Broadcast which fires when <paramref name="action"/> is unbound
		 from a key, and returns a Signal.
		 </summary>
		 <param name="action">The action string to listen for.
		 </param>
		 <param name="func">
		 The Action to connect.
		 </param>
		 <para>Argument 0: The KeyCode that was bound previously.
		 </para>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> or <paramref name="func"/> is null.
		 </exception>
		 */
		public static Signal<KeyCode> OnKeyUnbound(string action, Action<KeyCode> func){
			if (action == null)
				throw new ArgumentNullException("action");
			if (func == null)
				throw new ArgumentNullException("func");
			if (!keyUnboundEvents.ContainsKey(action))
				keyUnboundEvents.Add(action, new Broadcast<KeyCode>());
			Broadcast<KeyCode> bc = keyUnboundEvents[action];
			return bc.Connect(func);
		}
		/**
		 <summary>
		 Connects an Action to a Broadcast which fires when an action is unbound
		 from a key, and returns a Signal.
		 </summary>
		 <param name="func">
		 The Action to connect.
		 </param>
		 <para>Argument 0: The action which the key was unbound from.
		 </para>
		 <para>Argument 1: The KeyCode that was bound previously.
		 </para>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="func"/> is null.
		 </exception>
		 */
		public static Signal<string, KeyCode> OnKeyUnbound(Action<string, KeyCode> func){
			if (func == null)
				throw new ArgumentNullException("func");
			return keyUnbound.Connect(func);
		}
		/**
		 <summary>
		 Connects an Action to a Broadcast which fires when a mouse button event happens on a button bound to
		 <paramref name="action"/>, and returns a Signal.
		 </summary>
		 <param name="action">The action string to listen for.
		 </param>
		 <param name="func">The Action to connect.
		 </param>
		 <para>Argument 0: An InputEvent representing the input.
		 </para>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> or <paramref name="func"/> is null.
		 </exception>
		 */
		public static Signal<InputEvent> OnMouse(string action, Action<InputEvent> func){
			if (action == null)
				throw new ArgumentNullException("action");
			if (func == null)
				throw new ArgumentNullException("func");
			if (!mouseActionEvents.ContainsKey(action))
				mouseActionEvents.Add(action, new Broadcast<InputEvent>());
			Broadcast<InputEvent> bc = mouseActionEvents[action];
			return bc.Connect(func);
		}
		/**
		 <summary>
		 Connects an Action to a Broadcast which fires when mouse button
		 <paramref name="button"/> is interacted with, and returns a Signal.
		 </summary>
		 <param name="button">The mouse button.
		 </param>
		 <param name="func">
		 The Action to connect.
		 </param>
		 <para>Argument 0: An InputEvent representing the input.
		 </para>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="func"/> is null.
		 </exception>
		 */
		public static Signal<InputEvent> OnMouse(int button, Action<InputEvent> func){
			if (func == null)
				throw new ArgumentNullException("func");
			if (!mouseEvents.ContainsKey(button))
				mouseEvents.Add(button, new Broadcast<InputEvent>());
			Broadcast<InputEvent> bc = mouseEvents[button];
			return bc.Connect(func);
		}
		/**
		 <summary>
		 Connects an Action to a Broadcast which fires when a mouse button
		 is interacted with, and returns a Signal.
		 </summary>
		 <param name="func">
		 The Action to connect.
		 </param>
		 <para>Argument 0: An InputEvent representing the input.
		 </para>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="func"/> is null.
		 </exception>
		 */
		public static Signal<InputEvent> OnMouse(Action<InputEvent> func){
			if (func == null)
				throw new ArgumentNullException("func");
			return mouseEvent.Connect(func);
		}
		/**
		 <summary>
		 Connects an Action to a Broadcast which fires when <paramref name="action"/> is bound
		 to a mouse button, and returns a Signal.
		 </summary>
		 <param name="action">The action string to listen for.
		 </param>
		 <param name="func">The Action to connect.
		 </param>
		 <para>Argument 0: The mouse button which was bound.
		 </para>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> or <paramref name="func"/> is null.
		 </exception>
		 */
		public static Signal<int> OnMouseBound(string action, Action<int> func){
			if (action == null)
				throw new ArgumentNullException("action");
			if (func == null)
				throw new ArgumentNullException("func");
			if (!mouseBoundEvents.ContainsKey(action))
				mouseBoundEvents.Add(action, new Broadcast<int>());
			Broadcast<int> bc = mouseBoundEvents[action];
			return bc.Connect(func);
		}
		/**
		 <summary>
		 Connects <paramref name="func"/> to an event which fires when a mouse button is bound.
		 </summary>
		 <param name="func">The Action to connect.
		 </param>
		 <para>Argument 0: The action that the button was bound to.
		 </para>
		 <para>Argument 1: The mouse button which was bound.
		 </para>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="func"/> is null.
		 </exception>
		 */
		public static Signal<string, int> OnMouseBound(Action<string, int> func){
			if (func == null)
				throw new ArgumentNullException("func");
			return mouseBound.Connect(func);
		}
		/**
		 <summary>
		 Connects an Action to a Broadcast which fires when the
		 mouse is moved, and returns a Signal.
		 </summary>
		 <param name="func">
		 The Action to connect.
		 </param>
		 <para>Argument 0: An InputEvent representing the input.
		 </para>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="func"/> is null.
		 </exception>
		 */
		public static Signal<InputEvent> OnMouseMove(Action<InputEvent> func){
			if (func == null)
				throw new ArgumentNullException("func");
			return mouseMove.Connect(func);
		}
		/**
		 <summary>
		 Connects an Action to a Broadcast which fires when <paramref name="action"/> is unbound
		 from a mouse button, and returns a Signal.
		 </summary>
		 <param name="action">The action string to listen for.
		 </param>
		 <param name="func">
		 The Action to connect.
		 </param>
		 <para>Argument 0: The mouse button that was bound previously.
		 </para>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> or <paramref name="func"/> is null.
		 </exception>
		 */
		public static Signal<int> OnMouseUnbound(string action, Action<int> func){
			if (action == null)
				throw new ArgumentNullException("action");
			if (func == null)
				throw new ArgumentNullException("func");
			if (!mouseUnboundEvents.ContainsKey(action))
				mouseUnboundEvents.Add(action, new Broadcast<int>());
			Broadcast<int> bc = mouseUnboundEvents[action];
			return bc.Connect(func);
		}
		/**
		 <summary>
		 Connects an Action to a Broadcast which fires when an action is unbound
		 from a mouse button, and returns a Signal.
		 </summary>
		 <param name="func">
		 The Action to connect.
		 </param>
		 <para>Argument 0: The action which the mouse button was unbound from.
		 </para>
		 <para>Argument 1: The mouse button that was bound previously.
		 </para>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="func"/> is null.
		 </exception>
		 */
		public static Signal<string, int> OnMouseUnbound(Action<string, int> func){
			if (func == null)
				throw new ArgumentNullException("func");
			return mouseUnbound.Connect(func);
		}
		/**
		 <summary>
		 Connects an Action to a Broadcast which fires when the mouse scroll wheel
		 is interacted with, and returns a Signal.
		 </summary>
		 <param name="func">
		 The Action to connect.
		 </param>
		 <para>Argument 0: An InputEvent representing the input.
		 </para>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="func"/> is null.
		 </exception>
		 */
		public static Signal<InputEvent> OnScroll(Action<InputEvent> func){
			if (func == null)
				throw new ArgumentNullException("func");
			return scrollEvent.Connect(func);
		}
		/**
		 <summary>
		 Connects an Action to a Broadcast which fires when a <see cref="Sequence"/>
		 bound to <paramref name="action"/> is completed successfully, and returns
		 a <see cref="Signal"/>.
		 </summary>
		 <param name="action">The action the sequence is bound to.
		 </param>
		 <param name="func">
		 The Action to connect.
		 </param>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> or <paramref name="func"/> is null.
		 </exception>
		 */
		public static Signal<Sequence> OnSequence(string action, Action func){
			if (action == null)
				throw new ArgumentNullException("action");
			if (func == null)
				throw new ArgumentNullException("func");
			if (!seqEvents.ContainsKey(action))
				seqEvents.Add(action, new Broadcast<Sequence>());
			Broadcast<Sequence> bc = seqEvents[action];
			return bc.Connect(_ => func());
		}
		/**
		 <summary>
		 Connects an Action to a Broadcast which fires when a <see cref="Sequence"/>
		 bound to <paramref name="action"/> is completed successfully, and returns
		 a <see cref="Signal"/>.
		 </summary>
		 <param name="action">The action the sequence is bound to.
		 </param>
		 <param name="func">
		 The Action to connect.
		 </param>
		 <para>Argument 0: The sequence.
		 </para>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> or <paramref name="func"/> is null.
		 </exception>
		 */
		public static Signal<Sequence> OnSequence(string action, Action<Sequence> func){
			if (action == null)
				throw new ArgumentNullException("action");
			if (func == null)
				throw new ArgumentNullException("func");
			if (!seqEvents.ContainsKey(action))
				seqEvents.Add(action, new Broadcast<Sequence>());
			Broadcast<Sequence> bc = seqEvents[action];
			return bc.Connect(func);
		}
		/**
		 <summary>
		 Connects an Action to a Broadcast which fires when a <see cref="Sequence"/>
		 is bound to <paramref name="action"/>, and returns a <see cref="Signal"/>.
		 </summary>
		 <param name="action">The action to listen for.
		 </param>
		 <param name="func">
		 The Action to connect.
		 </param>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> or <paramref name="func"/> is null.
		 </exception>
		 */
		public static Signal<Sequence> OnSequenceBound(string action, Action<Sequence> func){
			if (action == null)
				throw new ArgumentNullException("action");
			if (func == null)
				throw new ArgumentNullException("func");
			if (!seqBoundEvents.ContainsKey(action))
				seqBoundEvents.Add(action, new Broadcast<Sequence>());
			Broadcast<Sequence> bc = seqBoundEvents[action];
			return bc.Connect(func);
		}
		/**
		 <summary>
		 Connects an Action to a Broadcast which fires when a <see cref="Sequence"/>
		 is bound, and returns a <see cref="Signal"/>.
		 </summary>
		 <param name="func">
		 The Action to connect.
		 </param>
		 <para>Argument 0: The action which the Sequence was unbound from.
		 </para>
		 <para>Argument 1: The Sequence that was bound to the action.</para>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="func"/> is null.
		 </exception>
		 */
		public static Signal<string, Sequence> OnSequenceBound(Action<string, Sequence> func){
			if (func == null)
				throw new ArgumentNullException("func");
			return seqBound.Connect(func);
		}
		/**
		 <summary>
		 Connects an Action to a Broadcast which fires when a <see cref="Sequence"/>
		 is unbound from <paramref name="action"/>, and returns a <see cref="Signal"/>.
		 </summary>
		 <param name="action">The action to listen for.
		 </param>
		 <param name="func">
		 The Action to connect.
		 </param>
		 <para>Argument 0: The Sequence that was previously bound.</para>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> or <paramref name="func"/> is null.
		 </exception>
		 */
		public static Signal<Sequence> OnSequenceUnbound(string action, Action<Sequence> func){
			if (action == null)
				throw new ArgumentNullException("action");
			if (func == null)
				throw new ArgumentNullException("func");
			if (!seqUnboundEvents.ContainsKey(action))
				seqUnboundEvents.Add(action, new Broadcast<Sequence>());
			Broadcast<Sequence> bc = seqUnboundEvents[action];
			return bc.Connect(func);
		}
		/**
		 <summary>
		 Connects an Action to a Broadcast which fires when a <see cref="Sequence"/>
		 is unbound, and returns a <see cref="Signal"/>.
		 </summary>
		 <param name="func">
		 The Action to connect.
		 </param>
		 <para>Argument 0: The action which the Sequence was unbound from.
		 </para>
		 <para>Argument 1: The Sequence that was previously bound.</para>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="func"/> is null.
		 </exception>
		 */
		public static Signal<string, Sequence> OnSequenceUnbound(Action<string, Sequence> func){
			if (func == null)
				throw new ArgumentNullException("func");
			return seqUnbound.Connect(func);
		}
	}
}