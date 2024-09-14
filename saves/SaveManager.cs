using System.Numerics;

namespace YarEngine.Saves;

public delegate object LoadType(string s);
public delegate string SaveType(object o);
public static class SaveManager {
	public static string savePath = "save.txt";
	public static string debugSavePath = "debug.txt";
	private static Dictionary<Type, SaveType> saveMethods = new() {
		{typeof(string[]), i=>{return string.Join(",",(string[])i);}},
		{typeof(Vector2), i=>{return ((Vector2)i).X+","+((Vector2)i).Y;}},
	};
	private static Dictionary<Type, LoadType> loadMethods = new(){
		{typeof(string), i=>{return i;}},
		{typeof(int), i=>{return int.Parse((string)i);}},
		{typeof(bool), i=>{ return i=="True";}},
		{typeof(float), i=>{return float.Parse((string)i);}},
		{typeof(string[]), i=>{return i.Split(",");}},
		{typeof(Vector2),i=>{
			string[] strs = i.Split(",");
			if(strs.Length<2){
				return Vector2.Zero;
			}
			return new Vector2(float.Parse(strs[0]), float.Parse(strs[1]));
		}}


	};

	public static void AddType<T>(SaveType saveMethod, LoadType loadMethod) {
		saveMethods[typeof(T)] = saveMethod;
		loadMethods[typeof(T)] = loadMethod;
	}
	public static void SaveData<T>(T data, string name, string? path = null) {
		path ??= savePath;
		SortedDictionary<string, string> variables = GetSaveDataDict(path);
		if (saveMethods.ContainsKey(typeof(T))) {
			variables[name] = saveMethods[typeof(T)](data);
		}
		else {
			variables[name] = data.ToString();
		}

		File.WriteAllText(path, "");
		foreach (KeyValuePair<string, string> pair in variables) {
			File.AppendAllText(path, pair.Key + ":" + pair.Value + "\n");
		}
	}
	public static T? GetData<T>(string name, string? path = null) {
		path ??= savePath;


		Console.WriteLine(GetSaveDataDict(path) + path);
		if (!GetSaveDataDict(path).TryGetValue(name, out string raw)) {
			raw = "";
		}
		return (T)loadMethods[typeof(T)](raw);
	}
	public static SortedDictionary<string, string> GetSaveDataDict(string? path = null) {
		path ??= savePath;

		string[] data = [];
		if (File.Exists(path)) {
			data = File.ReadAllLines(path);
		}
		SortedDictionary<string, string> result = new();
		foreach (string s in data) {
			int splitIndex = s.IndexOf(":");
			if (splitIndex != -1) {
				result.Add(s[..splitIndex], s[(splitIndex + 1)..]);
			}
		}
		return result;
	}
}
