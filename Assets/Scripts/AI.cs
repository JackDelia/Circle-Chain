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
	public BasicBehaviour[] blocks;


	void Start(){

		game = gameObject.GetComponent<SpawningProtocol> ();
		maneuvers = new int[][]{
			new int[] {0,0,3},
			new int[] {0,3},
			new int[] {3},
			new int[] {1,3},
			new int[] {1,1,3},
			new int[] {1,1,1,3},
			new int[] {4,2,2,0,0,3},
			new int[] {4,2,2,0,3},
			new int[] {4,2,2,3},
			new int[] {4,2,2,1,3},
			new int[] {4,2,2,1,1,3},
			new int[] {4,2,2,1,1,1,3},
			new int[] {2,0,0,3},
			new int[] {2,0,3},
			new int[] {2,3},
			new int[] {2,1,3},
			new int[] {2,1,1,3},
			new int[] {4,2,2,2,0,3},
			new int[] {4,2,2,2,3},
			new int[] {4,2,2,2,1,3},
			new int[] {4,2,2,2,1,1,3},
			new int[] {4,2,2,2,1,1,1,3}};
		nextBlock = true;
		maneuverposition = 0;
		selectManeuver ();
		move = getBestMove ();
		exec = false;
		}
	// Update is called once per frame
	void Update () {
		if (exec == true) {
			move = getBestMove ();
			exec = false;
				}

	}

	int getBestMove(){
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
		BasicBehaviour[] hypothetical;
		int decision = 0;
		int confidence = -10000;

		for (int i = 0; i<22; i++) {
			hypothetical = new BasicBehaviour[60];
			for (int k = 0; k<hypothetical.Length; k++) {
				hypothetical[i] = game.spaces[i];
			}

			int tempCon = 0;
			if(losesGame(i))
				tempCon-=1000;


			int posa = 0;
			int posb = 0;
			if(i<6){
				posa = 0+i;
				posb = 6+i;
			}
			if(i>5 && i<12){
				posa = 6 + i - 6;
				posb = 0 + i - 6;
			}
			if(i>11 && i<17){
				posa = 0 + i - 12;
				posb = 1 + i - 12;
			}
			if(i>16){
				posa = 1 + i - 17;
				posb = 0 + i - 17;
			}
			while(hypothetical[posa+6] == null && posa < 42){
				posa += 6;
			}
			while(hypothetical[posb+6] == null && posb < 42){
				posb += 6;
			}
			hypothetical[posa] = blocks[0];
			hypothetical[posb] = blocks[1];

			tempCon += getChainVal(hypothetical);
			tempCon -= getHeightVal(hypothetical);

			if(tempCon > confidence){
				decision = i;
				confidence = tempCon;
			}
				}

		currentManeuver = decision;
	}

	bool losesGame(int man){
		if (game.spaces [12] && game.spaces [13] && game.spaces [14] && game.spaces [15]
						&& game.spaces [16] && game.spaces [17]) {
			return false;
				}
		bool loss = false;
		if (game.spaces [6]) {
			loss = (man == 0 || man == 6 || man == 12 || man == 17);
				}

		if (game.spaces [12]) {
			loss = (man == 0 || man == 6);
		}
		
		if (game.spaces [7]) {
			loss = (man == 1 || man == 7 || man == 12 || man == 13 || man == 17 || man == 18);
		}

		if (game.spaces [13]) {
			loss = (man == 1 || man == 7);
		}

		if (game.spaces [8]) {
			loss = (man == 2 || man == 8 || man == 13 || man == 14 || man == 18 || man == 19);
		}

		if (game.spaces [14]) {
			loss = (man == 2 || man == 8);
		}

		if (game.spaces [9]) {
			loss = (man == 3 || man == 9 || man == 14 || man == 15 || man == 19 || man == 20);
		}

		if (game.spaces [15]) {
			loss = (man == 3 || man == 9);
		}

		if (game.spaces [10]) {
			loss = (man == 4 || man == 10 || man == 15 || man == 116 || man == 20 || man == 21);
		}

		if (game.spaces [16]) {
			loss = (man == 4 || man == 10);
		}

		if (game.spaces [11]) {
			loss = (man == 5 || man == 11 || man == 16 || man == 21);
		}

		if (game.spaces [17]) {
			loss = (man == 5 || man == 11);
		}
		if (loss)
						Debug.Log ("Nope ");
		return loss;
	}

	int getChainVal(BasicBehaviour[] hyp){
		return 2;
		}

	int getHeightVal(BasicBehaviour[] hyp){
		int ret = 0;
		for(int i = 18; i <= 23; i++){
			if(hyp[i])
				ret++;
			if(hyp[i-6])
				ret += 5;
			if(hyp[i-12])
				ret += 8;
		}
		return ret;
	}
}
