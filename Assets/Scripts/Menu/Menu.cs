using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using BalloonBasket.Tech;
using BalloonBasket.Game;


public class Menu : MonoBehaviour {
	public static readonly string SCORE_KEY = "BEST_SCORE";

	[SerializeField] private GameObject _gameHolder;
	[SerializeField] private Buttons _buttons;
	[SerializeField] private List<MenuButton> _menuButtons;
	[SerializeField] private Texture2D _downImage;
	[SerializeField] private Texture2D _upImage;
	[SerializeField] private Texture2D _hoverImage;
	[SerializeField] private Texture2D _highlightedImage;

	[SerializeField] private ScoreBoard _highScore;

	public float BestTime {
		get {
			return PlayerPrefs.GetFloat(Menu.SCORE_KEY);
		}
		set {
			PlayerPrefs.SetFloat(Menu.SCORE_KEY, value);
			PlayerPrefs.Save();
			this._highScore.SetScore(value);
		}
	}

	void OnGUI() {
		if (GUI.Button (new Rect (5f, 5f, 100f, 100f), "FADE IN")) {
			this._highScore.FadeIn(1f);
		}
		if (GUI.Button (new Rect (205f, 5f, 100f, 100f), "FADE OUT")) {
			this._highScore.FadeOut(1f);
		}
	}

	void Awake() {
		Holoville.HOTween.HOTween.Init(false, false, true);
		Holoville.HOTween.HOTween.EnableOverwriteManager();

		if (!PlayerPrefs.HasKey (Menu.SCORE_KEY) || Mathf.Approximately(this.BestTime, 0f)) {
			//this._highScore.gameObject.SetActive(false);
			//this.BestTime = 0f;
		}

		Sprite downButton = Sprite.Create(this._downImage,
		                                  new Rect(0f, 0f, this._downImage.width, this._downImage.height),
		                                    new Vector2(0.5f, 0.5f), 1f);
		Sprite upButton = Sprite.Create(this._upImage,
		                                new Rect(0f, 0f, this._upImage.width, this._upImage.height),
		                                  new Vector2(0.5f, 0.5f), 1f);
		Sprite hoverButton = Sprite.Create(this._hoverImage,
		                                   new Rect(0f, 0f, this._hoverImage.width, this._hoverImage.height),
		                                  new Vector2(0.5f, 0.5f), 1f);
		Sprite highlightedButton = Sprite.Create(this._highlightedImage,
		                                         new Rect(0f, 0f, this._highlightedImage.width, this._highlightedImage.height),
		                                  new Vector2(0.5f, 0.5f), 1f);
		
		
		foreach (MenuButton button in this._menuButtons) {
			button.DownState = downButton;
			button.UpState = upButton;
			button.HighlightedState = highlightedButton;
			button.HoverState = hoverButton;
			button.onButtonClick -= this.OnButtonClick;
			button.onButtonClick += this.OnButtonClick;
		}
	}

	private GameObject _clickedGo = null;
	private void OnButtonClick(GameObject go) {
		this._clickedGo = go;
		Holoville.HOTween.Tweener tweener = Holoville.HOTween.HOTween.To(this._highScore.gameObject.transform, 0.5f,
		                                                                 new Holoville.HOTween.TweenParms().Prop("localPosition", new Vector3(323f, 326f, 0f)).OnComplete(this.StartGame));
		//tweener.onComplete += StartGame;
	}

	private void StartGame() {
		this.gameObject.SetActive (false);
		this._gameHolder.SetActive (true);
		int index = this._menuButtons.IndexOf(this._clickedGo.GetComponent<MenuButton>());
		if(index != 3) {
			this._gameHolder.GetComponent<BalloonBasketMain>().StartLevel(LevelReader.ParseLevel(string.Format("level{0}", index+1)));
		} else {
			this._gameHolder.GetComponent<BalloonBasketMain>().StartRandom();
		}
	}

	void Update() {

	}
}
