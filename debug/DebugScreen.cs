using YarEngine.Graphics;
using YarEngine.Saves;
using Raylib_cs;

namespace YarEngine.Debug;

public class DebugScreen {
	private Dictionary<string, DebugModule> possibleModules = [];
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
		terminal.AddCommand("fontSize", FontSizeCommand, false);
	}
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
	public void AddModule(DebugModule m) {
		possibleModules[m.name] = m;
		if (moduleBlacklist.Contains(m.name)) {
			return;
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

	public void SaveAddModule(string m) {
		moduleBlacklist.Remove(m);
		if (possibleModules.ContainsKey(m)) {
			AddModule(possibleModules[m]);
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

	public void SetFont(Font f, float scale = 2) {
		int size = (int)(f.BaseSize * scale);
		terminal.font = f;
		terminal.fontSize = size;
		foreach (DebugModule m in activeModules) {
			m.font = f;
			m.fontSize = size;
		}
	}
}
