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
	public float timer= Time.time;

	public BlockController[] hypothetical;

	//first is central
	//0: red
	//1: blue
	//2: green
	//3: yellow
	//4: purple
	public BlockController[] blocks;


	void Start(){

		game = gameObject.GetComponent<SpawningProtocol> ();
		maneuvers = new int[][]{
			new int[] {0,0,3},
			new int[] {0,3},
			new int[] {3},
			new int[] {1,3},
			new int[] {1,1,3},
			new int[] {1,1,1,3},
			new int[] {2,2,0,0,3},
			new int[] {2,2,0,3},
			new int[] {2,2,3},
			new int[] {2,2,1,3},
			new int[] {2,2,1,1,3},
			new int[] {2,2,1,1,1,3},
			new int[] {2,0,0,3},
			new int[] {2,0,3},
			new int[] {2,3},
			new int[] {2,1,3},
			new int[] {2,1,1,3},
			new int[] {2,2,2,0,3},
			new int[] {2,2,2,3},
			new int[] {2,2,2,1,3},
			new int[] {2,2,2,1,1,3},
			new int[] {2,2,2,1,1,1,3}};
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
		timer = Time.time;

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
		//BlockController[] hypothetical;
		int decision = 0;
		int confidence = -10000;

		for (int i = 0; i<22; i++) {
			hypothetical = new BlockController[60];
			for (int k = 0; k<hypothetical.Length; k++) {
				hypothetical[k] = game.spaces[k];
			}

			int tempCon = 0;
			if(losesGame(i)){
				tempCon-=1000;
			}


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
			//Debug.Log (i + ": " + tempCon);
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
			Debug.Log("Despair");
			return false;
				}
		bool loss = false;
		if (game.spaces [6]) {
			loss = (man == 0 || man == 6 || man == 12 || man == 17);
				}

		if (game.spaces [12]) {
			return (man == 0 || man == 6);
		}
		
		if (game.spaces [7]) {
			return (man == 1 || man == 7 || man == 12 || man == 13 || man == 17 || man == 18);
		}

		if (game.spaces [13]) {
			return (man == 1 || man == 7);
		}

		if (game.spaces [8]) {
			return (man == 2 || man == 8 || man == 13 || man == 14 || man == 18 || man == 19);
		}

		if (game.spaces [14] || game.spaces[20] || game.spaces[26]) {
			return (man == 2 || man == 8);
		}

		if (game.spaces [9]) {
			return (man == 3 || man == 9 || man == 14 || man == 15 || man == 19 || man == 20);
		}

		if (game.spaces [15]) {
			return (man == 3 || man == 9);
		}

		if (game.spaces [10]) {
			return (man == 4 || man == 10 || man == 15 || man == 116 || man == 20 || man == 21);
		}

		if (game.spaces [16]) {
			return (man == 4 || man == 10);
		}

		if (game.spaces [11]) {
			return (man == 5 || man == 11 || man == 16 || man == 21);
		}

		if (game.spaces [17]) {
			return (man == 5 || man == 11);
		}
		return loss;
	}

	int getChainVal(BlockController[] hyp){
		int ret = 0;

		ArrayList inCluster = new ArrayList ();
		for (int i= 47; i>=0; i--) {
						if (hyp [i] != null && hyp [i].color != "n") {
								inCluster.AddRange (clearBlocks (hyp, inCluster, hyp [i]));
								inCluster = prune (inCluster);
						}
			if(inCluster.Count >=4){
				ret += 10;
			}
			else if(inCluster.Count == 3)
			        ret+= 4;        
			else if(inCluster.Count ==2)
				ret+= 1;
			if(Time.time-timer>.5f)
				break;
				
		}
		return ret;
		}

	//recursive helper method for clearing blocks
	//here repurposed to find optimal block placement
	ArrayList clearBlocks(BlockController[] spaces, ArrayList inCluster, BlockController b){
		inCluster.Add(b.pos);

		if (b.pos-6 >=0 && spaces [b.pos - 6] != null && spaces [b.pos - 6].color.Equals (b.color) 
		    && !(inCluster == null) && !inCluster.Contains (b.pos - 6) && Time.time - timer > .5) {
			
			inCluster.AddRange(clearBlocks(spaces, inCluster,spaces[b.pos-6]));
		}
		
		if (spaces [b.pos + 6] != null && spaces [b.pos + 6].color.Equals (b.color) 
		    && !(inCluster == null) && !inCluster.Contains (b.pos + 6) && Time.time - timer > .5) {
			inCluster.AddRange(clearBlocks(spaces, inCluster,spaces[b.pos+6]));
		}
		
		if (spaces [b.pos - 1] != null && spaces [b.pos - 1].color.Equals (b.color) 
		    && !inCluster.Contains (b.pos - 1) && b.pos % 6 != 0 && Time.time - timer > .5) {
			
			inCluster.AddRange(clearBlocks(spaces, inCluster,spaces[b.pos-1]));
		}
		
		if (spaces [b.pos + 1] != null && spaces [b.pos + 1].color.Equals (b.color) 
		    && !inCluster.Contains (b.pos + 1) && b.pos % 6 != 5 && Time.time - timer > .5) {
			
			inCluster.AddRange(clearBlocks(spaces , inCluster,spaces[b.pos+1]));
		}
		
		return prune (inCluster);
	}
	
	//gets rid of repeated numbers
	ArrayList prune(ArrayList toPrune){
		ArrayList ret = new ArrayList ();
		foreach (int a in toPrune)
			if (!ret.Contains (a))
				ret.Add (a);
		return ret;
	}


	int getHeightVal(BlockController[] hyp){
		int ret = 0;
		for(int i = 24; i <= 29; i++){
			if(hyp[i])
				ret++;
			if(hyp[i-6])
				ret += 3;
			if(hyp[i-12])
				ret += 6;
			if(hyp[i-18])
				ret+=10;
		}
		return ret;
	}

}
