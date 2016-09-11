using UnityEngine;
using System.Collections;

public interface TileDelegate {
	bool canPlaceTileAt (int i, int j);
	void placeTileAt (GameObject go, int i, int j);
}

// This class has a misleading name. It's the script attached to a Tile GameObject.
public class TilePrefab : MonoBehaviour {

	public Vector3 destinationPosition;
	public Vector3 destinationScale;
	public Quaternion destinationRotation;

	private bool isDragging;

	// setter injection
	public GameObject board; 
	public BoardConfiguration boardConfig;
	public TileDelegate del;
	public bool isFixed = false;

	public void Update() {
		if (!isDragging) {
			gameObject.transform.position = Vector3.MoveTowards (gameObject.transform.position, destinationPosition, 20.0f * Time.deltaTime);  
			gameObject.transform.rotation = Quaternion.RotateTowards (gameObject.transform.rotation, destinationRotation, 200.0f * Time.deltaTime);
		}
	}

	private Vector3 screenPoint;
	private Vector3 offset;
	private float originalDistance;

	private float delta = 0;

	public void OnMouseDown() {
		if (isFixed) return;

		delta = 0;

		isDragging = true;

		// touch down
		screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

		// initial touch down state
		GameObject surface = board.transform.FindChild ("Surface").gameObject;
		Plane plane = new Plane (surface.transform.up, surface.transform.position);
		originalDistance = plane.GetDistanceToPoint (gameObject.transform.position);
	}

	public void OnMouseDrag() {
		if (isFixed) return;

		delta += Time.deltaTime;

		// position tracking
		Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
		transform.position = curPosition;

		// rotation tracking
		GameObject surface = board.transform.FindChild ("Surface").gameObject;
		Plane plane = new Plane (surface.transform.up, surface.transform.position);
		// TODO set rotation of self.transform to reflect distance from plane.
		float distance = plane.GetDistanceToPoint (gameObject.transform.position);

		Quaternion q = Quaternion.Euler(Mathf.Lerp(0, 90, distance / originalDistance), transform.rotation.y, transform.rotation.z);
		transform.rotation = q;
	}
	public void OnMouseUp() {
		if (isFixed) return;

		Debug.Log ("dragged for " + delta + " time units.");

		bool shouldReturn = false;
		if (delta < 0.05f) { // return the tile to the rack
			shouldReturn = true;
		}

		GameObject surface = board.transform.FindChild ("Surface").gameObject;
		Plane plane = new Plane (surface.transform.up, surface.transform.position);
		float distance = plane.GetDistanceToPoint (gameObject.transform.position);
		if (distance < 3 && !shouldReturn) {
			// on mouse up, position the tile appropriately (centered inside a square)

			int dimension = boardConfig.dimension;

			// Calculate x position of new tile on the rack
			float width = board.transform.FindChild ("BoardBox").gameObject.GetComponent<Collider> ().bounds.size.x;
			Vector3 boardOrigin = new Vector3 (board.transform.position.x + width / 2.0f, 0, board.transform.position.z - width / 2.0f);

			float i = Mathf.Floor (Mathf.Lerp (0.0f, (float)dimension, -(transform.position.x - boardOrigin.x) / width));
			float j = Mathf.Floor (Mathf.Lerp (0.0f, (float)dimension, (transform.position.z - boardOrigin.z) / width));

			if ((int)i >= dimension) {
				i = dimension - 1;
			} else if ((int)i < 0) {
				i = 0;
			}

			if ((int)j >= dimension) {
				j = dimension - 1;
			} else if ((int)j < 0) {
				j = 0;
			}

			if (del.canPlaceTileAt((int)i, (int)j)) {
				del.placeTileAt(gameObject, (int)i, (int)j);

				float spaceWidth = width / dimension;
				destinationPosition = new Vector3 (-i * spaceWidth + boardOrigin.x - (spaceWidth / 2), surface.transform.position.y, j * spaceWidth + boardOrigin.z + (spaceWidth / 2));
				destinationRotation = Quaternion.Euler (Vector3.up);
			}
		}
		isDragging = false;
	}
				
}
