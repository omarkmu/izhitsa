using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Izhitsa.EventManager;

namespace Izhitsa {
	/**
	 * <summary>Class which facilitates input handling.</summary>
	 */
	public static partial class InputManager {
		/// <summary>A boolean representing whether or not either alt key is down.</summary>
		public static bool Alt
			=> Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
		/// <summary>A boolean representing whether or not either control key is down.</summary>
		public static bool Control
			=> Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
		/**
		 * <summary>
		 * An event which fires when a key is bound.
		 * </summary>
		 * <para>arg0: `{string}` The name of the key that was bound.
		 * </para>
		 * <para>arg1: `{KeyCode}` The KeyCode which was bound.
		 * </para>
		 */
		public static Broadcast KeyBound {
			get { return keyBound; }
			set {}
		}
		/**
		 * <summary>
		 * An event which fires when a key is unbound.
		 * </summary>
		 * <para>arg0: `{string}` The name of the key that was unbound.
		 * </para>
		 * <para>arg1: `{KeyCode}` The previous KeyCode value of the binding.
		 * </para>
		 */
		public static Broadcast KeyUnbound {
			get { return keyUnbound; }
			set {}
		}
		/**
		 * <summary>
		 * An event which fires when a sequence is bound.
		 * </summary>
		 * <para>arg0: `{string}` The name of the key that was bound.
		 * </para>
		 * <para>arg1: `{Sequence}` The Sequence which was bound.
		 * </para>
		 */
		public static Broadcast SequenceBound {
			get { return seqBound; }
			set {}
		}
		/**
		 * <summary>
		 * An event which fires when a sequence is unbound.
		 * </summary>
		 * <para>arg0: `{string}` The name of the sequence that was unbound.
		 * </para>
		 * <para>arg1: `{Sequence}` The previous Sequence of the binding.
		 * </para>
		 */
		public static Broadcast SequenceUnbound {
			get { return seqUnbound; }
			set {}
		}
		/// <summary>A boolean representing whether or not either shift key is down.</summary>
		public static bool Shift
			=> Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		

		/// <summary>A container which associates strings and Axes.</summary>
		private static Dictionary<string, Axis> axes { get; }
			= new Dictionary<string, Axis>();
		/// <summary>A container which associates strings and KeyCodes.</summary>
		private static Dictionary<string, List<KeyCode>> boundKeys { get; }
			= new Dictionary<string, List<KeyCode>>();
		/// <summary>A container which associates strings and Sequences.</summary>
		private static Dictionary<string, Sequence> boundSeqs { get; }
			= new Dictionary<string, Sequence>();
		/// <summary>Contains held down keys, and the time which they were pressed.</summary>
		private static Dictionary<KeyCode, float> heldKeys { get; }
			= new Dictionary<KeyCode, float>();
		/// <summary>Primary KeyBound event.</summary>
		private static Broadcast keyBound = new Broadcast();
		/// <summary>Contains KeyBound events.</summary>
		private static Dictionary<string, Broadcast> keyBoundEvents { get; }
			= new Dictionary<string, Broadcast>();
		/// <summary>Primary KeyUnbound event.</summary>
		private static Broadcast keyUnbound = new Broadcast();
		/// <summary>Contains KeyUnbound events.</summary>
		private static Dictionary<string, Broadcast> keyUnboundEvents { get; }
			= new Dictionary<string, Broadcast>();
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
		
		
		/// <summary>Input event types for use in InputEvents and SequenceElements.</summary>
		public enum InputEventType {
			/// <summary>Default value.</summary>
			None,
			/// <summary>A key was pressed down.</summary>
			KeyDown,
			/// <summary>A key was released.</summary>
			KeyUp,
			/// <summary>Key is being held down.</summary>
			KeyHeld,
			/// <summary>The scroll wheel was scrolled.</summary>
			Scroll,
			/// <summary>Any key event occurred.</summary>
			Any
		}
		/// <summary>Flags which represent when a sequence should be interrupted.</summary>
		[Flags]
		public enum InterruptFlags {
			/// <summary>Never interrupt.</summary>
			None = 0,
			/// <summary>When a different key is released.</summary>
			DifferentKeyUp = 1,
			/// <summary>When a different key is pressed.</summary>
			DifferentKeyDown = 2,
			/// <summary>When a different key is held down.</summary>
			DifferentKeyHeld = 4,
			/// <summary>When the same key is released.</summary>
			SameKeyUp = 8,
			/// <summary>When the same key is pressed.</summary>
			SameKeyDown = 16,
			/// <summary>When the same key is held down.</summary>
			SameKeyHeld = 32,
			/// <summary>When the mouse wheel is scrolled.</summary>
			Scroll = 64,
			/// <summary>When any event occurs on a different key.</summary>
			DifferentKey = DifferentKeyUp | DifferentKeyDown | DifferentKeyHeld,
			/// <summary>When any event occurs on the same key.</summary>
			SameKey = SameKeyUp | SameKeyDown | SameKeyHeld,
			/// <summary>When any event occurs.</summary>
			Any = DifferentKey | SameKey | Scroll
		}

		/// <summary>Contains information about input events.</summary>
		internal struct InputEvent {
			/// <summary>The mouse button this event occurred on. If it equals -1, this isn't a mouse event.</summary>
			public int Button { get; internal set; }
			/// <summary>The mouse scroll wheel delta.</summary>
			public float Delta { get; internal set; }
			/// <summary>The time, in seconds, that the key has/had been held down.</summary>
			public float HeldDuration { get; internal set; }
			/// <summary>The key related to this event.</summary>
			public KeyCode Key { get; internal set; }
			/// <summary>The input type of this event.</summary>
			public InputEventType Type { get; internal set; }


			public InputEvent(int button, float heldDur, KeyCode key, InputEventType type, float delta){
				Button = button;
				HeldDuration = heldDur;
				Key = key;
				Type = type;
				Delta = delta;
			}
		}


		/**
		 * <summary>
		 * Binds an action string to a `KeyCode`.
		 * </summary>
		 * <param name="action">The action to bind to a key.
		 * </param>
		 * <param name="keyCode">The `KeyCode` to bind.
		 * </param>
		 * <param name="clear">If true, any previously bound keys to this action
		 * will be removed.
		 * </param>
		 * <exception cref="ArgumentNullException">Thrown if `<paramref name="action"/>` is `null`.
		 * </exception>
		 */
		public static void BindKey(string action, KeyCode keyCode, bool clear = false){
			if (action == null)
				throw new ArgumentNullException("action");
			if (boundKeys.ContainsKey(action)){
				List<KeyCode> keyList = boundKeys[action];
				if (clear){
					for (int i = 0; i < keyList.Count; i++){
						KeyUnbound.Fire(action, keyList[i]);
						tryFire(keyUnboundEvents, action, keyList[i]);
					}
					keyList = boundKeys[action] = new List<KeyCode>();
				}
				keyList.Add(keyCode);
				KeyBound.Fire(action, keyCode);
				tryFire(keyBoundEvents, action, keyCode);
			} else {
				boundKeys.Add(action, new List<KeyCode>(){ keyCode });
				KeyBound.Fire(action, keyCode);
				tryFire(keyBoundEvents, action, keyCode);
			}
		}
		/**
		 * <summary>
		 * Binds an action string to `KeyCode`s.
		 * </summary>
		 * <param name="action">The action to bind to the keys.
		 * </param>
		 * <param name="keyCodes">The `KeyCode`s to bind.
		 * </param>
		 * <exception cref="ArgumentNullException">Thrown if `<paramref name="action"/>` is `null`.
		 * </exception>
		 */
		public static void BindKey(string action, params KeyCode[] keyCodes){
			if (action == null)
				throw new ArgumentNullException("action");
			foreach(KeyCode key in keyCodes) BindKey(action, key, false);
		}
		/**
		 * <summary>
		 * Binds a name to a `Sequence`.
		 * </summary>
		 * <param name="seqName">The name to bind to the sequence.
		 * </param>
		 * <param name="seq">The `Sequence` to bind.
		 * </param>
		 */
		public static Sequence BindSequence(string seqName, Sequence seq){
			if (boundSeqs.ContainsKey(seqName)){
				SequenceUnbound.Fire(seqName, boundSeqs[seqName]);
				tryFire(seqUnboundEvents, seqName, boundSeqs[seqName]);
				boundSeqs[seqName] = seq;
				SequenceBound.Fire(seqName, seq);
				tryFire(seqUnboundEvents, seqName, seq);
			} else {
				boundSeqs.Add(seqName, seq);
				SequenceBound.Fire(seqName, seq);
				tryFire(seqUnboundEvents, seqName, seq);
			}
			return seq;
		}
		/**
		 * <summary>
		 * Binds a name to a `Sequence`.
		 * </summary>
		 * <param name="seqName">The name to bind to the sequence.
		 * </param>
		 * <param name="args">The `SequenceElement`s to convert into a sequence.
		 * </param>
		 */
		public static Sequence BindSequence(string seqName, params SequenceElement[] args){
			return BindSequence(seqName, new Sequence(args));
		}
		/**
		 * <summary>
		 * Binds a name to a `Sequence`.
		 * </summary>
		 * <param name="seqName">The name to bind to the sequence.
		 * </param>
		 * <param name="args">The `KeyCode`s to convert into a sequence.
		 * </param>
		 */
		public static Sequence BindSequence(string seqName, params KeyCode[] args){
			return BindSequence(seqName, new Sequence(args));
		}
		/**
		 * <summary>
		 * Associates an input axis with the name `<paramref name="name"/>`.
		 * </summary>
		 * <param name="name">The name to bind to the Axis.
		 * </param>
		 * <param name="axis">The Axis object to bind.
		 * </param>
		 * <exception cref="ArgumentNullException">Thrown if `<paramref name="name"/>` is `null`.
		 * </exception>
		 * <exception cref="ArgumentException">Thrown if an axis named `<paramref name="name"/>` already exists.
		 * </exception>
		 */
		public static Axis CreateAxis(string name, Axis axis){
			if (name == null)
				throw new ArgumentNullException(name);
			if (axes.ContainsKey(name))
				throw new ArgumentException($"An axis named \"{name}\" already exists.");
			axes.Add(name, axis);
			return axis;
		}
		/**
		 * <summary>
		 * Creates an input axis associated with the name `<paramref name="name"/>` using
		 * a negative and positive KeyCode.
		 * </summary>
		 * <param name="name">The name to bind to the Axis.
		 * </param>
		 * <param name="negative">The negative KeyCode.
		 * </param>
		 * <param name="positive">The positive KeyCode.
		 * </param>
		 * <exception cref="ArgumentNullException">Thrown if `<paramref name="name"/>` is `null`.
		 * </exception>
		 * <exception cref="ArgumentException">Thrown if an axis named `<paramref name="name"/>` already exists.
		 * </exception>
		 */
		public static Axis CreateAxis(string name, KeyCode negative, KeyCode positive){
			if (name == null)
				throw new ArgumentNullException(name);
			if (axes.ContainsKey(name))
				throw new ArgumentException($"An axis named \"{name}\" already exists.");
			Axis axis = new Axis(negative, positive);
			axes.Add(name, axis);
			return axis;
		}
		/**
		 * <summary>
		 * Creates an input axis associated with the name `<paramref name="name"/>` using
		 * a negative and positive KeyCode.
		 * </summary>
		 * <param name="name">The name to bind to the Axis.
		 * </param>
		 * <param name="negative">The list of negative KeyCodes.
		 * </param>
		 * <param name="positive">The list of positive KeyCodes.
		 * </param>
		 * <exception cref="ArgumentNullException">Thrown if `<paramref name="name"/>` is `null`.
		 * </exception>
		 * <exception cref="ArgumentException">Thrown if an axis named `<paramref name="name"/>` already exists.
		 * </exception>
		 */
		public static Axis CreateAxis(string name, List<KeyCode> negatives, List<KeyCode> positives){
			if (name == null)
				throw new ArgumentNullException(name);
			if (axes.ContainsKey(name))
				throw new ArgumentException($"An axis named \"{name}\" already exists.");
			Axis axis = new Axis(negatives, positives);
			axes.Add(name, axis);
			return axis;
		}
		/// <todo>Create GetAxis</todo>
		// public static float GetAxis(string name)
		/**
		 * <summary>
		 * Returns the Axis object associated with a name.
		 * </summary>
		 * <param name="name">The name associated with the Axis object.
		 * </param>
		 * <exception cref="ArgumentNullException">Thrown if `<paramref name="name"/>` is `null`.
		 * </exception>
		 */
		public static Axis GetAxisObject(string name){
			if (name == null)
				throw new ArgumentNullException(name);
			return (axes.ContainsKey(name)) ? axes[name] : null;
		}
		/**
		 * <summary>
		 * Returns the raw input value from an Axis.
		 * </summary>
		 * <param name="name">The name of the Axis object to check.
		 * </param>
		 * <exception cref="ArgumentNullException">Thrown if `<paramref name="name"/>` is `null`.
		 * </exception>
		 * <exception cref="ArgumentException">Thrown if an axis named `<paramref name="name"/>` is not associated
		 * with an Axis.
		 * </exception>
		 */
		public static float GetAxisRaw(string name){
			if (name == null)
				throw new ArgumentNullException(name);
			if (!axes.ContainsKey(name))
				throw new ArgumentException($"Axis \"{name}\" has not been defined.");
			return axes[name].GetRawValue();
		}
		/**
		 * <summary>
		 * Returns the keys bound to a string.
		 * </summary>
		 * <param name="action">
		 * The name of the key to check.
		 * </param>
		 * <exception cref="ArgumentNullException">Thrown if `<paramref name="action"/>` is `null`.
		 * </exception>
		 */
		public static List<KeyCode> GetBoundKeys(string action){
			if (action == null)
				throw new ArgumentNullException("action");
			if (!boundKeys.ContainsKey(action))
				return new List<KeyCode>();
			return new List<KeyCode>(boundKeys[action]);
		}
		/**
		 * <summary>
		 * Returns true while any of the keys bound to `<paramref name="action"/>` are held down.
		 * </summary>
		 * <param name="action">The name of the key to check.
		 * </param>
		 * <exception cref="ArgumentNullException">Thrown if `<paramref name="action"/>` is `null`.
		 * </exception>
		 * <exception cref="ArgumentException">Thrown if `<paramref name="action"/>` is not bound to any keys.
		 * </exception>
		 */
		public static bool GetKey(string action){
			if (action == null)
				throw new ArgumentNullException("action");
			if (!boundKeys.ContainsKey(action))
				throw new ArgumentException($"\"{action}\" has not been bound to any keys.", "action");
			foreach(KeyCode key in boundKeys[action])
				if (Input.GetKey(key)) return true;
			return false;
		}
		/**
		 * <summary>
		 * Checks if any of the keys bound to `<paramref name="action"/>` were pressed during the frame.
		 * </summary>
		 * <param name="action">The name of the key to check.
		 * </param>
		 * <returns>A boolean representing whether or not the key was pressed.</returns>
		 * <exception cref="ArgumentNullException">Thrown if `<paramref name="action"/>` is `null`.
		 * </exception>
		 * <exception cref="ArgumentException">Thrown if `<paramref name="action"/>` is not bound to any keys.
		 * </exception>
		 */
		public static bool KeyDown(string action){
			if (action == null)
				throw new ArgumentNullException("action");
			if (!boundKeys.ContainsKey(action))
				throw new ArgumentException($"\"{action}\" has not been bound to any keys.", "action");
			foreach(KeyCode key in boundKeys[action])
				if (Input.GetKeyDown(key)) return true;
			return false;
		}
		/**
		 * <summary>
		 * Checks if any of the keys bound to `<paramref name="action"/>` were released during the frame.
		 * </summary>
		 * <param name="action">The name of the key to check.
		 * </param>
		 * <returns>A boolean representing whether or not the key was released.</returns>
		 * <exception cref="ArgumentNullException">Thrown if `<paramref name="action"/>` is `null`.
		 * </exception>
		 * <exception cref="ArgumentException">Thrown if `<paramref name="action"/>` is not bound to any keys.
		 * </exception>
		 */
		public static bool KeyUp(string action){
			if (action == null)
				throw new ArgumentNullException("action");
			if (!boundKeys.ContainsKey(action))
				throw new ArgumentException($"\"{action}\" has not been bound to any keys.", "action");
			foreach(KeyCode key in boundKeys[action])
				if (Input.GetKeyUp(key)) return true;
			return false;
		}
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
		/**
		 * <summary>
		 * Unbinds a key.
		 * </summary>
		 * <param name="action">The action to unbind from.
		 * </param>
		 * <param name="keyToUnbind">The KeyCode to unbind.
		 * </param>
		 * <exception cref="ArgumentNullException">Thrown if `<paramref name="action"/>` is `null`.
		 * </exception>
		 * <exception cref="ArgumentException">Thrown if `<paramref name="action"/>` is not bound to any keys.
		 * </exception>
		 */
		public static void UnbindKey(string action, KeyCode keyToUnbind){
			if (action == null)
				throw new ArgumentNullException("action");
			if (!boundKeys.ContainsKey(action))
				throw new ArgumentException($"\"{action}\" has not been bound to any keys.", "action");
			foreach(KeyCode key in boundKeys[action]){
				if (key == keyToUnbind){
					KeyUnbound.Fire(action, key);
					tryFire(keyUnboundEvents, action, key);
				}
			}
		}
		/**
		 * <summary>
		 * Unbinds a key.
		 * </summary>
		 * <param name="action">The action to unbind from.
		 * </param>
		 * <exception cref="ArgumentNullException">Thrown if `<paramref name="action"/>` is `null`.
		 * </exception>
		 * <exception cref="ArgumentException">Thrown if `<paramref name="action"/>` is not bound to any keys.
		 * </exception>
		 */
		public static void UnbindKeys(string action){
			if (action == null)
				throw new ArgumentNullException("action");
			if (!boundKeys.ContainsKey(action))
				throw new ArgumentException($"\"{action}\" has not been bound to any keys.", "action");
			foreach(KeyCode key in boundKeys[action]){
				KeyUnbound.Fire(action, key);
				tryFire(keyUnboundEvents, action, key);
			}
			boundKeys.Remove(action);
		}
		
		/**
		 * <summary>
		 * Handles events, converts them into `InputEvent`s,
		 * and registers them for `Sequence` checking.
		 * </summary>
		 * <param name="ev">The `Event` to convert and handle.</param>
		 */
		internal static IEnumerator handleEvent(Event ev){
			bool valid = true;

			int button = -1;
			float heldDuration = 0.0f;
			float delta = 0.0f;
			KeyCode key = KeyCode.None;
			InputEventType type = InputEventType.None;
			
			switch (ev.type){
				case EventType.KeyDown:
					key = ev.keyCode;
					if (heldKeys.ContainsKey(key)){
						float time = heldKeys[key];
						type = InputEventType.KeyHeld;
						heldDuration = Time.time - time;
					} else {
						type = InputEventType.KeyDown;
						heldKeys[key] = Time.time;
					}
					break;
				case EventType.KeyUp:
					key = ev.keyCode;
					type = InputEventType.KeyUp;
					if (heldKeys.ContainsKey(key)){
						float time = heldKeys[key];
						heldKeys.Remove(key);
						heldDuration = Time.time - time;
					}
					break;
				case EventType.MouseDown:
					button = ev.button;
					key = (KeyCode)(323 + button);
					type = InputEventType.KeyDown;
					if (button > 6){
						valid = false;
						break;
					}
					if (heldKeys.ContainsKey(key)){
						float time = heldKeys[key];
						type = InputEventType.KeyHeld;
						heldDuration = Time.time - time;
					} else {
						type = InputEventType.KeyDown;
						heldKeys[key] = Time.time;
					}

					heldKeys[key] = Time.time;
					break;
				case EventType.MouseUp:
					key = (KeyCode)(323 + ev.button);
					type = InputEventType.KeyUp;
					if (ev.button > 6){
						valid = false;
						break;
					}
					if (heldKeys.ContainsKey(key)){
						float time = heldKeys[key];
						heldKeys.Remove(key);
						heldDuration = Time.time - time;
					}
					break;
				case EventType.ScrollWheel:
					type = InputEventType.Scroll;
					delta = ev.delta.y;
					break;
			}
			
			if (valid) registerEvent(new InputEvent(button, heldDuration, key, type, delta));
			yield return null;
		}

		/**
		 * <summary>
		 * Returns the `InterruptFlags` which are true for the
		 * event-element pair.
		 * </summary>
		 * <param name="ev">The `InputEvent`.
		 * </param>
		 * <param name="elem">The `SequenceElement`.
		 * </param>
		 */
		private static InterruptFlags getInterruptFlags(InputEvent ev, SequenceElement elem){
			InterruptFlags flags = InterruptFlags.None;
			if (ev.Key == elem.Key){
				if (ev.Type == InputEventType.KeyUp) flags = InterruptFlags.SameKeyUp;
				if (ev.Type == InputEventType.KeyDown) flags = InterruptFlags.SameKeyDown;
				if (ev.Type == InputEventType.KeyHeld) flags = InterruptFlags.SameKeyHeld;
				if (ev.Type == InputEventType.Scroll) flags = InterruptFlags.Scroll;
			} else {
				if (ev.Type == InputEventType.KeyUp) flags = InterruptFlags.DifferentKeyUp;
				if (ev.Type == InputEventType.KeyDown) flags = InterruptFlags.DifferentKeyDown;
				if (ev.Type == InputEventType.KeyHeld) flags = InterruptFlags.DifferentKeyHeld;
			}
			return flags;
		}
		/**
		 * <summary>
		 * Registers an `InputEvent` with valid `Sequence`s.
		 * </summary>
		 * <param name="ev">The `InputEvent` to register.</param>
		 */
		private static void registerEvent(InputEvent ev){
			foreach (KeyValuePair<string, Sequence> pair in boundSeqs){
				string name = pair.Key;
				Sequence seq = pair.Value;
				if (seq == null) continue;
				if (seq.MaxStep < 0) continue;

				SequenceElement elem = seq.Current;
				InterruptFlags flags = getInterruptFlags(ev, elem);
				if ((elem.InterruptFlags & flags) == flags){
					seq.Reset();
					continue;
				}

				if (ev.Key == elem.Key){
					float margin = ((seq.lastStepTime == 0.0f) ? 0 : Time.time - seq.lastStepTime);
					float duration = ev.HeldDuration;
					float delta = ev.Delta;
					float last = Time.time - seq.lastStepTime;

					bool inMargin = (seq.CurrentStep == 0 || (last >= elem.MinMargin && last <= elem.MaxMargin));
					bool inDuration = (duration >= elem.MinDuration && duration <= elem.MaxDuration);
					bool inDelta = (delta >= elem.MinDelta && delta <= elem.MaxDelta);

					if (inDuration && inMargin && inDelta){
						if (ev.Type == elem.Type){
							seq.lastStepTime = Time.time;
							if (seq.CurrentStep++ == seq.MaxStep){
								Broadcast bc = (seqEvents.ContainsKey(name)) ? seqEvents[name] : null;
								bc?.Fire();
							}
						}
					} else if (duration > elem.MaxDuration || !inMargin || !inDelta){
						seq.Reset();
					}
				}
			}
		}
		/**
		 * <summary>
		 * Attempts to fire a Broadcast using a dictionary and a key.
		 * </summary>
		 * <param name="dict">The `Dictionary` to check.
		 * </param>
		 * <param name="key">The key.
		 * </param>
		 * <param name="args">The arguments to fire the Broadcast with.</param>
		 */
		private static void tryFire(Dictionary<string, Broadcast> dict, string key, params object[] args){
			if (dict.ContainsKey(key)) dict[key].Fire(args);
		}
	}
}