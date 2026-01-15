namespace YarEngine.Inputs;

public class Button {
	public static float maxBuffer = 0.1f;
	private float bufferTimer = 0;
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
		JustReleased = false;

		if (bufferTimer <= 0) {
			JustPressed = false;
		}

		// JustPressed = false;
		if (heldThisFrame) {
			bufferTimer -= (float)time;
			if (!Held) {
				JustPressed = true;
				bufferTimer = maxBuffer;
			}
			Held = true;
			HoldTime += time;
		}
		else {
			bufferTimer = 0;
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
		bufferTimer = 0;

	}

	public override string ToString() {
		return "Button{" + "held:" + Held + "}";
	}
	public bool PressBuffered(bool consume = true) {
		if (!JustPressed) {
			return false;
		}
		Consume();
		return true;
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


