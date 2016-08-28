using System.Collections;
using System.Collections.Generic;

public sealed class Permutator <T> {
	private T[] arr;
	private int[] permSwappings;

	public Permutator(T[] arr) {
		this.arr = (T[])arr.Clone();
		this.permSwappings = new int[arr.Length];
		for(int i = 0;i < permSwappings.Length;i++)
			permSwappings[i] = i;
	}

	public Permutator(T[] arr, int permSize) {
		this.arr = (T[])arr.Clone();
		this.permSwappings = new int[permSize];
		for(int i = 0;i < permSwappings.Length;i++)
			permSwappings[i] = i;
	}

	public T[] next() {
		if (arr == null)
			return null;
		T[] res = new T[permSwappings.Length];
		arr.CopyTo(res, 0);
		//Prepare next
		int i = permSwappings.Length-1;
		while (i >= 0 && permSwappings[i] == arr.Length - 1) {
			swap(i, permSwappings[i]); //Undo the swap represented by permSwappings[i]
			permSwappings[i] = i;
			i--;
		}

		if (i < 0)
			arr = null;
		else {   
			int prev = permSwappings[i];
			swap(i, prev);
			int next = prev + 1;
			permSwappings[i] = next;
			swap(i, next);
		}

		return res;
	}

	private void swap(int i, int j) {
		T tmp = arr[i];
		arr[i] = arr[j];
		arr[j] = tmp;
	}

}
