using System.Collections;
using System.Collections.Generic;
using System;

public enum BagConfiguration {
	STANDARD,
	STANDARD_NO_WILDCARDS,
	ALL_VOWELS // because fuck you
}


public sealed class Bag {

	private List<Tile> list;
	private Random generator;

	public Bag(BagConfiguration config) {
		this.list = new List<Tile>();
		this.generator = new Random();

		switch(config) {
		case BagConfiguration.STANDARD: 
				break;
		case BagConfiguration.STANDARD_NO_WILDCARDS:  {
				Dictionary<char, int> freqs = new Dictionary<char, int>();
				freqs.Add('A', 16);
				freqs.Add('B', 4);
				freqs.Add('C', 6);
				freqs.Add('D', 8);
				freqs.Add('E', 24);
				freqs.Add('F', 4);
				freqs.Add('G', 5);
				freqs.Add('H', 5);
				freqs.Add('I', 13);
				freqs.Add('J', 2);
				freqs.Add('K', 2);
				freqs.Add('L', 7);
				freqs.Add('M', 6);
				freqs.Add('N', 13);
				freqs.Add('O', 15);
				freqs.Add('P', 4);
				freqs.Add('Q', 2);
				freqs.Add('R', 13);
				freqs.Add('S', 10);
				freqs.Add('T', 15);
				freqs.Add('U', 7);
				freqs.Add('V', 3);
				freqs.Add('W', 4);
				freqs.Add('X', 2);
				freqs.Add('Y', 4);
				freqs.Add('Z', 2);
				foreach(KeyValuePair<char, int> entry in freqs) {
					// do something with entry.Value or entry.Key
					for (int i = 0; i < entry.Value; i++) {
						list.Add(new Tile(TileType.LETTER, entry.Key));
					}
				}
				break;
			}
		case BagConfiguration.ALL_VOWELS: 
				list.Add(new Tile(TileType.LETTER, 'A'));
			list.Add(new Tile(TileType.LETTER, 'E'));
			list.Add(new Tile(TileType.LETTER, 'I'));
			list.Add(new Tile(TileType.LETTER, 'O'));
			list.Add(new Tile(TileType.LETTER, 'U'));
				break;
		}
	}

	public Tile draw() {
		if (list.Count == 0) {
			return null;
		}
		int index = generator.Next (list.Count);
		Tile tile = list [index];
		list.RemoveAt (index);
		return tile;
	}

	public int tilesRemaining() {
		return list.Count;
	}
}
