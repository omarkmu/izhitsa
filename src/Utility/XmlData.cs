using System;

namespace Izhitsa.Utility {
	public partial class Database {
		/**
		 <summary>
		 Contains XML data as a string, for usage in <see cref="Database"/>.
		 </summary>
		 */
		[System.Serializable]
		public struct XmlData {
			/// <summary>The type of the XmlData.</summary>
			public Type Type;
			/// <summary>The XML.</summary>
			public string Value;

			/**
			 <summary>
			 Creates new XML data.
			 </summary>
			 <param name="value">The XML.
			 </param>
			 <param name="type">The value type.
			 </param>
			 */
			public XmlData(string value, Type type){
				Type = type;
				Value = value;
			}

			/**
			 <summary>
			 Deserializes the XML using the <see cref="Type"/> and returns the resultant object.
			 </summary>
			 */
			public object GetValue()
				=> Database.XmlDeserialize(Value, Type);
			/**
			 <summary>
			 Deserializes the XML to an object of type T.
			 </summary>
			 */
			public T GetValue<T>(){
				object obj = GetValue();
				return (obj is T) ? (T)obj : default(T);
			}
		} 
	}
}