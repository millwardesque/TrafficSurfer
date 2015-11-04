using UnityEngine;
using System.Collections;

public enum CameraZoomState {
	Idle,
	ZoomIn,
	ZoomOut,
};

public enum CameraFollowState
{
    Free,
    Follow
}

[RequireComponent (typeof(Camera))]
public class CameraTools : MonoBehaviour {
	public float minZoom = 2f;
	public float normalZoom = 4f;
	public float zoomSpeed = 5f;
    public Transform followTarget = null;
	Camera m_camera;
	
	CameraZoomState m_zoomState = CameraZoomState.Idle;
	public CameraZoomState ZoomState {
		get { return m_zoomState; }
		set {
			m_zoomState = value;
		}
	}

    CameraFollowState m_followState = CameraFollowState.Free;
    public CameraFollowState FollowState
    {
        get { return m_followState; }
        set {
            m_followState = value;
        }
    }

	void Awake() {
		m_camera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        UpdateZoomState();
        UpdateFollowState();
    }

    void UpdateZoomState()
    {
        if (ZoomState == CameraZoomState.ZoomIn)
        {
            if (m_camera.orthographicSize > minZoom)
            {
                m_camera.orthographicSize = Mathf.Clamp(m_camera.orthographicSize - zoomSpeed * Time.deltaTime, minZoom, normalZoom);
            }

            if (Input.GetKeyUp(KeyCode.C))
            {
                ZoomState = CameraZoomState.ZoomOut;
            }
        }
        else if (ZoomState == CameraZoomState.ZoomOut)
        {
            if (m_camera.orthographicSize < normalZoom)
            {
                m_camera.orthographicSize += zoomSpeed * Time.deltaTime;
            }
            else
            {
                m_camera.orthographicSize = normalZoom;
                ZoomState = CameraZoomState.Idle;
            }
        }
        else if (ZoomState == CameraZoomState.Idle)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                ZoomState = CameraZoomState.ZoomIn;
            }
        }
    }

    void UpdateFollowState()
    {
        if (FollowState == CameraFollowState.Follow)
        {
            if (followTarget != null)
            {
                transform.position = new Vector3(followTarget.transform.position.x, followTarget.transform.position.y, transform.position.z);
            }
        }
    }
}
