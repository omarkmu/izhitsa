using UnityEngine;

namespace Izhitsa {
	public static partial class InputManager {
		/**
		 * <summary>
		 * Class which represents an element in a Sequence.
		 * </summary>
		 */
		public class SequenceElement {
			/// <summary>Flags used to check if the sequence should be interrupted.
			/// `<see cref="InterruptFlags.DifferentKeyDown"/>` by default.</summary>
			public InterruptFlags InterruptFlags { get; set; } = InterruptFlags.DifferentKeyDown;
			/// <summary>The KeyCode to check. `<see cref="KeyCode.None"/>` by default.</summary>
			public KeyCode Key { get; set; } = KeyCode.None;
			/// <summary>The maximum scroll wheel delta before invalidity.</summary>
			/// `<see cref="float.MaxValue"/>` by default.</summary>
			public float MaxDelta { get; set; } = float.MaxValue;
			/// <summary>The maximum duration of the keypress before invalidity.
			/// `<see cref="float.MaxValue"/>` by default.</summary>
			public float MaxDuration { get; set; } = float.MaxValue;
			/// <summary>The maximum time which can pass since the last element in the sequence before invalidity.
			/// `<see cref="float.MaxValue"/>` by default.</summary>
			public float MaxMargin { get; set; } = float.MaxValue;
			/// <summary>The minimum scroll wheel delta for the sequence to be valid.</summary>
			/// `<see cref="float.MinValue"/>` by default.</summary>
			public float MinDelta { get; set; } = float.MinValue;
			/// <summary>The minimum duration of the keypress for the element to be valid. `0` by default.</summary>
			public float MinDuration { get; set; } = 0.0f;
			/// <summary>The minimum time which has to pass since the last element in the sequence for the element to be valid.
			/// `0` by default.</summary>
			public float MinMargin { get; set; } = 0.0f;
			/// <summary>The element's input type. `<see cref="InputEventType.KeyDown"/>` by default.</summary>
			public InputEventType Type { get; set; } = InputEventType.KeyDown;
			
			/**
			 * <summary>Creates a default SequenceElement.</summary>
			 */
			public SequenceElement(){}
			/**
			 * <summary>
			 * Creates a SequenceElement.
			 * </summary>
			 * <param name="key">The KeyCode to check.
			 * </param>
			 * <param name="type">The element's input type.
			 * </param>
			 * <param name="flags">Flags used to check if the sequence should be interrupted.
			 * </param>
			 * <param name="minDuration">The minimum duration of the keypress for the element to
			 * be valid.
			 * </param>
			 * <param name="maxDuration">The maximum duration of the keypress before invalidity.
			 * </param>
			 * <param name="minMargin">The minimum time which has to pass since the last element
			 * in the sequence for the element to be valid.
			 * </param>
			 * <param name="maxMargin">The maximum time which can pass since the last element in
			 * the sequence before invalidity.
			 * </param>
			 */
			public SequenceElement(KeyCode key, InputEventType type = InputEventType.KeyDown,
				InterruptFlags flags = InterruptFlags.DifferentKeyDown, float minDuration = 0.0f,
				float maxDuration = float.MaxValue, float minMargin = 0.0f, float maxMargin = float.MaxValue,
				float minDelta = float.MinValue, float maxDelta = float.MaxValue
			){
				Key = key;
				Type = type;
				InterruptFlags = flags;
				MinDuration = minDuration;
				MaxDuration = maxDuration;
				MinMargin = minMargin;
				MaxMargin = maxMargin;
				MinDelta = minDelta;
				MaxDelta = maxDelta;
			}
		}
	}
}