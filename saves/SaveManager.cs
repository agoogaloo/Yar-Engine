using System.Numerics;
using Raylib_cs;

namespace YarEngine.Saves;

public delegate object LoadType(string s);
public delegate string SaveType(object o);
public static class SaveManager {
	public static string savePath = "save.txt";
	private static Dictionary<Type, SaveType> saveMethods = new() {
		{typeof(string[]), i=>{
				return string.Join(",",(string[])i);}},
		{typeof(Vector2), i=>{
				return ((Vector2)i).X+","+((Vector2)i).Y;}},
		{typeof(Color), i=>{
				return ((Color)i).R+","+((Color)i).G+","+((Color)i).B+","+((Color)i).A;}},
	};
	private static Dictionary<Type, LoadType> loadMethods = new(){
		{typeof(string), i=>{return i;}},
		{typeof(int), i=>{return int.Parse((string)i);}},
		{typeof(bool), i=>{ return i=="True";}},
		{typeof(float), i=>{return float.Parse((string)i);}},
		{typeof(string[]), i=>{return i.Split(",");}},
		//vector loader
		{typeof(Vector2),i=>{
			string[] strs = i.Split(",");
			if(strs.Length<2){
				return Vector2.Zero;
			}
			return new Vector2(float.Parse(strs[0]), float.Parse(strs[1]));
		}},
		//colour loader
		{typeof(Color),i=>{
			string[] strs = i.Split(",");
			if(strs.Length<4){
				return Color.White ;
			}
			return new Color(int.Parse(strs[0]), int.Parse(strs[1]),int.Parse(strs[2]),int.Parse(strs[3]));
		}}

	};

	public static void AddType<T>(SaveType saveMethod, LoadType loadMethod) {
		saveMethods[typeof(T)] = saveMethod;
		loadMethods[typeof(T)] = loadMethod;
	}

	public static void SaveData<T>(string name, T data, string? path = null) {
		path ??= savePath;
		SortedDictionary<string, string> variables = GetSaveDataDict(path);
		if (saveMethods.ContainsKey(typeof(T))) {
			variables[name] = saveMethods[typeof(T)](data);
		}
		else {
			variables[name] = data.ToString();
		}
		// making the folder for the file if it doesnt exist
		if (!File.Exists(path)) {
			string folder = Path.GetDirectoryName(path);
			Console.WriteLine("making folder: " + folder);
			Directory.CreateDirectory(folder);
		}

		// writing new data to file
		File.WriteAllText(path, "");
		foreach (KeyValuePair<string, string> pair in variables) {
			File.AppendAllText(path, pair.Key + ":" + pair.Value + "\n");
		}
	}
	public static bool DataExists(string name, string? path = null) {
		path ??= savePath;
		if (GetSaveDataDict(path).ContainsKey(name)) {
			return true;
		}
		return false;
	}
	/**<summary>
	 * tries to load data from the given save file,
	 * returns def value if the name isnt found in the save
	 * <summary>
	 */
	public static T GetData<T>(string name, T def, string? path = null) {
		path ??= savePath;

		SortedDictionary<string, string> dict = GetSaveDataDict(path);
		if (dict.ContainsKey(name)) {
			return (T)loadMethods[typeof(T)](dict[name]);
		}
		Console.WriteLine("data name " + name + "not found in " + path);
		return def;
	}
	/**<summary>
	 * tries to load data from the given save file,
	 * returns value loaded from an empty string if it isn't in the save file
	 * <summary>
	 */
	public static T? GetData<T>(string name, string? path = null) {
		path ??= savePath;
		Console.WriteLine(GetSaveDataDict(path) + path);
		//setting the raw data as an empty string if there isn't any saved data with that name
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
