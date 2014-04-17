using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using BalloonBasket.Tech;

public class LevelReader : MonoBehaviour {
	public static readonly char O_BALLOON_START   = 'X';
	public static readonly char O_EMPTY_SPACE     = '0';
	public static readonly char O_MINE            = '1';
	public static readonly char O_SEAGULL         = '2';
	public static readonly char O_TREETOP         = '3';
	public static readonly char O_TREETOPTALL     = '4';
	public static readonly char O_WIND150         = '8';
	public static readonly char O_WIND200         = '9';
	public static readonly char O_FINISH          = 'F';

	public static List<List<char>> ParseLevel(string filename) {
		TextAsset asset = Utils.LoadResource (filename) as TextAsset;

		List<List<char>> data = new List<List<char>>();

		int rI = 0;
		int cI = 0;

		data.Add(new List<char>());

		bool skipChar = false;
		foreach (char c in asset.text) {
			if(c == '\n') {
				Debug.Log ("New Line");
				data.Add(new List<char>());
				rI++;
				cI = 0;
				//skipChar = true;
			} else if(c == LevelReader.O_BALLOON_START) {
				Debug.Log (string.Format("Balloon Start at {0},{1}", rI, cI));
			} else if(c == LevelReader.O_EMPTY_SPACE) {
				Debug.Log (string.Format("Empty at {0},{1}", rI, cI));
			} else if(c == LevelReader.O_MINE) {
				Debug.Log (string.Format("Mine at {0},{1}", rI, cI));
			} else if(c == LevelReader.O_SEAGULL) {
				Debug.Log (string.Format("Seagull at {0},{1}", rI, cI));
			} else if(c == LevelReader.O_TREETOP) {
				Debug.Log (string.Format("Treetop at {0},{1}", rI, cI));
			} else if(c == LevelReader.O_TREETOPTALL) {
				Debug.Log (string.Format("TreetopTall at {0},{1}", rI, cI));
			} else if(c == LevelReader.O_WIND150) {
				Debug.Log (string.Format("Wind Small at {0},{1}", rI, cI));
			} else if(c == LevelReader.O_WIND200) {
				Debug.Log (string.Format("Wind Large at {0},{1}", rI, cI));
			} else if(c == LevelReader.O_FINISH) {
				Debug.Log (string.Format("Finish at {0},{1}", rI, cI));
			} else {
				Debug.Log("Unknown char: " + c);
			}
			//if(skipChar) {
				data[rI].Add(c);
				skipChar = false;
			//}
			cI++;
		}

		return data;
	}
}
