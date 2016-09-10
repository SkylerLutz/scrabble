﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameController : MonoBehaviour, TileDelegate, GameDelegate {

	// Model Objects
	public Game game;
	public Player[] players;
	public Player active;

	// View Object
	public Transform view;

	// GameObjects
	public GameObject rack;
	public GameObject board;

	// Prefabs
	public GameObject tilePrefab;

	// Internal/Intermediate state
	private GameObject[,] boardRepresentation; // 2d array of tile views
	private GameObject[,] moveContext; // 2d array of tile views placed during this move
	private GameObject[,] solution; // 2d array of tile views for a potential move
	private GameObject[] rackRepresentation; // array of tile views on the player rack

	private List<Tile> fixedTiles;
	private List<Coordinate> fixedPositions;
	private List<Tile> playerMoveContextTiles;
	private List<Coordinate> playerMoveContextCoordinates;

	// Configuration
	public RackConfiguration rackConfig;
	public BoardConfiguration boardConfig;

	// Factories
	public GameObject produceTile(char letter, int pointValue, Vector3 position, Quaternion rotation) {

		GameObject tile = (GameObject)Instantiate(tilePrefab, position, rotation, view);
		Text letterLabel = tile.transform.FindChild("TextCanvas").gameObject.transform.FindChild("Letter").gameObject.GetComponent<Text>();
		Text valueLabel = tile.transform.FindChild("TextCanvas").gameObject.transform.FindChild("Value").gameObject.GetComponent<Text>();

		letterLabel.text = "" + letter;
		valueLabel.text = pointValue.ToString();
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
		solution = new GameObject[boardConfig.dimension,boardConfig.dimension];

		for (int i = 0; i < boardConfig.dimension; i++) {
			for (int j = 0; j < boardConfig.dimension; j++) {
				boardRepresentation [i, j] = null;
				moveContext [i, j] = null;
				solution [i, j] = null;
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
		GameObject instance = produceTile(tile.getLetter(), 4, Vector3.up, rack.transform.rotation);

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
		p.destinationRotation = tile.transform.rotation;
	}

	// Tile Delegate Calls
	public bool canPlaceTileAt (int i, int j) {
		// check out of bounds
		if (i < 0 || i >= boardConfig.dimension || j < 0 || j >= boardConfig.dimension) {
			return false;
		}

		// search fixed tiles at i, j
		if (fixedPositions.Contains(new Coordinate(i, j))) {
			return false;
		}

		// search for user placed tiles at i, j
		if (playerMoveContextCoordinates.Contains (new Coordinate (i, j))) {
			return false;
		}

		return true;
	}
	public void placeTileAt (GameObject go, int i, int j) {

		// remove tile from rack
		for (int n = 0; n < rackRepresentation.Length; n++) {
			if (go == rackRepresentation[n]) {
				Debug.Log("removed g.o. from rack");
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
				go = produceTile (tile.getLetter (), 4, Vector3.up, Quaternion.Euler (Vector3.up));
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

//		if (Input.GetMouseButtonDown (0)) {
//
//			Tile t = new Tile (TileType.LETTER, 'A');
//			addTile (t);
//		}
		if (Input.GetMouseButtonDown (1)) {

			// TODO: save button
//			commitMove();
			Debug.Log("Solving...");
			game.solve(active);
		}
	}

	private void setupInitialState() {

		int numTiles = rackConfig.maxTiles;
		int numResults = 3;
		ScrabbleBoardConfiguration size = ScrabbleBoardConfiguration.STANDARD;

		Player p1 = new Player();
		Player p2 = new Player();
		Player[] ps = { p1, p2 };
		this.players = ps;

		ScrabbleBoard board = new ScrabbleBoard(size);
		// previously placed tiles
		board.setTile(new Tile(TileType.LETTER, 'H'), 3, 2);
		board.setTile(new Tile(TileType.LETTER, 'E'), 3, 3);
		board.setTile(new Tile(TileType.LETTER, 'L'), 3, 4);
		board.setTile(new Tile(TileType.LETTER, 'L'), 3, 5);
		board.setTile(new Tile(TileType.LETTER, 'O'), 3, 6);

		this.game = new Game(new ScrabbleGameConfiguration(numResults, new PlayerConfiguration(numTiles)), board, players, this);
		this.game.start();

	}

	private void syncViewBoardToModelBoard() {


		// diff the model board with the view board and insert game objects for each change.
		for (int i = 0; i < boardConfig.dimension; i++) {
			for (int j = 0; j < boardConfig.dimension; j++) {

				GameObject go = moveContext [i, j];
				if (go != null) {
					Debug.Log ("found game object at (" + i + ", " + j + ")");
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
//		game.solve(player);
	}
	public void predictionsDetermined(Player player, Coordinate coordinate, List<PredictionResult> predictions) {

	}
	public void solutionDetermined(Player player, List<PredictionResult> predictions) {
		Debug.Log("Board solved. " + predictions.Count + " possible moves were found.");
//		if (predictions.Count > 0) {
//			AbstractPlayerMove best = predictions[0];
//			game.play(player, best);
//		}
//		else {
//			Debug.Log("Player: " + player + " has no options to move.");
//		}
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
