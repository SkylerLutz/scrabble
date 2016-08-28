using System.Collections;
using System.Collections.Generic;
using System;


public sealed class PlayerMoveBroker {

	private ScrabblePlayerMoveScoring scoring;
	private ScrabbleBoard board;
	private Bag bag;
	private PlayerConfiguration config;
	public PlayerMoveBroker(ScrabblePlayerMoveScoring scoring, ScrabbleBoard board, PlayerConfiguration config, Bag bag) {
		this.scoring = scoring;
		this.board = board;
		this.config = config;
		this.bag = bag;
	}

	public int brokerMove(Player player, AbstractPlayerMove move) {
		ScrabbleScoringDirection direction = Raycasting.getDirection(move.coordinates);

		if (move.tiles.Count != move.coordinates.Count) {
			return -1;
		}
		
		if (move is PredictionResult) {
			commit(player, move);
			return ((PredictionResult)move).score;
		}

		Tile[,] predictedTiles = new Tile[board.dimension,board.dimension];
		for (int i = 0; i < move.tiles.Count; i++) {
			Tile tile = move.tiles[i];
			Coordinate coordinate = move.coordinates[i];
			predictedTiles[coordinate.x,coordinate.y] = tile;
		}
		PredictedContext context = new PredictedContext(predictedTiles);
		PredictedBoard ephemeral = new PredictedBoard(board, context);
	
		int score = scoring.score(ephemeral, move.coordinates, direction);

		if (score == -1) {
			return -1;
		}

		commit(player, move);
		
		return score;
	}
	private void commit(Player player, AbstractPlayerMove move) {

		for (int i = 0; i < move.tiles.Count; i++) {
			Tile tile = move.tiles[i];
			Coordinate coordinate = move.coordinates[i];
			board.setTile(tile, coordinate.x, coordinate.y);
		}

		use(player, move.tiles);
		
	}
	private void use(Player player, List<Tile> playedTiles) {
		foreach (Tile play in playedTiles) {
			bool b = player.tiles.Remove(play);
			if (!b) {
				// TODO: Throw a proper exception -- the player didnt have this tile.
				Environment.Exit(1);
//				System.out.println("Player doesn't have that tile");
//				System.exit(1);
			}
		}
		replenish(player);
	}
	public void replenish(Player player) {
		for (int i = player.tiles.Count; i < config.tileCount; i++) {
			Tile draw = bag.draw();
			if (draw == null) {
				break;
			}
			player.tiles.Add(draw);
		}
	}
}
