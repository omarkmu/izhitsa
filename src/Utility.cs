using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Izhitsa {
	/**
	 * <summary>
	 * Auxiliary utility class, which contains various useful methods and wraps
	 * other methods to add extra functionality.
	 * </summary>
	 */
	public static class Utility {
		/// <summary>The asset cache.</summary>
		private static Dictionary<string, UnityEngine.Object> assetCache { get; set; }
			= new Dictionary<string, UnityEngine.Object>();
		/// <summary>Image data associated with a texture.</summary>
		private static Dictionary<Texture2D, ImageData> data { get; set; }
			= new Dictionary<Texture2D, ImageData>();
		
		/**
		 * <summary>
		 * Struct which contains the visible (non-transparent)
		 * points of an image.
		 * </summary>
		 */
		public struct ImageData {
			/// <summary>The visible points of the image.</summary>
			public List<Vector2> VisiblePoints { get; set; }
			/**
			 * <summary>Creates new ImageData.
			 * </summary>
			 * <param name="vps">A List of the image's visible points.</param>
			 */
			public ImageData(List<Vector2> vps){
				VisiblePoints = vps;
			}
		}

		/**
		 * <summary>
		 * Clones a texture.
		 * </summary>
		 * <param name="src">The texture to clone.</param>
		 */
		public static Texture2D CloneTexture(Texture2D src){
			Texture2D copy = new Texture2D(src.width, src.height, src.format, src.mipmapCount > 1);
			copy.LoadRawTextureData(src.GetRawTextureData());
			copy.Apply();
			return copy;
		}
		/**
		 * <summary>
		 * Combines two textures.
		 * </summary>
		 * <param name="img1">The first texture.
		 * </param>
		 * <param name="img2">The second texture.
		 * </param>
		 * <exception cref="System.ArgumentNullException">
		 * Thrown when either texture is null.
		 * </exception>
		 */
		public static Texture2D CombineTextures(Texture2D img1, Texture2D img2){
			if (img1 == null) throw new ArgumentNullException("img1");
			if (img2 == null) throw new ArgumentNullException("img2");

			Texture2D dest = CloneTexture(img1);
			Action<int, int> perform = (x, y)=>{
				Color src = img1.GetPixel(x, y);
				Color other = img2.GetPixel(x, y);
				Color final = Color.Lerp(src, other, other.a / 1.0f);

				dest.SetPixel(x, y, final);
			};
			if (!img1.DataExists()) UpdateImageData(img1);
			if (!img2.DataExists()) UpdateImageData(img2);

			Dictionary<Vector2, bool> check = new Dictionary<Vector2, bool>();
			foreach (Vector2 p in data[img1].VisiblePoints){
				perform((int)p.x, (int)p.y);
				check[p] = true;
			}
			foreach (Vector2 p in data[img2].VisiblePoints){
				if (!check.ContainsKey(p)) perform((int)p.x, (int)p.y);
			}
			dest.Apply();
			return dest;
		}
		/**
		 * <summary>
		 * Combines a variable amount of textures.
		 * </summary>
		 * <param name="args">The textures to combine.
		 * </param>
		 * <exception cref="System.ArgumentNullException">
		 * Thrown when a texture is null.
		 * </exception>
		 */
		public static Texture2D CombineTextures(params Texture2D[] args){
			if (args.Length == 0) return null;
			if (args.Length == 1) return CloneTexture(args[0]);
			Texture2D tex = args[0];
			for (int i = 1; i < args.Length; i++){
				if (args[i] == null) throw new System.ArgumentNullException($"Argument {i}");
				tex = CombineTextures(tex, args[i]);
			}

			return tex;
		}
		/**
		 * <summary>
		 * Returns a boolean representing whether or not data for a texture exists.
		 * </summary>
		 * <param name="tex">The texture to check.
		 * </param>
		 */
		public static bool DataExists(this Texture2D tex) => data.ContainsKey(tex);
		/**
		 * <summary>
		 * Loads a resource and caches it.
		 * </summary>
		 * <param name="path">The path to the resource.
		 * </param>
		 * <param name="forceReload">If this is true, the resource is reloaded
		 * even if it exists in the cache.
		 * </param>
		 */
		public static T LoadResource<T>(string path, bool forceReload = false)
		where T : UnityEngine.Object {
			if (assetCache.ContainsKey(path) && !forceReload) return assetCache[path] as T;
			T resource = Resources.Load<T>(path);
			assetCache.Add(path, resource);
			return resource;
		}
		/**
		 * <summary>
		 * Loads all assets in a folder or file at path in a Resource folder.
		 * </summary>
		 * <param name="path">The path to the folder or file.
		 * </param>
		 * <returns>A List of type T, containing the assets.
		 * </returns>
		 */
		public static List<T> LoadAllResources<T>(string path)
		where T : UnityEngine.Object {
			UnityEngine.Object[] objs = Resources.LoadAll(path);
			List<T> resources = new List<T>();
			foreach (UnityEngine.Object obj in objs) resources.Add(obj as T);
			return resources;
		}
		/**
		 * <summary>
		 * Replaces one color in a texture with another.
		 * </summary>
		 * <param name="src">The source texture.
		 * </param>
		 * <param name="toReplace">The color to replace.
		 * </param>
		 * <param name="repl">The replacement color.
		 * </param>
		 * <param name="useSrcAlpha">If this is true, the alpha from the source
		 * image is used.
		 * </param>
		 * <param name="ignoreAlpha">If this is true, the color is replaced even if
		 * the alpha doesn't match.
		 * </param>
		 * <exception cref="System.ArgumentNullException">Thrown when `<paramref name="src"/>`
		 * is null.</exception>
		 */
		public static Texture2D ReplaceTextureColor(Texture2D src, Color toReplace, Color repl,
			bool useSrcAlpha = false, bool ignoreAlpha = false
		){
			if (src == null) throw new ArgumentNullException("src");
			Texture2D dest = CloneTexture(src);

			Func<Color, Color, bool> equals = (color, otherColor)=>{
				if (color.r != otherColor.r) return false;
				if (color.g != otherColor.g) return false;
				if (color.b != otherColor.b) return false;
				return true;
			};
			Action<int, int, Color> perform = (x, y, srcColor)=>{
				if (srcColor == toReplace || (ignoreAlpha && equals(srcColor, toReplace))){
					if (useSrcAlpha)
						dest.SetPixel(x, y, new Color(repl.r, repl.g, repl.b, srcColor.a));
					else
						dest.SetPixel(x, y, repl);
				}
			};

			if (!src.DataExists()) UpdateImageData(src);
			foreach(Vector2 p in data[src].VisiblePoints)
				perform((int)p.x, (int)p.y, src.GetPixel((int)p.x, (int)p.y));

			dest.Apply();
			return dest;
		}
		/**
		 * <summary>
		 * Starts a coroutine.
		 * </summary>
		 * <param name="routine">An IEnumerator to run.
		 * </param>
		 */
		public static Coroutine StartCoroutine(IEnumerator routine) => Proxy.startCoroutine(routine);
		/**
		 * <summary>
		 * Updates the image data associated with a texture.
		 * </summary>
		 * <param name="tex">The texture to update.</param>
		 */
		public static ImageData UpdateImageData(Texture2D tex){
			List<Vector2> vis = new List<Vector2>();
			for (int x = 0; x < tex.width ; x++){
				for (int y = 0; y < tex.height; y++){
					if (tex.GetPixel(x, y).a != 0)
						vis.Add(new Vector2(x, y));
				}
			}
			
			ImageData imgData = new ImageData(vis);
			if (data.ContainsKey(tex)) data[tex] = imgData;
			else data.Add(tex, imgData);

			return imgData;
		}
	}
}