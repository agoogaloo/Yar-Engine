using System.Numerics;
using YarEngine.Entities;
using YarEngine.Graphics;
using Raylib_cs;

namespace YarEngine.Debug;

public abstract class DebugModule {
	public Font font;
	public string name;
	public DebugModule(Font? f = null) {
		this.font = f ?? Raylib.GetFontDefault();
		name = this.GetType().Name;
	}
	public virtual void OnAdd() { }
	public virtual void Update(double time) { }
	public virtual void DrawPixel(GameCamera cam) { }
	public virtual void DrawFull(GameCamera cam, float pixelScale) { }
}

public class FPSDisplay : DebugModule {
	public Vector2 pos = new(3, 3);
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
			Raylib.DrawTextEx(font, "Entities:" + EntityManager.EntityCount, pos, font.BaseSize * 2, 1, colour);
		}

	}
}
