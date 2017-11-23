using UnityEngine;

namespace Izhitsa {
	namespace InputManagement {
		public static partial class InputManager {
			/// <summary>Contains information about input events.</summary>
			public struct InputEvent {
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
		}
	}
}