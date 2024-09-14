
using System.Numerics;
using Raylib_cs;

namespace YarEngine.Physics;

public abstract class Shape {
	public virtual Vector2 Centre { get; set; }
	public virtual float CentreY {
		get { return Centre.Y; }
		set { Centre = new(Centre.X, value); }
	}
	public virtual float CentreX {
		get { return Centre.X; }
		set { Centre = new(value, Centre.Y); }
	}
	public virtual bool Intersects(Shape s, bool snapColliders = true) {
		Shape self = this;
		if (snapColliders) {
			s = s.SnapToGrid();
			self = self.SnapToGrid();
		}
		if (s is Circle c) {

			return self.IntersectsCircle(c);
		}
		else if (s is Rect r) {
			return self.IntersectsRect(r);
		}
		Console.WriteLine("WARNING: no method for checking collision with " + s.GetType());
		return false;

	}
	public abstract Shape SnapToGrid();
	protected abstract bool IntersectsRect(Rect r);
	protected abstract bool IntersectsCircle(Circle c);


}

public class Circle : Shape {
	public float radius;
	public Circle(float x, float y, float rad) {
		Centre = new(x, y);
		this.radius = rad;
	}
	public Circle(Vector2 centre, float radius) {
		this.Centre = centre;
		this.radius = radius;
	}
	protected override bool IntersectsCircle(Circle c) {
		return Raylib.CheckCollisionCircles(Centre, radius, c.Centre, c.radius);
	}

	protected override bool IntersectsRect(Rect r) {
		return Raylib.CheckCollisionCircleRec(Centre, radius, r.rectangle);
	}

	public override Shape SnapToGrid() {
		float roundedRad = (float)(Math.Round(radius * 2) / 2);
		if (roundedRad % 1 == 0) {
			return new Circle((int)Math.Round(Centre.X), (int)Math.Round(Centre.Y), roundedRad);

		}
		return new Circle((int)Math.Round(Centre.X - 0.5) + 0.5f, (int)Math.Round(Centre.Y - 0.5) + 0.5f, roundedRad);
	}
}
public class Rect(float x = 0, float y = 0, float width = 0, float height = 0) : Shape {
	public Rectangle rectangle = new(x, y, width, height);
	public float X {
		get { return rectangle.X; }
		set { rectangle.X = value; }
	}
	public float Y {
		get { return rectangle.Y; }
		set { rectangle.Y = value; }
	}
	public float Width {
		get { return rectangle.Width; }
		set { rectangle.Width = value; }
	}

	public float Height {
		get { return rectangle.Height; }
		set { rectangle.Height = value; }
	}
	public override Vector2 Centre {
		get {
			return new Vector2(rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Height / 2);
		}
		set {
			rectangle.X = value.X - Width / 2;
			rectangle.Y = value.Y - Height / 2;
		}
	}

	public override Shape SnapToGrid() {
		return new Rect((int)Math.Round(X), (int)Math.Round(Y), (int)Math.Round(Width), (int)Math.Round(Height));
	}

	protected override bool IntersectsCircle(Circle c) {
		return Raylib.CheckCollisionCircleRec(c.Centre, c.radius, rectangle);
	}

	protected override bool IntersectsRect(Rect r) {
		return Raylib.CheckCollisionRecs(rectangle, r.rectangle);
	}
}
