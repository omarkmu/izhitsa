namespace Izhitsa.Utility {
	public partial class Database {
		[System.Serializable]
		public struct XmlData {
			public string Value;

			public XmlData(string value){
				Value = value;
			}
		} 
	}
}