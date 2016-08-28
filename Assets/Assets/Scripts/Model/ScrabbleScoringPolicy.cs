using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public sealed class ScrabbleScoringPolicy {

	public ScrabbleScoringPolicy() {}

	public bool isBingo(String word) {
		return word.Length >= 7;
	}

	public int score(String word, List<TileSpace> spaces) {

		if (word.Length != spaces.Count) {
			return 0;
		}

		int multiplier = 1;
		int adder = 0;
	
		for (int i = 0; i < spaces.Count; i++) {
			TileSpace space = spaces[i];
			if (space.type == TileSpaceType.TRIPLE_WORD_SCORE) {
				multiplier *= 3;
			}
			else if (space.type == TileSpaceType.DOUBLE_WORD_SCORE) {
				multiplier *= 2;
			}
			else if (space.type == TileSpaceType.CENTER) {
				multiplier *= 2;
			}
			else if (space.type == TileSpaceType.TRIPLE_LETTER_SCORE) {
				adder += valueOf(word.ToCharArray()[i]) * 2;
			}
			else if (space.type == TileSpaceType.DOUBLE_LETTER_SCORE) {
				adder += valueOf(word.ToCharArray()[i]);
			}
		}	

		int score = 0;
		foreach (char letter in word.ToCharArray()) {
			score += valueOf(letter);
		}

		score += adder;
		score *= multiplier;
		
		if (isBingo(word)) {
			score += 50;
		}	
	
		return score;
	} 
	
	private int valueOf(char letter) {
		return getScore(letter);
	}

	private int[] scores = {1,3,3,2,1,4,2,4,1,8,5,1,3,1,1,3,10,1,1,1,1,4,4,8,4,10};
	private int getScore(char letter) {
		int index = getIndex(letter);
		return scores[index];
	}
	private int getIndex(char letter) {
		return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(letter);
	}
}
