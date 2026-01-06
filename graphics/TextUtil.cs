using System.Numerics;
using Raylib_cs;

namespace YarEngine.Graphics;

public class TextUtil {

	/**
	 *
	 * pos is the top, center of the text box
	 * lines can be seperated with '\n'
	 **/
	public static void Justify(Font font, string text, Vector2 pos, GameCamera cam, Color? tint = null, float spacing = 1) {
		string[] lines = text.Split("\n");
		float y = pos.Y;

		foreach (string l in lines) {
			// x = 
			Vector2 size = Raylib.MeasureTextEx(font, l, font.BaseSize, 1);
			Vector2 lPos = new(pos.X - (size.X / 2) + 0.5f, y);
			//Vector2 lPos = new((int)(pos.X - (size.X / 2) + 0.5f), y);
			cam.DrawText(font, l, lPos, tint);

			//y += (int)(size.Y + spacing + 0.5f);
			y += (int)(size.Y + spacing + 0.5f);
		}


	}
}
