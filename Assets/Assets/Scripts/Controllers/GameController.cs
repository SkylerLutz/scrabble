using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameController : MonoBehaviour, TileDelegate, GameDelegate {

	// Model Objects
	public Game game;
	public Player[] players;
	public Player active;
	public List<PredictionResult> activePredictions;

	// View Object
	public Transform view;

	// GameObjects
	public GameObject rack;
	public GameObject board;

	public GameObject prediction1;
	public GameObject prediction2;
	public GameObject prediction3;

	// Prefabs
	public GameObject tilePrefab;
	public GameObject spacePrefab;

	// Internal/Intermediate state
	private GameObject[,] boardRepresentation; // 2d array of tile views
	private GameObject[,] moveContext; // 2d array of tile views placed during this move
	private GameObject[] solution; // array of tile views for a potential move
	private GameObject[] rackRepresentation; // array of tile views on the player rack

	private List<Tile> fixedTiles;
	private List<Coordinate> fixedPositions;
	private List<Tile> playerMoveContextTiles;
	private List<Coordinate> playerMoveContextCoordinates;

	// Configuration
	public RackConfiguration rackConfig;
	public BoardConfiguration boardConfig;

	// Factories
	public GameObject produceSpace(TileSpaceType type, Vector3 position) {

		GameObject space = (GameObject)Instantiate(spacePrefab, position, Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f)), view);

		TextMesh label = space.transform.FindChild("Label").gameObject.GetComponent<TextMesh>();
		GameObject plane = space.transform.FindChild ("Plane").gameObject;
		string text = null;
		Color c = new Color ();
		c.a = 1.0f;

		switch (type) {
		case TileSpaceType.CENTER:
			text = "";
			c.r = 235.0f / 255.0f;
			c.g = 152.0f / 255.0f;
			c.b = 163.0f / 255.0f;
			break;
		case TileSpaceType.DOUBLE_LETTER_SCORE:
			text = "DOUBLE\nLETTER\nSCORE";
			c.r = 186.0f / 255.0f;
			c.g = 220.0f / 255.0f;
			c.b = 238.0f / 255.0f;
			break;
		case TileSpaceType.DOUBLE_WORD_SCORE:
			text = "DOUBLE\nWORD\nSCORE";
			c.r = 235.0f / 255.0f;
			c.g = 152.0f / 255.0f;
			c.b = 163.0f / 255.0f;
			break;
		case TileSpaceType.NORMAL:
			text = "";
			c.r = 170.0f / 255.0f;
			c.g = 172.0f / 255.0f;
			c.b = 136.0f / 255.0f;
			break;

		case TileSpaceType.TRIPLE_LETTER_SCORE:
			text = "TRIPLE\nLETTER\nSCORE";
			c.r = 89.0f / 255.0f;
			c.g = 160.0f / 255.0f;
			c.b = 205.0f / 255.0f;
			break;

		case TileSpaceType.TRIPLE_WORD_SCORE:
			text = "TRIPLE\nWORD\nSCORE";
			c.r = 226.0f / 255.0f;
			c.g = 42.0f / 255.0f;
			c.b = 62.0f / 255.0f;
			break;
		}

		label.text = text;
		plane.GetComponent<MeshRenderer> ().material.color = c;
//		label.color = c;

		return space;
	}

	public GameObject produceTile(char letter, int pointValue, Vector3 position, Quaternion rotation) {

		GameObject tile = (GameObject)Instantiate(tilePrefab, position, rotation, view);
		//		TilePrefab pf = tile.GetComponent<TilePrefab> ();
		//		pf.destinationPosition = position;
		//		pf.destinationRotation = rotation;

		Text letterLabel = tile.transform.FindChild("TextCanvas").gameObject.transform.FindChild("Letter").gameObject.GetComponent<Text>();
		Text valueLabel = tile.transform.FindChild("TextCanvas").gameObject.transform.FindChild("Value").gameObject.GetComponent<Text>();

		letterLabel.text = "" + letter;
		valueLabel.text = pointValue.ToString();
		Color c = new Color ();
		c.a = 1.0f;
		c.r = 227.0f / 255.0f;
		c.g = 223.0f / 255.0f;
		c.b = 168.0f / 255.0f;
		letterLabel.color = c;
		valueLabel.color = c;
		return tile;
	}

	// Lifecycle methods
	void Start() {

//		GameTests tests = new GameTests();
//		tests.gameTest();
//		PredictionTests.standardTest ();

		init();

		setupInitialState();
	}
	void init() {
		// TODO: setup initial state

		rackRepresentation = new GameObject[rackConfig.maxTiles];
		for (int i = 0; i < rackConfig.maxTiles; i++) {
			rackRepresentation [i] = null;
		}

		boardRepresentation = new GameObject[boardConfig.dimension,boardConfig.dimension];
		moveContext = new GameObject[boardConfig.dimension,boardConfig.dimension];
		solution = new GameObject[boardConfig.dimension];

		for (int i = 0; i < boardConfig.dimension; i++) {
			solution [i] = null;
		}

		for (int i = 0; i < boardConfig.dimension; i++) {
			for (int j = 0; j < boardConfig.dimension; j++) {
				boardRepresentation [i, j] = null;
				moveContext [i, j] = null;
			}
		}

		fixedTiles = new List<Tile> ();
		fixedPositions = new List<Coordinate> ();

		playerMoveContextTiles = new List<Tile> ();
		playerMoveContextCoordinates = new List<Coordinate> ();
	}

	private int addTile(Tile tile) {
		// TODO: make the tiles come from a sensible vector3 origin

		// TODO: Get tile point value
		GameObject instance = produceTile(tile.getLetter(), this.game.valueOf(tile), Vector3.up, rack.transform.rotation);

		int index = insertTileToRack (instance);
		if (index == -1) {
			Destroy (instance);
			return -1;
		} else {
			setPositionForTileAtIndex (instance, index);
		}
		return index;
	}
	// return the rack index the tile was inserted into, or negative -1 if it could not be inserted.
	private int insertTileToRack(GameObject tile) {
		for (int i = 0; i < rackRepresentation.Length; i++) {
			if (rackRepresentation [i] == null) {
				rackRepresentation [i] = tile;
				return i;
			}
		}
		return -1; 
	}
	private void setPositionForTileAtIndex(GameObject tile, int index) {

		// Calculate x position of new tile on the rack
		float width = rack.GetComponent<Collider> ().bounds.size.x;
		float origin = rack.transform.position.x + width / 2.0f;

		// TODO: add half the width of a tile?
		float tilePositionX = -index * (width / rackConfig.maxTiles) + origin; 
		Vector3 tileV3 = new Vector3(tilePositionX, rack.transform.position.y, rack.transform.position.z);

		// Rotation of new tile is the same as the Rack's rotation

		// TODO: Concavity (rotation)
		//		Vector3 right = new Vector3 (0, 0, Mathf.Lerp(75.0f, -75.0f, (float)(index + 1) / (float)config.maxTiles));
		//		Debug.Log (right);
		//		Vector3 tileOrigin = new Vector3 (tileV3.x, tileV3.y, tileV3.z - 1);
		//		tileObject.destination = tileV3;
		//		tileObject.orientation = right;

		// position
		TilePrefab p = tile.GetComponent<TilePrefab>();
		p.del = this;
		p.board = board;
		p.boardConfig = boardConfig;
		p.destinationPosition = tileV3;
//		p.destinationRotation = tile.transform.rotation;
//		Debug.Log ("ROTATION: " + tile.transform.rotation.eulerAngles);
		p.destinationRotation = Quaternion.Euler (new Vector3(90.0f, 0.0f, 0.0f));
	}

	// Tile Delegate Calls
	public bool canPlaceTileAt (int i, int j) {
		// check out of bounds
		if (i < 0 || i >= boardConfig.dimension || j < 0 || j >= boardConfig.dimension) {
			return false;
		}

		// search fixed tiles at i, j
		if (fixedPositions.Contains(new Coordinate(i, j))) {
			Debug.Log ("You can't place that tile here, there is a fixed tile.");
			return false;
		}

		// search for user placed tiles at i, j
		if (playerMoveContextCoordinates.Contains (new Coordinate (i, j))) {
			Debug.Log ("You can't place that tile here, you already put a tile here.");
			return false;
		}

		return true;
	}
	public void placeTileAt (GameObject go, int i, int j) {

		// remove tile from rack
		for (int n = 0; n < rackRepresentation.Length; n++) {
			if (go == rackRepresentation[n]) {
				rackRepresentation[n] = null;
			}
		}

		bool wasMove = false;
		int prev_x = -1;
		int prev_y = -1;
		// add to & adjust the move context
		for (int x = 0; x < moveContext.GetLength(0); x++) {

			for (int y = 0; y < moveContext.GetLength(0); y++) {
				if (moveContext [x, y] == go) {
					moveContext [x, y] = null;
					wasMove = true;
					prev_x = x;
					prev_y = y;
					goto Outer; // break nested loop
				}
			}
		}
		Outer:
		moveContext [i, j] = go;



		Text letterLabel = go.transform.FindChild("TextCanvas").gameObject.transform.FindChild("Letter").gameObject.GetComponent<Text>();

		Coordinate c = new Coordinate (i, j);
		char letter = letterLabel.text.ToCharArray () [0];
		Debug.Log ("Dropping tile " + letter + " at: " + c);

		if (wasMove && prev_x != -1 && prev_y != -1) {
			for (int k = 0; k < playerMoveContextCoordinates.Count; k++) {
				Coordinate temp = playerMoveContextCoordinates [k];
				if (temp.x == prev_x && temp.y == prev_y) {
					playerMoveContextCoordinates.RemoveAt (k);
					playerMoveContextTiles.RemoveAt(k);
				}
			}
		}

		playerMoveContextTiles.Add(new Tile(TileType.LETTER, letter));
		playerMoveContextCoordinates.Add (c);
	}

	private void commitMove() {
		PlayerMove move = new PlayerMove (playerMoveContextTiles.ToArray(), playerMoveContextCoordinates.ToArray());

		Debug.Log ("committing the move: " + move);

		int result = game.play (active, move);
		if (result == -1) {
			// TODO: handle error
			Debug.Log("invalid move");
		} else {
			playerMoveContextTiles = new List<Tile> ();
			playerMoveContextCoordinates = new List<Coordinate> ();
		}

		// TODO: Clear player move context stuff once the valid callback comes back from the game model.
	}

	void Update() {

		if ((fixedTiles == null || fixedPositions == null) || fixedTiles.Count != fixedPositions.Count) {
			return;
		}
		for (int i = 0; i < fixedTiles.Count; i++) {

			GameObject go = null;
			Tile tile = fixedTiles [i];
			Coordinate coordinate = fixedPositions [i];
			if (boardRepresentation [coordinate.x, coordinate.y] == null) {
				go = produceTile (tile.getLetter (), this.game.valueOf(tile), Vector3.up, Quaternion.Euler (Vector3.up));
				boardRepresentation [coordinate.x, coordinate.y] = go;

				// position

				int dimension = boardConfig.dimension;

				// Calculate x position of new tile on the rack
				float width = board.transform.FindChild ("BoardBox").gameObject.GetComponent<Collider> ().bounds.size.x;
				Vector3 boardOrigin = new Vector3 (board.transform.position.x + width / 2.0f, 0, board.transform.position.z - width / 2.0f);

				float spaceWidth = width / dimension;
				GameObject surface = board.transform.FindChild ("Surface").gameObject;

				Vector3 centerOffset = new Vector3 (-(spaceWidth / 2.0f), 0, (spaceWidth / 2.0f));
				Vector3 oneOver = new Vector3 (-coordinate.x * spaceWidth,0,coordinate.y * spaceWidth);
				TilePrefab p = go.GetComponent<TilePrefab>();
				p.isFixed = true;
				p.del = this;
				p.board = board;
				p.boardConfig = boardConfig;
				p.destinationPosition = boardOrigin + centerOffset + oneOver + surface.transform.position;
				p.destinationRotation = Quaternion.Euler (Vector3.up);

			}
		}
	}
	void LateUpdate() {
		
		if (Input.GetMouseButtonDown(0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast (ray, out hit)) {
				if (hit.transform.tag.Contains ("prediction")) {
					PredictionResult result = null;

					if (hit.transform.tag.Contains ("1") && activePredictions.Count > 0) {
						result = activePredictions [0];
					}
					else if (hit.transform.tag.Contains ("2") && activePredictions.Count > 1) {
						result = activePredictions [1];
					}
					else if (hit.transform.tag.Contains ("3") && activePredictions.Count > 2) {
						result = activePredictions [2];
					}

					if (result != null) { // place the tiles on the board
						foreach (GameObject ghost in solution) {
							Destroy (ghost);
						}
						//displayPrediction(result);
						placePrediction(result);
						commitMove ();
					}
				}
				else if (hit.transform.tag == "tile") {

					for (int i = 0; i < boardConfig.dimension; i++) {
						for (int j = 0; j < boardConfig.dimension; j++) {
							GameObject move = moveContext [i, j];
							if (move == null)
								continue;
							GameObject instance = hit.transform.gameObject;
							if (move.GetInstanceID () == instance.GetInstanceID ()) {
								Debug.Log ("found the clicked tile");

								moveContext [i, j] = null;
								int index = playerMoveContextCoordinates.IndexOf(new Coordinate(i,j));
								Debug.Log ("INDEX: " + index);
								playerMoveContextCoordinates.RemoveAt(index);
								playerMoveContextTiles.RemoveAt(index);

								int rackIndex = insertTileToRack (instance);
								if (rackIndex == -1) {
									Destroy (instance);
								} else {
									setPositionForTileAtIndex (instance, rackIndex);
								}

								goto Outer;
							}
						}
					}
					Outer: 
					Debug.Log ("put a tile back");
				}
				
			}

		}
		if (Input.GetMouseButtonDown (1)) {

			// TODO: save button
//			commitMove();
			Debug.Log("Solving...");
//			game.solve(active);
		}
	}

	private GameObject[] rackTilesFromPrediction(PredictionResult result) {
		GameObject[] rack = new GameObject[rackRepresentation.Length];
		bool[] dirty = new bool[rackRepresentation.Length];
		int index = 0;

		for (int i = 0; i < result.tiles.Length; i++) {

			Tile tile = result.tiles [i];

			for (int j = 0; j < rackRepresentation.Length; j++) {
				GameObject tileObject = rackRepresentation [j];

				GameObject canvas = tileObject.transform.FindChild ("TextCanvas").gameObject;
				GameObject letter = canvas.transform.FindChild ("Letter").gameObject;
				if ((dirty[j] == null || dirty[j] == false) && tile.getLetter ().ToString () == letter.GetComponent<Text> ().text) {
					rack [index] = tileObject;
					dirty [j] = true;
					index += 1;
					break;
				}
			}
		}
		return rack;
	}

	private void displayPrediction(PredictionResult result) {

		List<GameObject> ghosts = new List<GameObject> ();
		for (int i = 0; i < result.tiles.Length; i++) {
			Tile tile = result.tiles [i];

			float width = board.transform.FindChild ("BoardBox").gameObject.GetComponent<Collider> ().bounds.size.x;
			float spaceWidth = width / boardConfig.dimension;
			Vector3 boardOrigin = new Vector3 (board.transform.position.x + width / 2.0f, 0, board.transform.position.z - width / 2.0f);
			GameObject surface = board.transform.FindChild ("Surface").gameObject;

			Vector3 pos = new Vector3 (-result.coordinates [i].x * spaceWidth + boardOrigin.x - (spaceWidth / 2), surface.transform.position.y, result.coordinates [i].y * spaceWidth + boardOrigin.z + (spaceWidth / 2));
			Quaternion rot = Quaternion.Euler (Vector3.up);
			GameObject go = produceTile (tile.getLetter (), this.game.valueOf(tile), pos, rot);
			// TODO: make ghost translucent.
			// go.GetComponent<Renderer> ().material.color.a = 0.5f;
			TilePrefab pf = go.GetComponent<TilePrefab> ();
			pf.destinationPosition = pos;
			pf.destinationRotation = rot;
			ghosts.Add (go);
		}
		solution = ghosts.ToArray ();
	}

	private void placePrediction(PredictionResult result) {
		
		GameObject[] rack = rackTilesFromPrediction (result);

		for (int i = 0; i < rack.Length; i++) {
			GameObject tile = rack [i];
			if (tile == null)
				continue;
			placeTileAt (tile, result.coordinates [i].x, result.coordinates [i].y);
			float width = board.transform.FindChild ("BoardBox").gameObject.GetComponent<Collider> ().bounds.size.x;
			float spaceWidth = width / boardConfig.dimension;
			Vector3 boardOrigin = new Vector3 (board.transform.position.x + width / 2.0f, 0, board.transform.position.z - width / 2.0f);
			GameObject surface = board.transform.FindChild ("Surface").gameObject;
			tile.GetComponent<TilePrefab> ().destinationPosition = new Vector3 (-result.coordinates [i].x * spaceWidth + boardOrigin.x - (spaceWidth / 2), surface.transform.position.y, result.coordinates [i].y * spaceWidth + boardOrigin.z + (spaceWidth / 2));
			tile.GetComponent<TilePrefab> ().destinationRotation = Quaternion.Euler (Vector3.up);
		}
	}
	private void setupInitialState() {

		int numTiles = rackConfig.maxTiles;
		int numResults = 3;
		ScrabbleBoardConfiguration size = ScrabbleBoardConfiguration.STANDARD;

		Player p1 = new Player();
		Player p2 = new Player();
		Player[] ps = { p1 };
		this.players = ps;

		ScrabbleBoard board = new ScrabbleBoard(size);
		// previously placed tiles
		board.setTile(new Tile(TileType.LETTER, 'H'), 3, 2);
		board.setTile(new Tile(TileType.LETTER, 'E'), 3, 3);
		board.setTile(new Tile(TileType.LETTER, 'L'), 3, 4);
		board.setTile(new Tile(TileType.LETTER, 'L'), 3, 5);
		board.setTile(new Tile(TileType.LETTER, 'O'), 3, 6);


		for (int i = 0; i < board.dimension; i++) {
			for (int j = 0; j < board.dimension; j++) {
				int dimension = boardConfig.dimension;

				// Calculate x position of new tile on the rack
				float width = this.board.transform.FindChild ("BoardBox").gameObject.GetComponent<Collider> ().bounds.size.x;
				Vector3 boardOrigin = new Vector3 (this.board.transform.position.x + width / 2.0f, 0, this.board.transform.position.z - width / 2.0f);

				float spaceWidth = width / dimension;
				GameObject surface = this.board.transform.FindChild ("Surface").gameObject;

				Coordinate coordinate = new Coordinate (i, j);

				Vector3 centerOffset = new Vector3 (-(spaceWidth / 2.0f), 0, (spaceWidth / 2.0f));
				Vector3 oneOver = new Vector3 (-coordinate.x * spaceWidth,0,coordinate.y * spaceWidth);

				produceSpace (board.getSpace (i, j).type, boardOrigin + centerOffset + oneOver + surface.transform.position);
			}
		}


		this.game = new Game(new ScrabbleGameConfiguration(numResults, new PlayerConfiguration(numTiles)), board, players, this);
		this.game.start();

	}

	private void syncViewBoardToModelBoard() {


		// diff the model board with the view board and insert game objects for each change.
		for (int i = 0; i < boardConfig.dimension; i++) {
			for (int j = 0; j < boardConfig.dimension; j++) {

				GameObject go = moveContext [i, j];
				if (go != null) {
					moveContext [i, j] = null;
					boardRepresentation [i, j] = go;
				}


				Coordinate coordinate = new Coordinate(i, j);
				Tile tile = game.board.getTile (i, j);
				if (tile != null && !fixedPositions.Contains (coordinate)) {
					fixedPositions.Add (coordinate);
					fixedTiles.Add (tile);
				}
			}
		}
	}
	// Game Callbacks
	public void playerDrewTiles(Player player, Tile[] drawnTiles) {
		foreach(Tile tile in drawnTiles) {
			addTile (tile);
		}
	}
		
	public void playersTurn(Player player) {
		Debug.Log("It's player: " + player + "'s turn.");
		this.active = player;

		syncViewBoardToModelBoard ();

		playerMoveContextTiles = new List<Tile> ();
		playerMoveContextCoordinates = new List<Coordinate> ();

		//game.solve(player);
	}
	public void predictionsDetermined(Player player, Coordinate coordinate, List<PredictionResult> predictions) {

	}
	public void solutionDetermined(Player player, List<PredictionResult> predictions) {
		
		this.activePredictions = predictions;

		Debug.Log ("Board solved. " + predictions.Count + " possible moves were found.");

		if (predictions.Count > 0) {
			prediction1.GetComponent<TextMesh> ().text = predictions [0].ToString ();
			placePrediction(predictions[0]);
			commitMove ();
		}
		if (predictions.Count > 1) {
			prediction2.GetComponent<TextMesh> ().text = predictions [1].ToString ();
		}
		if (predictions.Count > 2) {
			prediction3.GetComponent<TextMesh> ().text = predictions [2].ToString ();
		}


	}
	public void playerScored(Player player, int score) { 
		Debug.Log ("Player " + player + " scored " + score + " points.");
		Debug.Log (game.board);
	}
	public void playerWon(Player player) {
		//		System.exit(0);
	}
	public void scoreboardUpdated(Scoreboard scoreboard) {
		// stub
//		Debug.Log(scoreboard);
	}
}
