using UnityEngine;
using System.Collections;

public interface BoardControllerDelegate {
	void removeTileFromBoard (TilePrefab tile);
}

public class BoardController : MonoBehaviour {

	public BoardControllerDelegate Del;

//
//	void LateUpdate() {
//		if (Input.GetMouseButtonDown (0)) {
//
//			RaycastHit hitInfo = new RaycastHit();
//			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo)) {
//				if (hitInfo.transform.gameObject.tag == "TileTag") {
//					Del.removeTileFromBoard (hitInfo.transform.gameObject as TilePrefab);
//					// calulate the tile that was hit w/o raycasts
//				}
//			}
//		}
//
//		if (Input.GetMouseButtonDown (1)) {
//			Debug.Log ("right click");
//		}
//	}


	public GameObject board;

	public BoardConfiguration config;
	private TilePrefab[][] boardRepresentation;
	private TilePrefab[][] moveContext;
	private Vector3 offset;

	void Start() {

		initBoardRepresentation ();
	}

	private void initBoardRepresentation() {

		for (int i = 0; i < config.dimension; i++) {
			for (int j = 0; j < config.dimension; j++) {
				boardRepresentation [i] [j] = null;
				moveContext [i] [j] = null;
			}
		}
	}
	public void onMouseDown(TilePrefab tile) {
		dropped = null;

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		GameObject surface = board.transform.FindChild ("Surface").gameObject;
		Plane plane = new Plane (surface.transform.up, surface.transform.position);
		float f;
		Vector3 mouseDownAt = Vector3.up;
		if (plane.Raycast(ray, out f)) {
			mouseDownAt = ray.GetPoint(f);
		}

		offset = tile.transform.position - mouseDownAt;
	}
	public void onMouseDrag(TilePrefab tile) {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		GameObject surface = board.transform.FindChild ("Surface").gameObject;
		Plane plane = new Plane (surface.transform.up, surface.transform.position);
		float f;
		Vector3 mouseDownAt = Vector3.up;
		if (plane.Raycast(ray, out f)) {
			mouseDownAt = ray.GetPoint(f);
		}
		tile.transform.position = mouseDownAt + offset;
	}

	public void onMouseUp(TilePrefab tile) {
		// on mouse up, position the tile appropriately (centered inside a square)

		int dimension = config.dimension;

		// Calculate x position of new tile on the rack
		float width = board.transform.FindChild ("BoardBox").gameObject.GetComponent<Collider> ().bounds.size.x;
		Vector3 boardOrigin = new Vector3 (board.transform.position.x - width / 2.0f, 0, board.transform.position.z - width / 2.0f);

		Vector3 curr = tile.transform.position;

	
		float i = Mathf.Floor (Mathf.Lerp (0.0f, (float)config.dimension, (curr.x - boardOrigin.x) / width));
		float j = Mathf.Floor (Mathf.Lerp (0.0f, (float)config.dimension, (curr.z - boardOrigin.z) / width));

		if ((int)i >= config.dimension) {
			i = config.dimension - 1;
		} else if ((int)i < 0) {
			i = 0;
		}

		if ((int)j >= config.dimension) {
			j = config.dimension - 1;
		} else if ((int)j < 0) {
			j = 0;
		}



		float spaceWidth = width / dimension;

		GameObject surface = board.transform.FindChild ("Surface").gameObject;



		dropped = tile;
		destination = new Vector3 (i * spaceWidth + boardOrigin.x + (spaceWidth / 2), surface.transform.position.y, j * spaceWidth + boardOrigin.z + (spaceWidth / 2));
		originalDistance = Vector3.Distance (tile.transform.position, destination);


//		moveContext [i] [j] = tile;
	}



	private float originalDistance;
	void FixedUpdate() {
		if (dropped != null) {
			float distance = Vector3.Distance (dropped.transform.position, destination);
			dropped.transform.position = Vector3.MoveTowards (dropped.transform.position, destination, Mathf.Lerp(30, 100, Mathf.Abs(distance / originalDistance)) * Time.deltaTime);  
			dropped.transform.rotation = Quaternion.RotateTowards (dropped.transform.rotation, Quaternion.Euler(Vector3.up), 200.0f * Time.deltaTime);
		}
	}

	private Vector3 destination;
	private TilePrefab dropped = null;
}
