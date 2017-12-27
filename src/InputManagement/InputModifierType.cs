namespace Izhitsa.InputManagement {
	/**
	 <summary>Describes how input modifiers should be handled.</summary>
		*/
	public enum InputModifierType {
		/// <summary>All modifier keys must be held down.</summary>
		All,
		/// <summary>All modifier keys must be held down, and pressed in order.
		/// This only works with <see cref="SequenceElement.Modifiers"/>.</summary>
		Ordered,
		/// <summary>At least one modifier key must be held down.</summary>
		Any,
		/// <summary>Only one modifier key can be held down.</summary>
		One
	}
}