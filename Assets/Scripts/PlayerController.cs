using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float                MoveSpeed = 4.0f;
    public float                InteractionRange = 10.0f;

    private float               m_translation;
    private float               m_straffe;
    private CameraController    m_cameraController;
    private PlayerUI            m_uiController;
    private Ray                 m_ray;
    private RaycastHit          m_hit;
    private PointOfInterest     m_hoverPoint;
    private LayerMask           m_layerMask;
    private PointOfInterest     m_poiHit;
    private CharacterController m_char;

    private void Awake()
    {
        m_cameraController  = GetComponentInChildren<CameraController>();
        m_uiController      = GetComponentInChildren<PlayerUI>();
        m_layerMask         = LayerMask.GetMask("Default", "PointOfInterest");
        m_char              = GetComponent<CharacterController>();
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

        m_char.Move(transform.TransformDirection(new Vector3(m_straffe, 0, m_translation)));
    }

    private void ProcessRaycast()
    {
        m_ray = m_cameraController.Camera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));

        m_poiHit = null;
        if (Physics.Raycast(m_ray, out m_hit, InteractionRange, m_layerMask) && 
            (m_poiHit = m_hit.collider.GetComponent<PointOfInterest>()))
        {
            if (m_poiHit != m_hoverPoint)
            {
                m_hoverPoint    = m_poiHit;
                ComUtility.Send(new POIProtocolObject(m_poiHit.ID));
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
