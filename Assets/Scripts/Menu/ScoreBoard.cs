using System;
using System.Collections;

using UnityEngine;

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
		StartCoroutine (FadeCoroutine(duration, 1f));
	}

	public void FadeOut(float duration) {
		StartCoroutine (FadeCoroutine(duration, 0f));
	}

	// TODO Fix
	private IEnumerator FadeCoroutine(float duration, float to) {
		float progress = 0f;
		float step = duration * 0.01f;
		float from = this.Alpha;
		float alphaStep = 0.01f * duration;
		float alphaProgress = 0f;
		while (progress < duration/*!Mathf.Approximately(this.Alpha, to)*/) {
			this.Alpha = Mathf.Lerp(from, to, alphaProgress);
			progress += step;
			alphaProgress += alphaStep;
			Debug.Log ("progress = " + progress);
			yield return new WaitForEndOfFrame();
		}
	}
}
