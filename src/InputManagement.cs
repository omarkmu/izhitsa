using System;
using System.Collections.Generic;
using UnityEngine;
using static Izhitsa.EventManager;

namespace Izhitsa {
	/**
	 * <summary>Class which facilitates input handling.</summary>
	 */
	public static class InputManager {
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
		 * <param name="arg0">`{string}` The name of the key that was bound.
		 * </param>
		 * <param name="arg1">`{KeyCode}` The KeyCode which was bound.
		 * </param>
		 */
		public static Broadcast KeyBound {
			get { return keyBound; }
			set {}
		}
		/**
		 * <summary>
		 * An event which fires when a key is unbound.
		 * </summary>
		 * <param name="arg0">`{string}` The name of the key that was unbound.
		 * </param>
		 * <param name="arg1">`{KeyCode}` The previous KeyCode value of the binding.
		 * </param>
		 */
		public static Broadcast KeyUnbound { 
			get { return keyUnbound; }
			set {}
		}
		/**
		 * <summary>
		 * An event which fires when a sequence is bound.
		 * </summary>
		 * <param name="arg0">`{string}` The name of the key that was bound.
		 * </param>
		 * <param name="arg1">`{Sequence}` The Sequence which was bound.
		 * </param>
		 */
		public static Broadcast SequenceBound {
			get { return seqBound; }
			set {}
		}
		/**
		 * <summary>
		 * An event which fires when a sequence is unbound.
		 * </summary>
		 * <param name="arg0">`{string}` The name of the sequence that was unbound.
		 * </param>
		 * <param name="arg1">`{Sequence}` The previous Sequence of the binding.
		 * </param>
		 */
		public static Broadcast SequenceUnbound {
			get { return seqUnbound; }
			set {}
		}
		
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
		
		/// <summary>Input event types for use in InputEvents.</summary>
		public enum InputEventType {
			/// <summary>Default value.</summary>
			None,
			/// <summary>A key was pressed down.</summary>
			KeyDown,
			/// <summary>A key was released.</summary>
			KeyUp,
			/// <summary>Key is being held down.</summary>
			KeyHeld,
			/// <summary>Any key event occurred.</summary>
			Any
		}
		/// <summary>Flags to represent when a sequence should be interrupted.</summary>
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
			/// <summary>When any event occurs on a different key.</summary>
			DifferentKey = DifferentKeyUp | DifferentKeyDown | DifferentKeyHeld,
			/// <summary>When any event occurs on the same key.</summary>
			SameKey = SameKeyUp | SameKeyDown | SameKeyHeld,
			/// <summary>When any event occurs.</summary>
			Any = DifferentKey | SameKey
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
		internal static void handleEvent(Event ev){
			bool valid = true;
			KeyCode code;
			InputEvent iEvent = new InputEvent();
			switch(ev.type){
				case EventType.KeyDown:
					if (heldKeys.ContainsKey(ev.keyCode)){
						float time = heldKeys[ev.keyCode];
						iEvent.Type = InputEventType.KeyHeld;
						iEvent.HeldDuration = Time.time - time;
					} else {
						iEvent.Type = InputEventType.KeyDown;
						heldKeys[ev.keyCode] = Time.time;
					}
					iEvent.Key = ev.keyCode;
					break;
				case EventType.KeyUp:
					if (heldKeys.ContainsKey(ev.keyCode)){
						float time = heldKeys[ev.keyCode];
						heldKeys.Remove(ev.keyCode);
						iEvent.HeldDuration = Time.time - time;
					}
					iEvent.Type = InputEventType.KeyUp;
					iEvent.Key = ev.keyCode;
					break;
				case EventType.MouseDown:
					code = (KeyCode)(323 + ev.button);
					if (ev.button > 6 || heldKeys.ContainsKey(code)){
						valid = false;
						break;
					}

					iEvent.Type = InputEventType.KeyDown;
					iEvent.Key = code;
					heldKeys[code] = Time.time;
					break;
				case EventType.MouseUp:
					code = (KeyCode)(323 + ev.button);
					if (ev.button > 6){
						valid = false;
						break;
					}
					if (heldKeys.ContainsKey(code)){
						float time = heldKeys[code];
						heldKeys.Remove(code);
						iEvent.HeldDuration = Time.time - time;
					}
					iEvent.Type = InputEventType.KeyUp;
					iEvent.Key = code;
					break;
				default:
					break;
			}
			
			if (valid) registerEvent(iEvent);
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
					float last = Time.time - seq.lastStepTime;
					bool inMargin = (seq.CurrentStep == 0 || (last >= elem.MinMargin && last <= elem.MaxMargin));
					bool inDuration = (duration >= elem.MinDuration && duration <= elem.MaxDuration);
					if (inDuration && inMargin){
						if (ev.Type == elem.Type){
							seq.lastStepTime = Time.time;
							if (seq.CurrentStep++ == seq.MaxStep){
								Broadcast bc = (seqEvents.ContainsKey(name)) ? seqEvents[name] : null;
								bc?.Fire();
							}
						}
					} else if (duration > elem.MaxDuration || !inMargin){
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

		/**
		 * <summary>
		 * Class which represents an ordered sequence of input events.
		 * </summary>
		 */
		public class Sequence {
			/// <summary>The current SequenceElement. (Read Only)</summary>
			public SequenceElement Current => elements[CurrentStep];
			/// <summary>The current step, where the sequence pointer is right now. (Read Only)</summary>
			public int CurrentStep {
				get {
					return cStep;
				}
				internal set {
					if (value > MaxStep) cStep = 0;
					else cStep = value;

					if (cStep == 0)
						lastStepTime = 0.0f;
				}
			}
			/// <summary>The maximum amount of steps before completion and reset. (Read Only)</summary>
			public int MaxStep => elements.Length - 1;

			/// <summary>The elements which make up the Sequence.</summary>
			internal SequenceElement[] elements { get; set; }
			/// <summary>The recorded `Time.time` from the last step's execution.</summary>
			internal float lastStepTime { get; set; } = 0.0f;
			
			private int cStep = 0;

			/**
			 * <summary>
			 * Sequence indexer, for getting/setting SequenceElements.
			 * </summary>
			 * <param name="index">The integer to index the array with.
			 * Must be between 0 and `MaxStep`.
			 * </param>
			 */
			public SequenceElement this[int index]{
				get {
					if (index < 0 || index > MaxStep)
						throw new ArgumentOutOfRangeException("index");
					return elements[index];
				}
				set {
					if (index < 0 || index >= MaxStep)
						throw new ArgumentOutOfRangeException("index");
					elements[index] = value;
				}
			}
			/**
			 * <summary>
			 * Creates a new Sequence.
			 * </summary>
			 * <param name="args">The SequenceElements to create a Sequence out of.</param>
			 */
			public Sequence(params SequenceElement[] args){
				elements = args;
			}
			/**
			 * <summary>
			 * Creates a new Sequence out of `KeyCode`s.
			 * </summary>
			 * <param name="args">The KeyCodes to create a Sequence out of.</param>
			 */
			public Sequence(params KeyCode[] args){
				SequenceElement[] elems = new SequenceElement[args.Length];
				for (int i = 0; i < args.Length; i++){
					SequenceElement elem = new SequenceElement();
					elem.Key = args[i];
					elems[i] = elem;
				}
				elements = elems;
			}

			/**
			 * <summary>
			 * Resets the sequence.
			 * </summary>
			 */
			public void Reset(){
				CurrentStep = 0;
			}
			/**
			 * <summary>
			 * Sets `propName` property to `value` in all of the sequence's `SequenceElement`s,
			 * and returns the sequence.
			 * </summary>
			 * <param name="propName">A `SequenceElement` float property name.
			 * </param>
			 * <param name="value">The value to assign to the property.</param>
			 */
			public Sequence Set(string propName, float value){
				if (propName == null) throw new ArgumentNullException("propName");
				switch(propName){
					case "MaxDuration":
						foreach(SequenceElement elem in elements)
							elem.MaxDuration = value;
						break;
					case "MaxMargin":
						foreach(SequenceElement elem in elements)
							elem.MaxMargin = value;
						break;
					case "MinDuration":
						foreach(SequenceElement elem in elements)
							elem.MinDuration = value;
						break;
					case "MinMargin":
						foreach(SequenceElement elem in elements)
							elem.MinMargin = value;
						break;
					default:
						throw new ArgumentException($"Invalid property name: \"{propName}\"" +
						" for type \"string\".");
				}
				return this;
			}
			/**
			 * <summary>
			 * Sets InterruptFlags to `value` in all of the sequence's `SequenceElement`s,
			 * and returns the sequence.
			 * </summary>
			 * <param name="value">The value to assign to the property.</param>
			 */
			public Sequence Set(InterruptFlags value){
				foreach(SequenceElement elem in elements) elem.InterruptFlags = value;
				return this;
			}
			/**
			 * <summary>
			 * Sets Key to `value` in all of the sequence's `SequenceElement`s,
			 * and returns the sequence.
			 * </summary>
			 * <param name="value">The value to assign to the property.</param>
			 */
			public Sequence Set(KeyCode key){
				foreach(SequenceElement elem in elements) elem.Key = key;
				return this;
			}
			/**
			 * <summary>
			 * Sets Type to `value` in all of the sequence's `SequenceElement`s,
			 * and returns the sequence.
			 * </summary>
			 * <param name="value">The value to assign to the property.</param>
			 */
			public Sequence Set(InputEventType type){
				foreach(SequenceElement elem in elements) elem.Type = type;
				return this;
			}
		}
		/**
		 * <summary>
		 * Class which represents an element in a Sequence.
		 * </summary>
		 */
		public class SequenceElement {
			/// <summary>Flags used to check if the sequence should be interrupted. (Default: `InterruptFlags.DifferentKeyDown`)</summary>
			public InterruptFlags InterruptFlags { get; set; } = InterruptFlags.DifferentKeyDown;
			/// <summary>The KeyCode to check. (Default: `KeyCode.None`)</summary>
			public KeyCode Key { get; set; } = KeyCode.None;
			/// <summary>The maximum duration of the keypress before invalidity. (Default: `float.MaxValue`)</summary>
			public float MaxDuration { get; set; } = float.MaxValue;
			/// <summary>The maximum time which can pass since the last element in the sequence before invalidity. (Default: `float.MaxValue`)</summary>
			public float MaxMargin { get; set; } = float.MaxValue;
			/// <summary>The minimum duration of the keypress for the element to be valid. (Default: `0`)</summary>
			public float MinDuration { get; set; } = 0.0f;
			/// <summary>The minimum time which has to pass since the last element in the sequence for the element to be valid. (Default: `0`)</summary>
			public float MinMargin { get; set; } = 0.0f;
			/// <summary>The element's input type. (Default: `InputEventType.KeyDown`)</summary>
			public InputEventType Type { get; set; } = InputEventType.KeyDown;
			
			/**
			 * <summary>Creates an empty SequenceElement.</summary>
			 */
			public SequenceElement(){}
			/**
			 * <summary>
			 * Creates a SequenceElement.
			 * </summary>
			 * <param name="key">The KeyCode to check.
			 * </param>
			 * <param name="type">The element's input type.
			 * </param>
			 * <param name="flags">Flags used to check if the sequence should be interrupted.
			 * </param>
			 * <param name="minDuration">The minimum duration of the keypress for the element to
			 * be valid.
			 * </param>
			 * <param name="maxDuration">The maximum duration of the keypress before invalidity.
			 * </param>
			 * <param name="minMargin">The minimum time which has to pass since the last element
			 * in the sequence for the element to be valid.
			 * </param>
			 * <param name="maxMargin">The maximum time which can pass since the last element in
			 * the sequence before invalidity.
			 * </param>
			 */
			public SequenceElement(KeyCode key, InputEventType type = InputEventType.KeyDown,
				InterruptFlags flags = InterruptFlags.DifferentKeyDown, float minDuration = 0.0f,
				float maxDuration = float.MaxValue, float minMargin = 0.0f, float maxMargin = float.MaxValue
			){
				Key = key;
				Type = type;
				InterruptFlags = flags;
				MinDuration = minDuration;
				MaxDuration = maxDuration;
				MinMargin = minMargin;
				MaxMargin = maxMargin;
			}
		}
		/**
		 * <summary>
		 * Small class which represents input events.
		 * </summary>
		 */
		internal class InputEvent {
			/// <summary>The time, in seconds, that the key has/had been held down.</summary>
			public float HeldDuration { get; internal set; } = 0.0f;
			/// <summary>The key related to this event.</summary>
			public KeyCode Key { get; internal set; } = KeyCode.None;
			/// <summary>The input type of this event.</summary>
			public InputEventType Type { get; internal set; } = InputEventType.None;
		}
	}
}