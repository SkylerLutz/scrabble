using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameController : MonoBehaviour, TileDelegate {

	public Transform view;

	public Game game;

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

		GameTests tests = new GameTests();
		tests.gameTest();

//		PredictionTests.standardTest ();

		init();
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
		Text letterLabel = go.transform.FindChild("TextCanvas").gameObject.transform.FindChild("Letter").gameObject.GetComponent<Text>();

		playerMoveContextTiles.Add(new Tile(TileType.LETTER, letterLabel.text.ToCharArray()[0]));
		playerMoveContextCoordinates.Add (new Coordinate (i, j));
	}

	private void commitMove() {
		PlayerMove move = new PlayerMove (playerMoveContextTiles, playerMoveContextCoordinates);

		Player mock = new Player ();
		game.play (mock, move);

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
				Vector3 boardOrigin = new Vector3 (board.transform.position.x - width / 2.0f, 0, board.transform.position.z - width / 2.0f);

				float spaceWidth = width / dimension;
				GameObject surface = board.transform.FindChild ("Surface").gameObject;

				Vector3 centerOffset = new Vector3 ((spaceWidth / 2.0f), 0, (spaceWidth / 2.0f));
				Vector3 oneOver = new Vector3 (coordinate.x * spaceWidth,0,coordinate.y * spaceWidth);
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

		if (Input.GetMouseButtonDown (0)) {

			Tile t = new Tile (TileType.LETTER, 'A');
			addTile (t);
		}
		if (Input.GetMouseButtonDown (1)) {

			// TODO: save button
//			commitMove();
			setupInitialState();
		}
	}

	private void setupInitialState() {
		Tile t1 = new Tile (TileType.LETTER, 'H');
		Tile t2 = new Tile (TileType.LETTER, 'E');
		Tile t3 = new Tile (TileType.LETTER, 'L');
		Tile t4 = new Tile (TileType.LETTER, 'L');
		Tile t5 = new Tile (TileType.LETTER, 'O');

		Coordinate c1 = new Coordinate (2, 4);
		Coordinate c2 = new Coordinate (3, 4);
		Coordinate c3 = new Coordinate (4, 4);
		Coordinate c4 = new Coordinate (5, 4);
		Coordinate c5 = new Coordinate (6, 4);

		fixedTiles.Add (t1);
		fixedTiles.Add (t2);
		fixedTiles.Add (t3);
		fixedTiles.Add (t4);
		fixedTiles.Add (t5);

		fixedPositions.Add (c1);
		fixedPositions.Add (c2);
		fixedPositions.Add (c3);
		fixedPositions.Add (c4);
		fixedPositions.Add (c5);

	}
}
