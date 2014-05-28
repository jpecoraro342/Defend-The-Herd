using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	public GameObject wolf;
	public GUIText waveText;
	public GUIText coinText; 
	
	private int waveNumber;
	private float maxWait;
	private bool isDuringWave;
	
	private int wolvesToSpawn;
	
	private GameObject farmer;
	private Farmer_Movement farmerMovement;
	// members for shop screen
	public GUIContent shopImage;
	public GUISkin mySkin; 
	public GUIContent nextWave;
	public GUIContent stoneWall; 
	public GUIContent laserWall; 
	public GUIContent mace; 
	public GUIContent HALOSword; 
	public GUIContent sword;  

	private int killCounter;

	GameObject[] fences;
	Fence_AI[] fenceAI;
	
	void Start() {
		killCounter = 0;
		fences = GameObject.FindGameObjectsWithTag ("fence");
		fenceAI = new Fence_AI[fences.Length];
		for (int i = 0; i < fences.Length; i ++) {
			fenceAI[i] = (Fence_AI) fences[i].GetComponent(typeof(Fence_AI));
		}
		farmer = GameObject.FindGameObjectWithTag ("farmer");
		farmerMovement = (Farmer_Movement) farmer.GetComponent (typeof(Farmer_Movement));
		isDuringWave = true;
		waveNumber = 1;
		maxWait = 3f;
		wolvesToSpawn = 2 + waveNumber*4;
		waveText.text = "Wave: " + waveNumber;
		coinText.text = "Coins: " + farmerMovement.getMoney (); 
		StartCoroutine (SpawnWolves());
		StartCoroutine (CheckEndOfWave ());
	}
	
	void theShopMenu() {
		
		GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height)); 
		
		GUI.Box (new Rect(0, 0, Screen.width, Screen.height), shopImage); 
		
		GUI.Label (new Rect (Screen.width /1.2f, 30f, 150, 50), "Coins: " + farmerMovement.getMoney ()); 
		GUI.Label (new Rect (Screen.width / 2f + 15, 30f, 150, 50), "   = 5 Coins"); 
		
		if (GUI.Button (new Rect (25, Screen.height/1.2f, 158, 46), nextWave)) { 
			incrementWave (); 
		}
		if (GUI.Button (new Rect (40 , Screen.height/1.55f, 150, 100), stoneWall)) { 
			if(farmerMovement.getMoney() >= 10) {
				upgradeFences(1);
				farmerMovement.subtractCoins(10);
			}
		}
		if (GUI.Button (new Rect (Screen.width / 3.37f, Screen.height/1.6f, 150, 100), laserWall)) { 
			if(farmerMovement.getMoney() >= 15) {
				upgradeFences(2);
				farmerMovement.subtractCoins(15);
			}
		}
		if (GUI.Button (new Rect (Screen.width / 1.935f, Screen.height/1.777f, 150, 100), mace)) { 
			if(farmerMovement.getMoney() >= 5) {
				if (farmerMovement.setWeapon(1))
					farmerMovement.subtractCoins(5);
			}
		}
		if (GUI.Button (new Rect (Screen.width / 1.52f, Screen.height/1.6f, 150, 100), HALOSword)) { 
			if(farmerMovement.getMoney() >= 15) {
				if (farmerMovement.setWeapon(3))
					farmerMovement.subtractCoins(15);
			}
		}
		if (GUI.Button (new Rect (Screen.width / 1.252f, Screen.height/1.777f, 150, 100), sword)) { 
			if(farmerMovement.getMoney() >= 10) {
				if (farmerMovement.setWeapon(2))
					farmerMovement.subtractCoins(10);
			}
		}
		GUI.EndGroup(); 
		
	}
	
	void OnGUI() {
		if (!isDuringWave) {
			farmerMovement.setCanMove(false);
			GUI.skin = mySkin; 
			
			theShopMenu (); 
		}
	}
	
	void Update () {
		coinText.text = "Coins: " + farmerMovement.getMoney (); 
	}

	void incrementWave() {
		waveNumber ++;
		waveText.text = "Wave: " + waveNumber;
		wolvesToSpawn = 2 + waveNumber*4;
		isDuringWave = true;
		if (waveNumber > 5) {
			maxWait = 5 * 3f/waveNumber;
		}
		farmerMovement.setCanMove (true);
		farmerMovement.resetWolvesDestroyed ();
		updateFences ();
		StartCoroutine(SpawnWolves());
		StartCoroutine (CheckEndOfWave());
	}

	void updateFences() {
		for (int i = 0; i < fenceAI.Length; i++) {
			fenceAI[i].newWave();
		}
	}

	void upgradeFences(int upgradeLevel) {
		for (int i = 0; i < fenceAI.Length; i++) {
			fenceAI[i].upgradeFence(upgradeLevel);
		}
	}

	public void incrementKillCounter() {
		killCounter ++;
		if (killCounter >= 3) 
			StartCoroutine ("GG");
	}

	IEnumerator GG() {
		yield return new WaitForSeconds (2f);
		Application.LoadLevel("GG");
	}
	
	IEnumerator SpawnWolves() {
		yield return new WaitForSeconds (2);
		for (int i = 0; i < wolvesToSpawn; i++) {
			Vector3 spawnLocation = new Vector3(10, Random.Range(-4f,4f), 0);
			Instantiate(wolf, spawnLocation, Quaternion.identity);
			yield return new WaitForSeconds(Random.Range(.2f,maxWait));
		}
	}

	IEnumerator CheckEndOfWave() {
		while (isDuringWave) {
			if (farmerMovement.getWolvesDestroyed () == wolvesToSpawn) {
				yield return new WaitForSeconds(2f);
				isDuringWave = false;
				yield break;
			}
			yield return null;
		}
	}
}
