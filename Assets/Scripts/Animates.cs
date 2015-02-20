using UnityEngine;
using System.Collections;

public class Animates : MonoBehaviour {

	public Texture[] frames;
	public int framesPerSecond = 10;
	
	void Update() {
		int index = (int) ((Time.time * framesPerSecond) % frames.Length);
		renderer.material.mainTexture = frames[index];
	}

}
