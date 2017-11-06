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
		private static Dictionary<string, UnityEngine.Object> cache { get; set; }
			= new Dictionary<string, UnityEngine.Object>();
		private static Dictionary<Texture2D, ImageData> data { get; set; }
			= new Dictionary<Texture2D, ImageData>();
		
		public static Texture2D CloneTexture(Texture2D src){
			Texture2D copy = new Texture2D(src.width, src.height, src.format, src.mipmapCount > 1);
			copy.LoadRawTextureData(src.GetRawTextureData());
			copy.Apply();
			return copy;
		}
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
		public static Texture2D CombineTextures(params Texture2D[] args){
			if (args.Length == 0) return null;
			if (args.Length == 1) return CloneTexture(args[0]);
			Texture2D tex = args[0];
			for (int i = 1; i < args.Length; i++)
				tex = CombineTextures(tex, args[i]);

			return tex;
		}
		public static bool DataExists(this Texture2D tex) => data.ContainsKey(tex);
		public static T LoadResource<T>(string path, bool forceReload = false)
		where T : UnityEngine.Object {
			if (cache.ContainsKey(path) && !forceReload) return cache[path] as T;
			T resource = Resources.Load<T>(path);
			cache.Add(path, resource);
			return resource;
		}
		public static List<T> LoadAllResources<T>(string path)
		where T : UnityEngine.Object {
			UnityEngine.Object[] objs = Resources.LoadAll(path);
			List<T> resources = new List<T>();
			foreach (UnityEngine.Object obj in objs) resources.Add(obj as T);
			return resources;
		}
		public static Texture2D ReplaceTextureColor(Texture2D src, Color toReplace, Color repl,
			bool alphaMatch = false, bool ignoreAlpha = false
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
				if (!alphaMatch){
					if (srcColor == toReplace) dest.SetPixel(x, y, repl);
					else if (ignoreAlpha && equals(srcColor, toReplace)) dest.SetPixel(x, y, repl);
				} else {
					if (equals(srcColor, toReplace)){
						dest.SetPixel(x, y, new Color(repl.r, repl.g, repl.b, srcColor.a));
					}
				}
			};

			if (!src.DataExists()) UpdateImageData(src);
			foreach(Vector2 p in data[src].VisiblePoints)
				perform((int)p.x, (int)p.y, src.GetPixel((int)p.x, (int)p.y));

			dest.Apply();
			return dest;
		}
		public static Coroutine StartCoroutine(IEnumerator routine) => Main.startCoroutine(routine);
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

		public class CoroutineData {
			public object Result { get; set; }
			public Coroutine Coroutine { get; set; }
			private IEnumerator enumerator { get; set; }

			public CoroutineData(IEnumerator e){
				enumerator = e;
				Coroutine = StartCoroutine(Run());
			}
			private IEnumerator Run(){
				while (enumerator.MoveNext()){
					Result = enumerator.Current;
					yield return Result;
				}
			}
		}
		public class ImageData {
			public List<Vector2> VisiblePoints { get; set; }
			public ImageData(List<Vector2> vps){
				VisiblePoints = vps;
			}
		}
	}
}