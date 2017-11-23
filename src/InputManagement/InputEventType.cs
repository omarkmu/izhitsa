namespace Izhitsa {
	namespace InputManagement {
		public static partial class InputManager {
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
		}
	}
}