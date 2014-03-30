using UnityEngine;
using System.Collections;

public class Touch : MonoBehaviour {
	private Vector2 offset = new Vector2(100,100);
	
	void OnGUI () {
		for (int i = 0; i < Input.touchCount; ++i)
		{
			Vector2 pos = Input.GetTouch(i).position + offset;
			GUI.Label(new Rect(pos.x, Screen.height - pos.y, 50, 30), "(X) #" + i);
		}
	}
}
