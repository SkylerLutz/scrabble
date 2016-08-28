//using System.Collections;
//using System.Collections.Generic;
//
//public sealed class ScrabbleDictionary {
//
//	private Trie dict;
//	private Permutator<string> permutator;
//	public ScrabbleDictionary(Trie d, Permutator<string> p) {
//		this.dict = d;
//		this.permutator = p;
//	}
//
//	public List<string> wordsForLetters(char[] letters) {
//		List<string> words = new List<string>();
//		List<string> permutations = permutator.permutate(letters.ToString());
//		foreach (string permutation in permutations) { 
//			if (dict.isWord(permutation)) {
//				words.Add(permutation);
//			}
//		}
//		return words;
//	}
//}
