using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public interface RackControllerDelegate {
	void placeTileOnBoard (TilePrefab tile);
}

public class RackController : MonoBehaviour {

	public RackControllerDelegate Del;

	public GameObject rack;
	private Tile[] tiles;
	private TilePrefab[] tileObjects;

	public RackConfiguration config;
	public TileFactory tileFactory;

	public Plane surface; // as generic as possible

	void Start() {
		
		initTiles ();

//		for (int i = 0; i < config.maxTiles; i++) {
//			Tile t = new Tile ();
//			addTile (t);
//		}
	}

	private void initTiles() {
		// internal representation of tile positions on the rack
		tiles = new Tile[config.maxTiles];

		// references to GameObjects on screen.
		tileObjects = new TilePrefab[config.maxTiles];


		for (int i = 0; i < config.maxTiles; i++) {
			tiles [i] = null;
			tileObjects [i] = null;
		}
	}

	public int addTile(Tile tile) {
		int index = insertTileToRack (tile);
		if (index == -1) {
			return -1;
		} else {
			placeTileOnScreenAtIndex (tile, index);
		}
		return index;
	}
	// return the rack index the tile was inserted into, or negative -1 if it could not be inserted.
	private int insertTileToRack(Tile tile) {
		for (int i = 0; i < tiles.Length; i++) {
			if (tiles [i] == null) {
				tiles [i] = tile;
				return i;
			}
		}
		return -1; 
	}
	private void placeTileOnScreenAtIndex(Tile tile, int index) {
		

		// Calculate x position of new tile on the rack
		float width = rack.GetComponent<Collider> ().bounds.size.x;
		float origin = rack.transform.position.x + width / 2.0f;

		// TODO: add half the width of a tile?
		float tilePositionX = -index * (width / config.maxTiles) + origin; 
		Vector3 tileV3 = new Vector3(tilePositionX, rack.transform.position.y, rack.transform.position.z);


		// Rotation of new tile is the same as the Rack's rotation

//		Vector3 right = new Vector3 (0, 0, Mathf.Lerp(75.0f, -75.0f, (float)(index + 1) / (float)config.maxTiles));
//		Debug.Log (right);
//		Vector3 tileOrigin = new Vector3 (tileV3.x, tileV3.y, tileV3.z - 1);
		TilePrefab tileObject = tileFactory.produceTile('B', 4, tileV3, rack.transform.rotation);
//		tileObject.destination = tileV3;
//		tileObject.orientation = right;


		//Text letterLabel = tileObject.transform.FindChild("TextCanvas").gameObject.transform.FindChild("Letter").gameObject.GetComponent<Text>();
		//Text valueLabel = tileObject.transform.FindChild("TextCanvas").gameObject.transform.FindChild("Value").gameObject.GetComponent<Text>();

		// TODO: Pull char and value off Tile, update labels
//		letterLabel.text = "A";
//		valueLabel.text = "1";

		tileObjects [index] = tileObject;
	}

	public void shuffle() {
		// TODO: shuffle internal representation and re-display new ordering on screen. 
	}

	private Vector3 offset;
	private Vector3 screenPoint;
	private float originalDistance;
	private Quaternion originalQuaternion;
	private Vector3 originalPosition;
	public void onMouseDown(TilePrefab tile) {
		Debug.Log ("on mouse down");

		returnable = null;
		shouldReturnTileToRack = false;
		originalDistance = surface.GetDistanceToPoint (tile.transform.position);
		originalQuaternion = tile.transform.rotation;
		originalPosition = tile.transform.position;

		screenPoint = Camera.main.WorldToScreenPoint(tile.transform.position);
		offset = tile.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, tile.transform.position.z));
	}
	public void onMouseDrag(TilePrefab tile) {
		Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, tile.transform.position.z);

		Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
		tile.transform.position = curPosition;

		float currDistance = surface.GetDistanceToPoint (tile.transform.position);

		Quaternion q = Quaternion.Euler(Mathf.Lerp(0, 90, currDistance / originalDistance), tile.transform.rotation.y, tile.transform.rotation.z);
		tile.transform.rotation = q;
	}

	private bool shouldReturnTileToRack;
	private TilePrefab returnable;
	public void onMouseUp(TilePrefab tile) {
		float finalDistance = surface.GetDistanceToPoint (tile.transform.position);
		if (finalDistance > 5.0) {
			// return the tile to the rack

			shouldReturnTileToRack = true;
			returnable = tile;
		} else {
			Del.placeTileOnBoard (tile);
		}
	}

	void Update() {
		if (shouldReturnTileToRack && returnable != null) {
			returnable.transform.position = Vector3.MoveTowards (returnable.transform.position, originalPosition, 20.0f * Time.deltaTime);  
			returnable.transform.rotation = Quaternion.RotateTowards (returnable.transform.rotation, originalQuaternion, 200.0f * Time.deltaTime);
		}
	}
}
