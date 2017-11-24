using System;

namespace Izhitsa {
	namespace InputManagement {
		[Flags]
		public enum InputModifiers {
			/// <summary>No modifiers.</summary>
			None = 0,
			/// <summary>Either Alt key.</summary>
			Alt = 1,
			/// <summary>Either Control key.</summary>
			Control = 2,
			/// <summary>Either Shift key.</summary>
			Shift = 4,
			/// <summary>Either Windows key.</summary>
			Windows = 8,
			/// <summary>The Command key.</summary>
			Command = 16,
			/// <summary>The Apple Command key.</summary>
			Apple = 32,
			/// <summary>Control + Alt.</summary>
			ControlAlt = Control | Alt,
			/// <summary>A Windows or command key.
			/// ModifierType should be `<see cref="InputModifierType.Any"/>` or `<see cref="InputModifierType.One"/>`
			/// for this to function as "or" rather than "and".</summary>
			WindowsOrCommand = Windows | Command | Apple
		}
	}
}