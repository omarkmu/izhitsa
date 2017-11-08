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
	public class Main : MonoBehaviour {
		/** <summary>
		 * Proxy object which allows access to MonoBehaviour
		 * methods in static classes.
		 * </summary>
		 */
		private static Main obj;
		/// <summary>Is the application quitting?</summary>
		private static bool quitting = false;

		/**
		 * <summary>
		 * Static constructor which creates a GameObject
		 * and attaches a Main component.
		 * </summary>
		 */
		static Main(){
			GameObject go = new GameObject("IzhitsaMain");
			go.AddComponent<Main>();
		}
		/**
		 * <summary>
		 * Sets up Singleton and deletes any Main components which aren't
		 * the Singleton. Also sets adds the object to DontDestroyOnLoad.
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
		 * Passes events to the InputManager so they can be
		 * handled.
		 * </summary>
		 */
		void OnGUI(){
			Event e = Event.current;
			EventType type = e.type;
			if (type == EventType.Layout || type == EventType.Repaint || type == EventType.Ignore)
				return;
			if (type == EventType.KeyDown && e.keyCode == KeyCode.None) return;
			InputManager.handleEvent(e);
		}
		/**
		 * <summary>
		 * Recreates the Izhitsa GameObject if it was deleted.
		 * </summary>
		 */
		void OnDestroy(){
			obj = null;
			if (quitting) return;
			GameObject go = new GameObject("IzhitsaMain");
			go.AddComponent<Main>();
		}
		/**
		 * <summary>
		 * Sets a flag to let `<see cref="Main.OnDestroy"/>` know
		 * not to create a new GameObject when the application is
		 * quitting.
		 * </summary>
		 */
		void OnApplicationQuit(){
			quitting = true;
		}

		/**
		 * <summary>
		 * Starts a coroutine using the proxy object.
		 * </summary>
		 * <param name="e">An IEnumerator to run.
		 * </param>
		 */
		internal static Coroutine startCoroutine(IEnumerator e) => obj?.StartCoroutine(e);
	}
}
