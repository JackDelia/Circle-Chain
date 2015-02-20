using UnityEngine;
using System.Collections;

public class Optional : MonoBehaviour {
	
	public float volume = 1;
	public float speed = 1;
	public string playMode; 

	//used to adjust/store options

	void Start(){
		DontDestroyOnLoad (this);
		}

	void Update(){
		if (!(Application.loadedLevelName.Equals ("GameOver")))
						playMode = Application.loadedLevelName;
	}
	public void changeVol(float f){
				volume = f;
		}

	public void changeSpeed(float f){

		speed = 1.5f - f;
	}

}
