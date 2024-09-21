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
	public DebugModule(Font? f = null, float fontScale = 2, string configFolder = "res/saves/debug/") {
		this.font = f ?? Raylib.GetFontDefault();
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
		return def;
	}
	protected void SaveProp<T>(string name, T data) {
		SaveManager.SaveData<T>(data, name, configPath);
	}
}

public class FPSDisplay : DebugModule {
	public Vector2 pos;

	public FPSDisplay() {
		Console.WriteLine(pos = LoadProp<Vector2>("pos", new Vector2(3, 3), "display location"));
		pos = LoadProp<Vector2>("pos", new Vector2(3, 3), "display location");
		SaveProp<Vector2>("pos", pos);

	}

	public override void DrawFull(GameCamera cam, float scale) {
		Raylib.DrawFPS((int)pos.X, (int)pos.Y);
	}
}
public class EntityCount : DebugModule {
	public Vector2 pos;
	public bool showLayers;
	public Color colour;

	public EntityCount(Vector2? pos = null, bool showLayers = false, Font? font = null, Color? colour = null) : base(font) {
		this.pos = pos ?? new(3, 18);
		this.colour = colour ?? Color.White;
		this.showLayers = showLayers;
	}

	public override void DrawFull(GameCamera cam, float scale) {
		if (showLayers) {
		}
		else {
			Raylib.DrawTextEx(font, "Entities:" + EntityManager.EntityCount, pos, fontSize, 1, colour);
		}
	}
}
