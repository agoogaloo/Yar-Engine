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
	public int fontSize = 1;
	private bool generateConfig;
	public DebugModule(Font? f = null, float fontScale = 2, string configFolder = "res/saves/debug/") {
		this.font = f ?? Raylib.GetFontDefault();
		this.generateConfig = SaveManager.GetData<bool>("generateConfig", "res/saves/debug/misc.config");
		fontSize = (int)(font.BaseSize * fontScale);
		name = this.GetType().Name;
		configPath = configFolder + name + ".config";
	}
	public virtual void OnAdd() { }
	public virtual void Update(double time) { }
	public virtual void DrawPixel(GameCamera cam) { }
	public virtual void DrawFull(GameCamera cam, float pixelScale) { }

	protected T LoadProp<T>(string name, T def, string description = "") {
		configProps[name] = description;
		if (SaveManager.DataExists(name, configPath)) {
			return SaveManager.GetData<T>(name, configPath);
		}
		if (generateConfig) {
			SaveProp<T>(name, def);
		}
		return def;
	}
	protected void SaveProp<T>(string name, T data) {
		SaveManager.SaveData<T>(data, name, configPath);
	}
}

public class FPSDisplay : DebugModule {
	public Vector2 pos;

	public FPSDisplay() {
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

	public EntityCount(Vector2? pos = null, Font? font = null) : base(font) {
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
