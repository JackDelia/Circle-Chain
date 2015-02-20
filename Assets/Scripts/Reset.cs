using UnityEngine;
using System.Collections;

public class Reset : MonoBehaviour {

	public void goBack(){
		Application.LoadLevel (FindObjectOfType<Optional> ().playMode);
		}
}
