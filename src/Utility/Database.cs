using Izhitsa.Events;
using Izhitsa.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace Izhitsa.Utility {
	/**
	 <summary>Facilitates data storage and retrieval.</summary>
	 */
	[Serializable]
	public partial class Database : IEnumerable<KeyValuePair<string, object>> {
		/// <summary>Should the Database autosave?</summary>
		public bool Autosave {
			get { return autosave; }
			set {
				if (autosave == value) return;
				autosave = value;
				autoTask?.Cancel();
				if (autosave){
					autoTask = new Task();
					autoTask.Run(auto(autoTask));
				}
			}
		}
		/// <summary>Should the data be compressed while autosaving?</summary>
		public bool AutosaveCompress {
			get { return autosaveCompress; }
			set { autosaveCompress = value; }
		}
		/// <summary>The time to wait between autosaves, in seconds.</summary>
		public float AutosaveInterval {
			get { return autosaveInterval; }
			set { autosaveInterval = value; }
		}
		/// <summary>The name of the Database.</summary>
		public string Name => name;
		/// <summary>A broadcast which fires when an autosave begins.</summary>
		public Broadcast OnAutosaveStart { get; } = new Broadcast();
		/// <summary>A broadcast which fires when an autosave ends.</summary>
		public Broadcast OnAutosaveEnd { get; } = new Broadcast();
		/// <summary>The path to save to and load from.</summary>
		public string Path {
			get {
				return path;
			}
			set {
				path = value ?? System.IO.Path.Combine(Application.persistentDataPath, Name);
			}
		}


		/// <summary>Should the Database autosave?</summary>
		private bool autosave = false;
		/// <summary>Should the data be compressed while autosaving?</summary>
		private bool autosaveCompress = true;
		/// <summary>The time to wait between autosaves, in seconds.</summary>
		private float autosaveInterval = 300;
		/// <summary>The task which handles autosaving.</summary>
		private Task autoTask;
		/// <summary>The name of the Database.</summary>
		private string name;
		/// <summary>The dictionary in which data is stored.</summary>
		private Dictionary<string, object> data;
		/// <summary>The path to save and load from.</summary>
		private string path;

		/// <summary>A dictionary containing all registered databases.</summary>
		private static Dictionary<string, Database> databases
			= new Dictionary<string, Database>();
		//


		public object this[string key] {
			get { return Get(key); }
		}


		/**
		 <summary>
		 Creates a database with the name <paramref name="name"/> and the path
		 <paramref name="path"/>.
		 </summary>
		 <param name="name">The name of the Database.
		 </param>
		 <param name="path">The path to save to and load from.
		 </param>
		 */
		public Database(string name, string path = null){
			if (name == null)
				name = "Data";
			this.name = name;
			Path = path;
			data = new Dictionary<string, object>();
		}


		/// <summary>Clears the Database.</summary>
		public void Clear() => data.Clear();
		/**
		 <summary>
		 Returns a boolean representing whether or not the database
		 contains <paramref name="key"/>.
		 </summary>
		 <param name="key">The key to check for.
		 </param>
		 <exception cref="ArgumentNullExcepton">Thrown if <paramref name="key"/>
		 is null.</exception>
		 */
		public bool Contains(string key){
			if (key == null)
				throw new ArgumentNullException("key");
			return data.ContainsKey(key);
		}
		/**
		 <summary>
		 Deletes the database file.
		 </summary>
		 <returns>
		 A boolean representing whether or not the file
		 was deleted successfully.
		 </returns>
		 */
		public bool Delete(){
			if (Exists()){
				try {
					File.Delete(path);
					return true;
				} catch (Exception){
					return false;
				}
			}
			return false;
		}
		/**
		 <summary>
		 Returns a boolean representing whether or not
		 a file exists at <see cref="Path"/>.
		 </summary>
		 */
		public bool Exists() => File.Exists(path);
		/**
		 <summary>
		 Returns the object of type T associated with the
		 <paramref name="key"/>, or the default of that type if the key
		 does not exist or is of an incorrect type.
		 </summary>
		 <param name="key">The key to get.
		 </param>
		 <param name="canThrow">Should the method throw an exception if
		 there's a problem retrieving the value?
		 </param>
		 <exception cref="Exception">Thrown if there's a problem retrieving
		 the value.
		 </exception>
		 <exception cref="ArgumentNullException">Thrown if
		 <paramref name="key"/> is null.
		 </exception>
		 */
		public T Get<T>(string key, bool canThrow = false){
			if (key == null)
				throw new ArgumentNullException("key");
			T ret = default(T);
			if (data.ContainsKey(key)){
				if (data[key] is T){
					ret = (T)data[key];
				} else if (data[key] is XmlData){
					try {
						XmlData xData = (XmlData)data[key];
						object obj = XmlDeserialize(xData.Value, typeof(T));
						if (obj is T){
							ret = (T)obj;
						} else if (canThrow){
							throw new Exception($"Type does not match XML type.");
						}
					} catch (Exception){
						if (canThrow) throw;
					}
				} else if (canThrow){
					throw new Exception($"Type does not match value type.");
				}
			} else if (canThrow){
				throw new Exception($"{key} does not exist in the Database.");
			}
			return ret;
		}
		/**
		 <summary>
		 Returns the object associated with the
		 <paramref name="key"/>, or the null if the key
		 does not exist.
		 </summary>
		 <param name="key">The key to get.
		 </param>
		 */
		public object Get(string key){
			if (data.ContainsKey(key))
				return data[key];
			return null;
		}
		/**
		 <summary>
		 Loads the database from a file located at
		 <paramref name="path"/>.
		 </summary>
		 <param name="path">The path to load from. <see cref="Path"/>
		 by default.
		 </param>
		 <param name="isCompressed">Was the file compressed?
		 </param>
		 <exception cref="IOException">Thrown if there was a problem
		 loading the database.</exception>
		 */
		public void Load(string path = null, bool isCompressed = true){
			if (path == null) path = Path;
			Exception ex;
			int result = tryLoad(path, isCompressed, out ex);
			if (result == 1){
				throw ex;
			} else if (result == 2){
				throw new IOException($"A file does not exist at \"{path}\".");
			}
		}
		/**
		 <summary>Removes the data associated with the <paramref name="key"/>.
		 </summary>
		 */
		public void Remove(string key){
			if (data.ContainsKey(key))
				data.Remove(key);
		}
		/**
		 <summary>
		 Saves the database to a file located at
		 <paramref name="path"/>.
		 </summary>
		 <param name="path">The path to save to. <see cref="Path"/>
		 by default.
		 </param>
		 <param name="compress">Should the file be compressed?
		 </param>
		 <exception cref="IOException">Thrown if there was a problem
		 saving the database.</exception>
		 */
		public void Save(string path = null, bool compress = true){
			if (path == null) path = Path;
			Exception ex;
			if (!trySave(path, compress, out ex))
				throw ex;
		}
		/**
		 <summary>Sets <paramref name="key"/> to <paramref name="value"/>.
		 </summary>
		 <param name="key">The key to set.
		 </param>
		 <param name="value">The value to set.
		 </param>
		 <exception cref="ArgumentException">Thrown if the type of <paramref name="value"/>
		 is not serializable.</exception>
		 */
		public void Set<T>(string key, T value){
			object obj = value;
			if (!typeof(T).IsSerializable){
				string xml;
				try {
					xml = XmlSerialize(value);
					obj = new XmlData(xml, typeof(T));
				} catch (Exception){
					throw new ArgumentException("Value must be serializable.", "value");
				}
			}
			data[key] = obj;
		}
		/**
		 <summary>
		 Attempts to get the object of type T associated with the
		 <paramref name="key"/>. Returns a boolean representing success or failure.
		 </summary>
		 <param name="key">The key to get.
		 </param>
		 <param name="value">If successful, the value associated with the key.
		 Otherwise, the default of the type.
		 </param>
		 <exception cref="ArgumentNullException">Thrown if
		 <paramref name="key"/> is null.
		 </exception>
		 */
		public bool TryGet<T>(string key, out T value){
			value = default(T);
			try {
				value = Get<T>(key, true);
			} catch (Exception){
				return false;
			}
			return true;
		}
		/**
		 <summary>
		 Attempts to load the database from a file located at
		 <paramref name="path"/>. Returns a boolean representing success or failure.
		 </summary>
		 <param name="path">The path to load from. <see cref="Path"/>
		 by default.
		 </param>
		 <param name="isCompressed">Was the file compressed?
		 </param>
		 */
		public bool TryLoad(string path = null, bool isCompressed = true){
			if (path == null) path = Path;
			Exception ex;
			if (tryLoad(path, isCompressed, out ex) == 0){
				return true;
			}
			return false;
		}
		/**
		 <summary>
		 Attempts to save the database to a file located at
		 <paramref name="path"/>. Returns a boolean representing success or failure.
		 </summary>
		 <param name="path">The path to save to. <see cref="Path"/>
		 by default.
		 </param>
		 <param name="compress">Should the file be compressed?
		 </param>
		 */
		public bool TrySave(string path = null, bool compress = true){
			if (path == null) path = Path;
			Exception ex;
			if (!trySave(path, compress, out ex)){
				return false;
			}
			return true;
		}
		/**
		 <summary>Sets <paramref name="key"/> to <paramref name="value"/>.
		 Returns a boolean representing success or failure.
		 </summary>
		 <param name="key">The key to set.
		 </param>
		 <param name="value">The value to set.
		 </param>
		 */
		public bool TrySet<T>(string key, T value){
			try {
				Set(key, value);
			} catch (Exception){
				return false;
			}
			return true;
		}

		/**
		 <summary>
		 Creates a database named <paramref name="name"/> at
		 <paramref name="path"/>, and registers it.
		 </summary>
		 <param name="name">The name of the Database.
		 </param>
		 <param name="path">The path to save to and load from.
		 </param>
		 */
		public static Database Create(string name, string path = null){
			if (name == null)
				name = "Data";
			if (databases.ContainsKey(name))
				throw new ArgumentException($"The database \"{name}\" already exists.", "name");
			Database db = new Database(name, path);
			databases.Add(name, db);
			return db;
		}
		/**
		 <summary>
		 Returns the registered Database named <paramref name="name"/>, or null if a Database
		 with that name has not been registered.
		 </summary>
		 */
		public static Database GetDatabase(string name)
			=> (databases.ContainsKey(name)) ? databases[name] : null;
		/**
		 <summary>
		 Returns a dictionary containing all registered Databases.
		 </summary>
		 */
		public static Dictionary<string, Database> GetAllDatabases()
			=> new Dictionary<string, Database>(databases);
		/**
		 <summary>
		 Attempts to serialize <paramref name="obj"/> into XML.
		 </summary>
		 <param name="obj">The object to serialize.
		 </param>
		 <exception cref="InvalidOperationException">Thrown if
		 <paramref name="obj"/> cannot be serialized.
		 </exception>
		 */
		public static string XmlSerialize(object obj){
			StringBuilder sb = new StringBuilder();
			using (TextWriter writer = new StringWriter(sb)){
				XmlSerializer serializer = new XmlSerializer(obj.GetType());
				serializer.Serialize(writer, obj);
			}
			return sb.ToString();
		}
		/**
		 <summary>
		 Attempts to deserialize <paramref name="data"/> from XML into
		 a <paramref name="type"/> object.
		 </summary>
		 <param name="data">The string to deserialize.
		 </param>
		 <param name="type">The type of the object to deserialize to.
		 </param>
		 <exception cref="XmlException">Thrown if the XML is invalid.
		 </exception>
		 */
		public static object XmlDeserialize(string data, Type type){
			object res;
			using (TextReader reader = new StringReader(data)){
				XmlSerializer serializer = new XmlSerializer(type);
				res = serializer.Deserialize(reader);
			}
			return res;
		}

		/**
		 <summary>
		 Handles autosaving.
		 </summary>
		 <param name="task">The autosaving task.
		 </param>
		 */
		private IEnumerator auto(Task task){
			while (true){
				yield return new WaitForSecondsRealtime(AutosaveInterval);
				if (task.CancelRequested)
					throw new TaskCanceledException();
				OnAutosaveStart.Fire();
				TrySave(null, autosaveCompress);
				OnAutosaveEnd.Fire();
			}
		}
		/**
		 <summary>
		 Attempts to load the database from a file located at
		 <paramref name="path"/>. Returns a number representing success or failure.
		 </summary>
		 <param name="path">The path to load from. <see cref="Path"/>
		 by default.
		 </param>
		 <param name="isCompressed">Is the file compressed?
		 </param>
		 */
		private int tryLoad(string path, bool isCompressed, out Exception ex){
			ex = null;
			try {
				if (Exists()){
					using (FileStream fs = File.Open(path, FileMode.Open)){
						BinaryFormatter formatter = new BinaryFormatter();
						if (isCompressed){
							using (GZipStream gs = new GZipStream(fs, CompressionMode.Decompress)){
								data = (Dictionary<string, object>)formatter.Deserialize(gs);
							}
						} else {
							data = (Dictionary<string, object>)formatter.Deserialize(fs);
						}
					}
					return 0;
				} else {
					return 2;
				}
			} catch (Exception e){
				ex = e;
				return 1;
			}
		}
		/**
		 <summary>
		 Attempts to save the database to a file located at
		 <paramref name="path"/>. Returns a boolean representing success or failure.
		 </summary>
		 <param name="path">The path to save to. <see cref="Path"/>
		 by default.
		 </param>
		 <param name="compress">Should the file be compressed?
		 </param>
		 */
		private bool trySave(string path, bool compress, out Exception ex){
			ex = null;
			try {
				using (FileStream fs = File.Create(path)){
					BinaryFormatter formatter = new BinaryFormatter();
					if (compress){
						using (GZipStream gs = new GZipStream(fs, CompressionMode.Compress)){
							formatter.Serialize(gs, data);
						}
					} else {
						formatter.Serialize(fs, data);
					}
				}
				return true;
			} catch (Exception e){
				ex = e;
				return false;
			}
		}

		
		#region IEnumerable
		public IEnumerator<KeyValuePair<string, object>> GetEnumerator(){
			object value;
			foreach(KeyValuePair<string, object> pair in data){
				value = pair.Value;
				if (value is XmlData){
					value = ((XmlData)value).GetValue();
					yield return new KeyValuePair<string, object>(pair.Key, value);
				} else {
					yield return pair;
				}
			}
		}
		IEnumerator IEnumerable.GetEnumerator(){
			return this.GetEnumerator();
		}
		#endregion
	}
}