using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
public sealed class GameTests: GameDelegate {

	public GameTests() {
		game = null;
		Player p1 = new Player();
		Player p2 = new Player();
		Player[] ps = { p1, p2 };
		this.players = ps;
	}
	private Game game;
	private Player[] players;
	public void gameTest() {

		ScrabbleBoard board = new ScrabbleBoard(ScrabbleBoardConfiguration.LARGE);
		
		this.game = new Game(new ScrabbleGameConfiguration(3), board, players, this);
		this.game.start();
	}

	public void playerDrawTiles(Player player) {
		//Debug.Log("Player " + player + "'s rack contains: " + Arrays.toString(player.tiles.toArray()));
	}
	private static String stringify<T>(T[] array) {
		string s = "";
		foreach (T e in array) {
			s += e.ToString () + " ";
		}
		return s;
	}
	public void playersTurn(Player player) {
		Debug.Log ("Players turn: " + player);
		Debug.Log ("Players rack: " + GameTests.stringify(player.tiles.ToArray()));
		game.solve(player);
	}
	public void predictionsDetermined(Player player, Coordinate coordinate, List<PredictionResult> predictions) {

	}
	private List<PredictionResult> predictions = null;
	public void solutionDetermined(Player player, List<PredictionResult> predictions) {
		this.predictions = predictions;
		if (predictions.Count > 0) {
			AbstractPlayerMove best = predictions[0];
			game.play(player, best);
		}
		else {
			Debug.Log("Player: " + player + " has no options to move.");
		}
	}
	public void playerScored(Player player, int score) { 
		// stub
	}
	public void playerWon(Player player) {
//		System.exit(0);

	}
	public void scoreboardUpdated(Scoreboard scoreboard) {
		// stub
		Debug.Log(scoreboard);
	}
	public void boardUpdated(ScrabbleBoard board) {
		Debug.Log("Board");
		Debug.Log(board);
		
		for (int i = 0; i < players.Length; i++) {
			Debug.Log("Player " + (i+1) + "'s rack contains: " + GameTests.stringify(players[i].tiles.ToArray()));
		}
	}
}
