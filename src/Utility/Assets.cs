using System.Collections.Generic;
using UnityEngine;

namespace Izhitsa.Utility {
	/**
	 <summary>
	 Provides asset-related utilities.
	 </summary>
	 */
	public static class Assets {
		/// <summary>The asset cache.</summary>
		private static Dictionary<string, UnityEngine.Object> assetCache { get; set; }
			= new Dictionary<string, UnityEngine.Object>();
		
		/**
		 <summary>
		 Loads a resource and caches it.
		 </summary>
		 <param name="path">The path to the resource.
		 </param>
		 <param name="forceReload">If this is true, the resource is reloaded
		 even if it exists in the cache.
		 </param>
		 */
		public static T Load<T>(string path, bool forceReload = false)
		where T : UnityEngine.Object {
			if (assetCache.ContainsKey(path) && !forceReload) return assetCache[path] as T;
			T resource = Resources.Load<T>(path);
			assetCache.Add(path, resource);
			return resource;
		}
		/**
		 <summary>
		 Loads all assets in a folder or file at path in a Resource folder.
		 </summary>
		 <param name="path">The path to the folder or file.
		 </param>
		 <returns>A List of type T, containing the assets.
		 </returns>
		 */
		public static List<T> LoadAll<T>(string path)
		where T : UnityEngine.Object {
			UnityEngine.Object[] objs = Resources.LoadAll(path);
			List<T> resources = new List<T>();
			foreach (UnityEngine.Object obj in objs) resources.Add(obj as T);
			return resources;
		}
	}
}