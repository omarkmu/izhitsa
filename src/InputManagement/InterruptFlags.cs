using System;

namespace Izhitsa {
	namespace InputManagement {
		public static partial class InputManager {
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
				/// <summary>When the mouse is moved.</summary>
				MouseMove = 128,
				/// <summary>When any event occurs on a different key.</summary>
				DifferentKey = DifferentKeyUp | DifferentKeyDown | DifferentKeyHeld,
				/// <summary>When any event occurs on the same key.</summary>
				SameKey = SameKeyUp | SameKeyDown | SameKeyHeld,
				/// <summary>When any event occurs.</summary>
				Any = DifferentKey | SameKey | Scroll
			}
		}
	}
}