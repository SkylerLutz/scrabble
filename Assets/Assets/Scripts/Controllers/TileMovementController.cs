using UnityEngine;
using System.Collections;

public class TileMovementController : MonoBehaviour, RackControllerDelegate, BoardControllerDelegate {

	public BoardController boardController;
	public RackController rackController;

	void Start() {

		rackController.Del = this;
		boardController.Del = this;

		// set rack controller's plane to board controller's surface plane
		GameObject surface = boardController.board.transform.FindChild ("Surface").gameObject;
		Plane plane = new Plane (surface.transform.up, surface.transform.position);
		rackController.surface = plane;
	}

	public void placeTileOnBoard(TilePrefab tile) {
		// hand off tile to board controller.
		tile.Del = boardController;
		boardController.onMouseUp (tile);
	}

	public void removeTileFromBoard (TilePrefab tile) {
		// hand tile back to the rack's control.
		tile.Del = rackController;
		rackController.onMouseUp (tile);
	}
}
