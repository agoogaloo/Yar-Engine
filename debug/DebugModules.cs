using System.Numerics;
using YarEngine.Entities;
using YarEngine.Graphics;
using YarEngine.Saves;
using Raylib_cs;

namespace YarEngine.Debug;

public abstract class DebugModule {
	public String configPath;
	public Dictionary<string, string> configProps { get; private set; } = new();
	public Font font;
	public string name;
	public float fontSize = 2;
	private bool generateConfig;
	public DebugModule(Font? f = null, string? configFolder = null) {
		font = f ?? DebugScreen.defaultFont;
		fontSize = font.BaseSize * DebugScreen.fontScale;
		configFolder ??= DebugScreen.configFolder;
		generateConfig = SaveManager.GetData<bool>("generateConfig", DebugScreen.configFolder + "misc.config");
		name = GetType().Name;
		configPath = configFolder + name + ".config";
		Console.WriteLine(name + "fontsize:" + DebugScreen.fontScale + ", " + fontSize);
	}
	public virtual void OnAdd() { }
	public virtual void Update(double time) { }
	public virtual void DrawPixel(GameCamera cam) { }
	public virtual void DrawFull(GameCamera cam, float pixelScale) { }

	protected T LoadProp<T>(string name, T def, string description = "") {
		configProps[name] = description;
		T propValue = SaveManager.GetData<T>(name, def, configPath);
		if (generateConfig) {
			SaveProp<T>(name, propValue);
		}
		return propValue;
	}
	protected void SaveProp<T>(string name, T data) {
		SaveManager.SaveData<T>(name, data, configPath);
	}
}

public class FPSDisplay : DebugModule {
	public Vector2 pos;

	public FPSDisplay() : base() {
		pos = LoadProp<Vector2>("pos", new Vector2(3, 3), "display location");
	}
	public override void DrawFull(GameCamera cam, float scale) {
		Raylib.DrawFPS((int)pos.X, (int)pos.Y);
	}
}

public class EntityCount : DebugModule {
	public Vector2 pos;
	public bool showLayers;
	public Color colour;

	public EntityCount(Font? font = null) : base(font) {
		this.pos = LoadProp<Vector2>("pos", new(3, 18), "display location");
		this.colour = LoadProp<Color>("colour", Color.White, "text colour");
		this.showLayers = LoadProp<bool>("showLayers", false, "(not implemented) show number of entities in each layer ");
	}

	public override void DrawFull(GameCamera cam, float scale) {
		if (showLayers) {
		}
		else {
			Raylib.DrawTextEx(font, "Entities:" + EntityManager.EntityCount, pos, fontSize, 1, colour);
		}
	}
}

public class Macro : DebugModule {

	private Action a;
	private Vector2 pos;
	private KeyboardKey key;
	public Macro(Action act, string name, KeyboardKey key) : base() {
		this.name = "M-" + name;
		configPath = DebugScreen.configFolder + name + ".config";
		this.key = key;
		a = act;
		// don't show by default
		pos = LoadProp<Vector2>("pos", new Vector2(-10, -100), "display location");
	}
	public override void DrawFull(GameCamera cam, float scale) {

		Raylib.DrawTextEx(font, name + ":" + key, pos, fontSize, 1, Color.White);
	}
	public override void Update(double time) {
		if (Raylib.IsKeyPressed(key)) {
			a();
		}
	}
}
