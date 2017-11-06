using System.Collections;
using UnityEngine;

namespace Izhitsa {
	public class Main : MonoBehaviour {
		/// Proxy object which allows access to MonoBehaviour in static classes.
		private static Main obj;

		/**
		 * <summary>
		 * Static constructor which creates a GameObject
		 * and attaches Main.
		 * </summary>
		 */
		static Main(){
			GameObject go = new GameObject("IzhitsaMain");
			go.AddComponent<Main>();
		}
		/**
		 * <summary>
		 * Sets up Singleton, deletes any Main components which aren't
		 * the Singleton.
		 * </summary>
		 */
		void Awake(){
			if (obj != null && obj != this){
				Destroy(this);
				return;
			}
			obj = this;
			DontDestroyOnLoad(this);
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
		 * Starts a coroutine using the proxy object.
		 * </summary>
		 * <param name="e">An IEnumerator to run.
		 * </param>
		 */
		internal static Coroutine startCoroutine(IEnumerator e) => obj?.StartCoroutine(e);
	}
}