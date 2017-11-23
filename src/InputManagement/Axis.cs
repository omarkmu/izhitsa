using System.Collections.Generic;
using UnityEngine;

namespace Izhitsa {
	namespace InputManagement {
		/**
		 * <summary>Represents an input axis.</summary>
		 */
		public class Axis {
			/// <summary>If true, the raw value of the input can factor in multiple positive and negative key values.</summary>
			public bool Additive { get; set; } = false;
			/// <summary>Any numbers less than this value will register as zero.</summary>
			public float Dead { get; set; } = 0.001f;
			/// <summary>How fast the input will recenter.</summary>
			public float Gravity { get; set; } = 3f;
			/// <summary>If true, negative keys send positive values and vice versa.</summarY>
			public bool Invert { get; set; } = false;
			/// <summary>The keys to use as negative inputs.</summary>
			public List<KeyCode> NegativeKeys { get; set; }
			/// <summary>The keys to use as positive inputs.</summary>
			public List<KeyCode> PositiveKeys { get; set; }
			/// <summary>How fast the input will become 1 or -1.</summary>
			public float Sensitivity { get; set; } = 3f;


			/// <summary>The value from `<see cref="GetValue"/>`.</summary>
			private float value;
			/// <summary>The current frame count; used to avoid miscalculatons from multiple calls.</summary>
			private float frameCount = -1;


			/**
			 * <summary>Creates an Axis using a negative and positive KeyCode.
			 * </summary>
			 * <param name="negative">The negative KeyCode.
			 * </param>
			 * <param name="positive">The positive KeyCode.
			 * </param>
			 */
			public Axis(KeyCode negative, KeyCode positive){
				NegativeKeys = new List<KeyCode>(){ negative };
				PositiveKeys = new List<KeyCode>(){ positive };
			}
			/**
			 * <summary>Creates an Axis using a negative and positive KeyCode arrays.
			 * </summary>
			 * <param name="negatives">The negative KeyCode array.
			 * </param>
			 * <param name="positives">The positive KeyCode array.
			 * </param>
			 */
			public Axis(KeyCode[] negatives, KeyCode[] positives){
				NegativeKeys = new List<KeyCode>(negatives);
				PositiveKeys = new List<KeyCode>(positives);
			}
			/**
			 * <summary>Creates an Axis using a negative and positive KeyCode Lists.
			 * </summary>
			 * <param name="negatives">The negative KeyCode List.
			 * </param>
			 * <param name="positives">The positive KeyCode List.
			 * </param>
			 */
			public Axis(List<KeyCode> negatives, List<KeyCode> positives){
				NegativeKeys = new List<KeyCode>(negatives);
				PositiveKeys = new List<KeyCode>(positives);
			}


			/**
			 * <summary>Returns the smoothed value of the Axis.
			 * </summary>
			 * <param name="ignorePause">Should the `<see cref="InputManager.Paused"/>`
			 * state be ignored?</param>
			 */
			public float GetValue(bool ignorePause = false){
				if (InputManager.Paused && !ignorePause) return 0;
				if (frameCount == Time.frameCount) return value;
				frameCount = Time.frameCount;

				float target = GetRawValue();
				float mult = target < Mathf.Abs(value) ? Gravity : Sensitivity;
				value = Mathf.MoveTowards(value, target, mult * Time.deltaTime);

				return (Mathf.Abs(value) > Dead) ? value : 0f;
			}
			/**
			 * <summary>Returns the raw value of the Axis.
			 * </summary>
			 * <param name="ignorePause">Should the `<see cref="InputManager.Paused"/>`
			 * state be ignored?</param>
			 */
			public float GetRawValue(bool ignorePause = false){
				if (InputManager.Paused && !ignorePause) return 0;
				float val = 0;
				foreach (KeyCode key in PositiveKeys){
					if (Input.GetKey(key)){
						val += (Invert) ? -1 : 1;
						if (!Additive) break;
					}
				}
				foreach (KeyCode key in NegativeKeys){
					if (Input.GetKey(key)){
						val += (Invert) ? 1 : -1;
						if (!Additive) break;
					}
				}
				return val;
			}
		}
	}
}