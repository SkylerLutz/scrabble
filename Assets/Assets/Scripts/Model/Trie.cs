using System.Collections;
using System.Collections.Generic;
using System;

public sealed class Trie {
	
	private TrieNode[] roots;

	// constructor
	public Trie() {
		
		this.roots = new TrieNode[26]; // 26 letters -- english only
		// initialize self using words unix file
		string line;

		// Read the file and display it line by line.
		System.IO.StreamReader file = new System.IO.StreamReader("Assets/Assets/Scripts/Model/words");
		while((line = file.ReadLine()) != null)
		{
			insert(line);
		}

		file.Close();
	}

	public bool isWord(string word) {
		if (word.Length == 0) {
			return false;
		}	
		string upper = word.ToUpper();

		int index = getIndex(upper.ToCharArray()[0]);
		return isWord(upper, 0, roots[index]);
	}

	private bool isWord(string word, int index, TrieNode node) {

		// i am assuming input is safe (unit tests assert confidence)
//		if getIndex(word[index]) >= roots.count {
//			return false; //
//		}
	
		TrieNode next = null;
		if (node == null) {
			return false;
		}
		else if (word.Length == index + 1) {
			return node.isWord;
		}
		else if (node.nextNodes[getIndex(word.ToCharArray()[index + 1])] != null) {
			next = node.nextNodes[getIndex(word.ToCharArray()[index + 1])];
			return isWord(word, index + 1, next);
		}
		else {
			return false;
		}
	}
	
	// return the array index for a given letter. A-Z is 0-25
	private int getIndex(char letter) {
		return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(letter);
	}

	private void insert(string word) {
		insert(word, 0, roots);	
	}

	private void insert(string word, int index, TrieNode[] nodes) {
		int i = getIndex(word.ToCharArray()[index]);
		TrieNode curr = nodes[i];
		if (curr == null) {
			if (word.Length == index + 1) {
				TrieNode node = new TrieNode(true, word.ToCharArray()[index], new TrieNode[26]);
				nodes[i] = node;
			}
			else {
				TrieNode node = new TrieNode(false, word.ToCharArray()[index], new TrieNode[26]);
				nodes[i] = node;
				insert(word, index + 1, node.nextNodes);
			}
		}
		else {
			if (word.Length == index + 1) {
				TrieNode node = new TrieNode(true, curr.letter, curr.nextNodes);
				nodes[i] = node;
			}
			else {
				insert(word, index + 1, curr.nextNodes);
			}
		}
	}


	internal class TrieNode {
		internal bool isWord;
		internal char letter;
		internal TrieNode[] nextNodes;

		// constructor 
		internal TrieNode(bool isWord, char letter, TrieNode[] nextNodes) {
			this.isWord = isWord;
			this.letter = letter;
			this.nextNodes = nextNodes;
		}
	}
}
