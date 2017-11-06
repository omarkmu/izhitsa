using System;
using System.Collections.Generic;
using UnityEngine;
using static Izhitsa.EventManager;

/// <TODO>
/// * Make "unordered sequence" completely separate as `Keystroke` class.
/// 	* Add some sort of "modifier" functionality to `Keystroke` (control, alt, shift, etc.)
/// </TODO>
namespace Izhitsa {
	/**
	 * <summary>Class to facilitate input.</summary>
	 */
	public static class InputManager {
		/// A boolean representing whether or not either alt key is down.
		public static bool Alt
			=> Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
		/// A boolean representing whether or not either control key is down.
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
		
		/// A container which associates strings and KeyCodes.
		private static Dictionary<string, KeyCode> boundKeys { get; }
			= new Dictionary<string, KeyCode>();
		/// A container which associates strings and Sequences.
		private static Dictionary<string, Sequence> boundSeqs { get; }
			= new Dictionary<string, Sequence>();
		/// Contains held down keys, and the time which they were pressed.
		private static Dictionary<KeyCode, float> heldKeys { get; }
			= new Dictionary<KeyCode, float>();
		/// Primary KeyBound event.
		private static Broadcast keyBound = new Broadcast();
		/// Contains KeyBound events.
		private static Dictionary<string, Broadcast> keyBoundEvents { get; }
			= new Dictionary<string, Broadcast>();
		/// Primary KeyUnbound event.
		private static Broadcast keyUnbound = new Broadcast();
		/// Contains KeyUnbound events.
		private static Dictionary<string, Broadcast> keyUnboundEvents { get; }
			= new Dictionary<string, Broadcast>();
		/// Primary SequenceBound event.
		private static Broadcast seqBound { get; } = new Broadcast();
		/// Contains KeyBound events.
		private static Dictionary<string, Broadcast> seqBoundEvents { get; }
			= new Dictionary<string, Broadcast>();
		/// Contains sequence completion events.
		private static Dictionary<string, Broadcast> seqEvents { get; }
			= new Dictionary<string, Broadcast>();


		/// Primary SequenceUnbound event.
		private static Broadcast seqUnbound { get; } = new Broadcast();
		/// Contains SequenceUnbound events.
		private static Dictionary<string, Broadcast> seqUnboundEvents { get; }
			= new Dictionary<string, Broadcast>();
		
		/// Input event types for use in InputEvents.
		public enum InputEventType {
			/// Default value.
			None,
			/// A key was pressed down.
			KeyDown,
			/// A key was released.
			KeyUp,
			/// Key is being held down.
			KeyHeld,
			/// Any key event occurred.
			Any
		}
		/// Flags to represent when a sequence should be interrupted.
		[Flags]
		public enum InterruptFlags {
			/// Never interrupt.
			None = 0,
			/// When a different key is released.
			DifferentKeyUp = 1,
			/// When a different key is pressed.
			DifferentKeyDown = 2,
			/// When a different key is held down.
			DifferentKeyHeld = 4,
			/// When the same key is released.
			SameKeyUp = 8,
			/// When the same key is pressed.
			SameKeyDown = 16,
			/// When the same key is held down.
			SameKeyHeld = 32,
			/// When any event occurs on a different key.
			DifferentKey = DifferentKeyUp | DifferentKeyDown | DifferentKeyHeld,
			/// When any event occurs on the same key.
			SameKey = SameKeyUp | SameKeyDown | SameKeyHeld,
			/// When any event occurs.
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
		 * <param name="keyName">The name of the sequence.
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
		 * <param name="ev">The `Event` to convert.</param>
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
				if (seq.maxStep == -1) continue;

				SequenceElement elem = seq.current;
				InterruptFlags flags = getInterruptFlags(ev, elem);
				if ((elem.InterruptFlags & flags) == flags){
					seq.currentStep = 0;
					continue;
				}

				if (ev.Key == elem.Key){
					// Debug.Log(
					// 	$"Name: {name}; " +
					// 	$"Type: {elem.Type}; " +
					// 	$"EventType: {ev.Type}; " +
					// 	$"KeyCode: {ev.Key}; " +
					// 	$"Step: {seq.currentStep}; " +
					// 	$"MaxStep: {seq.maxStep}; " +
					// 	$"Duration: {ev.HeldDuration}; " +
					// 	$"Margin: {((seq.lastStepTime == 0.0f) ? 0 : Time.time - seq.lastStepTime)}; "+ 
					// 	$"Flags: {flags};"
					// );
					float duration = ev.HeldDuration;
					float last = Time.time - seq.lastStepTime;
					bool inMargin = (seq.currentStep == 0 || (last >= elem.MinMargin && last <= elem.MaxMargin));
					bool inDuration = (duration >= elem.MinDuration && duration <= elem.MaxDuration);
					if (inDuration && inMargin){
						if (ev.Type == elem.Type){
							seq.lastStepTime = Time.time;
							if (seq.currentStep++ == seq.maxStep){
								Broadcast bc = (seqEvents.ContainsKey(name)) ? seqEvents[name] : null;
								bc?.Fire();
							}
						}
					} else if (duration > elem.MaxDuration || last > elem.MaxMargin){
						seq.currentStep = 0;
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
			/// The current SequenceElement.
			internal SequenceElement current => elements[currentStep];
			/// The current step; where the sequence pointer is right now.
			internal int currentStep {
				get {
					return cStep;
				}
				set {
					if (value > maxStep) currentStep = 0;
					else cStep = value;

					if (cStep == 0)
						lastStepTime = 0.0f;
				}
			}
			/// The elements which make up the Sequence.
			internal SequenceElement[] elements { get; set; }
			/// The recorded `Time.time` from the last step's execution.
			internal float lastStepTime { get; set; } = 0.0f;
			/// The maximum amount of steps before completion and reset.
			internal int maxStep => elements.Length - 1;
			
			private int cStep = 0;

			/**
			 * <summary>
			 * Sequence indexer, for getting/setting SequenceElements.
			 * </summary>
			 * <param name="index">The integer to index the array with.
			 * Must be between 0 and `maxStep`.
			 * </param>
			 */
			public SequenceElement this[int index]{
				get {
					if (index < 0 || index > maxStep)
						throw new ArgumentOutOfRangeException("index");
					return elements[index];
				}
				set {
					if (index < 0 || index >= maxStep)
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
		}
		/**
		 * <summary>
		 * Class which represents an element in a Sequence.
		 * </summary>
		 */
		public class SequenceElement {
			/// Flags used to check if the sequence should be interrupted. (Default: `InterruptFlags.DifferentKeyDown`)
			public InterruptFlags InterruptFlags { get; set; } = InterruptFlags.DifferentKeyDown;
			/// The KeyCode to check. (Default: `KeyCode.None`)
			public KeyCode Key { get; set; } = KeyCode.None;
			/// The maximum duration of the keypress before invalidity. (Default: `float.MaxValue`)
			public float MaxDuration { get; set; } = float.MaxValue;
			/// The maximum time which can pass since the last element in the sequence before invalidity. (Default: `float.MaxValue`)
			public float MaxMargin { get; set; } = float.MaxValue;
			/// The minimum duration of the keypress for the element to be valid. (Default: `0`)
			public float MinDuration { get; set; } = 0.0f;
			/// The minimum time which has to pass since the last element in the sequence for the element to be valid. (Default: `0`)
			public float MinMargin { get; set; } = 0.0f;
			/// The element's input type. (Default: `InputEventType.KeyDown`)
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
			 * <param name="type">The element's input type. (Default: `InputEventType.KeyDown`)
			 * </param>
			 * <param name="flags">Flags used to check if the sequence should be interrupted.
			 * (Default: `InterruptFlags.DifferentKeyDown`)
			 * </param>
			 * <param name="minDuration">The minimum duration of the keypress for the element to
			 * be valid. (Default: `0`)
			 * </param>
			 * <param name="maxDuration">The maximum duration of the keypress before invalidity.
			 * (Default: `float.MaxValue`)
			 * </param>
			 * <param name="minMargin">The minimum time which has to pass since the last element
			 * in the sequence for the element to be valid. (Default: `0`)
			 * </param>
			 * <param name="maxMargin">The maximum time which can pass since the last element in
			 * the sequence before invalidity. (Default: `float.MaxValue`)
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
			/// The time, in seconds, that the key has/had been held down.
			public float HeldDuration { get; internal set; } = 0.0f;
			/// The key related to this event.
			public KeyCode Key { get; internal set; } = KeyCode.None;
			/// The input type of this event.
			public InputEventType Type { get; internal set; } = InputEventType.None;
		}
	}
}