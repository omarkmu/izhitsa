using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Izhitsa {
	/**
	 * <summary>
	 * Class to extend `GameObject`s and contain hierarchy-related methods.
	 * </summary>
	 */
	public static class Hierarchy {
		/**
		 * <summary>Destroys every GameObject in the hierarchy.</summary>
		 */
		public static void Clear(){
			foreach (GameObject go in GetChildren())
				GameObject.Destroy(go);
		}
		/**
		 * <summary>Destroys all child GameObjects.</summary>
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
		 * <param name="parent">A GameObject to set as the clone's parent. (Optional)
		 * </param>
		 */
		public static GameObject Clone(this GameObject go, GameObject parent = null)
			=> UnityEngine.Object.Instantiate(go, parent?.transform);
		/**
		 * <summary>
		 * Creates a game object with a primitive mesh renderer and appropriate collider.
		 * </summary>
		 * <param name="type">The type of primitive object to create.
		 * </param>
		 * <param name="parent">A GameObject to set as the clone's parent. (Optional)
		 * </param>
		 */
		public static GameObject CreatePrimitive(PrimitiveType type, GameObject parent = null){
			GameObject go = GameObject.CreatePrimitive(type);
			go.transform.SetParent(parent?.transform);
			return go;
		}
		/**
		 * <summary>Removes the GameObject.</summary>
		 */
		public static void Destroy(this GameObject go){
			if (go == null) throw new ArgumentNullException();
			GameObject.Destroy(go);
		}
		/**
		 * <summary>Unparents all children.</summary>
		 */
		public static void DetachChildren(this GameObject go)
			=> go?.transform.DetachChildren();
		/**
		 * <summary>
		 * Finds a GameObject by name and returns it, returning null if none can be found.
		 * </summary>
		 * <param name="name">The name of the GameObject to find.
		 * </param>
		 */
		public static GameObject Find(string name) => GameObject.Find(name);
		/**
		 * <summary>
		 * Finds a child GameObject by name and returns it, returning null if none can be found.
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
		 * Finds a child GameObject by name and returns it, returning null if none can be found.
		 * </summary>
		 * <param name="name">The name of the GameObject to find.
		 * </param>
		 */
		public static GameObject FindChild(this GameObject go, string name)
			=> go.transform.Find(name)?.gameObject;
		/**
		 * <summary>
		 * Returns a child by index.
		 * </summary>
		 * <param name="index">Index of the child to return.
		 * Must be smaller than child count.
		 * </param>
		 */
		public static GameObject GetChild(int index) => GetChildren()[index];
		/**
		 * <summary>
		 * Returns a child by index.
		 * </summary>
		 * <param name="index">Index of the child to return.
		 * Must be smaller than child count.
		 * </param>
		 */
		public static GameObject GetChild(this GameObject go, int index)
			=> go?.transform.GetChild(index).gameObject;
		/**
		 * <summary>Returns the amount of GameObjects in the hierarchy.</summary>
		 */
		public static int GetChildCount() => GetChildren().Length;
		/**
		 * <summary>Returns the amount of GameObjects in the GameObject.</summary>
		 */
		public static int GetChildCount(this GameObject go){
			if (go == null) throw new ArgumentNullException();
			return go.transform.childCount;
		}
		/**
		 * <summary>Returns an array containing the root GameObjects in the hierarchy.</summary>
		 */
		public static GameObject[] GetChildren()
			=> SceneManager.GetActiveScene().GetRootGameObjects();
		/**
		 * <summary>Returns an array containing this GameObject's children.</summary>
		 */
		public static GameObject[] GetChildren(this GameObject go){
			Transform tr = go.transform;
			GameObject[] children = new GameObject[tr.childCount];
			for (int i = 0; i < tr.childCount; i++){
				children[i] = tr.GetChild(i).gameObject;
			}
			return children;
		}
		/**
		 * <summary>Gets the sibling index.</summary>
		 */
		public static int GetSiblingIndex(this GameObject go){
			if (go == null) throw new ArgumentNullException("go");
			return go.transform.GetSiblingIndex();
		}
		/**
		 * <summary>
		 * Is this GameObject a child of parent?
		 * </summary>
		 * <param name="parent">The parent to check.
		 * </param>
		 */
		public static bool IsChildOf(this GameObject go, GameObject parent){
			if (go == null) throw new ArgumentNullException("go");
			return go.transform.IsChildOf(parent?.transform);
		}
		/**
		 * <summary>
		 * Sets the parent of the GameObject.
		 * </summary>
		 * <param name="parent">The parent GameObject to use.
		 * </param>
		 * <param name="worldPositionStays">If true, the parent-relative position, rotation,
		 * and scale are modified such that the object keeps the same world space position,
		 * rotation, and scale as before.
		 * </param>
		 */
		public static void SetParent(this GameObject go, GameObject parent, bool worldPositionStays = true)
			=> go?.transform.SetParent(parent?.transform, worldPositionStays);
		/**
		 * <summary>
		 * Sets the sibling index.
		 * </summary>
		 * <param name="index">
		 * Index to set.
		 * </param>
		 */
		public static void SetSiblingIndex(this GameObject go, int index){
			if (go == null) throw new ArgumentNullException("go");
			go.transform.SetSiblingIndex(index);
		}
	}
}