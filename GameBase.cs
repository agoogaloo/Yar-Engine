using System.Numerics;
using YarEngine.Debug;
using YarEngine.Graphics;
using YarEngine.Inputs;
using Raylib_cs;

namespace YarEngine;

public enum FullScreenMode {
	windowed, borderless, fullScreen
}
public static class GameBase {
	public static bool debugMode = false;
	public static DebugScreen debugScreen;
	private static float gameSpeedMulti = 1;
	private static bool perfectScaling = false;
	public static bool PerfectScaling {
		get { return perfectScaling; }
		set {
			perfectScaling = value;
			FixResize();
		}
	}
	private static FullScreenMode fullScreen = FullScreenMode.windowed;
	public static FullScreenMode FullScreen {
		get { return fullScreen; }
		set {
			fullScreen = value;
			FixFullScreen();
		}
	}
	public static float PixelScale { get; private set; } = 3;
	public static double updateTime = 1 / 120;
	public static Vector2 ScreenOffset { get; private set; } = Vector2.Zero;

	private static Vector2 gameSize = new(320, 180);
	public static Vector2 GameSize {
		get => gameSize;
		set {
			gameSize = value;
			FixResize();
		}
	}
	private static RenderTexture2D pixelTex;
	private static RenderTexture2D finalTex;
	private static Camera2D pixelCam;
	private static Rectangle gameRect;
	private static Rectangle screenRect;
	private static Rectangle windowRect;

	public static Color borderCol = Color.DarkPurple;

	public static Action<float> updateMethod = i => { };
	public static Action<GameCamera> pixelDraw = i => { };
	public static Action<GameCamera, float> screenDraw = (i, j) => { };
	public static GameCamera gameCam = new(new(GameSize.X, GameSize.Y));

	public static void Init(string name = "cool title") {
		Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
		// Raylib.SetConfigFlags(ConfigFlags.VSyncHint);
		Raylib.InitWindow((int)(gameSize.X * PixelScale), (int)(gameSize.Y * PixelScale), name);

		GameSize *= 1;
		pixelCam.Zoom = 1;

		pixelTex = Raylib.LoadRenderTexture((int)GameSize.X, (int)GameSize.Y);
		finalTex = Raylib.LoadRenderTexture((int)(GameSize.X * PixelScale), (int)(GameSize.Y * PixelScale));

		debugScreen = new();
		debugScreen.RegisterModule(delegate { return new FPSDisplay(); });
		debugScreen.RegisterModule(delegate { return new EntityCount(); });
		debugScreen.terminal.AddCommand("speed", SpeedCommand);
		debugScreen.terminal.AddCommand("skip", SkipCommand);
	}
	public static void Run() {
		double timePassed = 0;
		while (!Raylib.WindowShouldClose()) {
			timePassed += Raylib.GetFrameTime() * gameSpeedMulti;
			while (timePassed > updateTime) {
				Update(updateTime);
				timePassed -= updateTime;
			}

			if (debugMode) {
				debugScreen.Update(Raylib.GetFrameTime() * gameSpeedMulti);
			}
			if (Raylib.IsWindowResized()) {
				FixResize();
			}

			DrawGame();
		}
		Raylib.CloseWindow();
	}
	private static void Update(double time) {
		InputHandler.Update(time);
		updateMethod((float)time);
		if (InputHandler.GetButton("Debug") != null && InputHandler.GetButton("Debug").JustPressed) {
			debugMode = !debugMode;
			Console.WriteLine("Debug Mode:" + debugMode);
		}
	}

	public static void DrawGame() {
		//drawing the game at its original size to a texture
		Raylib.BeginTextureMode(pixelTex);
		Raylib.SetTextureFilter(pixelTex.Texture, TextureFilter.Point);
		Raylib.BeginMode2D(pixelCam);
		pixelDraw(gameCam);
		if (debugMode) {
			debugScreen.DrawPixel(gameCam);
		}
		Raylib.EndMode2D();
		Raylib.EndTextureMode();

		//drawing non pixel effects
		gameRect = new Rectangle(0, 0, gameSize.X, -gameSize.Y);
		screenRect = new Rectangle(0, 0, (int)(gameSize.X * PixelScale), (int)(gameSize.Y * PixelScale));

		Raylib.BeginTextureMode(finalTex);
		Raylib.DrawTexturePro(pixelTex.Texture, gameRect, screenRect, Vector2.Zero, 0, Color.White);
		screenDraw(gameCam, PixelScale);
		if (debugMode) {
			debugScreen.DrawFull(gameCam, PixelScale);
			// CollisionManager.DrawNames(Assets.smallPixel);
		}
		Raylib.EndTextureMode();

		//drawing final scaled texture to the screen
		screenRect = new Rectangle(0, 0, (int)(gameSize.X * PixelScale), (int)(-gameSize.Y * PixelScale));
		windowRect = new Rectangle((int)ScreenOffset.X, (int)ScreenOffset.Y, gameSize.X * PixelScale, gameSize.Y * PixelScale);

		Raylib.BeginDrawing();
		Raylib.ClearBackground(borderCol);
		Raylib.DrawTexturePro(finalTex.Texture, screenRect, windowRect, Vector2.Zero, 0, Color.White);
		// Raylib.DrawTexturePro(finalTex.Texture, souceRect, screenRect, screenOffset, 0, Color.White);
		Raylib.EndDrawing();
	}
	private static void FixResize() {
		//finding the biggest even pixel scale
		float xScale = Raylib.GetScreenWidth() / gameSize.X;
		float yScale = Raylib.GetScreenHeight() / gameSize.Y;
		Console.WriteLine("xSCale:" + xScale + " yScale" + yScale + " width:" + Raylib.GetScreenWidth() + " height:" + Raylib.GetScreenHeight());
		if (perfectScaling) {
			xScale = (int)xScale;
			yScale = (int)yScale;
		}
		PixelScale = Math.Min(xScale, yScale);
		//calculating offset, so the game is centred

		float xDiff = gameSize.X * PixelScale - Raylib.GetScreenWidth();
		float yDiff = gameSize.Y * PixelScale - Raylib.GetScreenHeight();

		//updating all the screen size dependant variables
		ScreenOffset = new((int)(-xDiff / 2), (int)(-yDiff / 2));
		pixelTex = Raylib.LoadRenderTexture((int)GameSize.X, (int)GameSize.Y);
		finalTex = Raylib.LoadRenderTexture((int)(GameSize.X * PixelScale), (int)(GameSize.Y * PixelScale));
	}
	private static void FixFullScreen() {
		GameSize *= 1;
		pixelCam.Zoom = 1;
		Console.WriteLine("Fullscreen:" + fullScreen + " bfscreen:" +
		 Raylib.IsWindowState(ConfigFlags.BorderlessWindowMode) + " fscreen:" + Raylib.IsWindowState(ConfigFlags.FullscreenMode));

		if (fullScreen == FullScreenMode.windowed) {
			Raylib.ClearWindowState(ConfigFlags.FullscreenMode);
			Raylib.ClearWindowState(ConfigFlags.BorderlessWindowMode);
			PixelScale = 3;
			int width = (int)(GameSize.X * PixelScale);
			int height = (int)(GameSize.Y * PixelScale);
			int screenW = Raylib.GetMonitorWidth(Raylib.GetCurrentMonitor());
			int screenH = Raylib.GetMonitorHeight(Raylib.GetCurrentMonitor());
			Raylib.SetWindowSize(width, height);
			Raylib.SetWindowPosition(screenW / 2 - width / 2, screenH / 2 - height / 2);
		}

		else if (fullScreen == FullScreenMode.borderless) {
			int width = Raylib.GetMonitorWidth(Raylib.GetCurrentMonitor());
			int height = Raylib.GetMonitorHeight(Raylib.GetCurrentMonitor());
			Raylib.SetWindowSize(width, height);
			Raylib.SetWindowState(ConfigFlags.FullscreenMode);
			Raylib.SetWindowState(ConfigFlags.BorderlessWindowMode);
			if (!Raylib.IsWindowFullscreen()) {
				Raylib.ToggleFullscreen();
			}
		}

		else if (fullScreen == FullScreenMode.fullScreen) {
			Console.WriteLine("WARNING: normal fullscreen is kinda broken because of window size not working right :(");
			int width = Raylib.GetMonitorWidth(Raylib.GetCurrentMonitor());
			int height = Raylib.GetMonitorHeight(Raylib.GetCurrentMonitor());
			Raylib.SetWindowSize(width, height);
			Raylib.SetWindowState(ConfigFlags.FullscreenMode);
			Raylib.ClearWindowState(ConfigFlags.BorderlessWindowMode);
			if (!Raylib.IsWindowFullscreen()) {
				Raylib.ToggleFullscreen();
			}
		}
		FixResize();
	}
	private static void SpeedCommand(string param) {
		bool validIn = float.TryParse(param, out float speed);
		if (validIn) {
			gameSpeedMulti = speed;
			debugScreen.terminal.Echo("game speed set to " + gameSpeedMulti);
		}
		else {
			debugScreen.terminal.Echo("params: <float> spd");
			debugScreen.terminal.Echo("multiplies the game speed by spd");
			debugScreen.terminal.Echo("current speed multiplier:" + gameSpeedMulti);
		}
	}
	private static void SkipCommand(string paramString) {
		float time = 0;
		string[] optionList = paramString.Split(' ');
		bool validIn = optionList.Length > 0 && float.TryParse(optionList[0], out time);
		int divisions = (int)(Math.Abs(time) * 30);
		if (optionList.Length > 1) {
			validIn &= int.TryParse(optionList[1], out divisions);
		}
		if (validIn) {
			for (int i = 0; i < divisions; i++) {
				updateMethod(time / divisions);
			}
		}
		else {
			debugScreen.terminal.Echo("params: <float> t, <int?> div");
			debugScreen.terminal.Echo("skips t seconds ahead with div update calls");
			debugScreen.terminal.Echo("longer skips should use more divisions");
		}
	}
}
