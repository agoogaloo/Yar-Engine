using CaptGrapple;
using YarEngine.Graphics;

namespace YarEngine.Entities;
public class EntityManager {
	public static int EntityCount {
		get {
			int result = 0;
			foreach (List<Entity> list in entities) {
				result += list.Count;
			}
			return result;
		}
		private set { }
	}
	private static List<List<Entity>> entities = [];
	private static List<Entity> entitiesToAdd = [];

	public static void Update(double updateTime) {
		foreach (List<Entity> list in entities) {
			foreach (Entity e in list) {
				e.Update(updateTime);
			}
		}

		foreach (List<Entity> list in entities) {
			for (int i = list.Count - 1; i >= 0; i--) {
				if (list[i].shouldRemove) {
					list[i].OnRemove();
					list.RemoveAt(i);
				}
			}
		}

		foreach (Entity e in entitiesToAdd) {
			AddEntity(e);
		}

		entitiesToAdd.Clear();

	}
	public static void Draw(GameCamera cam) {
		foreach (List<Entity> list in entities) {
			foreach (Entity e in list) {
				e.Draw(cam);
			}
		}
	}
	public static void QueueEntity(Entity e) {
		entitiesToAdd.Add(e);
	}
	private static void AddEntity(Entity e) {
		while (entities.Count <= e.UpdateIndex) {
			Console.WriteLine("entity list len" + entities.Count + " to short for updateIndex" + e.UpdateIndex + ". adding new list");
			entities.Add([]);
		}
		entities[e.UpdateIndex].Add(e);
		e.OnAdd();
	}
	public static void ClearLayer(params int[] layers) {
		foreach (int layer in layers) {
			foreach (Entity e in entities[layer]) {
				e.shouldRemove = true;
			}
		}
	}
	public static void ClearCommand(string options) {
		bool validIn = int.TryParse(options, out int layer);
		if (validIn) {
			ClearLayer(layer);
			return;
		}
		GameBase.debugScreen.terminal.Echo("args: <int> layer");
		GameBase.debugScreen.terminal.Echo("marks all entities in layer to get deleted");

	}
}
