
namespace YarEngine.Physics;

public interface ICollider<out T> {
	public Shape Bounds { get; protected set; }
	public bool ShowCollision { get; set; }
}
public class CollisionManager {
	//a wacky data structure that stores collision objects based on a specified type and group
	public static Dictionary<Type, Dictionary<string, LinkedList<ICollider<object>>>> groupDict = [];

	public static void AddCollider<T>(Collider<T> collider) {
		bool x = groupDict.TryAdd(typeof(T), []);
		bool y = groupDict[typeof(T)].TryAdd(collider.Group, []);
		LinkedList<ICollider<object>> colList = groupDict[typeof(T)][collider.Group];

		if (!colList.Contains((ICollider<object>)collider)) {
			colList.AddLast((ICollider<object>)collider);
		}
	}
	public static void RemoveCollider<T>(Collider<T> collider) {
		if (groupDict.ContainsKey(typeof(T)) && groupDict[typeof(T)].ContainsKey(collider.Group)) {
			groupDict[typeof(T)][collider.Group].Remove((ICollider<object>)collider);
		}
	}
	public static List<Type> GetLayerTypes() {
		return groupDict.Keys.ToList();
	}
	public static LinkedList<ICollider<Object>> GetLayer<T>(String name = "") {
		if (groupDict.ContainsKey(typeof(T)) && groupDict[typeof(T)].ContainsKey(name)) {
			return groupDict[typeof(T)][name];
		}
		Console.WriteLine("WARNING: Collision Layer " + name + " with type " + typeof(T) + " not found");
		return [];
	}

	public static void DoCollision<T>(Shape s, Action<Collider<T>> onCollide, string group = "", bool pixelSnapped = true) {
		//checking if there is a collision layer with the matching name and type
		foreach (ICollider<object> collider in GetLayer<T>(group)) {
			if (collider.Bounds.Intersects(s, pixelSnapped) ) {
				onCollide((Collider<T>)collider);
			}
		}
	}
	public void DoCollision<T>(Shape s, Action<T> onCollide, string group = "", bool pixelSnapped = true) {
		DoCollision<T>(s, i => onCollide(i.collisionObject), group, pixelSnapped);
	}
}
