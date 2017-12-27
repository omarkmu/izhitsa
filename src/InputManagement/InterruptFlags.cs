namespace Izhitsa.InputManagement {
	/// <summary>Flags which represent when a sequence should be interrupted and provide
	/// additional options for modifiers.</summary>
	[System.Flags]
	public enum InterruptFlags {
		/// <summary>Never interrupt.</summary>
		None = 0,
		/// <summary>When a different key is released.</summary>
		DifferentKeyUp = 1,
		/// <summary>When a different key is pressed.</summary>
		DifferentKeyDown = 2,
		/// <summary>When a different key is held down.</summary>
		DifferentKeyHeld = 4,
		/// <summary>When any event occurs on a different key.</summary>
		DifferentKey = DifferentKeyUp | DifferentKeyDown | DifferentKeyHeld,
		/// <summary>When the same key is released.</summary>
		SameKeyUp = 8,
		/// <summary>When the same key is pressed..?</summary>
		SameKeyDown = 16,
		/// <summary>When the same key is held down.</summary>
		SameKeyHeld = 32,
		/// <summary>When any event occurs on the same key.</summary>
		SameKey = SameKeyUp | SameKeyDown | SameKeyHeld,
		/// <summary>When the mouse wheel is scrolled.</summary>
		Scroll = 64,
		/// <summary>When the mouse is moved.</summary>
		MouseMove = 128,
		/// <summary>When a key which is a modifier in the SequenceElement has been released.</summary>
		ModifierKeyUp = 256,
		/// <summary>When a key which is a modifier in the SequenceElement has been pressed.</summary>
		ModifierKeyDown = 512,
		/// <summary>When a key which is a modifier in the SequenceElement is held down.</summary>
		ModifierKeyHeld = 1024,
		/// <summary>When any event occurs on a key which is a modifier in the SequenceElement.</summary>
		ModifierKey = ModifierKeyUp | ModifierKeyDown | ModifierKeyHeld,
		/// <summary>Interrupt even if the key is a modifier in the SequenceElement.</summary>
		NoIgnoreModifiers = 2048,
		/// <summary>When any event occurs.</summary>
		Any = DifferentKey | SameKey | ModifierKey | Scroll | MouseMove
	}
}