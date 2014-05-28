using UnityEngine;
using System.Collections;

public class Wolf_AI : MonoBehaviour {
	public float speed = 5f;
	private bool beginningMovement;
	private bool goatAttackMode;

	private int goatAttackingIndex;

	private Animator animator;

	private GameObject goat;

	GameObject[] goats;

	void Start() {
		goatAttackMode = false;
		animator = this.GetComponent<Animator>();
		beginningMovement = true;
	}

	void Update () {
		if (beginningMovement) {
			transform.position = Vector3.MoveTowards (transform.position, new Vector3 (-15, transform.position.y, 0), Time.deltaTime * speed);
			if (transform.position.x < 1.5) {
				beginningMovement = false;
				goatAttackMode = true;
				findGoat();
			}
		}
		else if (goatAttackMode) {
			if (goat == null) {
				if (!findGoat()) {
					return;
				}
			}
			transform.position = Vector3.MoveTowards (transform.position, goat.transform.position, Time.deltaTime * speed);
		}
	}

	bool findGoat() {
		goats = GameObject.FindGameObjectsWithTag ("goat");
		if (goats.Length == 0)
			return false;
		goatAttackingIndex = Random.Range(0, goats.Length);
		goat = goats[goatAttackingIndex];
		return true;
	}

	void OnTriggerEnter2D(Collider2D other) {
		string tag = other.tag;
		if (tag.Equals("fence")) {
			goatAttackMode = false;
			beginningMovement = false;
			animator.SetBool("attacking", true);
		}
		else if (tag.Equals("goat")) {
			goatAttackMode = false;
			beginningMovement = false;
			animator.SetBool("attacking", true);
		}
	}

	void OnTriggerStay2D(Collider2D other) {

	}

	void OnTriggerExit2D(Collider2D other) {
		animator.SetBool ("attacking", false);
		if (transform.position.x < 1.5) {
			goatAttackMode = true;
			findGoat();
		}
		else {
			beginningMovement = true;
		}
	}

	public void killWolf() {
		beginningMovement = false;
		goatAttackMode = false;
		animator.SetBool ("dead", true);
	}
}
