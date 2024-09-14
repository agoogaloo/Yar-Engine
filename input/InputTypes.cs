namespace YarEngine.Inputs;

public class Button {
	public bool Held { get; private set; }

	public double HoldTime { get; private set; }
	public bool JustPressed { get; private set; }
	public bool JustReleased { get; private set; }

	private bool heldThisFrame = false;

	public void Hold() {
		heldThisFrame = true;
	}
	/// <summary>
	/// called after all the input bindings have tried to press the button
	/// updates/sets all the held/justHeld/released fields
	/// </summary>
	public void Update(double time) {
		JustPressed = false;
		JustReleased = false;

		if (heldThisFrame) {
			if (!Held) {
				JustPressed = true;
			}
			Held = true;
			HoldTime += time;
		}
		else {
			if (Held) {
				JustReleased = true;
			}
			Held = false;
			HoldTime = 0;
		}

		heldThisFrame = false;
	}
	public void Consume() {
		JustPressed = false;

	}

	public override string ToString() {
		return "Button{" + "held:" + Held + "}";
	}
}

public class Analog {
	public float Value { get; private set; }
	public float maxFrameVal = 0;

	public void UpdateVal(float val) {
		maxFrameVal = Math.Max(val, maxFrameVal);
	}
	public void Update() {
		Value = maxFrameVal;
		maxFrameVal = 0;
	}
}


