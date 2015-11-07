using UnityEngine;
using System.Collections;

public class TargetCarIndicator : MonoBehaviour {
    public CarController targetCar;
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

	void Update() {
		scaleAugment += Time.deltaTime;
		while (scaleAugment > 360f) {
			scaleAugment -= 360f;
		}
		Vector2 minScale = new Vector2(1f, 1f);
		Vector2 scale = new Vector2 (Mathf.Sin(scaleAugment) + 1f, Mathf.Sin(scaleAugment) + 1f) * 0.5f;
		transform.localScale = minScale + scale;
	}

	// Update is called once per frame
	void LateUpdate () {
		if (targetCar == null) {
			return;
		}

	    if (!targetCar.GetComponent<SpriteRenderer>().isVisible)
        {
			if (sprite.sortingLayerName != "Player") {
				sprite.sortingLayerName = "Player";
				sprite.sortingOrder = 0;
			}

			float cameraWidth = Camera.main.orthographicSize * Camera.main.aspect;
			float cameraHeight = Camera.main.orthographicSize;

			float cameraMaxX = Camera.main.transform.position.x + cameraWidth - sprite.sprite.bounds.extents.x * transform.localScale.x;
			float cameraMinX = Camera.main.transform.position.x - cameraWidth + sprite.sprite.bounds.extents.x * transform.localScale.x;
			float cameraMaxY = Camera.main.transform.position.y + cameraHeight - sprite.sprite.bounds.extents.y * transform.localScale.y;
			float cameraMinY = Camera.main.transform.position.y - cameraHeight + sprite.sprite.bounds.extents.y * transform.localScale.y;
			transform.position = new Vector2(Mathf.Clamp(targetCar.transform.position.x, cameraMinX, cameraMaxX), Mathf.Clamp(targetCar.transform.position.y, cameraMinY, cameraMaxY));

			Vector2 direction = (targetCar.transform.position - Camera.main.transform.position);
			float angle = Vector2.Angle(transform.right, direction);
			Vector3 cross = Vector3.Cross((Vector3)transform.right, (Vector3)direction);
			if (cross.z < 0f) {
				angle *= -1f;
			}
			
			Quaternion newRotation = transform.rotation;
			newRotation.eulerAngles = new Vector3(0f, 0f, transform.rotation.eulerAngles.z + angle);
			transform.rotation = newRotation;
        }
        else
        {
			if (sprite.sortingLayerName != "Cars") {
				sprite.sortingLayerName = "Cars";
				sprite.sortingOrder = -2;
			}

			transform.position = targetCar.transform.position;
			transform.rotation = targetCar.transform.rotation;
        }
	}
}
