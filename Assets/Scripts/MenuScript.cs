using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {
	
	//Used to manage menu options

	public void start(){
		Application.LoadLevel ("SinglePlayer");
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		}
	public void ScoreAttack(){
		Application.LoadLevel ("ScoreAttack");
		}
	public void options(){
		Application.LoadLevel ("Options");
		}

	public void multiplayer(){
		Application.LoadLevel ("Multiplayer");
		}
	
	public void exit(){
		Application.Quit ();
		}
}
