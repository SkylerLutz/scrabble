using System;
using System.Collections;
using System.Collections.Generic;

public class PowerSet {
	public static List<List<T>> powerset<T>(List<T> list){

		var result = new List<List<T>> ();
		for (int i = 0; i < (1 << list.Count); i++) { // 1 << n == the n-th power of 2
			var sublist = new List<T> ();
			for (int j = 0; j < list.Count; j++) { // analyze each bit in "i"
				if ((i & (1 << j)) != 0) { // if the j-th bit of i is set...
					sublist.Add (list[j]); // add the item to the current sublist
				}
			}
			result.Add (sublist); // add the current sublist to the final result
		}



		return result;

//		List<List<T>> ps = new List<List<T>>();
//		ps.Add(new List<T>());   // add the empty set
//
//		// for every item in the original list
//		foreach (T item in list) {
//			List<List<T>> newPs = new List<List<T>>();
//
//			foreach (List<T> subset in ps) {
//				// copy all of the current powerset's subsets
//				newPs.Add(subset);
//
//				// plus the subsets appended with the current item
//				List<T> newSubset = new List<T>(subset);
//				newSubset.Add(item);
//				newPs.Add(newSubset);
//			}
//
//			// powerset is now powerset of list.subList(0, list.indexOf(item)+1)
//			ps = newPs;
//		}
//		return ps;


	}

}
