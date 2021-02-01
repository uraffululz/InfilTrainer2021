using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

	[SerializeField] GameObject sceneManager;

	Rigidbody rb;

	enum moveStates {disabled, standing, crouching}
	moveStates myMoveState;

	Vector3 groundDir;
	float gravityRotationSpeed = 10;

	float speed = 5;

	float rotSpeed = 1f;
	bool transitioningSurface = false;

	//Vector3 floorRayAngleOffset;


    void Awake() {
		sceneManager = GameObject.FindGameObjectWithTag("SceneManager");
        rb = GetComponent<Rigidbody>();

		//floorRayAngleOffset = new Vector3(0, 0, 30);

		myMoveState = moveStates.standing;
    }


    void Update() {
		if (myMoveState != moveStates.disabled) {
			Move();
			Rotate();
			ChangeStance();
		}

		Vector3 setGroundDir = FloorAngleCheck();
		groundDir = Vector3.Lerp(groundDir, setGroundDir, gravityRotationSpeed * Time.deltaTime);

		if (transitioningSurface) {
			RotateSelf(setGroundDir, Time.deltaTime, gravityRotationSpeed);
		}

		//Apply "downward" force to keep player attached to the current "walkable" surface
		rb.AddForce((transform.up * (rb.mass * Physics.gravity.y * 10))/* + (-Vector3.up * (rb.mass * Physics.gravity.y))*/);
	}


	void LateUpdate () {
		
	}


	void Move () {
		//float moveH = Input.GetAxisRaw("Horizontal");
		float moveV = Input.GetAxisRaw("Vertical");

		//Vector3 movement = new Vector3(0, 0, transform.localRotation.z + moveV);

		rb.velocity = transform.forward * (moveV * speed);
	}


	void Rotate() {
		float rotH = Input.GetAxisRaw("Horizontal");

		transform.Rotate(Vector3.up * rotH * rotSpeed, Space.Self);
	}


	void ChangeStance() {
		if (Input.GetKeyDown(KeyCode.LeftShift)) {
			switch (myMoveState) {
				case moveStates.standing:
					transform.localScale = new Vector3(1, .5f, 1);
					myMoveState = moveStates.crouching;
					break;
				case moveStates.crouching:
					transform.localScale = Vector3.one;
					myMoveState = moveStates.standing;
					break;
				default:
					break;
			}
		}
	}


	Vector3 FloorAngleCheck() {
		RaycastHit forwardHit;
		Ray floorCheckRay = new Ray(transform.position, transform.forward + (-transform.up/1.2f));

		RaycastHit backHit;
		Ray backCheckRay = new Ray(transform.position, -transform.forward + (-transform.up / 1.2f));

		RaycastHit downHit;
		Ray downCheckRay = new Ray(transform.position, -transform.up);

		//RaycastHit hitRay = new RaycastHit();
		Vector3 hitDir = transform.up;

		if (Physics.Raycast(backCheckRay, out backHit, 5f)) {
			//hitRay = backHit;
			hitDir = backHit.normal;
			//StartCoroutine(WallTransition());
			//return hitDir.normalized;
		}

		if (Physics.Raycast(downCheckRay, out downHit, 5f)) {
			//hitRay = downHit;
			hitDir = downHit.normal;
		}

		if (Physics.Raycast(floorCheckRay, out forwardHit, 5f)) {
			//hitRay = forwardHit;

			//if (hitRay.collider != null) {
				if (forwardHit.collider.CompareTag("ClimbingSurface") || forwardHit.collider.CompareTag("Floor")) {
					hitDir = forwardHit.normal;

					//sceneManager.GetComponent<SceneUIManager>().UpdateActionText("Press [C] to Change surface");
					//if (Input.GetKeyDown(KeyCode.C)) {
					//	//Climb onto forward surface
					//	//StartCoroutine(WallTransition());
					//	print("Climbing onto new surface");
					//}
				}
			//}
			else {
				sceneManager.GetComponent<SceneUIManager>().UpdateActionText("");
			}
		}

		StartCoroutine(WallTransition());
		
		Debug.DrawRay(transform.position, -transform.forward + (-transform.up/1.2f), Color.red);

		return hitDir.normalized;
	}


	void RotateSelf(Vector3 direction, float d, float gravitySpeed) {
		Vector3 lerpDir = Vector3.Lerp(transform.up, direction, gravitySpeed * d);
		transform.rotation = Quaternion.FromToRotation(transform.up, lerpDir) * transform.rotation;
	}


	IEnumerator WallTransition() {
		transitioningSurface = true;

		yield return new WaitForSeconds(.35f);
		transitioningSurface = false;
	}
}
