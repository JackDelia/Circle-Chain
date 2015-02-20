using UnityEngine;
using System.Collections;

public class RNG : MonoBehaviour {
	public int[] toSpawn;
	//RNG for the blocks

	void Start () {
		toSpawn = new int[10000];
		for (int i = 0; i<10000; i++) {
			toSpawn [i] = Random.Range (0, 5);
		} 
	}

}
