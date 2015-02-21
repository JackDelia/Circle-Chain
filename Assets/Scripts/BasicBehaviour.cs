using UnityEngine;
using System.Collections;

public class BasicBehaviour : MonoBehaviour {

	//whether the block is still active (not grounded)
	public bool live = false;

	//The non-central block rotates around the central one
	public bool central;

	//how long it's been since it fell last
	public float lastProgress;
	//how fast it falls in seconds per space
	public float speed = 1;

	//diameter of block 
	//used to see how far 1 space to the right is.
	public float step;

	//What spawned it
	public SpawningProtocol spawningProtocol;

	//the block it is attached to
	public GameObject partner;

	//where it currently is
	public int pos;

	//whether it is controlled by the player
	public bool p1;

	//variables for AI
	public float lastAIReact;
	public string[] possibleMoves = new string[] {"left", "right", "turn", "down"};

	//used for animations
	public Texture[] frames;
	public int framesPerSecond = 10;

	//r: red
	//b: blue
	//g: green
	//y: yellow
	//p: purple
	//n: black(useless)
	public string color;

	//0: bottom
	//1: right
	//2: top
	//3: left
	public int side;

	//Called upon initialization
	void Start () {
		lastAIReact = 0;
		lastProgress = Time.time;
		if(spawningProtocol != null)
			p1 = spawningProtocol.p1;
	}

	// Update is called once per frame
	void Update () {
		if (!live && partner)
						partner.GetComponent<BasicBehaviour> ().live = false;
		//reads button press if still live
		if (live) {
			//drops the block down if it's time to
			if(Time.time-lastProgress >= speed){
				progress();
			}
			if(Input.GetButtonDown("Fire2") && p1){
				InputMoves("right");
			}
			
			else if(Input.GetButtonDown("Fire1")&& p1){

				InputMoves( "left");
			}
			else if(Input.GetButtonDown("Fire3")&& p1){

				InputMoves("turn");
			}
			else if(Input.GetButtonDown("Jump") && p1){
				InputMoves ("down");
			}

			else if(!p1 && !(spawningProtocol.multi)){
				//INSERT AI HERE IF EVER
				//For now set to mimic player exactly

				if(Time.time-lastAIReact >= speed/2){
					if(spawningProtocol.gameObject.GetComponent<AI>().move == 4){
						if((central && pos == 8) || (!central && pos == 14)){
							spawningProtocol.gameObject.GetComponent<AI>().exec = true;
						}
						return;
					}

					InputMoves(possibleMoves[spawningProtocol.gameObject.GetComponent<AI>().move]); 
					//partner.GetComponent<BasicBehaviour>().InputMoves(possibleMoves[spawningProtocol.gameObject.GetComponent<AI>().move]);
					lastAIReact = Time.time;
					spawningProtocol.gameObject.GetComponent<AI>().exec = true;
				}

			}
		}
	}

	//executes the player's button press
	public void InputMoves(string button){
		if(button.Equals("right")
		   && pos%6 <5

		   && spawningProtocol.spaces[pos+1]==null
		   &&!(side == 3 && pos % 6 ==4)
		   &&!(side == 3 && spawningProtocol.spaces[pos+2]!=null)
		   &&!(side == 2 && spawningProtocol.spaces[pos+7]!=null)){
			
			Vector3 newpos = transform.position;
			newpos.x += 1.0f*step; 
			transform.position = newpos;
			pos++;
		}
		
		else if(button.Equals("left")
		        && pos % 6 > 0 
		        && spawningProtocol.spaces[pos-1]==null
		        &&!(side == 1 && pos % 6 ==1)
		        &&!(side == 1 && spawningProtocol.spaces[pos-2]!=null)
		        &&!(side == 2 && spawningProtocol.spaces[pos+5]!=null)){
			
			Vector3 newpos = transform.position;
			newpos.x -= 1.0f*step; 
			transform.position = newpos;
			pos--;
		}
		else if(button.Equals("turn")){
			
			if(!(side!=3 && pos%6 == 0)
			   &&!(side!=1 && pos % 6 == 5)
			   &&!(side!=0 && pos >= 42)
			   && rotateOpen()){
				if(!central)
					reOrient();
				else
					side = (side+1)%4;
			}
		}
		else if(button.Equals("down")){
			spawningProtocol.spaces[pos] = this;
			spawningProtocol.spaces[partner.GetComponent<BasicBehaviour>().pos] = partner.GetComponent<BasicBehaviour>();
			live = false;
			side = 10;
			partner.GetComponent<BasicBehaviour>().live = false;
			logic ();
			spawningProtocol.fallIn();
			spawningProtocol.clearBlocks();
			if(central){
			if(!spawningProtocol.multi){
				spawningProtocol.spawnNext();
			}
			else
			{
				spawningProtocol.gameObject.networkView.RPC("spawnNext",RPCMode.All);
			}
			}
		}


		}


	//rotates the block
	void reOrient(){
				Vector3 newpos = transform.position;
		switch (side) {
		case 0:pos -= 5;break;
		case 1:pos -= 7;break;
		case 2:pos += 5;break;
		case 3:pos += 7;break;
				}

		newpos.x += (((side-1.5f)*(side-1.5f))-1.25f)*step;
		side = (side + 1) % 4;
		newpos.y -=(((side-1.5f)*(side-1.5f))-1.25f)*step;
		transform.position = newpos;
		}

	//drops the block down one, performs necessary checks
	//if at the bottom, calls for a new block to be spawned
	public void progress(){
		if(live &&(pos>=42 || partner.GetComponent<BasicBehaviour>().pos>=42
		   || spawningProtocol.spaces[pos+6] != null 
		   || spawningProtocol.spaces[partner.GetComponent<BasicBehaviour>().pos+6] !=null)){

			spawningProtocol.spaces[pos] = this;
			spawningProtocol.spaces[partner.GetComponent<BasicBehaviour>().pos] = partner.GetComponent<BasicBehaviour>();
			live = false;
			side = 10;
			partner.GetComponent<BasicBehaviour>().live = false;
			logic ();
			spawningProtocol.fallIn();
			spawningProtocol.clearBlocks();
			if(!spawningProtocol.multi){
				spawningProtocol.spawnNext();
			}
			else
			{
				spawningProtocol.gameObject.networkView.RPC("spawnNext",RPCMode.All);
			}
		} 
		else {
			Vector3 newpos = transform.position;
			newpos.y -= 1.0f * step; 
			transform.position = newpos;
			pos+=6;
			lastProgress = Time.time;
				}
		}

	//resolves where the blocks fall after they hit the bottom
	public void logic(){
		while(spawningProtocol.spaces[pos+6] == null && pos<42){
			Vector3 newpos = transform.position;
			newpos.y -= 1.0f * step; 
			transform.position = newpos;
			spawningProtocol.spaces[pos]=null;
			pos+=6;
			spawningProtocol.spaces[pos] = this;
		}

		}

	//returns true if the block can rotate
	bool rotateOpen(){
				switch (side) {
				case 0:
			if(pos<5) 
				return false;
						return spawningProtocol.spaces[pos - 5] == null;
				case 1:
			if(pos<7)
				return false;
						return spawningProtocol.spaces [pos - 7] == null;
				case 2:
						return spawningProtocol.spaces [pos + 5] == null;
				case 3:
						return spawningProtocol.spaces [pos + 7] == null;
				}
		return false;
		}

	//destroys the block
	public void kill()
	{
		if (spawningProtocol && spawningProtocol.multi)
						Network.Destroy (gameObject.GetComponent<NetworkView> ().viewID);
				else
						Destroy (gameObject);
		animate ();
		}

	void animate() {
		int index = (int) ((Time.time * framesPerSecond) % frames.Length);
		renderer.material.mainTexture = frames[index];
	}
}
