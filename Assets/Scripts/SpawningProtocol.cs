using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpawningProtocol : MonoBehaviour {

	//Music and sounds
	public AudioSource SFX;
	public AudioSource BGM;

	public int score = 0;
	public Text scoreDisplay;

	//game options
	public Optional options;

	//how many blocks have spawned
	public int blockPlace;
	//the blocks to spawn.
	//numbers correlate to different colored blocks
	public int[] toSpawn = new int[] {};
	//An array of block prefabs used to call Instantiate 
	public GameObject[] spawnable;

	//Where all the not live blocks are
	//game numbers each space 0-47 left to right, top to bottom
	//8 rows, six columns
	public BasicBehaviour[] spaces;

	//the blocks in the "next" box
	public BasicBehaviour[] displayed;

	//used to clear blocks
	public bool checkAgain;
	public int combo;

	//Whether blocks spawned will be player controlled
	public bool p1;

	//The RNG
	public GameObject RNG;

	//the other spawner
	public SpawningProtocol brother;

	//Useless blocks to be spawned after the current block falls to the ground
	public int useless;

	//whether the game is multiplayer
	public bool multi;

	//if the game is over
	public bool gameEnd;

	//how fast blocks drop
	public float speed;

	//Used for Network creation
	private const string typeName = "Zcrnownnwncuwip";
	public string gameName = "";
	private HostData[] hostList;
	public GameObject enterer;
	
	// Use this for initialization
	void Start () {

		//checks if there are options set and uses them if there are
		//otherwise sets them to default
		if (FindObjectOfType (typeof(Optional)) != null)
						options = FindObjectOfType (typeof(Optional)) as Optional;
				else
						options = gameObject.AddComponent<Optional> ();
		SFX.volume = options.volume;
		BGM.volume = options.volume;
		speed = options.speed;

		//standardizes the speed in a multiplayer game
		if (multi)
						speed = 4;

		//initialize instance variables
		combo = 1;
		displayed = new BasicBehaviour[2];
		checkAgain = false;
		spaces = new BasicBehaviour[60];
		for (int i = 0; i<spaces.Length; i++) {
			spaces[i] = null;
				}
		blockPlace = 0;

						
		toSpawn = RNG.GetComponent<RNG> ().toSpawn;

				
		//toSpawn = new int[] {1,2,3,1,2,3,3,2,2,2,2,1,3,3,1,3,2,1,3,2,3,1,4,2,3,1,3,1,2,1,2,1,4,3,1,3,2,2,1,4,3,2,3,4,1,3,2,4,1,3,2,4,1,3,1,4,2,3,2,4,3,1,4,3,2,2,4,1,3};
		//toSpawn = new int[] {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1};

		//if it's singleplayer, start spawning blocks right away
		if(!multi)
			spawnNext();

	}

	//sets the name of the game
	//used in multiplayer
	public void setName(string nm){
		
		gameName = nm;
	}

	//starts the server
	private void StartServer()
	{
		DontDestroyOnLoad (this);
		enterer.GetComponent<killer> ().kill ();
		Network.InitializeServer(2, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
	}

	//gets available hosts
	private void RefreshHostList()
	{
		MasterServer.RequestHostList(typeName);
	}

	//updates the list of hosts when host list is recieved
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}

	//joins a server
	private void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}

	//sets the not host to the second player slot
	void OnConnectedToServer()
	{
		p1 = !p1;
	}

	//Starts the game once the players are connected
	void OnPlayerConnected()
	{
		gameObject.networkView.RPC ("spawnNext", RPCMode.All);
	}

	//creates the UI for setting up multiplayer games
	void OnGUI(){
				if (multi) {
						if (!Network.isClient && !Network.isServer) {
								if (GUI.Button (new Rect (100, 100, 250, 100), "Start Server")) {
										if (!(gameName.Equals ("")))
												StartServer ();
								}
			
								if (GUI.Button (new Rect (100, 250, 250, 100), "Refresh Hosts"))
										RefreshHostList ();
			
								if (hostList != null) {
										for (int i = 0; i < hostList.Length; i++) {
												if (GUI.Button (new Rect (400, 100 + (110 * i), 300, 100), hostList [i].gameName)) {
														JoinServer (hostList [i]);
														enterer.GetComponent<killer> ().kill ();
												}
										}
								}
						}
				}
		}

	//loads the game end scene
	public void GameOver(){
				Application.LoadLevel ("GameOver");
		}

	//spawns useless blocks
	public void spawnUseless(){
		int rows = useless / 6;
		int extra = useless % 6;
		while (rows >0) {
						
			for (int i = 0; i<6; i++) {
			
				Vector3 apos = transform.position;
				apos.x -= 198*2;
				apos.x += 198*i;
				GameObject blockA;
				if(multi)
				{
					blockA = Network.Instantiate (spawnable [5], apos, transform.rotation,0) as GameObject;
				}
				else{
					blockA = Instantiate (spawnable [5], apos, transform.rotation) as GameObject;		
				}
				blockA.GetComponent<BasicBehaviour> ().central = true;
				blockA.GetComponent<BasicBehaviour> ().spawningProtocol = this;
				blockA.GetComponent<BasicBehaviour> ().pos = i;
				spaces[i]= blockA.GetComponent<BasicBehaviour>();
				fallIn();

						} 
			rows--;
				}
		ArrayList slots = new ArrayList() {0,1,2,3,4,5};
		for(int v=0;v<extra;v++){
			int column = Random.Range(0,6);
			while(!slots.Contains(column)){
				column = Random.Range(0,6);
			}
			Vector3 apos = transform.position;
			apos.x -= 198*2;
			apos.x += 198*column;
			GameObject blockA;
			if(multi)
			{
				blockA = Network.Instantiate (spawnable [5], apos, transform.rotation,0) as GameObject;
			}
			else{
				blockA = Instantiate (spawnable [5], apos, transform.rotation) as GameObject;		
			}		
			blockA.GetComponent<BasicBehaviour> ().central = true;
			blockA.GetComponent<BasicBehaviour> ().spawningProtocol = this;
			blockA.GetComponent<BasicBehaviour> ().pos = column;
			spaces[column]= blockA.GetComponent<BasicBehaviour>();	
			fallIn();

		}
		useless = 0;
	}

	//spawns the next blocks
	[RPC]
	public void spawnNext(){

		//if the game is over, stop spawning blocks
		if (gameEnd)
						return;

		//spawn the useless blocks saved up
		spawnUseless ();

		//display the next blocks
		displayNext ();

		//up the speed if it's time to
		if (speed >= .2 && blockPlace % 20 == 0 && blockPlace != 0)
						speed -= .1f;

		//create the top, central block
		GameObject blockA;
		blockA = null;
		if (multi) {
			if(p1){
			blockA = Network.Instantiate (spawnable [toSpawn [blockPlace]], transform.position, transform.rotation,0) as GameObject;
			//blockA.GetComponent<BasicBehaviour> ().central = true;
			blockA.GetComponent<BasicBehaviour> ().spawningProtocol = this;
			blockA.GetComponent<BasicBehaviour> ().pos = 2;
			blockA.GetComponent<BasicBehaviour> ().side = 2;
			blockA.GetComponent<BasicBehaviour> ().live = true;
			blockA.GetComponent<BasicBehaviour> ().speed = speed;
			}
				} 
		else {
			blockA = Instantiate (spawnable [toSpawn [blockPlace]], transform.position, transform.rotation) as GameObject;	
			//blockA.GetComponent<BasicBehaviour> ().central = true;
			blockA.GetComponent<BasicBehaviour> ().spawningProtocol = this;
			blockA.GetComponent<BasicBehaviour> ().pos = 2;
			blockA.GetComponent<BasicBehaviour> ().side = 2;
			blockA.GetComponent<BasicBehaviour> ().live = true;
			blockA.GetComponent<BasicBehaviour> ().speed = speed;
				}
		blockPlace++;

		//spawn the bottom, not central block
		GameObject blockB;
		blockB = null;
		Vector3 bpos = transform.position;
		bpos.y -= 198;
		if (multi) {
			if(p1){
			blockB = Network.Instantiate (spawnable [toSpawn [blockPlace]], bpos, transform.rotation,0) as GameObject;
			blockB.GetComponent<BasicBehaviour> ().spawningProtocol = this;
			blockB.GetComponent<BasicBehaviour> ().pos = 8;
			blockB.GetComponent<BasicBehaviour> ().live = true;
			blockB.GetComponent<BasicBehaviour> ().speed = speed;
				blockA.GetComponent<BasicBehaviour> ().partner = blockB;
				blockB.GetComponent<BasicBehaviour> ().partner = blockA;
			}
		} 
		else {
			blockB = Instantiate (spawnable [toSpawn [blockPlace]], bpos, transform.rotation) as GameObject;
			blockB.GetComponent<BasicBehaviour> ().spawningProtocol = this;
			blockB.GetComponent<BasicBehaviour> ().pos = 8;
			blockB.GetComponent<BasicBehaviour> ().live = true;
			blockB.GetComponent<BasicBehaviour> ().speed = speed;	
			blockA.GetComponent<BasicBehaviour> ().partner = blockB;
			blockB.GetComponent<BasicBehaviour> ().partner = blockA;
		}
		blockB.GetComponent<BasicBehaviour> ().central = true;
		blockPlace++;

		//update the AI if applicable
		if (gameObject.GetComponent<AI> ()) {
			gameObject.GetComponent<AI> ().nextBlock = true;
			gameObject.GetComponent<AI> ().blocks = new BasicBehaviour[] {blockA.GetComponent<BasicBehaviour>(), blockB.GetComponent<BasicBehaviour>()};
		}

	}


	//displays the next blocks in the box
	//spawns actual blocks that are set to not be live
	void displayNext(){
		if (displayed [0] != null) {
			displayed [0].kill ();
			displayed [1].kill ();
		}

		Vector3 apos = transform.position;
		apos.x += 1056;
		apos.y -= 26;
		GameObject blockA;
		blockA = null;
		if (multi) {
			blockA = Network.Instantiate (spawnable [toSpawn [blockPlace + 2]], apos, transform.rotation,0) as GameObject;
				} 
		else {
			blockA = Instantiate (spawnable [toSpawn [blockPlace + 2]], apos, transform.rotation) as GameObject;
				}
		displayed [0] = blockA.GetComponent<BasicBehaviour> ();

		Vector3 bpos = transform.position;
		bpos.x += 1056;
		bpos.y -= 224;
		GameObject blockB;
		if (multi) {
			blockB = Network.Instantiate(spawnable[toSpawn[blockPlace+3]], bpos, transform.rotation,0) as GameObject;
				}
		else{
			blockB = Instantiate(spawnable[toSpawn[blockPlace+3]], bpos, transform.rotation) as GameObject;
		}
		displayed [1] = blockB.GetComponent<BasicBehaviour> ();

	}


	//has blocks fall in after they are cleared
	public void fallIn(){
		for(int v=41; v>=0;v--){
			if(spaces[v] != null && spaces[v+6]==null){
				int h = v;
				while(spaces[h+6]==null && h<42){
					spaces[h+6] = spaces[h];
					Vector3 newpos = spaces[h].transform.position;
					newpos.y -= 1.0f * spaces[h].step; 
					spaces[h].transform.position = newpos;
					spaces[h].pos+=6;
					spaces[h+6] = spaces[h];
					spaces[h] = null;
					h+=6;
				}
			}
		}
		}

	//clears blocks 
	 public void clearBlocks(){ 

		//if blocks touch the top, it's game over.
		for (int i = 0; i<6; i++) {
			if(spaces[i]){
				GameOver();
				return;
			}
		}

		//check agin used if blocks have fallen in so that the method will repeat itself
		checkAgain = false;

		//call the helper method to check if the blocks should disappear
		ArrayList inCluster = new ArrayList ();
		for (int i= 47; i>=0; i--) {
			if(spaces[i] != null && spaces[i].color!="n"){
				inCluster.AddRange(clearBlocks(inCluster,spaces[i]));
				inCluster = prune(inCluster);

				//clears blocks and plays sound effect if four are touching
				if(inCluster.Count >=4){
					SFX.Play();
					clearSpaces(inCluster);
					checkAgain = true;
					if(brother)
						brother.useless+=inCluster.Count*combo;
					if(scoreDisplay){
						score += inCluster.Count*combo;
						scoreDisplay.text =  "" + score;
						while (scoreDisplay.text.Length<5){
							scoreDisplay.text = "0" + scoreDisplay.text; 
						}
					}
					combo++;
				}
				inCluster = new ArrayList();
			}
				}

		if (checkAgain)
						clearBlocks ();
		combo = 1;
		}


	//recursive helper method for clearing blocks
	 ArrayList clearBlocks(ArrayList inCluster, BasicBehaviour b){
		inCluster.Add(b.pos);
		if (spaces [b.pos - 6] != null && spaces [b.pos - 6].color.Equals (b.color) 
		    && !(inCluster == null) && !inCluster.Contains (b.pos - 6)) {

			inCluster.AddRange(clearBlocks(inCluster,spaces[b.pos-6]));
				}

		if (spaces [b.pos + 6] != null && spaces [b.pos + 6].color.Equals (b.color) 
		    && !(inCluster == null) && !inCluster.Contains (b.pos + 6)) {
			inCluster.AddRange(clearBlocks(inCluster,spaces[b.pos+6]));
		}

		if (spaces [b.pos - 1] != null && spaces [b.pos - 1].color.Equals (b.color) 
		    && !inCluster.Contains (b.pos - 1) && b.pos % 6 != 0) {

			inCluster.AddRange(clearBlocks(inCluster,spaces[b.pos-1]));
		}

		if (spaces [b.pos + 1] != null && spaces [b.pos + 1].color.Equals (b.color) 
		    && !inCluster.Contains (b.pos + 1) && b.pos % 6 != 5) {

			inCluster.AddRange(clearBlocks(inCluster,spaces[b.pos+1]));
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

	//deletes the blocks cleared
	void clearSpaces(ArrayList a){
		foreach (int i in a) {
			if(spaces[i]!=null){
			spaces[i].kill ();
			spaces[i] = null;
				if(spaces[i+1] != null && spaces[i+1].color.Equals("n") && i%6!=5){
					spaces[i+1].kill ();
					spaces[i+1] = null;
				}
				if(spaces[i-1] != null && spaces[i-1].color.Equals("n") && i%6!=0){
					spaces[i-1].kill ();
					spaces[i-1] = null;
				}
				if(spaces[i+6] != null && spaces[i+6].color.Equals("n")){
					spaces[i+6].kill ();
					spaces[i+6] = null;
				}
				if(spaces[i-6] != null && spaces[i-6].color.Equals("n")){
					spaces[i-6].kill ();
					spaces[i-6] = null;
				}
			}
				}

		//remaining blocks fall in
		fallIn ();
		}

}
		                     