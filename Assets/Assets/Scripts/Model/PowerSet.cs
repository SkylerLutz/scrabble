using System;
using System.Collections;
using System.Collections.Generic;

public class PowerSet {
	public static List<List<Tile>> powerset(List<Tile> list){
		List<List<Tile>> ps = new List<List<Tile>>();
		ps.Add(new List<Tile>());   // add the empty set

		// for every item in the original list
		foreach (Tile item in list) {
			List<List<Tile>> newPs = new List<List<Tile>>();

			foreach (List<Tile> subset in ps) {
				// copy all of the current powerset's subsets
				newPs.Add(subset);

				// plus the subsets appended with the current item
				List<Tile> newSubset = new List<Tile>(subset);
				newSubset.Add(item);
				newPs.Add(newSubset);
			}

			// powerset is now powerset of list.subList(0, list.indexOf(item)+1)
			ps = newPs;
		}
		return ps;
	}

}
