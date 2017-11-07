using UnityEngine;
using System.Collections.Generic;

namespace Izhitsa {
	/**
	 * <summary>
	 * Class which serves as a wrapper for `UnityEngine.Random` and provides extended
	 * functionality.
	 * </summary>
	 */
	public static class Random {
		/// <summary>Returns a random point inside a circle with radius 1. (Read Only)</summary>
		public static Vector2 insideUnitCircle => UnityEngine.Random.insideUnitCircle;
		/// <summary>Returns a random point inside a sphere with radius 1. (Read Only)</summary>
		public static Vector3 insideUnitSphere => UnityEngine.Random.insideUnitSphere;
		/// <summary>Returns a random point on the surface of a sphere with radius 1. (Read Only)</summary>
		public static Vector3 onUnitSphere => UnityEngine.Random.onUnitSphere;
		/// <summary>Returns a random rotation. (Read Only)</summary>
		public static Quaternion rotation => UnityEngine.Random.rotation;
		/// <summary>Returns a random rotation with uniform distribution. (Read Only)</summary>
		public static Quaternion rotationUniform =>  UnityEngine.Random.rotationUniform;
		/// <summary>Gets/Sets the full internal state of the random number generator.</summary>
		public static UnityEngine.Random.State state {
			get {
				return UnityEngine.Random.state;
			}
			set {
				UnityEngine.Random.state = value;
			}
		}
		/// <summary>Returns a random number between 0.0 [inclusive] and 1.0 [inclusive]. (Read Only)</summary>
		public static float value => UnityEngine.Random.value;

		/**
		 * <summary>
		 * Picks and returns a random argument.
		 * </summary>
		 * <param name="args">Arguments to choose from.</param>
		 */
		public static T Pick<T>(params T[] args){
			if (args.Length < 1) return default(T);
			if (args.Length == 1) return args[0];
			return args[Rand(0, args.Length - 1)];
		}

		/**
		 * <summary>
		 * Returns a random integer between `min` [inclusive] and `max` [inclusive].
		 * </summary>
		 */
		public static int Rand(int min, int max) => UnityEngine.Random.Range(min, max + 1);
		/**
		 * <summary>
		 * Returns a random float between `min` [inclusive] and `max` [inclusive].
		 * </summary>
		 */
		public static float Rand(float min, float max) => UnityEngine.Random.Range(min, max);
		/**
		 * <summary>
		 * Returns a random color from black to `color`.
		 * </summary>
		 * <param name="color">The "maximum" color.
		 * </param>
		 */
		public static Color Rand(Color color) => Rand(new Color(0f, 0f, 0f, 1f), color);
		/**
		 * <summary>
		 * Returns a random color between two provided color values.
		 * Will use the minimums and maximums from either value to produce the result.
		 * </summary>
		 * <param name="one">The first color value.
		 * </param>
		 * <param name="two">The second color value.
		 * </param>
		 */
		public static Color Rand(Color one, Color two){
			float minR, minG, minB, minA;
			float maxR, maxG, maxB, maxA;

			minR = Mathf.Min(one.r, two.r);
			minG = Mathf.Min(one.g, two.g);
			minB = Mathf.Min(one.b, two.b);
			minA = Mathf.Min(one.a, two.a);

			maxR = Mathf.Max(one.r, two.r);
			maxG = Mathf.Max(one.g, two.g);
			maxB = Mathf.Max(one.b, two.b);
			maxA = Mathf.Max(one.a, two.a);

			return new Color(
				Rand(minR, maxR),
				Rand(minB, maxB),
				Rand(minG, maxG),
				Rand(minA, maxA)
			);
		}
		/**
		 * <summary>
		 * Returns a random Vector3 between two provided Vector3 values.
		 * Will use the minimums and maximums from either value to produce the result.
		 * </summary>
		 * <param name="one">The first vector value.
		 * </param>
		 * <param name="two">The second vector value.
		 * </param>
		 */
		public static Vector3 Rand(Vector3 one, Vector3 two){
			float minX, minY, minZ;
			float maxX, maxY, maxZ;

			minX = Mathf.Min(one.x, two.x);
			minY = Mathf.Min(one.y, two.y);
			minZ = Mathf.Min(one.z, two.z);

			maxX = Mathf.Max(one.x, two.x);
			maxY = Mathf.Max(one.y, two.y);
			maxZ = Mathf.Max(one.z, two.z);

			return new Vector3(
				Rand(minX, maxX),
				Rand(minY, maxY),
				Rand(minZ, maxZ)
			);
		}
		/**
		 * <summary>
		 * Returns a random element from a List of type `T`.
		 * </summary>
		 * <param name="list">A list to choose from.
		 * </param>
		 */
		public static T Rand<T>(List<T> list){
			if (list.Count > 1)
				return list[Rand(0, list.Count - 1)];
			if (list.Count == 1) return list[0];
			return default(T);
		}
		/**
		 * <summary>
		 * Returns a random color.
		 * </summary>
		 * <param name="randomAlpha">Whether or not to randomise alpha value.
		 * </param>
		 */
		public static Color RandomColor(bool randomAlpha = false){
			return new Color(
				value,
				value,
				value,
				(randomAlpha ? value : 1.0f)
			);
		}
		/**
		 * <summary>
		 * Initialises the random number generator state with a seed.
		 * </summary>
		 * <param name="seed">Seed used to initialise the random number generator.
		 * </param>
		 */
		public static void Seed(int seed) => UnityEngine.Random.InitState(seed);
	}
}