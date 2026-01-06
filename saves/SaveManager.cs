using System.Numerics;
using Raylib_cs;

namespace YarEngine.Saves;

public delegate object LoadType(string s);
public delegate string SaveType(object o);
public static class SaveManager {
	public static string saveFile = "save.txt", rootFolder = "res/saves/";
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
		{typeof(string[]), i=>{
								  if(i==""){ return new string[0];
								  }
								  return i.Split(",");}},
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

	public static void SaveData<T>(string name, T data, string? folder = null, string? file = null) {
		folder ??= rootFolder;
		file ??= saveFile;
		string path = folder + file;
		SortedDictionary<string, string> variables = GetSaveDataDict(folder, file);
		if (saveMethods.ContainsKey(typeof(T))) {
			variables[name] = saveMethods[typeof(T)](data);
		}
		else {
			variables[name] = data.ToString();
		}
		// making the folder for the file if it doesnt exist
		if (!File.Exists(path)) {
			string f = Path.GetDirectoryName(path);
			Console.WriteLine("making folder: " + f);
			Directory.CreateDirectory(f);
		}

		// create directories for save file if it doesn't exist
		if (!File.Exists(path)) {
			Directory.CreateDirectory(Path.GetDirectoryName(path));

		}

		// write save data to path
		File.WriteAllText(path, "");
		foreach (KeyValuePair<string, string> pair in variables) {
			File.AppendAllText(path, pair.Key + ":" + pair.Value + "\n");
		}
	}
	public static bool DataExists(string name, string? folder = null, string? file = null) {
		folder ??= rootFolder;
		file ??= saveFile;
		string path = folder + file;
		path ??= saveFile;
		if (GetSaveDataDict(folder, file).ContainsKey(name)) {
			return true;
		}
		return false;
	}
	/**<summary>
	 * tries to load data from the given save file,
	 * returns def value if the name isnt found in the save
	 * <summary>
	 */
	public static T GetData<T>(string name, T def, string? folder = null, string? file = null) {
		folder ??= rootFolder;
		file ??= saveFile;
		string path = folder + file;

		SortedDictionary<string, string> dict = GetSaveDataDict(folder, file);
		if (dict.ContainsKey(name)) {
			return (T)loadMethods[typeof(T)](dict[name]);
		}
		Console.WriteLine("data name " + name + " not found in " + path);
		return def;
	}
	/**<summary>
	 * tries to load data from the given save file,
	 * returns value loaded from an empty string if it isn't in the save file
	 * <summary>
	 */
	public static T GetData<T>(string name, string? folder = null, string? file = null) {
		folder ??= rootFolder;
		file ??= saveFile;
		string path = folder + file;
		// Console.WriteLine(GetSaveDataDict(folder, path) + path);
		//setting the raw data as an empty string if there isn't any saved data with that name
		var dict = GetSaveDataDict(folder, file);
		if (dict.ContainsKey(name)) {
			return (T)loadMethods[typeof(T)](dict[name]);
		}

		// if (dict.TryGetValue(name, out string raw)) {
		if (!File.Exists(path)) {
			throw new FileNotFoundException("Save file could not be found at '" + path + "' :(");
		}
		else {
			throw new FileNotFoundException("Value '" + name + "' not be found in '" + path + "' :(");
		}
	}
	public static SortedDictionary<string, string> GetSaveDataDict(string? folder=null, string? file = null) {
		folder ??= rootFolder;
		file ??= saveFile;
		string path = folder + file;

		string[] data = [];
		if (File.Exists(path)) {
			data = File.ReadAllLines(path);
		}
		SortedDictionary<string, string> result = [];
		foreach (string s in data) {
			int splitIndex = s.IndexOf(':');
			if (splitIndex != -1) {
				result.Add(s[..splitIndex], s[(splitIndex + 1)..]);
			}
			else {
				Console.WriteLine("WARNING! line '" + s + "' from file '" + path + "' has been corrupted");
			}
		}
		return result;
	}
}
