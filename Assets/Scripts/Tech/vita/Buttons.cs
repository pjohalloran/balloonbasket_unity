using UnityEngine;
using System.Collections;

public class Buttons : MonoBehaviour {
	private static readonly string joystick1 = "joystick 1 button ";
	private static readonly int CROSS = 0;
	private static readonly int CIRCLE = 1;
	private static readonly int SQUARE = 2;
	private static readonly int TRIANGLE = 3;
	private static readonly int L = 4;
	private static readonly int R = 5;
	private static readonly int SELECT = 6;
	private static readonly int START = 7;
	private static readonly int UP = 8;
	private static readonly int RIGHT = 9;
	private static readonly int DOWN = 10;
	private static readonly int LEFT = 11;

	public bool CrossPressed {
		get;
		protected set;
	}

	public bool CirclePressed {
		get;
		protected set;
	}

	public bool SquarePressed {
		get;
		protected set;
	}

	public bool TrianglePressed {
		get;
		protected set;
	}

	public bool LTriggerPressed {
		get;
		protected set;
	}

	public bool RTriggerPressed {
		get;
		protected set;
	}

	public bool SelectPressed {
		get;
		protected set;
	}

	public bool StartPressed {
		get;
		protected set;
	}

	public bool UpPressed {
		get;
		protected set;
	}

	public bool DownPressed {
		get;
		protected set;
	}

	public bool LeftPressed {
		get;
		protected set;
	}

	public bool RightPressed {
		get;
		protected set;
	}

	private void Awake() {
		Reset();
	}

	private void Reset() {
		this.SquarePressed = false;
		this.CirclePressed = false;
		this.TrianglePressed = false;
		this.CrossPressed = false;
		this.LTriggerPressed = false;
		this.RTriggerPressed = false;
		this.SelectPressed = false;
		this.StartPressed = false;
		this.UpPressed = false;
		this.DownPressed = false;
		this.LeftPressed = false;
		this.RightPressed = false;
	}

	public void PollState() {
		Reset();
		if (Input.GetKey(joystick1 + Buttons.CROSS)) {
			this.CrossPressed = true;
		}
		if (Input.GetKey(joystick1 + Buttons.CIRCLE)) {
			this.CirclePressed = true;
		}
		if (Input.GetKey(joystick1 + Buttons.SQUARE)) {
			this.SquarePressed = true;
		}
		if (Input.GetKey(joystick1 + Buttons.TRIANGLE)) {
			this.TrianglePressed = true;
		}
		if (Input.GetKey(joystick1 + Buttons.L)) {
			this.LTriggerPressed = true;
		}
		if (Input.GetKey(joystick1 + Buttons.R)) {
			this.RTriggerPressed = true;
		}
		if (Input.GetKey(joystick1 + Buttons.SELECT)) {
			this.SelectPressed = true;
		}
		if (Input.GetKey(joystick1 + Buttons.START)) {
			this.StartPressed = true;
		}
		if (Input.GetKey(joystick1 + Buttons.UP)) {
			this.UpPressed = true;
		}
		if (Input.GetKey(joystick1 + Buttons.DOWN)) {
			this.DownPressed = true;
		}
		if (Input.GetKey(joystick1 + Buttons.LEFT)) {
			this.LeftPressed = true;
		}
		if (Input.GetKey(joystick1 + Buttons.RIGHT)) {
			this.RightPressed = true;
		}
	}
}
