using UnityEngine;
using System.Collections;
using Com.LuisPedroFonseca.ProCamera2D;

public class TargetCarIndicator : MonoBehaviour {
	CarController m_targetCar;
	public CarController TargetCar {
		get { return m_targetCar; }
		set { 
			Debug.Log ("Setting target car to " + (value == null ? "<null>" : value.name));
			m_targetCar = value;
		}
	}

    SpriteRenderer sprite;
	float scaleAugment = 0f;

    public static TargetCarIndicator Instance = null;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            sprite = GetComponent<SpriteRenderer>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

	// Update is called once per frame
	void LateUpdate () {
		if (TargetCar == null) {
			return;
		}
		Camera mainCamera = ProCamera2D.Instance.GameCamera;
		float cameraWidth = ProCamera2D.Instance.GameCameraSize * mainCamera.aspect;
     	float cameraHeight = ProCamera2D.Instance.GameCameraSize;

		Bounds cameraBounds = new Bounds(mainCamera.transform.position, new Vector3(cameraWidth, cameraHeight, 1f));

		if (GameManager.Instance.Player.State != PlayerState.OnCar) {
			sprite.enabled = false;
		}
		else if (!TargetCar.GetComponent<SpriteRenderer>().bounds.Intersects(cameraBounds))
        {
			sprite.enabled = true;
			if (sprite.sortingLayerName != "Player") {
				sprite.sortingLayerName = "Player";
				sprite.sortingOrder = 0;
			}

			float cameraMaxX = mainCamera.transform.position.x + cameraWidth - sprite.sprite.bounds.extents.x * transform.localScale.x;
			float cameraMinX = mainCamera.transform.position.x - cameraWidth + sprite.sprite.bounds.extents.x * transform.localScale.x;
			float cameraMaxY = mainCamera.transform.position.y + cameraHeight - sprite.sprite.bounds.extents.y * transform.localScale.y;
			float cameraMinY = mainCamera.transform.position.y - cameraHeight + sprite.sprite.bounds.extents.y * transform.localScale.y;
			transform.position = new Vector2(Mathf.Clamp(TargetCar.transform.position.x, cameraMinX, cameraMaxX), Mathf.Clamp(TargetCar.transform.position.y, cameraMinY, cameraMaxY));

			Vector2 direction = (TargetCar.transform.position - Camera.main.transform.position);
			float angle = Vector2.Angle(transform.right, direction);
			Vector3 cross = Vector3.Cross((Vector3)transform.right, (Vector3)direction);
			if (cross.z < 0f) {
				angle *= -1f;
			}
			
			Quaternion newRotation = transform.rotation;
			newRotation.eulerAngles = new Vector3(0f, 0f, transform.rotation.eulerAngles.z + angle);
			transform.rotation = newRotation;

			Vector2 minScale = new Vector2(1f, 1f);
			scaleAugment = (TargetCar.transform.position - mainCamera.transform.position).magnitude / 9f;
			Vector2 scale = new Vector2 (scaleAugment, scaleAugment);
			transform.localScale = minScale + scale;
        }
        else
        {
			sprite.enabled = true;
			if (sprite.sortingLayerName != "Cars") {
				sprite.sortingLayerName = "Cars";
				sprite.sortingOrder = -2;
			}

			transform.position = TargetCar.transform.position;
			transform.rotation = TargetCar.transform.rotation;
			transform.localScale = new Vector2(1f, 1f);
        }
	}
}
