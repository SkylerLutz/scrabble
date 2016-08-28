using System.Collections;
using System.Collections.Generic;

public sealed class Scoreboard {
	private Dictionary<Player, int> scores;
	public Scoreboard(Player[] players) {
		scores = new Dictionary<Player, int>();
		foreach (Player p in players) {
			scores.Add(p, 0);
		}
	}
	public int getScore(Player player) {
		return scores[player];
	}
	public void score(Player player, int score) {
		scores[player] = getScore(player) + score;
	}
//	public string toString() {
//		String s = "Current Standings... \n";
//		int i = 1;
//		for (Map.Entry<Player, Integer> entry : scores.entrySet()) {
//			s += "Player " + i + ": " + entry.getValue() + " points.\n";
//		}
//		return s;
//	}
}

