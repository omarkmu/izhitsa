using System;
using System.Collections.Generic;
using UnityEngine;

namespace Izhitsa.InputManagement {
	/**
	 <summary>Represents an input axis.</summary>
	 */
	public class Axis {
		/// <summary>If true, the raw value of the input can factor in multiple positive and negative values.</summary>
		public bool Additive { get; set; } = false;
		/// <summary>Any numbers less than this value will register as zero.</summary>
		public float Dead { get; set; } = 0.001f;
		/// <summary>How fast the input will recenter.</summary>
		public float Gravity { get; set; } = 3f;
		/// <summary>If true, negative keys send positive values and vice versa.</summary>
		public bool Invert { get; set; } = false;
		/// <summary>Funcs to use as negative inputs. A Func in this list returning true counts towards negative Axis input.</summary>
		public List<Func<bool>> NegativeFuncs { get; private set; } = new List<Func<bool>>();
		/// <summary>The keys to use as negative inputs.</summary>
		public List<KeyCode> NegativeKeys { get; private set; } = new List<KeyCode>();
		/// <summary>Funcs to use as positive inputs. A Func in this list returning true counts towards positive Axis input.</summary>
		public List<Func<bool>> PositiveFuncs { get; private set; } = new List<Func<bool>>();
		/// <summary>The keys to use as positive inputs.</summary>
		public List<KeyCode> PositiveKeys { get; private set; } = new List<KeyCode>();
		/// <summary>How fast the input will become 1 or -1.</summary>
		public float Sensitivity { get; set; } = 3f;


		/// <summary>The current frame count; used to avoid miscalculatons from multiple calls.</summary>
		private float frameCount = -1;
		/// <summary>Is the value increasing?</summary>
		private bool? increasing;
		/// <summary>The value from <see cref="GetValue"/>.</summary>
		private float value;


		/**
		 <summary>Creates an Axis using a negative and positive KeyCode.
		 </summary>
		 <param name="negative">The negative KeyCode.
		 </param>
		 <param name="positive">The positive KeyCode.
		 </param>
		 */
		public Axis(KeyCode negative, KeyCode positive){
			NegativeKeys = new List<KeyCode>{ negative };
			PositiveKeys = new List<KeyCode>{ positive };
		}
		/**
		 <summary>Creates an Axis using negative and positive KeyCode arrays.
		 </summary>
		 <param name="negatives">The negative KeyCode array.
		 </param>
		 <param name="positives">The positive KeyCode array.
		 </param>
		 */
		public Axis(KeyCode[] negatives, KeyCode[] positives){
			NegativeKeys = new List<KeyCode>(negatives);
			PositiveKeys = new List<KeyCode>(positives);
		}
		/**
		 <summary>Creates an Axis using negative and positive KeyCode Lists.
		 </summary>
		 <param name="negatives">The negative KeyCode List.
		 </param>
		 <param name="positives">The positive KeyCode List.
		 </param>
		 */
		public Axis(List<KeyCode> negatives, List<KeyCode> positives){
			NegativeKeys = new List<KeyCode>(negatives);
			PositiveKeys = new List<KeyCode>(positives);
		}
		/**
		 <summary>Creates an Axis using negative and positive Func Lists.
		 </summary>
		 <param name="negatives">The negative Func List.
		 </param>
		 <param name="positives">The positive Func List.
		 </param>
		 */
		public Axis(List<Func<bool>> negatives, List<Func<bool>> positives){
			NegativeFuncs = new List<Func<bool>>(negatives);
			PositiveFuncs = new List<Func<bool>>(positives);
		}

		/**
		 <summary>Returns the smoothed value of the Axis.
		 </summary>
		 <param name="ignorePause">Should the <see cref="InputManager.Paused"/>
		 state be ignored?</param>
		 */
		public float GetValue(bool ignorePause = false){
			if (InputManager.Paused && !ignorePause) return 0;
			if (frameCount == Time.frameCount) return value;
			if (frameCount == -1) frameCount = Time.frameCount - 1;

			float target = GetRawValue();

			bool inc = target > Mathf.Abs(value);
			if (increasing == null) increasing = inc;
			float incDelta = (inc == increasing) ? (Time.frameCount - frameCount) : 1;
			float mult = (inc ? Sensitivity : Gravity) * incDelta;

			value = Mathf.MoveTowards(value, target, mult * Time.deltaTime);
			frameCount = Time.frameCount;
			increasing = inc;

			return (Mathf.Abs(value) > Dead) ? value : 0f;
		}
		/**
		 <summary>Returns the raw value of the Axis.
		 </summary>
		 <param name="ignorePause">Should the <see cref="InputManager.Paused"/>
		 state be ignored?</param>
		 */
		public float GetRawValue(bool ignorePause = false){
			if (InputManager.Paused && !ignorePause) return 0;
			float val = 0;
			bool success = false;
			foreach (KeyCode key in PositiveKeys){
				if (Input.GetKey(key)){
					val += (Invert) ? -1 : 1;
					success = true;
					if (!Additive) break;
				}
			}
			if (Additive || !success){
				foreach (Func<bool> func in PositiveFuncs){
					if (func()){
						val += (Invert) ? -1 : 1;
						if (!Additive) break;
					}
				}
			}

			success = false;
			foreach (KeyCode key in NegativeKeys){
				if (Input.GetKey(key)){
					val += (Invert) ? 1 : -1;
					success = true;
					if (!Additive) break;
				}
			}
			if (Additive || !success){
				foreach (Func<bool> func in NegativeFuncs){
					if (func()){
						val += (Invert) ? 1 : -1;
						if (!Additive) break;
					}
				}
			}

			return val;
		}
	}
}