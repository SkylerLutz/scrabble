using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public Transform view;

	public Game game;

	// GameObjects
	public GameObject rack;
	public GameObject board;

	// Prefabs
	public TilePrefab tilePrefab;

	// Internal/Intermediate state
	private TilePrefab[,] boardRepresentation; // 2d array of tile views
	private TilePrefab[,] moveContext; // 2d array of tile views placed during this move
	private TilePrefab[,] solution; // 2d array of tile views for a potential move
	private TilePrefab[] rackRepresentation; // array of tile views on the player rack

	// Animation
	Dictionary<Object, Transform> animations;

	// Configuration
	public RackConfiguration rackConfig;
	public BoardConfiguration boardConfig;

	// Factories
	public TilePrefab produceTile(char letter, int pointValue, Vector3 position, Quaternion rotation) {

		TilePrefab tile = (TilePrefab)Instantiate(tilePrefab, position, rotation, view);
		// TODO: set letter and value on the prefab's label.
		return tile;
	}


	// Lifecycle methods
	void Start() {

		init();
	}
	void init() {
		// TODO: setup initial state

		animations = new Dictionary<Object, Transform> ();

		rackRepresentation = new TilePrefab[rackConfig.maxTiles];
		for (int i = 0; i < rackConfig.maxTiles; i++) {
			rackRepresentation [i] = null;
		}

		boardRepresentation = new TilePrefab[boardConfig.dimension,boardConfig.dimension];
		moveContext = new TilePrefab[boardConfig.dimension,boardConfig.dimension];
		solution = new TilePrefab[boardConfig.dimension,boardConfig.dimension];

		for (int i = 0; i < boardConfig.dimension; i++) {
			for (int j = 0; j < boardConfig.dimension; j++) {
				boardRepresentation [i, j] = null;
				moveContext [i, j] = null;
				solution [i, j] = null;
			}
		}
	}

	private int addTile(Tile tile) {
		// TODO: make the tiles come from a sensible spot

		TilePrefab prefab = produceTile('B', 4, Vector3.up, rack.transform.rotation);
		//Text letterLabel = tileObject.transform.FindChild("TextCanvas").gameObject.transform.FindChild("Letter").gameObject.GetComponent<Text>();
		//Text valueLabel = tileObject.transform.FindChild("TextCanvas").gameObject.transform.FindChild("Value").gameObject.GetComponent<Text>();

		// TODO: Pull char and value off Tile, update labels
		//		letterLabel.text = "A";
		//		valueLabel.text = "1";
		int index = insertTileToRack (prefab);
		if (index == -1) {
			return -1;
		} else {
			setPositionForTileAtIndex (prefab, index);
		}
		return index;
	}
	// return the rack index the tile was inserted into, or negative -1 if it could not be inserted.
	private int insertTileToRack(TilePrefab tile) {
		for (int i = 0; i < rackRepresentation.Length; i++) {
			if (rackRepresentation [i] == null) {
				rackRepresentation [i] = tile;
				return i;
			}
		}
		return -1; 
	}
	private void setPositionForTileAtIndex(TilePrefab tile, int index) {

		// Calculate x position of new tile on the rack
		float width = rack.GetComponent<Collider> ().bounds.size.x;
		float origin = rack.transform.position.x + width / 2.0f;

		// TODO: add half the width of a tile?
		float tilePositionX = -index * (width / rackConfig.maxTiles) + origin; 
		Vector3 tileV3 = new Vector3(tilePositionX, rack.transform.position.y, rack.transform.position.z);


		// Rotation of new tile is the same as the Rack's rotation

		//		Vector3 right = new Vector3 (0, 0, Mathf.Lerp(75.0f, -75.0f, (float)(index + 1) / (float)config.maxTiles));
		//		Debug.Log (right);
		//		Vector3 tileOrigin = new Vector3 (tileV3.x, tileV3.y, tileV3.z - 1);
		//		tileObject.destination = tileV3;
		//		tileObject.orientation = right;

		Transform t = tile.transform;
		this.animations.Add (tile, t);
	}


	void Update() {
		Dictionary<Object, Transform> newAnimations = new Dictionary<Object, Transform> ();
		foreach(KeyValuePair<Object, Transform> entry in this.animations) {
			// do something with entry.Value or entry.Key

			if (entry.Key is TilePrefab) {
				TilePrefab tile = entry.Key as TilePrefab;
				tile.transform.position = Vector3.MoveTowards (tile.transform.position, entry.Value.position, 20.0f * Time.deltaTime);  
				tile.transform.rotation = Quaternion.RotateTowards (tile.transform.rotation, entry.Value.rotation, 200.0f * Time.deltaTime);

				// stop tracking any objects that no longer need to be animated.
				if (tile.transform != entry.Value) {
					newAnimations.Add (tile, entry.Value);
				}
			}
		}
		this.animations = newAnimations;
	}
	void LateUpdate() {

	}

	void OnMouseDown() {

	}
	void OnMouseDrag() {

	}
	void OnMouseUp() {

	}
}


// The game model's representation of a game.
public class Game {
	public Game() {

	}
}
// The game model's representation of a tile.
public class Tile {

	// char letter
	public Tile() {

	}
}