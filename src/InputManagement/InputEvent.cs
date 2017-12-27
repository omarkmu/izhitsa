using System.Collections.Generic;
using UnityEngine;

namespace Izhitsa.InputManagement {
	public static partial class InputManager {
		/// <summary>Contains information about input events.</summary>
		public struct InputEvent {
			/// <summary>The mouse button this event occurred on. If it equals -1, this isn't a mouse event.</summary>
			public int Button { get; internal set; }
			/// <summary>The mouse delta, used for scroll and move events.</summary>
			public Vector2 Delta { get; internal set; }
			/// <summary>The time, in seconds, that the key has been held down.</summary>
			public float HeldDuration { get; internal set; }
			/// <summary>The key related to this event.</summary>
			public KeyCode Key { get; internal set; }
			/// <summary>The time the keys in <see cref="KeysDown"/> were initially pressed.</summary>
			public Dictionary<KeyCode, float> KeyTimes { get; internal set; }
			/// <summary>The keys which were down at the time of the event.</summary>
			public List<KeyCode> KeysDown { get; internal set; }
			/// <summary>The mouse position, on applicable event types.</summary>
			public Vector2 Position { get; internal set; }
			/// <summary>The input type of this event.</summary>
			public InputEventType Type { get; internal set; }


			/**
			 <summary>
			 Creates an InputEvent.
			 </summary>
			 */
			public InputEvent(int button, float heldDuration, KeyCode key,
				InputEventType type, Vector2 delta, Vector2 position,
				List<KeyCode> keysDown, Dictionary<KeyCode, float> keyTimes
			){
				Button = button;
				HeldDuration = heldDuration;
				Key = key;
				Type = type;
				Delta = delta;
				Position = position;
				KeysDown = keysDown;
				KeyTimes = keyTimes;
			}


			/**
			 <summary>
			 Returns the InputEvent in a displayable format.
			 </summary>
			 */
			public override string ToString(){
				string ret = $"Type: {Type}";
				switch (Type){
					case InputEventType.KeyUp:
					case InputEventType.KeyDown:
					case InputEventType.KeyHeld:
						ret += $", Key: {Key}, HeldDuration: {HeldDuration}";
						if (Button != -1) ret += $", Button: {Button}, Position: {Position}";
						break;
					case InputEventType.MouseMove:
					case InputEventType.Scroll:
						ret += $", Position {Position}, Delta: {Delta}";
						break;
				}
				return ret;
			}
		}
	}
}