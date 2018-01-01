using System;

namespace Izhitsa.Utility {
	public partial class Database {
		[System.Serializable]
		public struct XmlData {
			public Type Type;
			public string Value;

			public XmlData(string value, Type type){
				Type = type;
				Value = value;
			}

			public object GetValue()
				=> Database.XmlDeserialize(Value, Type);
			public T GetValue<T>(){
				object obj = GetValue();
				return (obj is T) ? (T)obj : default(T);
			}
		} 
	}
}