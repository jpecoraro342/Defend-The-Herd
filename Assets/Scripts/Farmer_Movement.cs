using UnityEngine;
using System.Collections;

public class Farmer_Movement : MonoBehaviour {

	public float speed = 5f;

	private Animator animator;
	private Vector3 movePosition;
	private Vector3 previousPosition;
	private Vector3 beforeCollision;
	private bool canMove;
	private bool attacking = false;

	private int wolvesDestroyed;

	private int money;

	private bool[] weapons;
	
	void Start() {
		weapons = new bool[] {false, false, false };
		wolvesDestroyed = 0;
		canMove = true;
		animator = this.GetComponent<Animator> ();
		movePosition = gameObject.transform.position;
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
		attacking = true;
		if (other.tag.Equals("wolf")) {
			animator.SetBool("walking",true);
			StartCoroutine(destroyWolf(other));
		}
	}

	void OnTriggeStay2D(Collider2D other) {
		if (other.tag.Equals("wolf")) {
			animator.SetBool("walking",true);
		}
	}
	
	IEnumerator moveToLocation(Vector3 targetLocation) {
		while (transform.position != targetLocation) {
			previousPosition = transform.position;
			transform.position = Vector3.MoveTowards (transform.position, movePosition, Time.deltaTime * speed);
			yield return null;
		}
		animator.SetBool ("walking", false);
	}

	IEnumerator destroyWolf(Collider2D other) {
		Wolf_AI wolf = (Wolf_AI) other.gameObject.GetComponent(typeof(Wolf_AI));
		wolf.killWolf ();
		yield return new WaitForSeconds (.5f);
		Destroy(other.gameObject);
		animator.SetBool("walking",false);
		attacking = false;
		wolvesDestroyed++;
		money = money + randomizeReward (); 
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
