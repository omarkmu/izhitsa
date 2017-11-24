using System;
using System.Collections.Generic;
using UnityEngine;

namespace Izhitsa {
	namespace Utility {
		/**
		 * <summary>
		 * Provides various operations for Texture2Ds.
		 * </summary>
		 */
		public static class Textures {
			/// <summary>Image data associated with a texture.</summary>
			private static Dictionary<Texture2D, TextureData> data { get; set; }
				= new Dictionary<Texture2D, TextureData>();
			
			/**
			 * <summary>
			 * Struct which contains the visible (non-transparent)
			 * points of an image.
			 * </summary>
			 */
			public struct TextureData {
				/// <summary>The visible points of the image.</summary>
				public List<Vector2> VisiblePoints { get; set; }
				/**
				 * <summary>Creates new ImageData.
				 * </summary>
				 * <param name="vps">A List of the image's visible points.</param>
				 */
				public TextureData(List<Vector2> vps){
					VisiblePoints = vps;
				}
			}

			/**
			 * <summary>
			 * Clones a texture.
			 * </summary>
			 * <param name="src">The texture to clone.</param>
			 */
			public static Texture2D Clone(this Texture2D src){
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
			public static Texture2D Combine(this Texture2D img1, Texture2D img2){
				if (img1 == null) throw new ArgumentNullException("img1");
				if (img2 == null) throw new ArgumentNullException("img2");

				Texture2D dest = img1.Clone();
				Action<int, int> perform = (x, y)=>{
					Color src = img1.GetPixel(x, y);
					Color other = img2.GetPixel(x, y);
					Color final = Color.Lerp(src, other, other.a / 1.0f);

					dest.SetPixel(x, y, final);
				};
				if (!img1.DataExists()) UpdateTextureData(img1);
				if (!img2.DataExists()) UpdateTextureData(img2);

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
			public static Texture2D Combine(params Texture2D[] args){
				if (args.Length == 0) return null;
				if (args.Length == 1) return args[0].Clone();
				Texture2D tex = args[0];
				for (int i = 1; i < args.Length; i++){
					if (args[i] == null) throw new System.ArgumentNullException($"Argument {i}");
					tex = Combine(tex, args[i]);
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
			public static Texture2D ReplaceColor(this Texture2D src, Color toReplace, Color repl,
				bool useSrcAlpha = false, bool ignoreAlpha = false
			){
				if (src == null) throw new ArgumentNullException("src");
				Texture2D dest = src.Clone();

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

				if (!src.DataExists()) UpdateTextureData(src);
				foreach (Vector2 p in data[src].VisiblePoints)
					perform((int)p.x, (int)p.y, src.GetPixel((int)p.x, (int)p.y));

				dest.Apply();
				return dest;
			}
			/**
			 * <summary>
			 * Updates the image data associated with a texture.
			 * </summary>
			 * <param name="tex">The texture to update.</param>
			 */
			public static TextureData UpdateTextureData(this Texture2D tex){
				List<Vector2> vis = new List<Vector2>();
				for (int x = 0; x < tex.width ; x++){
					for (int y = 0; y < tex.height; y++){
						if (tex.GetPixel(x, y).a != 0)
							vis.Add(new Vector2(x, y));
					}
				}
				
				TextureData imgData = new TextureData(vis);
				if (data.ContainsKey(tex)) data[tex] = imgData;
				else data.Add(tex, imgData);

				return imgData;
			}
		}
	}
}