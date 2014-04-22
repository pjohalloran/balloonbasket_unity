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

		data.Add(new List<char>());

		bool skipChar = false;
		foreach (char c in asset.text) {
			if(c == '\n') {
				data.Add(new List<char>());
				rI++;
				skipChar = true;
			}
			if(!skipChar) {
				data[rI].Add(c);
			} else {
				skipChar = false;
			}
		}

		return data;
	}
}
