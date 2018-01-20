using Izhitsa.InputManagement;
using Izhitsa.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[assembly: AssemblyProduct("Izhitsa")]
[assembly: AssemblyTitle("Izhitsa")]
[assembly: AssemblyCopyright("Copyright © Omar M 2018")]
[assembly: AssemblyVersion("0.0.1")]
namespace Izhitsa {
	/**
	 <summary>
	 Provides MonoBehaviour method access to the other
	 Izhitsa classes.
	 </summary>
	 */
	public class Proxy : MonoBehaviour {
		/**
		 <summary>
		 Proxy object which allows access to MonoBehaviour
		 methods in static classes.
		 </summary>
		 */
		private static Proxy obj;
		/// <summary>Is the application quitting?</summary>
		private static bool quitting = false;
		/// <summary>KeyCodes which aren't registered by OnGUI.</summary>
		private static List<KeyCode> unregistered = new List<KeyCode> {
			KeyCode.LeftShift,
			KeyCode.RightShift
		};


		/**
		 <summary>
		 Sets up Singleton and deletes any Proxy GameObjects which aren't
		 the Singleton. Also adds the object to DontDestroyOnLoad.
		 </summary>
		 */
		void Awake(){
			if (obj != null && obj != this){
				Destroy(this);
				return;
			}
			obj = this;
			DontDestroyOnLoad(this);
			useGUILayout = false;
			Tasks.TaskUtils.main = System.Threading.Thread.CurrentThread;
		}
		/**
		 <summary>
		 Creates events for keys/events that aren't registered by OnGUI.
		 </summary>
		 */
		void Update(){
			List<Event> events = new List<Event>();
			foreach(KeyCode key in unregistered){
				if (Input.GetKey(key)){
					events.Add(new Event {
						type = EventType.KeyDown,
						keyCode = key
					});
				}
				if (Input.GetKeyUp(key)){
					events.Add(new Event {
						type = EventType.KeyUp,
						keyCode = key
					});
				}
			}
			foreach (Event ev in events) InputManager.handleEvent(ev);

			InputManager.handleEvent(new Event {
				type = EventType.MouseMove,
				mousePosition = Input.mousePosition
			});
			ThreadTask.answerRequests();
		}
		/**
		 <summary>
		 Passes events to the InputManager so that they can be handled.
		 </summary>
		 */
		void OnGUI(){
			Event e = Event.current;
			EventType type = e.type;
			if (type == EventType.Repaint || type == EventType.Ignore || type == EventType.Used)
				return;
			if (type == EventType.KeyDown && e.keyCode == KeyCode.None) return;
			e.mousePosition = Input.mousePosition;
			InputManager.handleEvent(e);
		}
		/**
		 <summary>
		 Recreates the Izhitsa GameObject if it was deleted.
		 </summary>
		 */
		void OnDestroy(){
			if (!quitting) createProxy();
		}
		/**
		 <summary>
		 Sets a flag to let <see cref="OnDestroy"/> know
		 not to create a new GameObject while the application is
		 quitting.
		 </summary>
		 */
		void OnApplicationQuit(){
			quitting = true;
			ThreadTask.terminate();
		}


		/**
		 <summary>
		 Starts a Coroutine using the proxy object.
		 </summary>
		 <param name="e">An IEnumerator to start the Coroutine with.
		 </param>
		 */
		internal static Coroutine startCoroutine(IEnumerator e) => obj?.StartCoroutine(e);

		/**
		 <summary>
		 Creates a GameObject and attaches a Proxy script to it.
		 </summary>
		 */
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void createProxy(){
			obj = null;
			GameObject go = new GameObject("IzhitsaProxy");
			go.AddComponent<Proxy>();
			go.hideFlags = HideFlags.HideInHierarchy;
		}
	}
}