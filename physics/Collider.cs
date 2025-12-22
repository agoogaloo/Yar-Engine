namespace YarEngine.Physics;


public class Collider<BaseType> : ICollider<BaseType> {
	public bool ShowCollision { get; set; } = true;
	public Shape Bounds { get; set; }
	public string Group { get; protected set; }
	public readonly BaseType collisionObject;


	public Collider(Shape bounds, BaseType collisionObject, string group = "", bool active = true) {
		this.Bounds = bounds;
		this.collisionObject = collisionObject;
		this.Group = group;
		if (active) Add();
	}

	public void DoCollision<T>(Action<Collider<T>> onCollide, string group = "", bool pixelSnapped = true) {
		//checking if there is a collision layer with the matching name and type
		foreach (ICollider<object> collider in CollisionManager.GetLayer<T>(group)) {
			if (collider.Bounds.Intersects(Bounds, pixelSnapped) && collider != this) {
				onCollide((Collider<T>)collider);
			}
		}
	}
	public void DoCollision<T>(Action<T> onCollide, string group = "", bool pixelSnapped = true) {
		DoCollision<T>(i => onCollide(i.collisionObject), group, pixelSnapped);
	}

	public void Remove() {
		CollisionManager.RemoveCollider(this);
	}

	public void Add() {
		CollisionManager.AddCollider(this);
	}

}
