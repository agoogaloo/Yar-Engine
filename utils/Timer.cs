namespace YarEngine.Utils;

public delegate void TimerEnd();
public class GameTimer {

	public double time, timeLeft;
	public bool started, loop;

	public TimerEnd? timeout;

	public GameTimer(double time, bool started = true, bool loop = false, TimerEnd? func = null) {
		this.time = time;
		timeLeft = time;
		this.started = started;
		this.loop = loop;
		this.timeout = func;
	}

	public void Update(double updateTime) {
		if (!started) {
			return;
		}
		if (timeLeft <= 0) {
			started = false;
			timeout?.Invoke();
			if (loop) {
				timeLeft = time;
				started = true;
			}
		}
		timeLeft -= updateTime;
	}
	public void Start() {
		started = true;
		timeLeft = time;
	}


}
