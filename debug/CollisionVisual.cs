using System.Numerics;
using System.Xml;
using YarEngine.Graphics;
using YarEngine.Physics;
using Raylib_cs;

namespace YarEngine.Debug;

public class ColVisualiser : DebugModule {
	public Vector2 pos;
	public bool showLayers, showCoords;
	public Color[] colours = [Color.Red, Color.Blue, Color.Green, Color.Black, Color.Gray];

	public ColVisualiser(Vector2? pos = null, bool showLayers = true, bool showCoords = false, Font? font = null) : base(font) {
		this.pos = pos ?? new(3, 35);
		this.showLayers = showLayers;
		this.showCoords = showCoords;
		GameBase.debugScreen.terminal.AddCommand("collisionCoords", ExtraCommands.HitBoxCoords);
	}
	public override void DrawFull(GameCamera cam, float pixelScale) {
		if (showLayers) {
			int groupIndex = 0;

			foreach (KeyValuePair<Type, Dictionary<string, LinkedList<ICollider<object>>>> typePair in CollisionManager.groupDict) {
				foreach (KeyValuePair<string, LinkedList<ICollider<object>>> layersPair in typePair.Value) {
					string text = typePair.Key.Name + "." + layersPair.Key + ":" + layersPair.Value.Count;
					Vector2 size = Raylib.MeasureTextEx(font, text, font.BaseSize * 2, 1);
					Color col = colours[groupIndex % colours.Length];
					DrawLayer(layersPair.Value, cam, pixelScale, col, text, new(pos.X, pos.Y + groupIndex * (size.Y + 2)));
					groupIndex++;
				}
			}
		}
	}
	private void DrawLayer(LinkedList<ICollider<Object>> objects, GameCamera cam, float pixelScale, Color col, string text, Vector2 textLoc) {
		//drawing layer label
		if (showLayers) {
			Vector2 size = Raylib.MeasureTextEx(font, text, font.BaseSize * 2, 1);
			Raylib.DrawRectangle((int)textLoc.X, (int)(textLoc.Y), (int)size.X + 3, (int)size.Y, col);
			Raylib.DrawTextEx(font, text, textLoc, font.BaseSize * 2, 1, Color.White);
		}
		//drawing collision boxes
		foreach (ICollider<object> node in objects) {
			if (node.ShowCollision) {
				DrawShape(node.Bounds, cam, pixelScale, col);
				DrawShape(node.Bounds.SnapToGrid(), cam, pixelScale, col);
			}
		}
	}
	private void DrawShape(Shape s, GameCamera cam, float pixelScale, Color col) {
		if (s is Circle c) {

			Vector2 drawLoc = c.Centre - cam.offset;
			Raylib.DrawCircleLinesV(drawLoc * pixelScale, c.radius * pixelScale, col);
		}
		else if (s is Rect r) {
			Vector2 loc = new(r.X - cam.offset.X, r.Y - cam.offset.Y);
			Raylib.DrawRectangleLines((int)(loc.X * pixelScale), (int)(loc.Y * pixelScale),
				(int)(r.Width * pixelScale), (int)(r.Height * pixelScale), col);
		}
		if (showCoords) {
			string locText = "(" + Math.Round(s.Centre.X) + " " + Math.Round(s.Centre.Y) + ")";
			Vector2 size = Raylib.MeasureTextEx(font, locText, font.BaseSize * 2, 1);
			Raylib.DrawTextEx(font, locText, (s.Centre - cam.offset) * pixelScale - size / 2, font.BaseSize * 2, 1, col);
		}

	}
}
