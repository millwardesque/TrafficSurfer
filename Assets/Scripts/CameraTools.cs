using UnityEngine;
using System.Collections;

public enum CameraZoomState {
	Idle,
	ZoomIn,
	ZoomOut
};

[RequireComponent (typeof(Camera))]
public class CameraTools : MonoBehaviour {
	public float minZoom = 2f;
	public float normalZoom = 4f;
	public float zoomSpeed = 5f;
	Camera m_camera;
	
	CameraZoomState m_state = CameraZoomState.Idle;
	public CameraZoomState State {
		get { return m_state; }
		set {
			m_state = value;
		}
	}

	void Awake() {
		m_camera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		if (State == CameraZoomState.ZoomIn) {
			if (m_camera.orthographicSize > minZoom) {
				m_camera.orthographicSize = Mathf.Clamp(m_camera.orthographicSize - zoomSpeed * Time.deltaTime, minZoom, normalZoom);
			}

			if (Input.GetKeyUp(KeyCode.C)) {
				State = CameraZoomState.ZoomOut;
			}
		}
		else if (State == CameraZoomState.ZoomOut) {
			if (m_camera.orthographicSize < normalZoom) {
				m_camera.orthographicSize += zoomSpeed * Time.deltaTime;
			}
			else {
				m_camera.orthographicSize = normalZoom;
				State = CameraZoomState.Idle;
			}
		}
		else if (State == CameraZoomState.Idle) {
			if (Input.GetKeyDown(KeyCode.C)) {
				State = CameraZoomState.ZoomIn;
			}
		}
	}
}
