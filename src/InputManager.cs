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
		
		/// <summary>A container which associates strings and KeyCodes.</summary>
		
		private static Dictionary<string, KeyCode> boundKeys { get; }
			= new Dictionary<string, KeyCode>();
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
			/// <summary>When the mouse wheel is scrolled.</summarY>
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
		 * Binds a name to a `KeyCode`.
		 * </summary>
		 * <param name="keyName">The name to bind to a key.
		 * </param>
		 * <param name="keyCode">The `KeyCode` to bind.
		 * </param>
		 */
		public static void BindKey(string keyName, KeyCode keyCode = KeyCode.None){
			if (boundKeys.ContainsKey(keyName)){
				KeyUnbound.Fire(keyName, boundKeys[keyName]);
				tryFire(keyUnboundEvents, keyName, boundKeys[keyName]);
				boundKeys[keyName] = keyCode;
				KeyBound.Fire(keyName, keyCode);
				tryFire(keyBoundEvents, keyName, keyCode);
			} else {
				boundKeys.Add(keyName, keyCode);
				KeyBound.Fire(keyName, keyCode);
				tryFire(keyBoundEvents, keyName, keyCode);
			}
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
		 * Returns true while the key bound to `keyName` is held down.
		 * If `keyName` isn't bound, the method will silently fail and
		 * return false.
		 * </summary>
		 * <param name="keyName">The name of the key to check.
		 * </param>
		 */
		public static bool GetKey(string keyName)
			=> boundKeys.ContainsKey(keyName) ?
				Input.GetKey(boundKeys[keyName]) :
				false;
		/**
		 * <summary>
		 * Returns the key bound to a name, or `KeyCode.None` if unmatched.
		 * </summary>
		 * <param name="keyName">
		 * The name of the key to check.
		 * </param>
		 */
		public static KeyCode GetBoundKey(string keyName)
			=> boundKeys.ContainsKey(keyName) ?
				boundKeys[keyName] :
				KeyCode.None;
		/**
		 * <summary>
		 * Checks if the key bound to `keyName` was pressed during the frame.
		 * </summary>
		 * <param name="keyName">The name of the key to check.
		 * </param>
		 * <returns>A boolean representing whether or not the key was pressed.</returns>
		 */
		public static bool KeyDown(string keyName)
			=> boundKeys.ContainsKey(keyName) ?
				Input.GetKeyDown(boundKeys[keyName]) :
				false;
		/**
		 * <summary>
		 * Checks if the key bound to `keyName` was released during the frame.
		 * </summary>
		 * <param name="keyName">
		 * The name of the key to check.
		 * </param>
		 * <returns>A boolean representing whether or not the key was released.</returns>
		 */
		public static bool KeyUp(string keyName)
			=> boundKeys.ContainsKey(keyName) ?
				Input.GetKeyUp(boundKeys[keyName]) :
				false;
		/**
		 * <summary>
		 * Connects a function to a Broadcast which fires when `keyName` is bound
		 * to a key, and returns a Signal.
		 * </summary>
		 * <param name="keyName">The name of the bound key to connect to.
		 * </param>
		 * <param name="func">
		 * The function to connect.
		 * </param>
		 */
		public static Signal OnKeyBound(string keyName, Action func){
			if (!(keyBoundEvents.ContainsKey(keyName)))
				keyBoundEvents.Add(keyName, new Broadcast());
			Broadcast bc = keyBoundEvents[keyName];
			return bc.Connect((args)=> func());
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
			if (!(seqEvents.ContainsKey(seqName)))
				seqEvents.Add(seqName, new Broadcast());
			Broadcast bc = seqEvents[seqName];
			return bc.Connect((args)=> func());
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
		 * <param name="keyName">The name of the key to unbind.</param>
		 */
		public static void UnbindKey(string keyName){
			if (!(boundKeys.ContainsKey(keyName))) return;
			KeyCode code = boundKeys[keyName];
			boundKeys.Remove(keyName);
			KeyUnbound.Fire(keyName, code);
			tryFire(keyUnboundEvents, keyName, code);
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
				if (seq.MaxStep < 1) continue;

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
					if (inDuration && inMargin){
						if (ev.Type == elem.Type){
							seq.lastStepTime = Time.time;
							if (seq.CurrentStep++ == seq.MaxStep){
								Broadcast bc = (seqEvents.ContainsKey(name)) ? seqEvents[name] : null;
								bc?.Fire();
							}
						}
					} else if (duration > elem.MaxDuration || !inDelta || !inMargin){
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