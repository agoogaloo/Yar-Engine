using System.Numerics;
using Raylib_cs;

namespace YarEngine.Inputs;

public enum GPadInput {
	//buttons
	FaceD, FaceR, X, Y, Start, Back, RStick, LStick, RShoulder, LShoulder,
	//dpad
	DUp, DLeft, DRight, DDown,
	//analog sticks
	RStickUp, RStickDown, RStickLeft, RStickRight, LStickUp, LStickDown, LStickLeft, LStickRight,
	//triggers
	RTrigger, LTrigger
}

public class InputHandler {
	public static double DeadZone { get; set; } = 0.25;


	private static Dictionary<string, Button> buttons = new();
	private static Dictionary<string, Analog> analogInputs = new();
	private static Dictionary<KeyboardKey, string> keyboardBinds = new();
	private static Dictionary<GPadInput, string> controllerBinds = new();


	public static void Update(double time) {
		/* var x = GamePad.GetState(PlayerIndex.One); */

		//updating all the keyboard inputs
		foreach (KeyValuePair<KeyboardKey, string> keybind in keyboardBinds) {
			if (Raylib.IsKeyDown(keybind.Key)) {
				//updating the input object depending on what type it is
				if (buttons.ContainsKey(keybind.Value)) {
					buttons[keybind.Value].Hold();
				}
				else if (analogInputs.ContainsKey(keybind.Value)) {
					analogInputs[keybind.Value].UpdateVal(1);
				}
				else {
					Console.WriteLine("Key '" + keybind.Key + "' does not belong to an input, but is still a keybind?");
				}
			}
		}
		//updating the controller inputs
		foreach (KeyValuePair<GPadInput, string> binding in controllerBinds) {
			float strength = GPadInputStength(binding.Key, 0);
			if (strength >= DeadZone) {
				if (buttons.ContainsKey(binding.Value)) {
					buttons[binding.Value].Hold();
				}
				else if (analogInputs.ContainsKey(binding.Value)) {
					analogInputs[binding.Value].UpdateVal(strength);
				}
				else {
					Console.WriteLine("input '" + binding.Key + "' does not belong to an input, but is still a controller binding?");
				}
			}
		}

		//updating the input objects
		foreach (KeyValuePair<string, Button> button in buttons) {
			button.Value.Update(time);
		}
		foreach (KeyValuePair<string, Analog> input in analogInputs) {
			input.Value.Update();
		}

	}
	/// <summary>
	/// a giant lookup to find the current input strength of a gamepad input
	/// 
	/// </summary>
	/// <param name="input"> what button/stick movement</param>
	/// <param name="player">the controller index</param>
	/// <returns>buttons return 1 or  if they are pressed or not, 
	/// sticks/triggers return a float from 0-1, 
	/// and an invalid input will return -1 (this should be impossible) </returns>
	private static float GPadInputStength(GPadInput input, int gamePad) {
		switch (input) {
			//buttons 
			case GPadInput.FaceD:
				return Raylib.IsGamepadButtonDown(gamePad, GamepadButton.RightFaceDown) ? 1 : 0;
			case GPadInput.FaceR:
				return Raylib.IsGamepadButtonDown(gamePad, GamepadButton.RightFaceRight) ? 1 : 0;

			/*case GPadInput.X: */
			/*	return GamePad.GetState(player).Buttons.X == ButtonState.Pressed ? 1 : 0; */
			/*case GPadInput.Y: */
			/*	return GamePad.GetState(player).Buttons.Y == ButtonState.Pressed ? 1 : 0; */
			/*case GPadInput.Start: */
			/*	return GamePad.GetState(player).Buttons.Start == ButtonState.Pressed ? 1 : 0; */
			/*case GPadInput.Back: */
			/*	return GamePad.GetState(player).Buttons.Back == ButtonState.Pressed ? 1 : 0; */
			/*case GPadInput.LShoulder: */
			/*	return GamePad.GetState(player).Buttons.LeftShoulder == ButtonState.Pressed ? 1 : 0; */
			/*case GPadInput.RShoulder: */
			/*	return GamePad.GetState(player).Buttons.RightShoulder == ButtonState.Pressed ? 1 : 0; */
			/*case GPadInput.LStick: */
			/*	return GamePad.GetState(player).Buttons.LeftStick == ButtonState.Pressed ? 1 : 0; */
			/*case GPadInput.RStick: */
			/*	return GamePad.GetState(player).Buttons.RightStick == ButtonState.Pressed ? 1 : 0; */
			/*//d-pad */
			/*case GPadInput.DUp: */
			/*	return GamePad.GetState(player).DPad.Up == ButtonState.Pressed ? 1 : 0; */
			/*case GPadInput.DDown: */
			/*	return GamePad.GetState(player).DPad.Down == ButtonState.Pressed ? 1 : 0; */
			/*case GPadInput.DLeft: */
			/*	return GamePad.GetState(player).DPad.Left == ButtonState.Pressed ? 1 : 0; */
			/*case GPadInput.DRight: */
			/*	return GamePad.GetState(player).DPad.Right == ButtonState.Pressed ? 1 : 0; */
			//joysticks 
			case GPadInput.LStickUp:
				return Math.Max(Raylib.GetGamepadAxisMovement(gamePad, GamepadAxis.LeftY), 0);
			case GPadInput.LStickDown:
				return Math.Max(-Raylib.GetGamepadAxisMovement(gamePad, GamepadAxis.LeftY), 0);
			case GPadInput.LStickLeft:
				return Math.Max(-Raylib.GetGamepadAxisMovement(gamePad, GamepadAxis.LeftX), 0);
			case GPadInput.LStickRight:
				return Math.Max(Raylib.GetGamepadAxisMovement(gamePad, GamepadAxis.LeftX), 0);
			/*case GPadInput.RStickUp: */
			/*	return Math.Max(GamePad.GetState(player).ThumbSticks.Right.Y, 0); */
			/*case GPadInput.RStickDown: */
			/*	return Math.Max(-GamePad.GetState(player).ThumbSticks.Right.Y, 0); */
			/*case GPadInput.RStickLeft: */
			/*	return Math.Max(-GamePad.GetState(player).ThumbSticks.Right.X, 0); */
			/*case GPadInput.RStickRight: */
			/*	return Math.Max(GamePad.GetState(player).ThumbSticks.Right.X, 0); */
			/*//triggers */
			/*case GPadInput.LTrigger: */
			/*	return GamePad.GetState(player).Triggers.Left; */
			/*case GPadInput.RTrigger: */
			/*	return GamePad.GetState(player).Triggers.Right; */
			default:
				return -1;
		}
	}

	public static void AddButtonBind(string name, KeyboardKey binding) {
		if (!buttons.ContainsKey(name)) {
			buttons.Add(name, new Button());
		}
		keyboardBinds.Add(binding, name);
	}
	public static void AddButtonBind(string name, GPadInput binding) {
		if (!buttons.ContainsKey(name)) {
			buttons.Add(name, new Button());
		}
		controllerBinds.Add(binding, name);
	}
	public static void AddAnalogBind(string name, KeyboardKey binding) {
		if (!analogInputs.ContainsKey(name)) {
			analogInputs.Add(name, new Analog());
		}
		keyboardBinds.Add(binding, name);
	}
	public static void AddAnalogBind(string name, GPadInput binding) {
		if (!analogInputs.ContainsKey(name)) {
			analogInputs.Add(name, new Analog());
		}
		controllerBinds.Add(binding, name);
	}
	public static Button GetButton(string name) {
		if (buttons.ContainsKey(name)) {
			return buttons[name];
		}
		return null;
	}
	public static Analog GetAnalog(string name) {
		if (analogInputs.ContainsKey(name)) {
			return analogInputs[name];
		}
		return null;

	}

	public static Vector2 MousePos() {
		Vector2 screenLoc = Raylib.GetMousePosition();
		return screenLoc - GameBase.ScreenOffset;
	}
	public static Vector2 MousePosPixel() {
		return MousePos() / GameBase.PixelScale;
	}
}

