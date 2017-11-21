using System.Collections;
using UnityEngine;

namespace Izhitsa {
	/**
	 * <summary>
	 * Class which attaches itself to a GameObject
	 * and provides MonoBehaviour method access to the other
	 * Izhitsa classes.
	 * </summary>
	 */
	public class Proxy : MonoBehaviour {
		/**
		 * <summary>
		 * Proxy object which allows access to MonoBehaviour
		 * methods in static classes.
		 * </summary>
		 */
		private static Proxy obj;
		/// <summary>Is the application quitting?</summary>
		private static bool quitting = false;

		/**
		 * <summary>
		 * Static constructor which creates the proxy.
		 * </summary>
		 */
		static Proxy() {
			createProxy();
		}
		/**
		 * <summary>
		 * Sets up Singleton and deletes any Main components which aren't
		 * the Singleton. Also adds the object to DontDestroyOnLoad.
		 * </summary>
		 */
		void Awake(){
			if (obj != null && obj != this){
				Destroy(this);
				return;
			}
			obj = this;
			DontDestroyOnLoad(this);
			useGUILayout = false;
		}
		/**
		 * <summary>
		 * Passes events to the InputManager so that they can be handled.
		 * </summary>
		 */
		void OnGUI(){
			Event e = Event.current;
			EventType type = e.type;
			if (type == EventType.Layout || type == EventType.Repaint || type == EventType.Ignore)
				return;
			if (type == EventType.KeyDown && e.keyCode == KeyCode.None) return;
			StartCoroutine(InputManager.handleEvent(e));
		}
		/**
		 * <summary>
		 * Recreates the Izhitsa GameObject if it was deleted.
		 * </summary>
		 */
		void OnDestroy(){
			if (!quitting) createProxy();
		}
		/**
		 * <summary>
		 * Sets a flag to let `<see cref="OnDestroy"/>` know
		 * not to create a new GameObject while the application is
		 * quitting.
		 * </summary>
		 */
		void OnApplicationQuit(){
			quitting = true;
		}

		/**
		 * <summary>
		 * Used to call the static constructor manually, if necessary.
		 * </summary>
		 */
		public static void Activate(){}
		/**
		 * <summary>
		 * Starts a coroutine using the proxy object.
		 * </summary>
		 * <param name="e">An IEnumerator to run.
		 * </param>
		 */
		internal static Coroutine startCoroutine(IEnumerator e) => obj?.StartCoroutine(e);
		/**
		 * <summary>
		 * Creates a GameObject and attaches a Proxy script to it.
		 * </summary>
		 */
		private static void createProxy(){
			obj = null;
			GameObject go = new GameObject("IzhitsaProxy");
			go.AddComponent<Proxy>();
			//go.hideFlags = HideFlags.HideInHierarchy;
		}
	}
}