using System.Numerics;
using Raylib_cs;
using YarEngine.Graphics;
using YarEngine.Inputs;

namespace YarEngine.Debug;

public class FModify : DebugModule {
	private static List<string> names = [];
	private static Dictionary<string, float> values = new();
	private Vector2 loc;
	private int textHeight, selected = -1;


	public FModify() {
		name = "FModify";
		loc = LoadProp<Vector2>("pos", new(300, 3), "display position");
		textHeight = (int)Raylib.MeasureTextEx(font, "P", fontSize, 1).Y + 1;
	}

	public override void Update(double time) {
		FindSelection();
		if (selected == -1) {
			return;
		}
		string selectedName = names[selected];
		// Console.WriteLine(selected);
		float speedMod = 1;


		if (Raylib.IsMouseButtonDown(MouseButton.Left)) {
			speedMod++;
		}
		if (Raylib.IsMouseButtonDown(MouseButton.Right)) {
			speedMod += 4;
		}

		if (Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift)) {
			speedMod *= 2;
			speedMod = 1f / speedMod;
		}

		values[selectedName] += 1 * speedMod * Raylib.GetMouseWheelMove();

	}
	private void FindSelection() {
		Vector2 pos = InputHandler.MousePos();
		selected = -1;

		// if mouse is outside the text, nothing is selected
		if (pos.X < loc.X || pos.X > loc.X + (5 * textHeight) || pos.Y < loc.Y || pos.Y > loc.Y + (names.Count * textHeight)) {
			return;
		}

		float relHeight = pos.Y - loc.Y;
		int idx = (int)Math.Floor(relHeight / textHeight);
		idx = Math.Min(names.Count - 1, idx);
		selected = idx;

	}

	public override void DrawFull(GameCamera cam, float pixelScale) {
		Vector2 drawLoc = loc;
		for (int i = 0; i < names.Count; i++) {
			Color col = Color.White;
			if (i == selected) {
				col = Color.Gray;
			}

			Raylib.DrawTextEx(font, names[i] + String.Format(":{0:F2}", values[names[i]]), drawLoc, fontSize, 1, col);
			drawLoc.Y += textHeight;
		}
	}

	public static float Get(string name, float def = 0f) {
		if (!values.ContainsKey(name)) {
			values[name] = def;
			names.Add(name);
		}
		return values[name];
	}
	public static void Set(string name, float val) {
		values[name] = val;
	}




}
