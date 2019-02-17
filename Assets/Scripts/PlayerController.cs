using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float                MoveSpeed = 6.0f;
    public float                InteractionRange = 10.0f;

    private float               m_translation;
    private float               m_straffe;
    private CameraController    m_cameraController;
    private PlayerUI            m_uiController;
    private Ray                 m_ray;
    private RaycastHit          m_hit;
    private PointOfInterest     m_hoverPoint;

    private void Awake()
    {
        m_cameraController  = GetComponentInChildren<CameraController>();
        m_uiController      = GetComponentInChildren<PlayerUI>();
    }

    private void Update()
    {
        if (GameManager.Instance?.LockStateManager.IsPaused ?? true)
        {
            return;
        }
        ProcessMove();
        ProcessRaycast();
    }

    private void ProcessMove()
    {
        m_translation   = Input.GetAxis("Vertical") * MoveSpeed * Time.deltaTime;
        m_straffe       = Input.GetAxis("Horizontal") * MoveSpeed * Time.deltaTime;

        transform.Translate(m_straffe, 0, m_translation);
    }

    private void ProcessRaycast()
    {
        m_ray = m_cameraController.Camera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));

        if (Physics.Raycast(m_ray, out m_hit, InteractionRange, 1 << LayerMask.NameToLayer("PointOfInterest")))
        {
            var poi = m_hit.collider.GetComponent<PointOfInterest>();

            if (poi != m_hoverPoint)
            {
                m_hoverPoint    = poi;
                var data        = GameManager.Instance.GetPOIByID(poi.ID);
                if (data == null)
                {
                    Debug.LogWarning("Failed to send poi data to browser. POI with ID " + poi.ID + " could not be found.");
                }
                else
                {
                    ComUtility.Send(data.ToProtocolObject());
                }
                //Debug.Log(data.Name + ":" + data.Description);
            }
        }
        else
        {
            if(m_hoverPoint != null)
            {
                ComUtility.Send(POIProtocolObject.None);
            }
            m_hoverPoint = null;
        }

        m_uiController.HighlightCursor(m_hoverPoint != null);
    }
}
