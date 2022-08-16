using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Farmer_Movement : MonoBehaviour {

	public float speed = 5f;

	private Animator animator;
	private Vector3 movePosition;
	private Vector3 previousPosition;
	private Vector3 beforeCollision;
	private bool canMove;
	private bool canKillWolf;

	private int wolvesDestroyed;

	private int money;

	private bool[] weapons;

	Dictionary<int, GameObject> wolvesToAttack;
	
	void Start() {
		weapons = new bool[] {false, false, false };
		wolvesDestroyed = 0;
		canMove = true;
		animator = this.GetComponent<Animator> ();
		movePosition = gameObject.transform.position;
		canKillWolf = true;
		wolvesToAttack = new Dictionary<int, GameObject> ();
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
			wolvesToAttack.Add(other.gameObject.GetInstanceID(), other.gameObject);
			if (canKillWolf) {
				StartCoroutine(destroyWolf());
			}
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		wolvesToAttack.Remove (other.gameObject.GetInstanceID ());
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

	IEnumerator destroyWolf() {
		canKillWolf = false;
		animator.SetBool("walking",true);
		var iterator = wolvesToAttack.GetEnumerator ();
		iterator.MoveNext ();
		GameObject dyingWolf = iterator.Current.Value;
		wolvesToAttack.Remove (dyingWolf.GetInstanceID ());
		Wolf_AI wolf = (Wolf_AI) dyingWolf.GetComponent(typeof(Wolf_AI));
		wolf.killWolf ();
		yield return new WaitForSeconds (.5f);
		Destroy(dyingWolf);
		canKillWolf = true;
		wolvesDestroyed++;
		money = money + randomizeReward (); 
		if (wolvesToAttack.Count != 0) {
			StartCoroutine(destroyWolf());
		}
		else {
			animator.SetBool("walking",false);
		}
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
