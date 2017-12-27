using Izhitsa.Events;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Izhitsa.InputManagement {
	/**
	 <summary>Facilitates input handling.</summary>
	 */
	public static partial class InputManager {
		/// <summary>A boolean representing whether or not either alt key is down.</summary>
		public static bool Alt
			=> Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
		/// <summary>A boolean representing whether or not either control key is down.</summary>
		public static bool Control
			=> Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
		/// <summary>If true, methods will not register/return key events.</summary>
		public static bool Paused { get; set; } = false;
		/// <summary>A boolean representing whether or not either shift key is down.</summary>
		public static bool Shift
			=> Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		///


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
		///


		private static Vector2? lastPosition;


		/**
		 <summary>
		 Binds an action string to a KeyCode.
		 </summary>
		 <param name="action">The action to bind to a key.
		 </param>
		 <param name="keyCode">The KeyCode to bind.
		 </param>
		 <param name="clear">If true, any previously bound keys to this action
		 will be removed.
		 </param>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.
		 </exception>
		 */
		public static void BindKey(string action, KeyCode keyCode, bool clear = false){
			if (action == null)
				throw new ArgumentNullException("action");
			if (boundKeys.ContainsKey(action)){
				List<KeyCode> keyList = boundKeys[action];
				if (clear){
					for (int i = 0; i < keyList.Count; i++){
						keyUnbound.Fire(action, keyList[i]);
						if (keyUnboundEvents.ContainsKey(action))
							keyUnboundEvents[action].Fire(keyList[i]);
					}
					keyList = boundKeys[action] = new List<KeyCode>();
				}
				keyList.Add(keyCode);
				keyBound.Fire(action, keyCode);
				if (keyBoundEvents.ContainsKey(action))
					keyBoundEvents[action].Fire(keyCode);
			} else {
				boundKeys.Add(action, new List<KeyCode>(){ keyCode });
				keyBound.Fire(action, keyCode);
				if (keyBoundEvents.ContainsKey(action))
					keyBoundEvents[action].Fire(keyCode);
			}
		}
		/**
		 <summary>
		 Binds an action string to KeyCodes.
		 </summary>
		 <param name="action">The action to bind to the keys.
		 </param>
		 <param name="keyCodes">The KeyCodes to bind.
		 </param>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.
		 </exception>
		 */
		public static void BindKey(string action, params KeyCode[] keyCodes){
			if (action == null)
				throw new ArgumentNullException("action");
			foreach (KeyCode key in keyCodes) BindKey(action, key, false);
		}
		/**
		 <summary>
		 Binds a name to a Sequence.
		 </summary>
		 <param name="seqName">The name to bind to the sequence.
		 </param>
		 <param name="seq">The Sequence to bind.
		 </param>
		 */
		public static Sequence BindSequence(string seqName, Sequence seq){
			if (boundSeqs.ContainsKey(seqName)){
				seqUnbound.Fire(seqName, boundSeqs[seqName]);
				if (seqUnboundEvents.ContainsKey(seqName))
					seqUnboundEvents[seqName].Fire(boundSeqs[seqName]);
				
				boundSeqs[seqName] = seq;

				seqBound.Fire(seqName, seq);
				if (seqBoundEvents.ContainsKey(seqName))
					seqBoundEvents[seqName].Fire(seq);
			} else {
				boundSeqs.Add(seqName, seq);
				seqBound.Fire(seqName, seq);
				if (seqBoundEvents.ContainsKey(seqName))
					seqBoundEvents[seqName].Fire(seq);
			}
			return seq;
		}
		/**
		 <summary>
		 Binds a name to a Sequence.
		 </summary>
		 <param name="seqName">The name to bind to the sequence.
		 </param>
		 <param name="args">The SequenceElements to convert into a sequence.
		 </param>
		 */
		public static Sequence BindSequence(string seqName, params SequenceElement[] args){
			return BindSequence(seqName, new Sequence(args));
		}
		/**
		 <summary>
		 Binds a name to a Sequence.
		 </summary>
		 <param name="seqName">The name to bind to the sequence.
		 </param>
		 <param name="args">The KeyCodes to convert into a sequence.
		 </param>
		*/
		public static Sequence BindSequence(string seqName, params KeyCode[] args){
			return BindSequence(seqName, new Sequence(args));
		}
		/**
		 <summary>
		 Associates an input axis with the name <paramref name="name"/>.
		 </summary>
		 <param name="name">The name to bind to the Axis.
		 </param>
		 <param name="axis">The Axis object to bind.
		 </param>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null.
		 </exception>
		 <exception cref="ArgumentException">Thrown if an axis named <paramref name="name"/> already exists.
		 </exception>
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
		 <summary>
		 Creates an input axis associated with the name <paramref name="name"/> using
		 a negative and positive KeyCode.
		 </summary>
		 <param name="name">The name to bind to the Axis.
		 </param>
		 <param name="negative">The negative KeyCode.
		 </param>
		 <param name="positive">The positive KeyCode.
		 </param>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null.
		 </exception>
		 <exception cref="ArgumentException">Thrown if an axis named <paramref name="name"/> already exists.
		 </exception>
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
		 <summary>
		 Creates an input axis associated with the name <paramref name="name"/> using
		 a negative and positive KeyCode.
		 </summary>
		 <param name="name">The name to bind to the Axis.
		 </param>
		 <param name="negatives">The list of negative KeyCodes.
		 </param>
		 <param name="positives">The list of positive KeyCodes.
		 </param>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null.
		 </exception>
		 <exception cref="ArgumentException">Thrown if an axis named <paramref name="name"/> already exists.
		 </exception>
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
		/**
		 <summary>
		 Returns a Dictionary&lt;string, List&lt;KeyCode&gt;&gt; containing all key bindings.
		 </summary>
		 */
		public static Dictionary<string, List<KeyCode>> GetAllBoundKeys(){
			return new Dictionary<string, List<KeyCode>>(boundKeys);
		}
		/**
		 <summary>
		 Returns the input value from an Axis.
		 </summary>
		 <param name="name">The name of the Axis object to check.
		 </param>
		 <param name="ignorePause">Should <see cref="InputManager.Paused"/> be ignored?
		 </param>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null.
		 </exception>
		 <exception cref="ArgumentException">Thrown if an axis named <paramref name="name"/> is not associated
		 with an Axis.
		 </exception>
		 */
		public static float GetAxis(string name, bool ignorePause = false){
			if (name == null)
				throw new ArgumentNullException(name);
			if (!axes.ContainsKey(name))
				throw new ArgumentException($"Axis \"{name}\" has not been defined.");
			if (Paused && !ignorePause) return 0;
			return axes[name].GetValue(true);
		}
		/**
		 <summary>
		 Returns the Axis object associated with a name.
		 </summary>
		 <param name="name">The name associated with the Axis object.
		 </param>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null.
		 </exception>
		 */
		public static Axis GetAxisObject(string name){
			if (name == null)
				throw new ArgumentNullException(name);
			return (axes.ContainsKey(name)) ? axes[name] : null;
		}
		/**
		 <summary>
		 Returns the raw input value from an Axis.
		 </summary>
		 <param name="name">The name of the Axis object to check.
		 </param>
		 <param name="ignorePause">Should <see cref="InputManager.Paused"/> be ignored?
		 </param>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null.
		 </exception>
		 <exception cref="ArgumentException">Thrown if an axis named <paramref name="name"/> is not associated
		 with an Axis.
		 </exception>
		 */
		public static float GetAxisRaw(string name, bool ignorePause = false){
			if (name == null)
				throw new ArgumentNullException(name);
			if (!axes.ContainsKey(name))
				throw new ArgumentException($"Axis \"{name}\" has not been defined.");
			if (Paused && !ignorePause) return 0;
			return axes[name].GetRawValue(true);
		}
		/**
		 <summary>
		 Returns the nth key bound to <paramref name="action"/>, returning 
		 KeyCode.None if out of bounds or if a key isn't bound to the action.
		 </summary>
		 <param name="action">
		 The name of the key to check.
		 </param>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.
		 </exception>
		 */
		public static KeyCode GetBoundKey(string action, int index = 0){
			if (action == null)
				throw new ArgumentNullException("action");
			List<KeyCode> keys = GetBoundKeys(action);
			if (keys.Count <= index || keys.Count == 0) return KeyCode.None;
			return keys[index];
		}
		/**
		 <summary>
		 Returns the keys bound to a string.
		 </summary>
		 <param name="action">
		 The name of the key to check.
		 </param>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.
		 </exception>
		 */
		public static List<KeyCode> GetBoundKeys(string action){
			if (action == null)
				throw new ArgumentNullException("action");
			if (!boundKeys.ContainsKey(action))
				return new List<KeyCode>();
			return new List<KeyCode>(boundKeys[action]);
		}
		/**
		 <summary>
		 Returns true while any of the keys bound to <paramref name="action"/> are held down.
		 </summary>
		 <param name="action">The name of the key to check.
		 </param>
		 <param name="ignorePause">Should <see cref="InputManager.Paused"/> be ignored?
		 </param>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.
		 </exception>
		 <exception cref="ArgumentException">Thrown if <paramref name="action"/> is not bound to any keys.
		 </exception>
		 */
		public static bool GetKey(string action, bool ignorePause = false){
			if (action == null)
				throw new ArgumentNullException("action");
			if (!boundKeys.ContainsKey(action))
				throw new ArgumentException($"\"{action}\" has not been bound to any keys.", "action");
			if (Paused && !ignorePause) return false;
			foreach (KeyCode key in boundKeys[action])
				if (Input.GetKey(key)) return true;
			return false;
		}
		/**
		 <summary>
		 Returns true while the key is held down.
		 </summary>
		 <param name="key">The KeyCode to check.
		 </param>
		 <param name="ignorePause">Should <see cref="InputManager.Paused"/> be ignored?
		 </param>
		 */
		public static bool GetKey(KeyCode key, bool ignorePause){
			if (Paused && !ignorePause) return false;
			return Input.GetKey(key);
		}
		/**
		 <summary>
		 Returns a list of currently held down keys.
		 </summary>
		 */
		public static List<KeyCode> GetKeysDown(){
			return new List<KeyCode>(heldKeys.Keys);
		}
		/**
		 <summary>
		 Returns the amount of time the key has been held down,
		 or -1 if the key is not currently pressed.
		 </summary>
		 */
		public static float GetKeyDuration(KeyCode key){
			if (heldKeys.ContainsKey(key)) return Time.time - heldKeys[key];
			return -1;
		}
		/**
		 <summary>
		 Returns true if any of the keys bound to <paramref name="action"/>
		 were pressed during the current frame.
		 </summary>
		 <param name="action">The name of the key to check.
		 </param>
		 <param name="ignorePause">Should <see cref="InputManager.Paused"/> be ignored?
		 </param>
		 <returns>A boolean representing whether or not the key was pressed.
		 </returns>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.
		 </exception>
		 <exception cref="ArgumentException">Thrown if <paramref name="action"/> is not bound to any keys.
		 </exception>
		 */
		public static bool KeyDown(string action, bool ignorePause = false){
			if (action == null)
				throw new ArgumentNullException("action");
			if (!boundKeys.ContainsKey(action))
				throw new ArgumentException($"\"{action}\" has not been bound to any keys.", "action");
			if (Paused && !ignorePause) return false;
			foreach (KeyCode key in boundKeys[action])
				if (Input.GetKeyDown(key)) return true;
			return false;
		}
		/**
		 <summary>
		 Returns true if the key was pressed during the current frame.
		 </summary>
		 <param name="key">The KeyCode to check.
		 </param>
		 <param name="ignorePause">Should <see cref="InputManager.Paused"/> be ignored?
		 </param>
		 */
		public static bool KeyDown(KeyCode key, bool ignorePause = false){
			if (Paused && !ignorePause) return false;
			return Input.GetKeyDown(key);
		}
		/**
		 <summary>
		 Checks if any of the keys bound to <paramref name="action"/> were released during the frame.
		 </summary>
		 <param name="action">The name of the key to check.
		 </param>
		 <param name="ignorePause">Should <see cref="InputManager.Paused"/> be ignored?
		 </param>
		 <returns>A boolean representing whether or not the key was released.
		 </returns>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.
		 </exception>
		 <exception cref="ArgumentException">Thrown if <paramref name="action"/> is not bound to any keys.
		 </exception>
		 */
		public static bool KeyUp(string action, bool ignorePause = false){
			if (action == null)
				throw new ArgumentNullException("action");
			if (!boundKeys.ContainsKey(action))
				throw new ArgumentException($"\"{action}\" has not been bound to any keys.", "action");
			if (Paused && !ignorePause) return false;
			foreach (KeyCode key in boundKeys[action])
				if (Input.GetKeyUp(key)) return true;
			return false;
		}
		/**
		 <summary>
		 Returns true if the key was released during the current frame.
		 </summary>
		 <param name="key">The KeyCode to check.
		 </param>
		 <param name="ignorePause">Should <see cref="InputManager.Paused"/> be ignored?
		 </param>
		 */
		public static bool KeyUp(KeyCode key, bool ignorePause = false){
			if (Paused && !ignorePause) return false;
			return Input.GetKeyUp(key);
		}
		/**
		 <summary>
		 Returns true if <paramref name="seq"/> was completed during the current frame.
		 </summary>
		 <param name="seq">A sequence to check.
		 </param>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="seq"/> is null.
		 </exception>
		 */
		public static bool SequenceCompleted(Sequence seq){
			if (seq == null)
				throw new ArgumentNullException("seq");
			return (seq.completionFrame == Time.frameCount);
		}
		/**
		 <summary>
		 Returns true if the sequence bound to <paramref name="action"/> was completed during
		 the current frame.
		 </summary>
		 <param name="action">The action to check.
		 </param>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.
		 </exception>
		 <exception cref="ArgumentException">Thrown if <paramref name="action"/> is not bound to a Sequence.
		 </exception>
		 */
		public static bool SequenceCompleted(string action){
			if (action == null)
				throw new ArgumentNullException("action");
			if (!boundSeqs.ContainsKey(action))
				throw new ArgumentException($"\"{action}\" has not been bound to a Sequence.", "action");
			return (boundSeqs[action].completionFrame == Time.frameCount);
		}
		/**
		 <summary>
		 Unbinds a key.
		 </summary>
		 <param name="action">The action to unbind from.
		 </param>
		 <param name="keyToUnbind">The KeyCode to unbind.
		 </param>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.
		 </exception>
		 <exception cref="ArgumentException">Thrown if <paramref name="action"/> is not bound to any keys.
		 </exception>
		 */
		public static void UnbindKey(string action, KeyCode keyToUnbind){
			if (action == null)
				throw new ArgumentNullException("action");
			if (!boundKeys.ContainsKey(action))
				throw new ArgumentException($"\"{action}\" has not been bound to any keys.", "action");
			while (boundKeys[action].Contains(keyToUnbind)){
				boundKeys[action].Remove(keyToUnbind);
				keyUnbound.Fire(action, keyToUnbind);
				if (keyUnboundEvents.ContainsKey(action))
					keyUnboundEvents[action].Fire(keyToUnbind);
			}
		}
		/**
		 <summary>
		 Unbinds a key.
		 </summary>
		 <param name="action">The action to unbind from.
		 </param>
		 <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.
		 </exception>
		 <exception cref="ArgumentException">Thrown if <paramref name="action"/> is not bound to any keys.
		 </exception>
		 */
		public static void UnbindKeys(string action){
			if (action == null)
				throw new ArgumentNullException("action");
			if (!boundKeys.ContainsKey(action))
				throw new ArgumentException($"\"{action}\" has not been bound to any keys.", "action");
			foreach (KeyCode key in boundKeys[action]){
				keyUnbound.Fire(action, key);
				if (keyUnboundEvents.ContainsKey(action))
					keyUnboundEvents[action].Fire(key);
			}
			boundKeys.Remove(action);
		}
		

		/**
		 <summary>
		 Handles events, converts them into InputEvents,
		 and registers them for Sequence checking and other input-related
		 jobs.
		 </summary>
		 <param name="ev">The Event to convert and handle.</param>
		 */
		internal static void handleEvent(Event ev){
			if (Paused) return;
			bool valid = true;

			int button = -1;
			float heldDuration = 0.0f;
			Vector2 delta = Vector2.zero;
			Vector2 position = ev.mousePosition;
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
					type = InputEventType.KeyDown;
					if (button > 6){
						key = KeyCode.None;
						valid = false;
						break;
					} else {
						key = (KeyCode)(323 + button);
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
					button = ev.button;
					type = InputEventType.KeyUp;
					if (ev.button > 6){
						key = KeyCode.None;
						valid = false;
						break;
					} else {
						key = (KeyCode)(323 + ev.button);
					}

					if (heldKeys.ContainsKey(key)){
						float time = heldKeys[key];
						heldKeys.Remove(key);
						heldDuration = Time.time - time;
					}
					break;
				case EventType.MouseMove:
					if (ev.mousePosition != lastPosition){
						type = InputEventType.MouseMove;
						delta = (lastPosition == null) ?
							Vector2.zero :
							ev.mousePosition - (Vector2)lastPosition;
						lastPosition = position = ev.mousePosition;
					} else {
						valid = false;
					}
					break;
				case EventType.ScrollWheel:
					type = InputEventType.Scroll;
					delta = ev.delta;
					break;
			}

			Dictionary<KeyCode, float> keyTimes = new Dictionary<KeyCode, float>(heldKeys);
			List<KeyCode> keys = new List<KeyCode>();
			foreach(KeyCode k in keyTimes.Keys) keys.Add(k);

			InputEvent iEvent = new InputEvent(
				button,
				heldDuration,
				key,
				type,
				delta,
				position,
				keys,
				keyTimes
			);

			if (type == InputEventType.Scroll){
				scrollEvent.Fire(iEvent);
			} else if (valid && type == InputEventType.MouseMove){
				mouseMove.Fire(iEvent);
			} else if (button != -1){
				mouseEvent.Fire(iEvent);
				if (mouseEvents.ContainsKey(button))
					mouseEvents[button].Fire(iEvent);
			} else {
				keyEvent.Fire(iEvent);
				if (keyEvents.ContainsKey(key))
					keyEvents[key].Fire(iEvent);
			}

			if (valid || (!valid && ev.type != EventType.MouseMove)){
				input.Fire(iEvent);
			}

			if (valid) registerEvent(iEvent);
		}


		/**
		 <summary>
		 Checks <paramref name="keysDown"/> to see if any of the <paramref name="modifier"/>
		 keys are contained.
		 </summary>
		 */
		private static bool contains(List<KeyCode> keysDown, InputModifiers modifier){
			switch (modifier){
				case InputModifiers.Control:
					return keysDown.Contains(KeyCode.LeftControl) || keysDown.Contains(KeyCode.RightControl);
				case InputModifiers.Alt:
					return keysDown.Contains(KeyCode.LeftAlt) || keysDown.Contains(KeyCode.RightAlt);
				case InputModifiers.Shift:
					return keysDown.Contains(KeyCode.LeftShift) || keysDown.Contains(KeyCode.RightShift);
				case InputModifiers.Windows:
					return keysDown.Contains(KeyCode.LeftWindows) || keysDown.Contains(KeyCode.RightWindows);
				case InputModifiers.Command:
					return keysDown.Contains(KeyCode.LeftCommand) || keysDown.Contains(KeyCode.RightCommand);
				case InputModifiers.Apple:
					return keysDown.Contains(KeyCode.LeftApple) || keysDown.Contains(KeyCode.RightApple);
				default:
					return false;
			}
		}
		/**
		 <summary>
		 Returns the InterruptFlags which are true for the
		 event-element pair.
		 </summary>
		 <param name="ev">The InputEvent.
		 </param>
		 <param name="elem">The SequenceElement.
		 </param>
		 */
		private static InterruptFlags getInterruptFlags(InputEvent ev, SequenceElement elem, ref bool isModifier){
			InterruptFlags flags = InterruptFlags.None;
			InputModifiers modifiers = elem.InputModifiers;

			if (ev.Type == InputEventType.MouseMove) flags = InterruptFlags.MouseMove;
			if (ev.Type == InputEventType.Scroll) flags = InterruptFlags.Scroll;
			if (ev.Key == elem.Key){
				if (ev.Type == InputEventType.KeyUp) flags = InterruptFlags.SameKeyUp;
				if (ev.Type == InputEventType.KeyDown) flags = InterruptFlags.SameKeyDown;
				if (ev.Type == InputEventType.KeyHeld) flags = InterruptFlags.SameKeyHeld;
			} else {
				if (ev.Type == InputEventType.KeyUp) flags = InterruptFlags.DifferentKeyUp;
				if (ev.Type == InputEventType.KeyDown) flags = InterruptFlags.DifferentKeyDown;
				if (ev.Type == InputEventType.KeyHeld) flags = InterruptFlags.DifferentKeyHeld;
				if ((elem.InterruptFlags & InterruptFlags.NoIgnoreModifiers) != 0) return flags;

				if (modifiers != InputModifiers.None){
					if ((modifiers & InputModifiers.Control) != 0){
						if (ev.Key == KeyCode.LeftControl || ev.Key == KeyCode.RightControl){
							isModifier = true;
						}
					}
					if ((modifiers & InputModifiers.Alt) != 0){
						if (ev.Key == KeyCode.LeftAlt || ev.Key == KeyCode.RightAlt){
							isModifier = true;
						}
					}
					if ((modifiers & InputModifiers.Shift) != 0){
						if (ev.Key == KeyCode.LeftShift || ev.Key == KeyCode.RightShift){
							isModifier = true;
						}
					}
					if ((modifiers & InputModifiers.Windows) != 0){
						if (ev.Key == KeyCode.LeftWindows || ev.Key == KeyCode.RightWindows){
							isModifier = true;
						}
					}
					if ((modifiers & InputModifiers.Command) != 0){
						if (ev.Key == KeyCode.LeftCommand || ev.Key == KeyCode.RightCommand){
							isModifier = true;
						}
					}
					if ((modifiers & InputModifiers.Apple) != 0){
						if (ev.Key == KeyCode.LeftApple || ev.Key == KeyCode.RightApple){
							isModifier = true;
						}
					}
				}
				foreach (KeyCode key in elem.modifiers){
					if (ev.Key == key){
						isModifier = true;
						break;
					}
				}
			}

			if (isModifier){
				switch (flags){
					case InterruptFlags.DifferentKeyUp:
						return InterruptFlags.ModifierKeyUp;
					case InterruptFlags.DifferentKeyDown:
						return InterruptFlags.ModifierKeyDown;
					case InterruptFlags.DifferentKeyHeld:
						return InterruptFlags.ModifierKeyHeld;
				}
			}
			return flags;
		}
		private static bool valid(InputEvent ev, SequenceElement elem, out bool isModifier){
			bool oneDown = false;
			bool noModifiers = true;
			isModifier = false;
			InputModifiers modifiers = elem.InputModifiers;
			InputModifierType type = elem.ModifierType;
			List<KeyCode> keysDown = ev.KeysDown;
			Dictionary<KeyCode, float> keyDurations = ev.KeyTimes;

			InterruptFlags flags = getInterruptFlags(ev, elem, ref isModifier);
			if ((elem.InterruptFlags & flags) != flags) {
				if (ev.Key == elem.Key){
					if (modifiers != InputModifiers.None){
						noModifiers = false;
						if ((modifiers & InputModifiers.Control) != 0){
							if (!contains(keysDown, InputModifiers.Control)){
								if (type == InputModifierType.All) return false;
							} else {
								if (type == InputModifierType.One && oneDown) return false;
								oneDown = true;
							}
						}
						if ((modifiers & InputModifiers.Alt) != 0){
							if (!contains(keysDown, InputModifiers.Alt)){
								if (type == InputModifierType.All) return false;
							} else {
								if (type == InputModifierType.One && oneDown) return false;
								oneDown = true;
							}
						}
						if ((modifiers & InputModifiers.Shift) != 0){
							if (!contains(keysDown, InputModifiers.Shift)){
								if (type == InputModifierType.All) return false;
							} else {
								if (type == InputModifierType.One && oneDown) return false;
								oneDown = true;
							}
						}
						if ((modifiers & InputModifiers.Windows) != 0){
							if (!contains(keysDown, InputModifiers.Windows)){
								if (type == InputModifierType.All) return false;
							} else {
								if (type == InputModifierType.One && oneDown) return false;
								oneDown = true;
							}
						}
						if ((modifiers & InputModifiers.Command) != 0){
							if (!contains(keysDown, InputModifiers.Command)){
								if (type == InputModifierType.All) return false;
							} else {
								if (type == InputModifierType.One && oneDown) return false;
								oneDown = true;
							}
						}
						if ((modifiers & InputModifiers.Apple) != 0){
							if (!contains(keysDown, InputModifiers.Apple)){
								if (type == InputModifierType.All) return false;
							} else {
								if (type == InputModifierType.One && oneDown) return false;
								oneDown = true;
							}
						}
					}

					float min = float.MinValue;
					foreach (KeyCode key in elem.modifiers){
						noModifiers = false;
						if (keyDurations.ContainsKey(key)){
							if (type == InputModifierType.One && oneDown)
								return false;
							if (type == InputModifierType.Ordered && keyDurations[key] < min)
								return false;
							min = keyDurations[key];
							oneDown = true;
						} else {
							if (type == InputModifierType.All) return false;
							if (type == InputModifierType.Ordered) return false;
						}
					}
					return noModifiers ? true : oneDown;
				} else {
					return true;
				}
			} else {
				isModifier = false;
			}
			return false;
		}
		/**
		 <summary>
		 Registers an InputEvent with valid Sequences.
		 </summary>
		 <param name="ev">The InputEvent to register.</param>
		 */
		private static void registerEvent(InputEvent ev){
			foreach (KeyValuePair<string, Sequence> pair in boundSeqs){
				string name = pair.Key;
				Sequence seq = pair.Value;
				if (seq == null) continue;
				if (seq.MaxStep < 0) continue;

				bool isModifier;
				SequenceElement elem = seq.Current;
				if (!valid(ev, elem, out isModifier)){
					if (!isModifier)
						seq.Reset();
					continue;
				}
				if (isModifier) continue;

				if (ev.Key == elem.Key && ev.Type == elem.Type){
					float duration = ev.HeldDuration;
					float deltaX = ev.Delta.x;
					float deltaY = ev.Delta.y;
					float last = Time.time - seq.lastStepTime;

					bool inMargin = (seq.CurrentStep == 0 || (last >= elem.MinMargin && last <= elem.MaxMargin));
					bool inDuration = (duration >= elem.MinDuration && duration <= elem.MaxDuration);
					bool inDelta =
						(deltaX >= elem.MinDeltaX && deltaX <= elem.MaxDeltaX) &&
						(deltaY >= elem.MinDeltaY && deltaY <= elem.MaxDeltaY);

					if (inDuration && inMargin && inDelta){
						seq.lastStepTime = Time.time;
						if (seq.CurrentStep++ == seq.MaxStep){
							seq.completionFrame = Time.frameCount;
							Broadcast bc = (seqEvents.ContainsKey(name)) ? seqEvents[name] : null;
							bc?.Fire();
						}
					} else if (duration > elem.MaxDuration || !inMargin || !inDelta){
						seq.Reset();
					}
				}
			}
		}
	}
}