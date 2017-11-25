using static Izhitsa.InputManagement.InputManager;
using System.Collections.Generic;
using UnityEngine;

namespace Izhitsa {
	namespace InputManagement {
		/**
		 <summary>
		 Represents an element in a Sequence.
		 </summary>
		 */
		public class SequenceElement {
			/// <summary>InputModifiers to be checked while checking the sequence.</summary>
			public InputModifiers InputModifiers { get; set; }
			/// <summary>Flags used to check if the sequence should be interrupted.
			/// <see cref="InterruptFlags.DifferentKeyDown"/> by default.</summary>
			public InterruptFlags InterruptFlags { get; set; }
			/// <summary>The KeyCode to check. <see cref="KeyCode.None"/> by default.</summary>
			public KeyCode Key { get; set; } = KeyCode.None;
			/// <summary>The maximum mouse X delta for the sequence to be valid.
			/// <see cref="float.MaxValue"/> by default.</summary>
			public float MaxDeltaX { get; set; } = float.MaxValue;
			/// <summary>The maximum mouse Y delta for the sequence to be valid.
			/// <see cref="float.MaxValue"/> by default.</summary>
			public float MaxDeltaY { get; set; } = float.MaxValue;
			/// <summary>The maximum duration of the keypress before invalidity.
			/// <see cref="float.MaxValue"/> by default.</summary>
			public float MaxDuration { get; set; } = float.MaxValue;
			/// <summary>The maximum time which can pass since the last element in the sequence before invalidity.
			/// <see cref="float.MaxValue"/> by default.</summary>
			public float MaxMargin { get; set; } = float.MaxValue;
			/// <summary>The minimum mouse X delta for the sequence to be valid.
			/// <see cref="float.MinValue"/> by default.</summary>
			public float MinDeltaX { get; set; } = float.MinValue;
			/// <summary>The minimum mouse Y delta for the sequence to be valid.
			/// <see cref="float.MinValue"/> by default.</summary>
			public float MinDeltaY { get; set; } = float.MinValue;
			/// <summary>The minimum duration of the keypress for the element to be valid. 0 by default.</summary>
			public float MinDuration { get; set; } = 0.0f;
			/// <summary>The minimum time which has to pass since the last element in the sequence for the element to be valid.
			/// 0 by default.</summary>
			public float MinMargin { get; set; } = 0.0f;
			/// <summary>Modifier keys which should be down at the time of this event.</summary>
			public List<KeyCode> Modifiers {
				get { return new List<KeyCode>(modifiers); }
				set {
					if (value == null){
						modifiers = new KeyCode[0];
						return;
					}
					modifiers = value.ToArray();
				}
			}
			/// <summary>InputModifierType which describes how modifiers should be handled.
			/// <see cref="InputModifierType.All"/> by default.</summary>
			public InputModifierType ModifierType { get; set; }
			/// <summary>The element's input type. <see cref="InputEventType.KeyDown"/> by default.</summary>
			public InputEventType Type { get; set; } = InputEventType.KeyDown;


			internal KeyCode[] modifiers;
			

			/**
			 <summary>
			 Creates a SequenceElement.
			 </summary>
			 <param name="key">The KeyCode to check.
			 </param>
			 <param name="type">The element's input type.
			 </param>
			 <param name="flags">Flags used to check if the sequence should be interrupted.
			 </param>
			 <param name="minDuration">The minimum duration of the keypress for the element to
			 be valid.
			 </param>
			 <param name="maxDuration">The maximum duration of the keypress before invalidity.
			 </param>
			 <param name="minMargin">The minimum time which has to pass since the last element
			 in the sequence for the element to be valid.
			 </param>
			 <param name="maxMargin">The maximum time which can pass since the last element in
			 the sequence before invalidity.
			 </param>
			 <param name="minXDelta">The minimum mouse X delta for the sequence to be valid.
			 </param>
			 <param name="maxXDelta">The maximum mouse X delta for the sequence to be valid.
			 </param>
			 <param name="minYDelta">The minimum mouse Y delta for the sequence to be valid.
			 </param>
			 <param name="minYDelta">The minimum mouse Y delta for the sequence to be valid.
			 </param>
			 <param name="modifiers">Keys to use as modifiers for the element.
			 </param>
			 <param name="inputModifiers">InputModifiers to use as modifiers for the element.
			 </param>
			 <param name="modifierType">The modifier type, for <paramref name="inputModifiers"/>.
			 </param>
			 */
			public SequenceElement(KeyCode key = KeyCode.None, InputEventType type = InputEventType.KeyDown,
				InterruptFlags flags = InterruptFlags.DifferentKeyDown, float minDuration = 0.0f,
				float maxDuration = float.MaxValue, float minMargin = 0.0f, float maxMargin = float.MaxValue,
				float minXDelta = float.MinValue, float maxXDelta = float.MaxValue, float minYDelta = float.MinValue,
				float maxYDelta = float.MaxValue, List<KeyCode> modifiers = null, InputModifiers inputModifiers = InputModifiers.None,
				InputModifierType modifierType = InputModifierType.All
			){
				Key = key;
				Type = type;
				InterruptFlags = flags;
				MinDuration = minDuration;
				MaxDuration = maxDuration;
				MinMargin = minMargin;
				MaxMargin = maxMargin;
				MinDeltaX = minXDelta;
				MaxDeltaX = maxXDelta;
				MinDeltaY = minYDelta;
				MaxDeltaY = maxYDelta;
				Modifiers = modifiers;
				InputModifiers = inputModifiers;
				ModifierType = modifierType;
			}
		}
	}
}