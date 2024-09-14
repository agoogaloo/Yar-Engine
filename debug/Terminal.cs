using System.Runtime.CompilerServices;
using System.Xml;
using YarEngine.Graphics;
using YarEngine.Saves;
using Raylib_cs;

namespace YarEngine.Debug;

public class Terminal : DebugModule {
	bool open = false;
	public bool cheatMode = false;
	Dictionary<string, Action<string>> commandList = [];

	public int LinesShown {
		get { return lines.Length; }
		set { lines = new string[value]; }
	}
	private int lineIndex = 0;
	string[] lines = new string[16];

	public Terminal() {
		cheatMode = SaveManager.GetData<bool>("cheatMode", SaveManager.debugSavePath);
		ClearTerminal("");
		Echo(" --WELCOME TO THE DEBUG TERMINAL--");
		if (cheatMode) {
			Echo(" cheatmode enabled");
		}
		AddCommand("help", Help, false);
		AddCommand("echo", Echo, false);
		AddCommand("clear", ClearTerminal, false);
		AddCommand("sil", SilentExecute, false);
		AddCommand("cheatMode", CheatMode, false);
	}


	public override void Update(double time) {
		string line = lines[lineIndex];
		int key = Raylib.GetCharPressed();
		if (Raylib.IsKeyDown(KeyboardKey.LeftControl) || Raylib.IsKeyDown(KeyboardKey.RightControl)) {
			if (Raylib.IsKeyPressed(KeyboardKey.Backslash)) {
				open = !open;
			}
			if (Raylib.IsKeyPressed(KeyboardKey.Down) || Raylib.IsKeyPressed(KeyboardKey.J)) {
				lineIndex++;
				lineIndex %= LinesShown;
			}
			if (Raylib.IsKeyPressed(KeyboardKey.Up) || Raylib.IsKeyPressed(KeyboardKey.K)) {
				lineIndex--;
				lineIndex = lineIndex < 0 ? lineIndex + LinesShown : lineIndex;
			}
		}
		else if (!open) {
			return;
		}
		else if (Raylib.IsKeyPressed(KeyboardKey.Tab)) {
			AutoFill();
		}
		else if (Raylib.IsKeyPressed(KeyboardKey.Enter)) {
			ExecuteString(lines[lineIndex]);
		}
		else if ((Raylib.IsKeyPressedRepeat(KeyboardKey.Backspace) || Raylib.IsKeyPressed(KeyboardKey.Backspace))
			 && lines[lineIndex].Length > 0) {
			lines[lineIndex] = line.Remove(line.Length - 1);
		}
		else if (key > 0) {
			if ((key >= 32) && (key <= 125)) {
				lines[lineIndex] += (char)key;
			}
		}
	}
	public void AutoFill() {
		string input = lines[lineIndex];
		string completion = "";
		int completionIndex = 0;
		Echo(lines[lineIndex]);
		foreach (string command in commandList.Keys) {
			Console.WriteLine("current completion:" + completion + " checking command:" + command);
			int commandIndex = command.IndexOf(input);
			if (commandIndex == -1) {
				continue;
			}
			if (completion == "") {
				completion = command;
				completionIndex = commandIndex;
			}
			else {
				Console.WriteLine("command:" + command + " cmdidx:" + commandIndex + " cpltnidx:" + completionIndex + " completion:" + completion);
				//checking if the front of the completion should be trimmed
				for (int i = 0; i < Math.Min(completionIndex, commandIndex); i++) {
					Console.WriteLine("i:" + i + " cmdidx:" + commandIndex + " cpltnidx:" + completionIndex + " completion:" + completion);
					if (commandIndex - i < 0 || completion[completionIndex - i - 1] != command[commandIndex - i - 1]) {
						completion = completion[(completionIndex - i)..];
						completionIndex = i;
						break;
					}
				}
				Console.WriteLine("trimmeing end");
				//checking if the end of the completion should be trimmed
				for (int i = input.Length; i < completion.Length; i++) {
					Console.WriteLine("i:" + i + " cmdidx:" + commandIndex + " cpltnidx:" + completionIndex + " completion:" + completion);

					if (i >= Math.Min(command.Length - commandIndex, completion.Length - completionIndex) ||
					 completion[completionIndex + i] != command[commandIndex + i]) {
						completion = completion[..(completionIndex + i)];
						break;
					}
				}
			}

			// lineIndex++;
			// lineIndex %= LinesShown;
			// lines[lineIndex] = command;
			Echo(command);
			Console.WriteLine("largest common string is " + completion);
		}
		Console.WriteLine("askjlh largest common string is " + completion);
		lines[lineIndex] = completion;
	}


	public override void DrawPixel(GameCamera cam) {
		if (!open) {
			return;
		}
		int lineHeight = (int)Raylib.MeasureTextEx(font, "a", font.BaseSize, 1).Y + 1;
		int width = 250;

		int lineY = (int)(GameBase.GameSize.Y - lineHeight) - 3;
		Raylib.DrawRectangle(3, lineY - lineHeight * (LinesShown - 1) - 4, width, lineHeight * LinesShown, new(50, 50, 90, 200));
		Raylib.DrawRectangle(3, lineY - 1, width, lineHeight + 1, new(25, 25, 50, 255));
		Raylib.DrawTextEx(font, ">" + lines[lineIndex], new(5, lineY), font.BaseSize, 1, Color.White);
		for (int i = 1; i < LinesShown; i++) {
			int lineDrawI = (lineIndex - i) % LinesShown;
			lineDrawI = lineDrawI < 0 ? lineDrawI + LinesShown : lineDrawI;

			string line = lines[lineDrawI];
			Raylib.DrawTextEx(font, line, new(6, lineY - 2 - lineHeight * i), font.BaseSize, 1, Color.White);

		}
	}
	public void ExecuteString(string line, bool output = true) {
		string[] startLineArr = (string[])lines.Clone();
		int startLineIndex = lineIndex;
		lines[lineIndex] = line;
		lineIndex++;
		lineIndex %= LinesShown;
		lines[lineIndex] = "";
		int splitIndex = line.IndexOf(' ');
		string command = line;
		string options = "";
		if (splitIndex != -1 && splitIndex != line.Length - 1) {
			options = line[(splitIndex + 1)..];
			command = line[..splitIndex];
		}
		Console.WriteLine("executing command '" + command + "' with options'" + options + "'");
		if (commandList.TryGetValue(command, out Action<string>? value)) {
			value(options);
			if (!output) {

				lines = startLineArr;
				lineIndex = startLineIndex;
			}
		}
		else {
			Echo("command '" + command + "' not found. type help for help");
		}
	}
	private void SilentExecute(string command) {
		if (command == "-help") {
			Echo("args: <string> command");
			Echo("executes command without outputing any text");
		}
		else {
			ExecuteString(command, false);
		}
	}
	public void AddCommand(string name, Action<String> command, bool cheatModeOnly = true) {
		if (!cheatModeOnly || cheatMode) {
			commandList[name] = command;
		}
	}
	public void AddCommand(string name, Action function, string helpText = " - no description", bool cheatModeOnly = true) {
		void command(string input) {
			if (input == "") {
				function();
				return;
			}
			string[] outputs = helpText.Split("\n");
			foreach (string s in outputs) {
				Echo(s);
			}
		}
		AddCommand(name, command, cheatModeOnly);
	}
	public void RemoveCommand(string name) {
		commandList.Remove(name);
	}
	public void Echo(string text) {
		lines[lineIndex] = text;
		lineIndex++;
		lineIndex %= LinesShown;
		lines[lineIndex] = "";
	}
	private void Help(string text) {
		int linesLeft = LinesShown - 2;
		List<string> commands = new(commandList.Keys);
		_ = int.TryParse(text, out int page);
		page = Math.Max(page, 1);

		Echo("page " + page + "/" + ((commands.Count - 1) / linesLeft + 2));
		if (page == 1) {
			Echo(" --WELCOME TO THE DEBUG MENU--");
			Echo("   " + commands.Count + " commands found");
			Echo("");
			Echo("-use help <int> to see command list");
			Echo("-seperate command options with spaces");
			Echo("-use 'command -help' to see what a command does");
			Echo("-only basic commands will work unless cheatmode");
			Echo(" is active and the game is reset");
		}
		else {
			int commandIndex = (page - 2) * linesLeft;
			while (commandIndex < commands.Count && linesLeft >= 1) {
				Echo(" - " + commands[commandIndex]);
				commandIndex++;
				linesLeft--;
			}
		}
	}
	private void ClearTerminal(string text) {
		if (text == "-help" || text == "help") {
			Echo("clears all text from the terminal");
			return;
		}
		Array.Fill(lines, "");
	}
	private void CheatMode(string options) {
		if (options == "-help" || options == "-h") {
			Echo("args: <bool> active");
			Echo("activates/deactivates cheat mode");
			Echo("you might need to restart for full effects");
			return;
		}
		if (options == "t" || options == "true") {
			cheatMode = true;
			SaveManager.SaveData<bool>(true, "cheatMode", SaveManager.debugSavePath);
			Echo("cheat mode activated");
			Echo("restart to apply changes");
			return;
		}
		if (options == "f" || options == "false") {
			SaveManager.SaveData<bool>(false, "cheatMode", SaveManager.debugSavePath);
			Echo("cheat mode deactivated");
			Echo("restart to apply changes");
			return;
		}
		Echo("invalid option. use -help or -h for help");
	}
}
