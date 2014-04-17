using System.Collections;

using UnityEngine;

using BalloonBasket.Tech;

public class LevelReader : MonoBehaviour {

	public void ParseLevel(string filename) {
		TextAsset asset = Utils.LoadResource (filename) as TextAsset;
		foreach (char c in asset.text) {

		}
	}
}
