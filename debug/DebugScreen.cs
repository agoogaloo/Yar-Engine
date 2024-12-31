using YarEngine.Graphics;
using YarEngine.Saves;
using Raylib_cs;

namespace YarEngine.Debug;

public class DebugScreen {
	private Dictionary<string, Func<DebugModule>> possibleModules = [];
	private List<DebugModule> activeModules = [];
	public Terminal terminal;
	public static string configFolder = "res/saves/debug/";
	public static float fontScale { get; protected set; }
	public static Font defaultFont { get; protected set; } = Raylib.GetFontDefault();

	private HashSet<string> moduleBlacklist = [];
	public DebugScreen() {
		fontScale = SaveManager.GetData<float>("fontScale", 3, configFolder + "misc.config");
		Console.WriteLine("screen fontsize:" + fontScale);
		moduleBlacklist = new(SaveManager.GetData<string[]>("blacklist", configFolder + "misc.config"));
		terminal = new();
		terminal.AddCommand("debugModAdd", SaveAddModule, false);
		terminal.AddCommand("debugModRmv", SaveRemoveModule, false);
		terminal.AddCommand("debugModListActive", ActiveModsCommand, "lists all available debug modules", false);
		terminal.AddCommand("debugModList", ListModsCommand, "lists all active debug modules", false);
		terminal.AddCommand("debugModPropList", ModulePropsCommand, false);
		terminal.AddCommand("debugModConfigGen", ConfigGenCommand, false);
		terminal.AddCommand("fontSize", FontSizeCommand, false);
		SetFont(terminal.font, fontScale);
	}

	public void Update(double time) {
		foreach (DebugModule m in activeModules) {
			m.Update(time);
		};
		terminal.Update(time);
	}
	public void DrawPixel(GameCamera cam) {
		foreach (DebugModule m in activeModules) {
			m.DrawPixel(cam);
		}
		terminal.DrawPixel(cam);
	}
	public void DrawFull(GameCamera cam, float pixelScale) {
		foreach (DebugModule m in activeModules) {
			m.DrawFull(cam, pixelScale);
		}
		terminal.DrawFull(cam, pixelScale);
	}

	public void RegisterModule(Func<DebugModule> modFunc) {
		DebugModule m = modFunc();
		possibleModules[m.name] = modFunc;
		if (moduleBlacklist.Contains(m.name)) {
			return;
		}
		activeModules.Add(m);
		m.OnAdd();
	}
	private void AddModule(DebugModule m) {
		if (moduleBlacklist.Contains(m.name)) {
			return;
		}
		for (int i = 0; i < activeModules.Count; i++) {
			if (activeModules[i].name == m.name) {
				activeModules.RemoveAt(i);
				break;
			}
		}
		activeModules.Add(m);
		m.OnAdd();
	}
	public void RemoveModule(DebugModule m) {
		activeModules.Remove(m);
	}
	public T? GetModule<T>() where T : DebugModule {
		foreach (DebugModule m in activeModules) {
			if (m is T t) {
				return t;
			}
		}
		return null;
	}

	public void SetFont(Font f, float scale = -1) {
		defaultFont = f;
		if (scale == -1) {
			scale = fontScale;
		}
		int size = (int)(f.BaseSize * scale);
		foreach (DebugModule m in activeModules) {
			m.font = f;
			m.fontSize = size;
		}
		terminal.font = f;
		terminal.fontSize = size;
	}

	//debug module commands
	private void FontSizeCommand(String options) {
		bool validIn = float.TryParse(options, out float scale);
		if (!validIn) {
			terminal.Echo("a float font scale is required");
			return;
		}
		SetFont(terminal.font, scale);
		terminal.Echo("fontSize set to " + scale);
		SaveManager.SaveData<float>("fontScale", scale, configFolder + "misc.config");
	}
	private void ListModsCommand() {
		terminal.Echo("Module List:");
		foreach (string m in possibleModules.Keys) {
			terminal.Echo(" -" + m);
		}
	}
	private void ActiveModsCommand() {
		terminal.Echo("active modules:");
		foreach (DebugModule m in activeModules) {
			terminal.Echo(" -" + m.name);
		}
	}
	private void ModulePropsCommand(string options) {
		if (possibleModules.ContainsKey(options)) {
			terminal.Echo(options + " property list:");
			//finding the module with the same name
			foreach (DebugModule m in activeModules) {
				if (m.name == options) {
					//oputputting the options
					foreach (KeyValuePair<string, string> prop in m.configProps) {
						terminal.Echo("- " + prop.Key + ": " + prop.Value);
					}
				}
			}
			return;
		}
		//displaying help message if input isn't valid
		terminal.Echo("params: <string> module");
		terminal.Echo("shows a description of all config options");
		terminal.Echo("for the given debug module");
		terminal.Echo("debug module must be active for this command to work");
		terminal.Echo("by default options are set in res/saves/debug/<modName>.config");
	}
	public void ConfigGenCommand(string option) {
		if (option == "-h" || option == "") {
			terminal.Echo("params: <string> module");
			terminal.Echo("generates a config file for the given module");
		}
		if (possibleModules.ContainsKey(option)) {
			terminal.Echo("Generating config");
			SaveManager.SaveData<bool>("generateConfig", true, "res/saves/debug/misc.config");
			SaveAddModule(option);
			SaveManager.SaveData<bool>("generateConfig", false, "res/saves/debug/misc.config");
		}
		return;

		//displaying help message if input isn't valid
		terminal.Echo("params: <string> module");
		terminal.Echo("generates a config for the given module");
		terminal.Echo("debug module must be active for this command to work");
		terminal.Echo("by default options are set in res/saves/debug/<modName>.config");

	}

	public void SaveAddModule(string m) {
		moduleBlacklist.Remove(m);
		if (possibleModules.ContainsKey(m)) {
			AddModule(possibleModules[m]());
		}
		else {
			terminal.Echo("module '" + m + "' not found and counld not be added");
			terminal.Echo("module has still been removed from the blacklist");
		}
		SaveManager.SaveData<string[]>("blacklist", [.. moduleBlacklist], configFolder + "misc.config");
	}

	public void SaveRemoveModule(string name) {
		moduleBlacklist.Add(name);
		SaveManager.SaveData<string[]>("blacklist", [.. moduleBlacklist], configFolder + "misc.config");
		for (int i = activeModules.Count - 1; i >= 0; i--) {
			if (activeModules[i].name == name) {
				activeModules.RemoveAt(i);
			}
		}
	}
}
