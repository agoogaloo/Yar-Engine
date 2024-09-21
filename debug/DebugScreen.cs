using YarEngine.Graphics;
using YarEngine.Saves;
using Raylib_cs;

namespace YarEngine.Debug;

public class DebugScreen {
	private Dictionary<string, Func<DebugModule>> possibleModules = [];
	private List<DebugModule> activeModules = [];
	public Terminal terminal;

	private HashSet<string> moduleBlacklist = [];
	public DebugScreen() {
		moduleBlacklist = new(SaveManager.GetData<string[]>("blacklist", SaveManager.debugSavePath));
		terminal = new();
		terminal.AddCommand("addDebugMod", SaveAddModule, false);
		terminal.AddCommand("rmvDebugMod", SaveRemoveModule, false);
		terminal.AddCommand("activeDebugMods", ActiveModsCommand, "lists all available debug modules", false);
		terminal.AddCommand("debugModList", ListModsCommand, "lists all active debug modules", false);
		terminal.AddCommand("debugModPropList", ModulePropsCommand, false);
		terminal.AddCommand("DebugModConfigGen", ConfigGenCommand, false);

		terminal.AddCommand("fontSize", FontSizeCommand, false);
	}

	public void Update(double time) {
		foreach (DebugModule m in activeModules) {
			m.Update(time);
		}
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

	public void SetFont(Font f, float scale = 2) {
		int size = (int)(f.BaseSize * scale);
		terminal.font = f;
		terminal.fontSize = size;
		foreach (DebugModule m in activeModules) {
			m.font = f;
			m.fontSize = size;
		}
	}

	//debug module commands
	private void FontSizeCommand(String options) {
		_ = float.TryParse(options, out float size);
		SetFont(terminal.font, size);
		terminal.Echo("set font scaled to " + size + " * base size");

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
		if (possibleModules.ContainsKey(option)) {
			terminal.Echo("Generating config");
			SaveManager.SaveData<bool>(true, "generateConfig", "res/saves/debug/misc.config");
			SaveAddModule(option);
			SaveManager.SaveData<bool>(false, "generateConfig", "res/saves/debug/misc.config");
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
		SaveManager.SaveData<string[]>([.. moduleBlacklist], "blacklist", SaveManager.debugSavePath);
	}

	public void SaveRemoveModule(string name) {
		moduleBlacklist.Add(name);
		SaveManager.SaveData<string[]>([.. moduleBlacklist], "blacklist", SaveManager.debugSavePath);
		for (int i = activeModules.Count - 1; i >= 0; i--) {
			if (activeModules[i].name == name) {
				activeModules.RemoveAt(i);
			}
		}
	}
}
