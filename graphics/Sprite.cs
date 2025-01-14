using System.Numerics;
using YarEngine.Physics;
using Raylib_cs;

namespace YarEngine.Graphics;

public class Sprite {
	private static bool showTextures = true;
	private Texture2D texture;
	public Texture2D Texture {
		get { return texture; }
		set {
			texture = value;
			frameWidth = texture.Width / hFrames;
			frameHeight = texture.Height;
		}
	}

	public Vector2 offset;
	public bool centered = true;
	public int frameWidth, frameHeight;
	public int hFrames = 1, vFrames = 1, frame = 0;
	public double frameDelay = 0.1f, frameTimer = 0;
	public float rotation = 0;
	public bool loop = true, playing = true;

	public Sprite(Texture2D texture, Vector2 offset = new()) : this(texture, 1, offset) { }
	public Sprite(Texture2D texture, int frames, Vector2 offset = new()) {
		this.texture = texture;
		this.offset = offset;
		hFrames = frames;
		frameWidth = texture.Width / frames;
		frameHeight = texture.Height;
		GameBase.debugScreen.terminal.AddCommand("showSpriteTex", HideTextureCommand);

	}

	public void Update(double time) {
		if (!playing) return;

		frameTimer += time;
		if (frameTimer > frameDelay) {
			frame++;
			frameTimer = 0;
		}
		if (frame >= hFrames * vFrames) {
			if (loop)
				frame = 0;
			else {
				frame--;
				playing = false;
			}
		}
	}
	public void Draw(GameCamera cam, Vector2 loc) {
		if (!showTextures) return;
		loc += offset;
		if (centered) {
			loc.X -= frameWidth / 2f - 0.5f;
			loc.Y -= frameHeight / 2f - 0.5f;
		}
		cam.DrawTexture(texture, loc, new Rectangle(frameWidth * frame, 0, frameWidth, frameHeight), rotation);
	}
	public void Draw(GameCamera cam, Shape shape) {
		Draw(cam, shape.Centre);
	}
	private static void HideTextureCommand(string options) {
		if (options.ToLower() == "t" || options == "true") {
			showTextures = true;
			return;
		}
		if (options.ToLower() == "f" || options == "false") {
			showTextures = false;
			return;
		}
		GameBase.debugScreen.terminal.Echo("params: <T/F> val");
		GameBase.debugScreen.terminal.Echo("sets whether to show sprite textures or not");
	}

}
