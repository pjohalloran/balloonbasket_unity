using UnityEngine;
using System.Collections;

public class Joystick : MonoBehaviour {
	public string Stick;

	void Update ()
	{
		const float smooth = 5.0F;
		const float tiltAngle = -90.0F;

		// See InputManager (Edit > Project > Input) for mapping from <string> to <axis>
		float x = Input.GetAxis(Stick + "StickH") * tiltAngle;
		float y = Input.GetAxis(Stick + "StickV") * tiltAngle;

		Quaternion target = Quaternion.Euler(y, x, 0);
		transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);
	}
}
