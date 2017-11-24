using Izhitsa.Events;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Izhitsa {
	namespace InputManagement {
		/**
		 * <summary>Facilitates input handling.</summary>
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
				foreach (KeyCode key in keyCodes) BindKey(action, key, false);
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
			 * <param name="negatives">The list of negative KeyCodes.
			 * </param>
			 * <param name="positives">The list of positive KeyCodes.
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
			/**
			 * <summary>
			 * Returns the input value from an Axis.
			 * </summary>
			 * <param name="name">The name of the Axis object to check.
			 * </param>
			 * <param name="ignorePause">Should `<see cref="InputManager.Paused"/>` be ignored?
			 * </param>
			 * <exception cref="ArgumentNullException">Thrown if `<paramref name="name"/>` is null.
			 * </exception>
			 * <exception cref="ArgumentException">Thrown if an axis named `<paramref name="name"/>` is not associated
			 * with an Axis.
			 * </exception>
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
			 * <param name="ignorePause">Should `<see cref="InputManager.Paused"/>` be ignored?
			 * </param>
			 * <exception cref="ArgumentNullException">Thrown if `<paramref name="name"/>` is `null`.
			 * </exception>
			 * <exception cref="ArgumentException">Thrown if an axis named `<paramref name="name"/>` is not associated
			 * with an Axis.
			 * </exception>
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
			 * <summary>
			 * Returns the first key bound to a string.
			 * </summary>
			 * <param name="action">
			 * The name of the key to check.
			 * </param>
			 * <exception cref="ArgumentNullException">Thrown if `<paramref name="action"/>` is `null`.
			 * </exception>
			 */
			public static KeyCode GetBoundKey(string action){
				if (action == null)
					throw new ArgumentNullException("action");
				List<KeyCode> keys = GetBoundKeys(action);
				if (keys.Count == 0) return KeyCode.None;
				return keys[0];
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
			 * <param name="ignorePause">Should `<see cref="InputManager.Paused"/>` be ignored?
			 * </param>
			 * <exception cref="ArgumentNullException">Thrown if `<paramref name="action"/>` is `null`.
			 * </exception>
			 * <exception cref="ArgumentException">Thrown if `<paramref name="action"/>` is not bound to any keys.
			 * </exception>
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
			 * <summary>
			 * Checks if any of the keys bound to `<paramref name="action"/>` were pressed during the frame.
			 * </summary>
			 * <param name="action">The name of the key to check.
			 * </param>
			 * <param name="ignorePause">Should `<see cref="InputManager.Paused"/>` be ignored?
			 * </param>
			 * <returns>A boolean representing whether or not the key was pressed.
			 * </returns>
			 * <exception cref="ArgumentNullException">Thrown if `<paramref name="action"/>` is `null`.
			 * </exception>
			 * <exception cref="ArgumentException">Thrown if `<paramref name="action"/>` is not bound to any keys.
			 * </exception>
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
			 * <summary>
			 * Checks if any of the keys bound to `<paramref name="action"/>` were released during the frame.
			 * </summary>
			 * <param name="action">The name of the key to check.
			 * </param>
			 * <param name="ignorePause">Should `<see cref="InputManager.Paused"/>` be ignored?
			 * </param>
			 * <returns>A boolean representing whether or not the key was released.
			 * </returns>
			 * <exception cref="ArgumentNullException">Thrown if `<paramref name="action"/>` is `null`.
			 * </exception>
			 * <exception cref="ArgumentException">Thrown if `<paramref name="action"/>` is not bound to any keys.
			 * </exception>
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
				while (boundKeys[action].Contains(keyToUnbind)){
					boundKeys[action].Remove(keyToUnbind);
					keyUnbound.Fire(action, keyToUnbind);
					if (keyUnboundEvents.ContainsKey(action))
						keyUnboundEvents[action].Fire(keyToUnbind);
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
				foreach (KeyCode key in boundKeys[action]){
					keyUnbound.Fire(action, key);
					if (keyUnboundEvents.ContainsKey(action))
						keyUnboundEvents[action].Fire(key);
				}
				boundKeys.Remove(action);
			}
			
			/**
			 * <summary>
			 * Handles events, converts them into `InputEvent`s,
			 * and registers them for `Sequence` checking and other input-related
			 * jobs.
			 * </summary>
			 * <param name="ev">The `Event` to convert and handle.</param>
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

				InputEvent iEvent = new InputEvent(button, heldDuration, key, type, delta, position);

				if (type == InputEventType.Scroll){
					scrollEvent.Fire(iEvent);
				} else if (type == InputEventType.MouseMove){
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
				if (ev.Type == InputEventType.MouseMove) flags = InterruptFlags.MouseMove;
				if (ev.Type == InputEventType.Scroll) flags = InterruptFlags.Scroll;
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
		}
	}
}