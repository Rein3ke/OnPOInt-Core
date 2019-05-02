using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float                MoveSpeed = 4.0f;
    public float                InteractionRange = 10.0f;

    private float               m_translation;
    private float               m_straffe;

    public CameraController CameraController => m_cameraController;
    private CameraController    m_cameraController;

    private PlayerUI            m_uiController;
    private Ray                 m_ray;
    private RaycastHit          m_hit;
    private PointOfInterest     m_hoverPoint;
    private LayerMask           m_layerMask;
    private PointOfInterest     m_poiHit;
    private CharacterController m_char;

    /// <summary>
    /// Called after instantiation - Declares the member variables of the class.
    /// </summary>
    private void Awake()
    {
        m_cameraController  = GetComponentInChildren<CameraController>();
        m_uiController      = GetComponentInChildren<PlayerUI>();
        m_layerMask         = LayerMask.GetMask("Default", "PointOfInterest");
        m_char              = GetComponent<CharacterController>();
    }

    /// <summary>
    /// Allows the player to move and evaluate a raycast if the application is not paused.
    /// </summary>
    private void Update()
    {
        if (GameManager.Instance?.LockStateManager.IsPaused ?? true)
        {
            return;
        }
        ProcessMove();
        ProcessRaycast();
    }

    /// <summary>
    /// Lets the character move based on the addition of the motion vectors.
    /// </summary>
    private void ProcessMove()
    {
        m_translation   = Input.GetAxis("Vertical") * MoveSpeed * Time.deltaTime;
        m_straffe       = Input.GetAxis("Horizontal") * MoveSpeed * Time.deltaTime;

        m_char.Move(transform.TransformDirection(new Vector3(m_straffe, 0, m_translation)));
    }

    /// <summary>
    /// Based on raytracing a ProtocolObject gets determined and passed to the ComUtility.
    /// </summary>
    private void ProcessRaycast()
    {
        // Creates a ray from the camera to the relative center of the screen.
        m_ray = m_cameraController.Camera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));

        // Determine if the ray hit something based on raytracing.
        m_poiHit = null;
        if (Physics.Raycast(m_ray, out m_hit, InteractionRange, m_layerMask) && 
            (m_poiHit = m_hit.collider.GetComponent<PointOfInterest>()))
        {
            // Sends the ID of the hit POI to the ComUtility as POIProtocolObject.
            if (m_poiHit != m_hoverPoint)
            {
                m_hoverPoint    = m_poiHit;
                ComUtility.Send(new POIProtocolObject(m_poiHit.ID));
            }
        }
        else
        {
            // Sends an empty POIProtocolObject to the ComUtility to indicate that nothing has been hit.
            if (m_hoverPoint != null)
            {
                ComUtility.Send(POIProtocolObject.None);
            }
            m_hoverPoint = null;
        }
        // The PlayerUI sends a Boolean value indicating whether the crosshairs need to be highlighted.
        m_uiController.HighlightCursor(m_hoverPoint != null);
    }
}
