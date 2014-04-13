using UnityEngine;
using System.Collections;

public class MenuButton : MonoBehaviour {
	public delegate void OnButtonClick(GameObject go);

	[SerializeField] private SpriteRenderer _sprite;
	[SerializeField] private BoxCollider _collider;

	public OnButtonClick onButtonClick;

	public Sprite UpState {
		get;
		set;
	}

	public Sprite DownState {
		get;
		set;
	}

	public Sprite HoverState {
		get;
		set;
	}

	public Sprite HighlightedState {
		get;
		set;
	}

	void OnMouseEnter() {
		Debug.Log ("OnMouseEnter");
		this._sprite.sprite = this.HoverState;
	}

	void OnMouseExit() {
		Debug.Log ("OnMouseExit");
		this._sprite.sprite = this.UpState;
	}

	void OnMouseDown() {
		Debug.Log ("OnMouseDown");
		this._sprite.sprite = this.DownState;
	}

	void OnMouseUp() {
		Debug.Log ("OnMouseUp");
		this._sprite.sprite = this.UpState;
	}

	void OnMouseUpAsButton() {
		if (this.onButtonClick != null) {
			Debug.Log ("OnMouseClick");
			this.onButtonClick(this.gameObject);
		}
	}
}
