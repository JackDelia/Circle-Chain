using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour {

	//0: left
	//1: right
	//2: turn
	//3: down
	public int move;


	public int[][] maneuvers;
	public bool exec;
	public bool nextBlock;
	public int currentManeuver;
	public int maneuverposition;
	public SpawningProtocol game;

	//first is central
	//0: red
	//1: blue
	//2: green
	//3: yellow
	//4: purple
	public int[] blocks;


	void Start(){

		game = gameObject.GetComponent<SpawningProtocol> ();
		maneuvers = new int[][]{
			new int[] {0,0,3},
			new int[] {0,3},
			new int[] {3},
			new int[] {1,3},
			new int[] {1,1,3},
			new int[] {1,1,1,3},
			new int[] {3,2,2,0,0,3},
			new int[] {3,2,2,0,3},
			new int[] {3,2,2,3},
			new int[] {3,2,2,1,3},
			new int[] {3,2,2,1,1,3},
			new int[] {3,2,2,1,1,1,3},
			new int[] {2,0,0,3},
			new int[] {2,0,3},
			new int[] {2,3},
			new int[] {2,1,3},
			new int[] {2,1,1,3},
			new int[] {3,2,2,2,0,3},
			new int[] {3,2,2,2,3},
			new int[] {3,2,2,2,1,3},
			new int[] {3,2,2,2,1,1,3},
			new int[] {3,2,2,2,1,1,1,3}};
		nextBlock = true;
		maneuverposition = 0;
		selectManeuver ();
		move = getBestMove ();
		}
	// Update is called once per frame
	void Update () {
		if (exec = true) {
			move = getBestMove ();
			exec = false;
				}

	}

	int getBestMove(){
		//return Random.Range (0, 5);

		if (nextBlock) {
			selectManeuver();
			maneuverposition = 0;
			nextBlock = false;
				}
			
		int ret = maneuvers [currentManeuver] [maneuverposition];
		if(maneuverposition < maneuvers[currentManeuver].Length-1)
			maneuverposition++;
		return ret;
		}

	void selectManeuver(){
		currentManeuver = Random.Range(0,22);
		if(currentManeuver == 2 || currentManeuver == 3
		   || currentManeuver == 8 || currentManeuver == 9
		   || currentManeuver == 13 || currentManeuver == 14 || currentManeuver == 15
		   || currentManeuver == 18 || currentManeuver == 19 || currentManeuver == 20)
			currentManeuver = Random.Range(0,22);
		if(currentManeuver == 2 || currentManeuver == 3
		   || currentManeuver == 8 || currentManeuver == 9
		   || currentManeuver == 13 || currentManeuver == 14 || currentManeuver == 15
		   || currentManeuver == 18 || currentManeuver == 19 || currentManeuver == 20)
			currentManeuver = Random.Range(0,22);
		if(currentManeuver == 2 || currentManeuver == 3
		   || currentManeuver == 8 || currentManeuver == 9
		   || currentManeuver == 13 || currentManeuver == 14 || currentManeuver == 15
		   || currentManeuver == 18 || currentManeuver == 19 || currentManeuver == 20)
			currentManeuver = Random.Range(0,22);
		if(currentManeuver == 2 || currentManeuver == 3
		   || currentManeuver == 8 || currentManeuver == 9
		   || currentManeuver == 13 || currentManeuver == 14 || currentManeuver == 15
		   || currentManeuver == 18 || currentManeuver == 19 || currentManeuver == 20)
			currentManeuver = Random.Range(0,22);
		if(currentManeuver == 2 || currentManeuver == 3
		   || currentManeuver == 8 || currentManeuver == 9
		   || currentManeuver == 13 || currentManeuver == 14 || currentManeuver == 15
		   || currentManeuver == 18 || currentManeuver == 19 || currentManeuver == 20)
			currentManeuver = Random.Range(0,22);


	
	}

}
