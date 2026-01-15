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
	public bool flipH = false, flipV = false;
	public int frameWidth, frameHeight;
	public int hFrames = 1, vFrames = 1, frame = 0;
	public float frameDelay = 0.1f, frameTimer = 0;
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

	public void Update(float time) {
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
		Rectangle source = new Rectangle(frameWidth * frame, 0, frameWidth, frameHeight);

		loc += offset;
		if (flipH) {
			source.Width *= -1;
			loc.X -= 2 * offset.X;
			// loc.X-=1;

		}
		if (flipV) {
			source.Height *= -1;
			loc.Y -= 2 * offset.Y;
			loc.Y-=1;
		}

		if (centered) {
			loc.X -= frameWidth / 2f - 0.5f;
			loc.Y -= frameHeight / 2f - 0.5f;
		}
		cam.DrawTexture(texture, loc, source);
	}
	public void Draw(GameCamera cam, Shape shape) {
		Draw(cam, shape.Centre);
	}
	public void Restart() {
		playing = true;
		frame = 0;
		frameTimer = 0;

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

