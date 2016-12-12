using UnityEngine;
using System.Collections;
using Com.LuisPedroFonseca.ProCamera2D;

public class TargetIndicator : MonoBehaviour {
    public bool onlyShowIfPlayerOnCar = false;

    public GameObject target;

    SpriteRenderer sprite;
    float scaleAugment = 0f;

    public static TargetIndicator Instance = null;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            sprite = GetComponent<SpriteRenderer>();
        }
        else {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void LateUpdate() {
        if (target == null) {
            return;
        }
        Camera mainCamera = ProCamera2D.Instance.GameCamera;
        float cameraHeight = 2 * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        Bounds cameraBounds = new Bounds(mainCamera.transform.position, new Vector3(cameraWidth, cameraHeight, 100f));
        if (onlyShowIfPlayerOnCar && GameManager.Instance.Player.State != PlayerState.OnCar) {
            sprite.enabled = false;
        }
        else if (!target.GetComponent<SpriteRenderer>().bounds.Intersects(cameraBounds)) {
            sprite.enabled = true;
            if (sprite.sortingLayerName != "Player") {
                sprite.sortingLayerName = "Player";
                sprite.sortingOrder = 0;
            }

            float cameraMaxX = mainCamera.transform.position.x + cameraWidth / 2f - sprite.sprite.bounds.extents.x * transform.localScale.x;
            float cameraMinX = mainCamera.transform.position.x - cameraWidth / 2f + sprite.sprite.bounds.extents.x * transform.localScale.x;
            float cameraMaxY = mainCamera.transform.position.y + cameraHeight / 2f - sprite.sprite.bounds.extents.y * transform.localScale.y;
            float cameraMinY = mainCamera.transform.position.y - cameraHeight / 2f + sprite.sprite.bounds.extents.y * transform.localScale.y;
            transform.position = new Vector2(Mathf.Clamp(target.transform.position.x, cameraMinX, cameraMaxX), Mathf.Clamp(target.transform.position.y, cameraMinY, cameraMaxY));

            Vector2 direction = (target.transform.position - Camera.main.transform.position);
            float angle = Vector2.Angle(transform.right, direction);
            Vector3 cross = Vector3.Cross((Vector3)transform.right, (Vector3)direction);
            if (cross.z < 0f) {
                angle *= -1f;
            }

            Quaternion newRotation = transform.rotation;
            newRotation.eulerAngles = new Vector3(0f, 0f, transform.rotation.eulerAngles.z + angle - 90f); // The -90f is due to the car's naturation rotation being at 90 degrees.
            transform.rotation = newRotation;

            Vector2 minScale = new Vector2(1f, 1f);
            scaleAugment = (target.transform.position - mainCamera.transform.position).magnitude / 9f;
            Vector2 scale = new Vector2(scaleAugment, scaleAugment);
            transform.localScale = minScale + scale;
        }
        else {
            sprite.enabled = true;

            // Put the target under the car to make things look nicer.
            if (sprite.sortingLayerName != "Cars" && target.GetComponentInChildren<CarController>() != null) {
                sprite.sortingLayerName = "Cars";
                sprite.sortingOrder = -2;
            }

            transform.position = target.transform.position;
            transform.rotation = target.transform.rotation;
            transform.localScale = new Vector2(2f, 2f);
        }
    }
}
