
namespace YarEngine.Debug;

public static class ExtraCommands {

	public static void HitBoxCoords(string options) {
		Terminal terminal = GameBase.debugScreen.terminal;
		ColVisualiser module = GameBase.debugScreen.GetModule<ColVisualiser>();
		if (module == null) {
			terminal.Echo("command can't do anything because there");
			terminal.Echo("is no collision visualiser");
			return;
		}
		if (options.ToLower() == "t" || options == "true") {
			module.showCoords = true;
			return;
		}
		if (options.ToLower() == "f" || options == "false") {
			module.showCoords = false;
			return;
		}

		terminal.Echo("params: <T/F> val");
		terminal.Echo("sets whether to collision coordinates or not");
	}
}
