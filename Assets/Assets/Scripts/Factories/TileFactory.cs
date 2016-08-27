using UnityEngine;
using System.Collections;

public class TileFactory : MonoBehaviour {

	public Transform view;
	public TilePrefab tilePrefab;
	public RackController rackController;
	public BoardController boardController;

	public TilePrefab produceTile(char letter, int pointValue, Vector3 position, Quaternion rotation) {
		
		TilePrefab tile = (TilePrefab)Instantiate(tilePrefab, position, rotation, view);
		tile.Del = rackController;
		// TODO: set letter and value on the prefab's label.
		return tile;
	}
}
