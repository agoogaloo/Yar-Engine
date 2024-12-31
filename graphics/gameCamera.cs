using System.Numerics;
using YarEngine.Physics;
using Raylib_cs;

namespace YarEngine.Graphics;

public class GameCamera(Vector2 size) {
	public Vector2 offset, screenSize = size;

	public void Update(float delta) {

	}
	public void DrawPixel(Vector2 point, Color col) {
		Raylib.DrawPixel((int)Math.Round(point.X + offset.X), (int)Math.Round(point.Y + offset.Y), col);
	}


	public void DrawTexture(Texture2D texture, Vector2 pos, Color? tint = null) {
		DrawTexture(texture, pos, new Rectangle(0, 0, texture.Width, texture.Height), tint);
	}
	public void DrawTexture(Texture2D texture, Vector2 pos, Rectangle sourceRect, Color? tint = null) {
		tint ??= Color.White;

		pos -= offset;
		// pos.X -= 0.5f;
		// pos.Y -= 0.5f;
		pos.X = (float)Math.Round(pos.X - 0.5f);
		pos.Y = (float)Math.Round(pos.Y - 0.5f);

		Raylib.DrawTextureRec(texture, sourceRect, pos, (Color)tint);
		// Raylib.DrawTexture(texture, (int)Math.Round(x - 0.5 - offset.X), (int)Math.Round(y - 0.5 - offset.X), (Color)tint);
	}
	public void DrawShape(Shape s, Color col) {
		if (s is Circle c) {
			DrawCircle(c, col);
		}
		else if (s is Rect r) {
			DrawRect(r, col);
		}
		else {
			Console.WriteLine("WARNING: no method for drawing shape" + s.GetType());
		}
	}
	public void DrawText(Font font, string text, Vector2 pos, Color? tint = null) {
		tint ??= Color.White;

		pos -= offset;
		pos.X = (float)Math.Round(pos.X - 0.5f);
		pos.Y = (float)Math.Round(pos.Y - 0.5f);

		Raylib.DrawTextEx(font, text, pos, font.BaseSize, 1, (Color)tint);
	}
	public void DrawLine(Vector2 p1, Vector2 p2, Color? tint = null) {
		tint ??= Color.White;
		p1 -= offset;
		p2 -= offset;
		Raylib.DrawLine((int)Math.Round(p1.X), (int)Math.Round(p1.Y), (int)Math.Round(p2.X), (int)Math.Round(p2.Y), (Color)tint);


	}
	private void DrawRect(Rect r, Color col) {
		Raylib.DrawRectangleLines((int)Math.Round(r.X - offset.X), (int)Math.Round(r.Y - offset.Y),
		 (int)(r.Width + 0.5), (int)(r.Height + 0.5), col);
	}
	private void DrawCircle(Circle c, Color col) {
		Vector2 drawLoc = c.Centre - offset;
		Raylib.DrawCircleLinesV(drawLoc, c.radius, col);
	}

	public void Centre(Vector2 position) {
		offset.X = position.X - screenSize.X / 2;
		offset.Y = position.Y - screenSize.Y / 2;

	}
}
