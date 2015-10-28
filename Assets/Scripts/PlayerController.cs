using UnityEngine;
using System.Collections;

public enum PlayerState {
	OnGround,
	OnCar,
	Jumping
};

public class PlayerController : MonoBehaviour {
	public static float minDeathSpeed = 1f;
	public float walkSpeed = 1f;
	public float jumpDistance = 0.3f;
	public float fromCarJumpDuration = 0.2f;
	public float fromGroundJumpDuration = 0.5f;
	public float jumpCooldown = 0.2f;
	public AudioClip walkSound;
	public AudioClip jumpSound;
	public AudioClip landOnCarSound;
	AudioSource audioSource;
	Animator animator;

	bool isMoving = false;
	float jumpCooldownRemaining = 0f;
	float activeJumpDuration = 0f;
	float jumpRemaining = 0f;
	float distanceToJump = 0f;
	CarController lastCar = null;

	PlayerState m_state = PlayerState.OnGround;
	public PlayerState State {
		get { return m_state; }
		set {
			PlayerState oldState = m_state;
			m_state = value;

			if (m_state == PlayerState.Jumping) {
				GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 1f);

				if (oldState == PlayerState.OnGround) {
					audioSource.Stop();
					activeJumpDuration = fromGroundJumpDuration;
					jumpRemaining = activeJumpDuration;

					distanceToJump = isMoving ? jumpDistance : 0f;
				}
				else if (oldState == PlayerState.OnCar) {
					activeJumpDuration = fromCarJumpDuration;
					jumpRemaining = activeJumpDuration;
					distanceToJump = jumpDistance;

					lastCar = transform.parent.GetComponent<CarController>();
					transform.SetParent(null, true);
				}

				audioSource.PlayOneShot(jumpSound);
			}
			else if (m_state == PlayerState.OnGround) {
				GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
				lastCar = null;

				if (oldState == PlayerState.Jumping) {
					jumpRemaining = 0f;
					distanceToJump = 0f;
					jumpCooldownRemaining = jumpCooldown;
				}
			}
			else if (m_state == PlayerState.OnCar) {
				GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0f);
				lastCar = null;

				if (oldState == PlayerState.Jumping) {
					jumpRemaining = 0f;
					distanceToJump = 0f;
					jumpCooldownRemaining = jumpCooldown;

					ScoreManager.Instance.Score += 1;
				}

				audioSource.PlayOneShot(landOnCarSound);
			}
		}
	}

	void Awake() {
		audioSource = GetComponent<AudioSource>();
		audioSource.clip = walkSound;

		animator = GetComponent<Animator>();
	}

	// Use this for initialization
	void Start () {
		State = PlayerState.OnGround;
	}
	
	// Update is called once per frame
	void Update () {
		if (State == PlayerState.OnGround) {
			isMoving = false;

			if (jumpCooldownRemaining > 0f) {
				jumpCooldownRemaining -= Time.deltaTime;
			}

			Quaternion rotation = transform.rotation;
			Vector2 offset = Vector2.zero;
			float rotationAngle = rotation.eulerAngles.z;
			bool movedLeftOrRight = false;

			if (Input.GetKey(KeyCode.LeftArrow)) {
				offset += new Vector2(-walkSpeed * Time.deltaTime, 0f);
				rotationAngle = 90f;
				movedLeftOrRight = true;
			}
			else if (Input.GetKey(KeyCode.RightArrow)) {
				offset += new Vector2(walkSpeed * Time.deltaTime, 0f);
				rotationAngle = -90f;
				movedLeftOrRight = true;
			}

			if (Input.GetKey(KeyCode.UpArrow)) {
				offset += new Vector2(0f, walkSpeed * Time.deltaTime);

				if (movedLeftOrRight) {
					rotationAngle = rotationAngle / 2f;
				}
				else {
					rotationAngle = 0f;
				}
			}
			else if (Input.GetKey(KeyCode.DownArrow)) {
				offset += new Vector2(0f, -walkSpeed * Time.deltaTime);

				if (movedLeftOrRight) {
					rotationAngle = 180f - rotationAngle / 2f;
				}
				else {
					rotationAngle = 180f;
				}
			}

			if (offset.magnitude > float.Epsilon) {
				if (!audioSource.isPlaying) {
					audioSource.Play();
				}
			}
			else {
				if (audioSource.isPlaying) {
					audioSource.Stop();
				}
			}

			if (offset.magnitude > 0.00001f) {
				isMoving = true;
			}

			rotation.eulerAngles = new Vector3(0, 0, rotationAngle);
			transform.rotation = rotation;
			if (Input.GetKeyDown(KeyCode.Space) && jumpCooldownRemaining <= 0f) {
				State = PlayerState.Jumping;
			}
			else {
				transform.position += (Vector3)offset;
				if (isMoving) {
					TriggerWalkingAnimation("Update");
				}
				else {
					TriggerIdleAnimation("Update");
				}
			}
		}
		else if (State == PlayerState.Jumping) {
			Vector2 positionChange = transform.up * Time.deltaTime * distanceToJump / activeJumpDuration;
			transform.position += (Vector3)positionChange;

			jumpRemaining -= Time.deltaTime;
			if (jumpRemaining <= 0f) {
				Debug.Log ("Jump Done");
				State = PlayerState.OnGround;
			}
		}
		else if (State == PlayerState.OnCar) {
			Quaternion rotation = transform.rotation;
			if (Input.GetKey(KeyCode.LeftArrow)) {
				rotation.eulerAngles = new Vector3(0, 0, 90.0f);
			}
			else if (Input.GetKey(KeyCode.RightArrow)) {
				rotation.eulerAngles = new Vector3(0, 0, -90.0f);
			}
			else if (Input.GetKey(KeyCode.UpArrow)) {
				rotation.eulerAngles = new Vector3(0, 0, 0.0f);
			}
			else if (Input.GetKey(KeyCode.DownArrow)) {
				rotation.eulerAngles = new Vector3(0, 0, 180.0f);
			}
			transform.rotation = rotation;

			if (jumpCooldownRemaining > 0f) {
				jumpCooldownRemaining -= Time.deltaTime;
			}
			if (Input.GetKey (KeyCode.Space) && jumpCooldownRemaining <= 0f) {
				State = PlayerState.Jumping;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D col) {
		CarController car = col.GetComponent<CarController>();
		if (car != null && car != lastCar) {
			if (State == PlayerState.OnGround && car.CurrentSpeed >= PlayerController.minDeathSpeed ) {
				GameManager.Instance.GameOver();
			}
			else if (State == PlayerState.Jumping) {
				State = PlayerState.OnCar;
				car.Colourize();
				this.transform.SetParent(car.transform);
				this.transform.position = car.transform.position;
			}
		}
	}

	public void ResetPlayer(Transform spawnLocation) {
		transform.SetParent(null);
		transform.position = spawnLocation.position;
		transform.rotation = spawnLocation.rotation;
		State = PlayerState.OnGround;
	}

	void TriggerJumpAnimation(string source) {
		animator.SetTrigger("Jumping");
	}

	void TriggerIdleAnimation(string source) {
		animator.SetTrigger("Idle");
	}

	void TriggerWalkingAnimation(string source) {
		animator.SetTrigger("Walking");
	}
}
