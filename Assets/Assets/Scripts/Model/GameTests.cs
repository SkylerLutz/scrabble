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

		// config
		int numTiles = 7;
		int numResults = 3;
		ScrabbleBoardConfiguration size = ScrabbleBoardConfiguration.LARGE;


		ScrabbleBoard board = new ScrabbleBoard(size);

		this.game = new Game(new ScrabbleGameConfiguration(numResults, new PlayerConfiguration(numTiles)), board, players, this);
		this.game.start();
	}

	public void playerDrewTiles(Player player, Tile[] drawnTiles) {
		//Debug.Log("Player " + player + "'s rack contains: " + Arrays.toString(player.tiles.toArray()));
	}
	private static String stringify<T>(T[] array) {
		string s = "";
		foreach (T e in array) {
			s += e.ToString () + " ";
		}
		return s;
	}
	private bool played = false;
	private DateTime start;

	public void playersTurn(Player player) {
		if (played == true)
			return;
		Debug.Log ("Players turn: " + player);
		Debug.Log ("Players rack: " + GameTests.stringify(player.tiles.ToArray()));

		start = DateTime.Now;
		game.solve(player);
	}
	public void predictionsDetermined(Player player, Coordinate coordinate, List<PredictionResult> predictions) {

	}
	private List<PredictionResult> predictions = null;
	public void solutionDetermined(Player player, List<PredictionResult> predictions) {
		this.predictions = predictions;
		if (predictions.Count > 0) {
			AbstractPlayerMove best = predictions[0];
			played = true;
			DateTime end = DateTime.Now;
			double ms = (end - start).TotalMilliseconds;
			Debug.Log ("Solution took " + ms + " ms.");
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
//	public void boardUpdated(ScrabbleBoard board) {
//		Debug.Log("Board");
//		Debug.Log(board);
//		
//		for (int i = 0; i < players.Length; i++) {
//			Debug.Log("Player " + (i+1) + "'s rack contains: " + GameTests.stringify(players[i].tiles.ToArray()));
//		}
//	}
}
