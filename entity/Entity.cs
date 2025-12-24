using YarEngine.Graphics;
namespace YarEngine.Entities;
public abstract class Entity {

	public int UpdateIndex { get; protected set; } = 1;
	public bool shouldRemove = false;
	public virtual void Update(float updateTime) { }
	public virtual void Draw(GameCamera cam) { }
	public virtual void OnRemove() { }
	public virtual void OnAdd() { }
}

