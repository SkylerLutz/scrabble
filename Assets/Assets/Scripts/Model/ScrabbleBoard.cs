using UnityEngine;

public enum ScrabbleBoardConfiguration {
	MICRO = 3,
	MINI = 5,
	STANDARD = 7,
	LARGE = 9
}

public class ScrabbleBoard {

	protected Tile[,] tiles;
	protected TileSpace[,] spaces;
	public int dimension;
	public ScrabbleBoard(ScrabbleBoard existing) {
		this.tiles = existing.tiles;
		this.spaces = existing.spaces;
		this.dimension = existing.dimension;
	}
	public ScrabbleBoard(ScrabbleBoardConfiguration config) {
		switch(config) {
		case ScrabbleBoardConfiguration.MICRO: {
				dimension = (int)config;
				this.tiles = new Tile[dimension, dimension];
				this.spaces = new TileSpace[dimension, dimension];
				setupMicroBoard();
				break;
			}
		case ScrabbleBoardConfiguration.MINI: {
				dimension = (int)config;
				this.tiles = new Tile[dimension,dimension];
				this.spaces = new TileSpace[dimension,dimension];
				setupMiniBoard();
				break;
			}
		case ScrabbleBoardConfiguration.STANDARD: {
				dimension = (int)config;
				this.tiles = new Tile[dimension,dimension];
				this.spaces = new TileSpace[dimension,dimension];
				setupStandardBoard();
				break;
			}
		case ScrabbleBoardConfiguration.LARGE: {
				dimension = (int)config;
				this.tiles = new Tile[dimension,dimension];
				this.spaces = new TileSpace[dimension,dimension];
				setupLargeBoard();
				break;
			}
		default:
			dimension = 0; 
			break;
		}

	}

	private void setupMicroBoard() {

		this.spaces [0, 0] = new TileSpace (TileSpaceType.NORMAL);
		this.spaces [0, 1] = new TileSpace (TileSpaceType.NORMAL); 
		this.spaces [0, 2] = new TileSpace (TileSpaceType.NORMAL);
		this.spaces [1, 0] = new TileSpace (TileSpaceType.NORMAL); 
		this.spaces [1, 1] = new TileSpace (TileSpaceType.NORMAL);
		this.spaces [1, 2] = new TileSpace (TileSpaceType.NORMAL);
		this.spaces [2, 0] = new TileSpace (TileSpaceType.NORMAL);
		this.spaces [2, 1] = new TileSpace (TileSpaceType.NORMAL); 
		this.spaces [2, 2] = new TileSpace (TileSpaceType.NORMAL);
	}
	private void setupMiniBoard() {
		this.spaces [0, 0] = new TileSpace(TileSpaceType.TRIPLE_WORD_SCORE);
		this.spaces [0, 1] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [0, 2] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [0, 3] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [0, 4] = new TileSpace(TileSpaceType.TRIPLE_WORD_SCORE);
		this.spaces [1, 0] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [1, 1] = new TileSpace(TileSpaceType.DOUBLE_WORD_SCORE);
		this.spaces [1, 2] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [1, 3] = new TileSpace(TileSpaceType.DOUBLE_WORD_SCORE);
		this.spaces [1, 4] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [2, 0] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [2, 1] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [2, 2] = new TileSpace(TileSpaceType.CENTER);
		this.spaces [2, 3] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [2, 4] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [3, 0] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [3, 1] = new TileSpace(TileSpaceType.DOUBLE_WORD_SCORE);
		this.spaces [3, 2] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [3, 3] = new TileSpace(TileSpaceType.DOUBLE_WORD_SCORE);
		this.spaces [3, 4] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [4, 0] = new TileSpace(TileSpaceType.TRIPLE_WORD_SCORE);
		this.spaces [4, 1] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [4, 2] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [4, 3] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [4, 4] = new TileSpace(TileSpaceType.TRIPLE_WORD_SCORE);

	}
	private void setupStandardBoard() {

		this.spaces [0, 0] = new TileSpace (TileSpaceType.TRIPLE_WORD_SCORE);
		this.spaces [0, 1] = new TileSpace (TileSpaceType.NORMAL);
		this.spaces [0, 2] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [0, 3] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [0, 4] = new TileSpace (TileSpaceType.NORMAL); // 4
		this.spaces [0, 5] = new TileSpace(TileSpaceType.NORMAL); // 5
		this.spaces [0, 6] = new TileSpace(TileSpaceType.TRIPLE_WORD_SCORE);// 6

		this.spaces [1, 0] = new TileSpace (TileSpaceType.NORMAL); // 0
		this.spaces [1, 1] = new TileSpace (TileSpaceType.DOUBLE_WORD_SCORE); // 1
		this.spaces [1, 2] = new TileSpace(TileSpaceType.NORMAL); // 2
		this.spaces [1, 3] = new TileSpace(TileSpaceType.NORMAL); // 3
		this.spaces [1, 4] = new TileSpace (TileSpaceType.NORMAL); // 4
		this.spaces [1, 5] = new TileSpace(TileSpaceType.DOUBLE_WORD_SCORE); // 5
		this.spaces [1, 6] = new TileSpace(TileSpaceType.NORMAL);// 6


		this.spaces [2, 0] = new TileSpace (TileSpaceType.NORMAL); // 0
		this.spaces [2, 1] = new TileSpace (TileSpaceType.NORMAL); // 1
		this.spaces [2, 2] = new TileSpace(TileSpaceType.DOUBLE_WORD_SCORE); // 2
		this.spaces [2, 3] = new TileSpace(TileSpaceType.NORMAL); // 3
		this.spaces [2, 4] = new TileSpace (TileSpaceType.DOUBLE_WORD_SCORE); // 4
		this.spaces [2, 5] = new TileSpace(TileSpaceType.NORMAL); // 5
		this.spaces [2, 6] = new TileSpace(TileSpaceType.NORMAL);// 6

		this.spaces [3, 0] = new TileSpace (TileSpaceType.NORMAL); // 0
		this.spaces [3, 1] = new TileSpace (TileSpaceType.NORMAL); // 1
		this.spaces [3, 2] = new TileSpace(TileSpaceType.NORMAL); // 2
		this.spaces [3, 3] = new TileSpace(TileSpaceType.CENTER); // 3
		this.spaces [3, 4] = new TileSpace (TileSpaceType.NORMAL); // 4
		this.spaces [3, 5] = new TileSpace(TileSpaceType.NORMAL); // 5
		this.spaces [3, 6] = new TileSpace(TileSpaceType.NORMAL);// 6

		this.spaces [4, 0] = new TileSpace (TileSpaceType.NORMAL); // 0
		this.spaces [4, 1] = new TileSpace (TileSpaceType.NORMAL); // 1
		this.spaces [4, 2] = new TileSpace(TileSpaceType.DOUBLE_WORD_SCORE); // 2
		this.spaces [4, 3] = new TileSpace(TileSpaceType.NORMAL); // 3
		this.spaces [4, 4] = new TileSpace (TileSpaceType.DOUBLE_WORD_SCORE); // 4
		this.spaces [4, 5] = new TileSpace(TileSpaceType.NORMAL); // 5
		this.spaces [4, 6] = new TileSpace(TileSpaceType.NORMAL);// 6

		this.spaces [5, 0] = new TileSpace (TileSpaceType.NORMAL); // 0
		this.spaces [5, 1] = new TileSpace (TileSpaceType.DOUBLE_WORD_SCORE); // 1
		this.spaces [5, 2] = new TileSpace(TileSpaceType.NORMAL); // 2
		this.spaces [5, 3] = new TileSpace(TileSpaceType.NORMAL); // 3
		this.spaces [5, 4] = new TileSpace (TileSpaceType.NORMAL); // 4
		this.spaces [5, 5] = new TileSpace(TileSpaceType.DOUBLE_WORD_SCORE); // 5
		this.spaces [5, 6] = new TileSpace(TileSpaceType.NORMAL);// 6


		this.spaces [6, 0] = new TileSpace (TileSpaceType.TRIPLE_WORD_SCORE);
		this.spaces [6, 1] = new TileSpace (TileSpaceType.NORMAL);
		this.spaces [6, 2] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [6, 3] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [6, 4] = new TileSpace (TileSpaceType.NORMAL); // 4
		this.spaces [6, 5] = new TileSpace(TileSpaceType.NORMAL); // 5
		this.spaces [6, 6] = new TileSpace(TileSpaceType.TRIPLE_WORD_SCORE);// 6

	}
	private void setupLargeBoard() {
		this.spaces [0, 0] = new TileSpace (TileSpaceType.TRIPLE_WORD_SCORE);
		this.spaces [0, 1] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [0, 2] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [0, 3] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [0, 4] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [0, 5] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [0, 6] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [0, 7] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [0, 8] = new TileSpace(TileSpaceType.TRIPLE_WORD_SCORE);

		this.spaces [1, 0] = new TileSpace (TileSpaceType.NORMAL);
		this.spaces [1, 1] = new TileSpace(TileSpaceType.DOUBLE_WORD_SCORE);
		this.spaces [1, 2] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [1, 3] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [1, 4] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [1, 5] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [1, 6] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [1, 7] = new TileSpace(TileSpaceType.DOUBLE_WORD_SCORE);
		this.spaces [1, 8] = new TileSpace(TileSpaceType.NORMAL);

		this.spaces [2, 0] = new TileSpace (TileSpaceType.NORMAL);
		this.spaces [2, 1] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [2, 2] = new TileSpace(TileSpaceType.DOUBLE_WORD_SCORE);
		this.spaces [2, 3] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [2, 4] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [2, 5] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [2, 6] = new TileSpace(TileSpaceType.DOUBLE_WORD_SCORE);
		this.spaces [2, 7] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [2, 8] = new TileSpace(TileSpaceType.NORMAL);

		this.spaces [3, 0] = new TileSpace (TileSpaceType.NORMAL);
		this.spaces [3, 1] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [3, 2] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [3, 3] = new TileSpace(TileSpaceType.DOUBLE_WORD_SCORE);
		this.spaces [3, 4] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [3, 5] = new TileSpace(TileSpaceType.DOUBLE_WORD_SCORE);
		this.spaces [3, 6] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [3, 7] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [3, 8] = new TileSpace(TileSpaceType.NORMAL);

		this.spaces [4, 0] = new TileSpace (TileSpaceType.NORMAL);
		this.spaces [4, 1] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [4, 2] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [4, 3] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [4, 4] = new TileSpace(TileSpaceType.CENTER);
		this.spaces [4, 5] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [4, 6] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [4, 7] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [4, 8] = new TileSpace(TileSpaceType.NORMAL);

		this.spaces [5, 0] = new TileSpace (TileSpaceType.NORMAL);
		this.spaces [5, 1] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [5, 2] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [5, 3] = new TileSpace(TileSpaceType.DOUBLE_WORD_SCORE);
		this.spaces [5, 4] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [5, 5] = new TileSpace(TileSpaceType.DOUBLE_WORD_SCORE);
		this.spaces [5, 6] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [5, 7] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [5, 8] = new TileSpace(TileSpaceType.NORMAL);

		this.spaces [6, 0] = new TileSpace (TileSpaceType.NORMAL);
		this.spaces [6, 1] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [6, 2] = new TileSpace(TileSpaceType.DOUBLE_WORD_SCORE);
		this.spaces [6, 3] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [6, 4] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [6, 5] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [6, 6] = new TileSpace(TileSpaceType.DOUBLE_WORD_SCORE);
		this.spaces [6, 7] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [6, 8] = new TileSpace(TileSpaceType.NORMAL);


	

		this.spaces [7, 0] = new TileSpace (TileSpaceType.NORMAL);
		this.spaces [7, 1] = new TileSpace(TileSpaceType.DOUBLE_WORD_SCORE);
		this.spaces [7, 2] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [7, 3] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [7, 4] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [7, 5] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [7, 6] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [7, 7] = new TileSpace(TileSpaceType.DOUBLE_WORD_SCORE);
		this.spaces [7, 8] = new TileSpace(TileSpaceType.NORMAL);

		this.spaces [8, 0] = new TileSpace (TileSpaceType.TRIPLE_WORD_SCORE);
		this.spaces [8, 1] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [8, 2] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [8, 3] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [8, 4] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [8, 5] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [8, 6] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [8, 7] = new TileSpace(TileSpaceType.NORMAL);
		this.spaces [8, 8] = new TileSpace(TileSpaceType.TRIPLE_WORD_SCORE);

	}
	public TileSpace getSpace(int i, int j) {
		if ((i < 0 || i >= dimension) || (j < 0 || j >= dimension)) {
			return null;
		}
		return spaces[i,j];
	}
	public virtual Tile getTile(int i, int j) {
//		Debug.Log ("getTile() base");
		if ((i < 0 || i >= dimension) || (j < 0 || j >= dimension)) {
			return null;
		}
		return tiles[i,j];
	}
	public bool setTile(Tile tile, int i, int j) {
		if ((i < 0 || i >= dimension) || (j < 0 || j >= dimension)) {
			return false;
		}
		// TODO: Demote tile type underneath to prevent getting a tile bonus twice
		tiles[i,j] = tile;
		return true;
	}

	public override string ToString() {
		string d = "";
		for (int j = 0; j < dimension; j++) {
			d = d + "|";
			for (int i = 0; i < dimension; i++) {
				Tile tile = getTile(i, j);
				TileSpace space = getSpace(i, j);
				if (tile != null) {
					char letter = tile.getLetter();
					//					if (letter != null) {
					d += letter + "|";
					//					}
					//					else {
					//						d += "?|";
					//					}
				}
				else {
					switch (space.type) {
					case TileSpaceType.NORMAL: d += "_|"; break;
					default: d += "*|"; break;
					}
				}
			}
			d += "\n";
		}
		return d;
	}
}

public sealed class PredictedContext {

	public Tile[,] predictedTiles;
	public PredictedContext(Tile[,] predictedTiles) {
		this.predictedTiles = predictedTiles;
	}

}
public class PredictedBoard: ScrabbleBoard {

	private PredictedContext context;

	public PredictedBoard(ScrabbleBoard existing, PredictedContext context) : base(existing) {

		this.context = context;
	}

	// override
	public override Tile getTile(int i, int j) {
//		Debug.Log ("getTile() derived");
		if ((i < 0 || i >= dimension) || (j < 0 || j >= dimension)) {
			return null;
		}
		Tile fixedTile = tiles[i,j];
		if (fixedTile != null) {
			return fixedTile;
		}
		else {
			return context.predictedTiles[i,j];
		}
	}

	public override string ToString() {
		string d = "";
		for (int j = 0; j < dimension; j++) {
			d = d + "|";
			for (int i = 0; i < dimension; i++) {
				Tile tile = getTile(i, j);
				TileSpace space = getSpace(i, j);
				if (tile != null) {
					char letter = tile.getLetter();
					//					if (letter != null) {
					d += letter + "|";
					//					}
					//					else {
					//						d += "?|";
					//					}
				}
				else {
					switch (space.type) {
					case TileSpaceType.NORMAL: d += "_|"; break;
					default: d += "*|"; break;
					}
				}
			}
			d += "\n";
		}
		return d;
	}
}

