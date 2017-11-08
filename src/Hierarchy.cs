using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Izhitsa {
	/**
	 * <summary>
	 * Class which contains extension methods for `<see cref="GameObject"/>`
	 * and other hierarchy-related methods.
	 * </summary>
	 */
	public static class Hierarchy {
		/// <summary>The amount of root `<see cref="GameObject"/>`s in the hierarchy.</summary>
		public static int ChildCount => GetChildCount();
		/**
		 * <summary>Destroys every `<see cref="GameObject"/>` in the hierarchy.</summary>
		 */
		public static void Clear(){
			foreach (GameObject go in GetChildren())
				GameObject.Destroy(go);
		}
		/**
		 * <summary>
		 * Destroys all child GameObjects inside of the GameObject.
		 * </summary>
		 * <param name="go">The GameObject.
		 * </param>
		 * <exception cref="ArgumentNullException">Thrown if
		 * `<paramref name="go"/>` is null.
		 * </exception>
		 */
		public static void Clear(this GameObject go){
			if (go == null) throw new ArgumentNullException();
			foreach (Transform tr in go.transform)
				GameObject.Destroy(tr.gameObject);
		}
		/**
		 * <summary>
		 * Clones the GameObject, and returns the clone.
		 * </summary>
		 * <param name="go">The GameObject to clone.
		 * </param>
		 * <param name="parent">A GameObject to set as the clone's parent.
		 * </param>
		 * <exception cref="ArgumentNullException">Thrown if
		 * `<paramref name="go"/>` is null.
		 * </exception>
		 */
		public static GameObject Clone(this GameObject go, GameObject parent = null){
			if (go == null) throw new ArgumentNullException("go");
			return UnityEngine.Object.Instantiate(go, parent?.transform);
		}
		/**
		 * <summary>
		 * Creates a game object with a primitive mesh renderer and appropriate collider.
		 * </summary>
		 * <param name="type">The type of primitive object to create.
		 * </param>
		 * <param name="parent">A GameObject to set as the GameObject's parent.
		 * </param>
		 */
		public static GameObject CreatePrimitive(PrimitiveType type, GameObject parent = null){
			GameObject go = GameObject.CreatePrimitive(type);
			go.transform.SetParent(parent?.transform);
			return go;
		}
		/**
		 * <summary>
		 * Removes the GameObject.
		 * </summary>
		 * <param name="go">The GameObject to destroy.
		 * </param>
		 * <exception cref="ArgumentNullException">Thrown if
		 * `<paramref name="go"/>` is null.
		 * </exception>
		 */
		public static void Destroy(this GameObject go){
			if (go == null) throw new ArgumentNullException();
			GameObject.Destroy(go);
		}
		/**
		 * <summary>
		 * Unparents all children.
		 * </summary>
		 * <param name="go">The GameObject to detach
		 * children from.
		 * </param>
		 * <exception cref="ArgumentNullException">Thrown if
		 * `<paramref name="go"/>` is null.
		 * </exception>
		 */
		public static void DetachChildren(this GameObject go){
			if (go == null) throw new ArgumentNullException("go");
			go.transform.DetachChildren();
		}
		/**
		 * <summary>
		 * Finds a `<see cref="GameObject"/>` by name and returns it, returning `null` if none can be found.
		 * </summary>
		 * <param name="name">The name of the GameObject to find.
		 * </param>
		 */
		public static GameObject Find(string name) => GameObject.Find(name);
		/**
		 * <summary>
		 * Finds a `<see cref="GameObject"/>` in the scene's root by name and returns it, returning `null` if none can be found.
		 * </summary>
		 * <param name="name">The name of the GameObject to find.
		 * </param>
		 */
		public static GameObject FindChild(string name){
			GameObject[] gameObjs = GetChildren();
			foreach (GameObject go in gameObjs){
				if (go.name == name) return go;
			}
			return null;
		}
		/**
		 * <summary>
		 * Finds a child GameObject by name and returns it, returning `null` if none can be found.
		 * </summary>
		 * <param name="go">The GameObject to search.
		 * </param>
		 * <param name="name">The name of the GameObject to find.
		 * </param>
		 * <exception cref="ArgumentNullException">Thrown if
		 * `<paramref name="go"/>` is null.
		 * </exception>
		 */
		public static GameObject FindChild(this GameObject go, string name){
			if (go == null) throw new ArgumentNullException("go");
			return go.transform.Find(name)?.gameObject;
		}
		/**
		 * <summary>
		 * Returns a root `<see cref="GameObject"/>` by index.
		 * </summary>
		 * <param name="index">Index of the child to return.
		 * Must be smaller than `<see cref="ChildCount"/>`.
		 * </param>
		 * <exception cref="ArgumentOutOfRangeException">Thrown if `<paramref name="index"/>` is
		 * smaller than `0` or bigger than/equal to `<see cref="ChildCount"/>`
		 * </exception>
		 */
		public static GameObject GetChild(int index){
			GameObject[] children = GetChildren();
			if (index < 0 || index >= children.Length)
				throw new ArgumentOutOfRangeException("index");
			return children[index];
		}
		/**
		 * <summary>
		 * Returns a child GameObject by index.
		 * </summary>
		 * <param name="go">The GameObject to get the child from.
		 * </param>
		 * <param name="index">Index of the child to return.
		 * Must be smaller than child count.
		 * </param>
		 * <exception cref="ArgumentNullException">Thrown if
		 * `<paramref name="go"/>` is null.
		 * </exception>
		 * <exception cref="ArgumentOutOfRangeException">Thrown if `<paramref name="index"/>` is
		 * smaller than `0` or bigger than/equal to `<see cref="GameObject.transform.childCount"/>`
		 * </exception>
		 */
		public static GameObject GetChild(this GameObject go, int index){
			if (go == null)
				throw new ArgumentNullException("go");
			if (index < 0 || index >= go.transform.childCount)
				throw new ArgumentOutOfRangeException("index");
			return go.transform.GetChild(index).gameObject;
		}
		/**
		 * <summary>Returns the amount of root `<see cref="GameObject"/>`s in the hierarchy.</summary>
		 */
		public static int GetChildCount() => GetChildren().Length;
		/**
		 * <summary>
		 * Returns the amount of child GameObjects in the GameObject.
		 * </summary>
		 * <param name="go">The GameObject whose children are to be counted.
		 * </param>
		 * <exception cref="ArgumentNullException">Thrown if
		 * `<paramref name="go"/>` is null.
		 * </exception>
		 */
		public static int GetChildCount(this GameObject go){
			if (go == null) throw new ArgumentNullException();
			return go.transform.childCount;
		}
		/**
		 * <summary>
		 * Returns an array containing the root `<see cref="GameObject"/>`s in the hierarchy.
		 * </summary>
		 */
		public static GameObject[] GetChildren()
			=> SceneManager.GetActiveScene().GetRootGameObjects();
		/**
		 * <summary>
		 * Returns an array containing this GameObject's children.
		 * </summary>
		 * <param name="go">The GameObject.
		 * </param>
		 * <exception cref="ArgumentNullException">Thrown if
		 * `<paramref name="go"/>` is null.
		 * </exception>
		 */
		public static GameObject[] GetChildren(this GameObject go){
			if (go == null) throw new ArgumentNullException("go");
			Transform tr = go.transform;
			GameObject[] children = new GameObject[tr.childCount];
			for (int i = 0; i < tr.childCount; i++){
				children[i] = tr.GetChild(i).gameObject;
			}
			return children;
		}
		/**
		 * <summary>
		 * Gets this GameObject's sibling index.
		 * </summary>
		 * <param name="go">The GameObject.
		 * </param>
		 * <exception cref="ArgumentNullException">Thrown if
		 * `<paramref name="go"/>` is null.
		 * </exception>
		 */
		public static int GetSiblingIndex(this GameObject go){
			if (go == null) throw new ArgumentNullException("go");
			return go.transform.GetSiblingIndex();
		}
		/**
		 * <summary>
		 * Is this GameObject a child of `<paramref name="parent"/>`?
		 * </summary>
		 * <param name="parent">The parent to check.
		 * </param>
		 * <exception cref="ArgumentNullException">Thrown if
		 * `<paramref name="go"/>` is null.
		 * </exception>
		 */ 
		public static bool IsChildOf(this GameObject go, GameObject parent){
			if (go == null) throw new ArgumentNullException("go");
			return go.transform.IsChildOf(parent?.transform);
		}
		/**
		 * <summary>
		 * Sets the parent of this GameObject.
		 * </summary>
		 * <param name="go">The GameObject.
		 * </param>
		 * <param name="parent">The parent GameObject to use.
		 * If this is `null`, the GameObject becomes a root GameObject.
		 * </param>
		 * <param name="worldPositionStays">If true, the parent-relative position, rotation,
		 * and scale are modified such that the object keeps the same world space position,
		 * rotation, and scale as before.
		 * </param>
		 * <exception cref="ArgumentNullException">Thrown if
		 * `<paramref name="go"/>` is null.
		 * </exception>
		 */
		public static void SetParent(this GameObject go, GameObject parent, bool worldPositionStays = true){
			if (go == null) throw new ArgumentNullException("go");
			go.transform.SetParent(parent?.transform, worldPositionStays);
		}
		/**
		 * <summary>
		 * Sets the sibling index of this GameObject.
		 * </summary>
		 * <param name="go">The GameObject.
		 * </param>
		 * <param name="index">Index to set.
		 * </param>
		 * <exception cref="ArgumentNullException">Thrown if
		 * `<paramref name="go"/>` is null.
		 * </exception>
		 */
		public static void SetSiblingIndex(this GameObject go, int index){
			if (go == null) throw new ArgumentNullException("go");
			go.transform.SetSiblingIndex(index);
		}
	}
}