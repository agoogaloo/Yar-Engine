using System.Numerics;

namespace YarEngine.Utils;

public static class Utils {
	public static Vector2 Rotate(Vector2 vec, float angle) {
		return new Vector2(vec.X * MathF.Cos(angle) - vec.Y * MathF.Sin(angle),
					vec.X * MathF.Sin(angle) + vec.Y * MathF.Cos(angle));

	}

}
