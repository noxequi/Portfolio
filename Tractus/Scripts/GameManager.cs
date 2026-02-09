using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Camera mapCamera;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private CameraMovement cameraMovement;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float cameraSpeed = 8f;
    [SerializeField] private float deadZone = 0.1f;
    [SerializeField] private Vector2 cameraMinBounds;
    [SerializeField] private Vector2 cameraMaxBounds;
    [SerializeField] private GameObject pausePanel;

    private enum GameState { MapView, Playing }
    private GameState currentState;

    void Start()
    {
        SwitchToMapView();
        Pauser.Pause();
    }

    void Update()
    {
        if (currentState == GameState.MapView)
        {
            MoveMapCamera();

            if (Input.GetKeyUp(KeyCode.Space))
            {
                SwitchToPlayingView();
            }
        }
    }

    private void SwitchToMapView()
    {
        currentState = GameState.MapView;
        mapCamera.gameObject.SetActive(true);
        playerCamera.gameObject.SetActive(false);
        cameraMovement.enabled = false;

        if (playerTransform != null)
        {
            Vector3 startPos = playerTransform.position;
            startPos.z = mapCamera.transform.position.z; 
            mapCamera.transform.position = startPos;
        }
        
        ClampCameraPosition();
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void SwitchToPlayingView()
    {
        Pauser.Resume();
        currentState = GameState.Playing;
        mapCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);
        cameraMovement.enabled = true;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void MoveMapCamera()
    {
        if (mapCamera == null) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(moveX) < deadZone) moveX = 0f;
        if (Mathf.Abs(moveY) < deadZone) moveY = 0f;

        if (moveX == 0f && moveY == 0f) return;

        Vector3 moveVector = new Vector3(moveX, moveY, 0).normalized * cameraSpeed * Time.unscaledDeltaTime;
        mapCamera.transform.position += moveVector;
        
        ClampCameraPosition();
    }
    
    private void ClampCameraPosition()
    {
        Vector3 clampedPosition = mapCamera.transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, cameraMinBounds.x, cameraMaxBounds.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, cameraMinBounds.y, cameraMaxBounds.y);
        mapCamera.transform.position = clampedPosition;
    }
}