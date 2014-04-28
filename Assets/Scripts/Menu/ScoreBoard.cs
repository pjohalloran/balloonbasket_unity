using System;
using System.Collections;

using UnityEngine;

using Holoville.HOTween;

using BalloonBasket.Tech;

public class ScoreBoard : MonoBehaviour {
	[SerializeField] private TextMesh _score;
	[SerializeField] private SpriteRenderer _bg;

	public float Alpha {
		get {
			return this._bg.color.a;
		} set {
			Color bgColor = this._bg.color;
			Color scoreColor = this._score.color;
			this._bg.color = new Color(bgColor.r, bgColor.g, bgColor.b, value);
			this._score.color = new Color(scoreColor.r, scoreColor.g, scoreColor.b, value);
		}
	}

	public void SetScore(float seconds) {
		this._score.text = Utils.SecondsToTimeString(seconds);
	}

	void Update() {
		if (Input.GetKeyUp (KeyCode.Z)) {
			PlayerPrefs.DeleteAll();
			PlayerPrefs.Save();
			Debug.Log("Cleared player prefs");
		}
	}

	public void FadeIn(float duration) {
		HOTween.To(this, 0.5f,
		           new TweenParms().Prop("Alpha", 1f).Ease(EaseType.EaseInOutExpo));
	}

	public void FadeOut(float duration) {
		HOTween.To(this, 0.5f,
		           new TweenParms().Prop("Alpha", 0f).Ease(EaseType.EaseInOutExpo));
	}
}
