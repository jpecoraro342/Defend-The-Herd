using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Farmer_Movement : MonoBehaviour {

	public float speed = 5f;

	public float damageMultiplier = 1;

	private Animator animator;
	private Vector3 movePosition;
	private Vector3 previousPosition;
	private Vector3 beforeCollision;
	private bool canMove;
	private bool canKillWolf;
	private int wolvesDestroyed;
	private int money;
	private bool[] weapons;
	
	Queue<GameObject> wolvesToAttack;
	
	void Start() {
		weapons = new bool[] {false, false, false };
		wolvesDestroyed = 0;
		canMove = true;
		animator = this.GetComponent<Animator> ();
		movePosition = gameObject.transform.position;
		canKillWolf = true;
		wolvesToAttack = new Queue<GameObject> ();
	}
	
	void Update () {
		if (canMove && (Input.GetMouseButtonDown (0) || Input.GetMouseButton(0))) {
			movePosition = Input.mousePosition;
			movePosition = Camera.main.ScreenToWorldPoint(movePosition);
			movePosition.z = transform.position.z;
			animator.SetBool("walking",true);
			StopCoroutine("moveToLocation");
			StartCoroutine("moveToLocation", movePosition);
		}
	}
	
	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag.Equals("wolf")) {
			animator.SetBool("walking",true);
			Wolf_AI wolf = (Wolf_AI) other.GetComponent(typeof(Wolf_AI));
			wolf.setInFarmerRange(true);
			wolvesToAttack.Enqueue(other.gameObject);
			Debug.Log("Wolf Queued: " + wolvesToAttack.Count);
			if (canKillWolf) {
				StartCoroutine(attackWolf());
			} 
		}
	}
	
	void OnTriggerExit2D(Collider2D other) {
		Wolf_AI wolf = (Wolf_AI) other.GetComponent(typeof(Wolf_AI));
		wolf.setInFarmerRange(false);
	}

	IEnumerator moveToLocation(Vector3 targetLocation) {
		while (transform.position != targetLocation) {
			previousPosition = transform.position;
			transform.position = Vector3.MoveTowards (transform.position, movePosition, Time.deltaTime * speed);
			yield return null;
		}
		//if we cant kill a wolf, we are attacking and shouldnt stop
		if (canKillWolf) {
			animator.SetBool ("walking", false);
		}
	}

	IEnumerator attackWolf() {
		canKillWolf = false;
		GameObject dyingWolf = wolvesToAttack.Dequeue ();
		if (dyingWolf == null) {
			Debug.Log("Loaded a previously dead wolf");
			if (wolvesToAttack.Count != 0) {
				StartCoroutine(attackWolf());
				yield break;
			}
			else {
				canKillWolf = true;
				yield break;
			}
		}
		animator.SetBool("walking",true);
		Debug.Log ("Wolf dequeued for attacking");
		Debug.Log (wolvesToAttack.Count);
		Wolf_AI wolf = (Wolf_AI) dyingWolf.GetComponent(typeof(Wolf_AI));
		if (!wolf.getIsDying()) {
			wolf.changeState();
		}
		while (!wolf.getIsDead()) {
			if (wolf.isInFarmerRange()) {
				wolf.dealDamage(damageMultiplier*Time.deltaTime);
				yield return null;
			}
			else {
				Debug.Log("wolf exited attack range");
				Debug.Log (wolvesToAttack.Count);
				if (wolvesToAttack.Count != 0) {
					Debug.Log("still wolves in the queue, continue attacking");
					StartCoroutine(attackWolf());
				}
				canKillWolf = true;
				yield break;
			}
		}
		Debug.Log ("attacking wolf died");
		animator.SetBool("walking", false);
		canKillWolf = true;
		wolvesDestroyed++;
		money = money + randomizeReward (); 
		StartCoroutine(destroyWolf(dyingWolf));
	}

	IEnumerator destroyWolf(GameObject wolfToKill) {
		Wolf_AI wolf = (Wolf_AI) wolfToKill.GetComponent(typeof(Wolf_AI));
		wolf.killWolf ();
		yield return new WaitForSeconds (.5f);
		Destroy (wolfToKill);
	}

	void OnCollisionEnter2D(Collision2D other) {
		//if game tag is fence
		StopCoroutine ("moveToLocation");
		animator.SetBool ("walking", false);
		beforeCollision = previousPosition;
		movePosition = beforeCollision;
		transform.position = previousPosition;
		gameObject.transform.rotation = Quaternion.identity;
	}

	void OnCollisionStay2D(Collision2D other) {
		StopCoroutine ("moveToLocation");
		animator.SetBool ("walking", false);
		movePosition = beforeCollision;
		transform.position = beforeCollision;
		gameObject.transform.rotation = Quaternion.identity;
	}

	void OnCollisionExit2D(Collision2D other) {
		gameObject.transform.rotation = Quaternion.identity;
		//if game tag is fence
		movePosition = previousPosition;
		transform.position = previousPosition;
	}

	public void setCanMove(bool canMove) {
		this.canMove = canMove;
	}

	public int getWolvesDestroyed() {
		return wolvesDestroyed;
	}

	public int getMoney() {
		return money; 
	}
	
	public void resetWolvesDestroyed() {
		wolvesDestroyed = 0;
	}
	
	public int randomizeReward() {
		int chance = Random.Range (0, 2);
		if(chance>0) {
			return 0;
		}
		else
			return 1;
	}

	public bool setWeapon(int i) {
		animator.SetInteger ("farmerType", i);
		if (!weapons[i-1]) {
			weapons [i - 1] = true;
			return true;
		}
		return false;
	}

	public void subtractCoins(int i) {
		money -= i;
	}
}
