using static Izhitsa.InputManagement.InputManager;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Izhitsa {
	namespace InputManagement {
		/**
		 <summary>
		 Represents an ordered sequence of input events.
		 </summary>
		 */
		public class Sequence {
			/// <summary>The current SequenceElement. (Read Only)</summary>
			public SequenceElement Current => (elements.Length > 0) ? elements[CurrentStep] : null;
			/// <summary>The current step, where the Sequence's pointer is right now. (Read Only)</summary>
			public int CurrentStep {
				get {
					return currentStep;
				}
				internal set {
					if (value > MaxStep) currentStep = 0;
					else currentStep = value;

					if (currentStep == 0)
						lastStepTime = 0.0f;
				}
			}
			/// <summary>The recorded Time.time from the last step's completion. (Read Only)</summary>
			public float LastStepTime => lastStepTime;
			/// <summary>The maximum amount of steps before completion and reset. (Read Only)</summary>
			public int MaxStep => elements.Length - 1;

			/// <summary>The frame in which this Sequence was completed.</summary>
			internal float completionFrame = -1f;
			/// <summary>The elements which make up the Sequence.</summary>
			internal SequenceElement[] elements { get; set; }
			/// <summary>The recorded Time.time from the last step's completion.</summary>
			internal float lastStepTime { get; set; } = 0.0f;
			
			/// <summary>The current step, where the Sequence's pointer is right now.</summary>
			private int currentStep = 0;


			/**
			 <summary>
			 Creates a new Sequence using <see cref="SequenceElement"/>s.
			 </summary>
			 <param name="args">The SequenceElements to create a Sequence out of.</param>
			 */
			public Sequence(params SequenceElement[] args){
				elements = args;
			}
			/**
			 <summary>
			 Creates a new Sequence out of <see cref="KeyCode"/>s.
			 </summary>
			 <param name="args">The KeyCodes to create a Sequence out of.</param>
			 */
			public Sequence(params KeyCode[] args){
				SequenceElement[] elems = new SequenceElement[args.Length];
				for (int i = 0; i < args.Length; i++){
					SequenceElement elem = new SequenceElement();
					elem.Key = args[i];
					elems[i] = elem;
				}
				elements = elems;
			}


			/**
			 <summary>
			 Sequence indexer, for getting/setting <see cref="SequenceElement"/>s.
			 </summary>
			 <param name="index">The integer to index the array with.
			 Must be between 0 and <see cref="MaxStep"/>.
			 </param>
			 <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/>
			 is not between 0 and <see cref="MaxStep"/> or there are no <see cref="SequenceElement"/>s in the Sequence.
			 </exception>
			 */
			public SequenceElement this[int index]{
				get {
					if (index < 0 || index > MaxStep || MaxStep == -1)
						throw new ArgumentOutOfRangeException("index");
					return elements[index];
				}
				set {
					if (index < 0 || index >= MaxStep || MaxStep == -1)
						throw new ArgumentOutOfRangeException("index");
					elements[index] = value;
				}
			}


			/**
			 <summary>
			 Resets the sequence.
			 </summary>
			 */
			public void Reset(){
				CurrentStep = 0;
			}
			/**
			 <summary>
			 Sets <paramref name="propName"/> property to <paramref name="value"/> in
			 all of the sequence's <see cref="SequenceElement"/>s, and returns the sequence.
			 </summary>
			 <param name="propName">A <see cref="SequenceElement"/> float property name.
			 </param>
			 <param name="value">The value to assign to the property.
			 </param>
			 <exception cref="ArgumentNullException">Thrown if <paramref name="propName"/> is null.
			 </exception>
			 */
			public Sequence Set(string propName, float value){
				if (propName == null) throw new ArgumentNullException("propName");
				switch(propName){
					case "MaxDuration":
						foreach (SequenceElement elem in elements)
							elem.MaxDuration = value;
						break;
					case "MaxMargin":
						foreach (SequenceElement elem in elements)
							elem.MaxMargin = value;
						break;
					case "MinDuration":
						foreach (SequenceElement elem in elements)
							elem.MinDuration = value;
						break;
					case "MinMargin":
						foreach (SequenceElement elem in elements)
							elem.MinMargin = value;
						break;
					case "MinDeltaX":
						foreach (SequenceElement elem in elements)
							elem.MinDeltaX = value;
						break;
					case "MinDeltaY":
						foreach (SequenceElement elem in elements)
							elem.MinDeltaY = value;
						break;
					case "MaxDeltaX":
						foreach (SequenceElement elem in elements)
							elem.MaxDeltaX = value;
						break;
					case "MaxDeltaY":
						foreach (SequenceElement elem in elements)
							elem.MaxDeltaY = value;
						break;
					default:
						throw new ArgumentException($"Invalid property name: \"{propName}\"" +
						" for type \"float\".");
				}
				return this;
			}
			/**
			 <summary>
			 Sets InterruptFlags to <paramref name="value"/> in all of the sequence's
			 <see cref="SequenceElement"/>s, and returns the sequence.
			 </summary>
			 <param name="value">The value to assign to the property.</param>
			 */
			public Sequence Set(InterruptFlags value){
				foreach (SequenceElement elem in elements) elem.InterruptFlags = value;
				return this;
			}
			/**
			 <summary>
			 Sets Key to <paramref name="value"/> in all of the sequence's
			 <see cref="SequenceElement"/>s, and returns the sequence.
			 </summary>
			 <param name="value">The value to assign to the property.</param>
			 */
			public Sequence Set(KeyCode value){
				foreach (SequenceElement elem in elements) elem.Key = value;
				return this;
			}
			/**
			 <summary>
			 Sets Type to <paramref name="value"/> in all of the sequence's
			 <see cref="SequenceElement"/>s, and returns the sequence.
			 </summary>
			 <param name="value">The value to assign to the property.</param>
			 */
			public Sequence Set(InputEventType value){
				foreach (SequenceElement elem in elements) elem.Type = value;
				return this;
			}
			/**
			 <summary>
			 Sets Modifiers to <paramref name="value"/> in all of the sequence's
			 <see cref="SequenceElement"/>s, and returns the sequence.
			 </summary>
			 <param name="value">The value to assign to the property.</param>
			 * 
			 */
			public Sequence Set(List<KeyCode> value){
				foreach (SequenceElement elem in elements) elem.Modifiers = value;
				return this;
			}
		}
	}
}