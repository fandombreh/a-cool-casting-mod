using UnityEngine;
using Cinemachine;
using System.Collections;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace GorillaTagCinematicCamera
{
    [BepInPlugin("com.yourname.gorillatag.cinematiccamera", "Cinematic Spectator Camera", "1.0.0")]
    public class CinematicCameraPlugin : BaseUnityPlugin
    {
        private static CinematicCameraController controller;
        private static bool isInitialized = false;
        
        private void Awake()
        {
            Logger.LogInfo("Cinematic Camera Plugin loaded!");
            
            // Initialize when the game is ready
            StartCoroutine(InitializeWhenReady());
        }
        
        private System.Collections.IEnumerator InitializeWhenReady()
        {
            // Wait for GorillaTagger to be available
            while (GorillaTagger.Instance == null)
            {
                yield return new WaitForSeconds(0.5f);
            }
            
            // Create our controller
            GameObject controllerObj = new GameObject("CinematicCameraController");
            DontDestroyOnLoad(controllerObj);
            controller = controllerObj.AddComponent<CinematicCameraController>();
            controller.Initialize(Logger);
            
            isInitialized = true;
            Logger.LogInfo("Cinematic Camera Controller ready!");
        }
        
        // Optional: Add UI for toggling camera modes
        private void Update()
        {
            if (!isInitialized) return;
            
            // Example: Toggle with a key (customize as needed)
            if (Input.GetKeyDown(KeyCode.F8))
            {
                controller.ToggleCamera();
            }
        }
    }

    // Main Camera Controller Class
    public class CinematicCameraController : MonoBehaviour
    {
        private ManualLogSource logger;
        private CinemachineVirtualCamera virtualCamera;
        private CinemachineFramingTransposer framingTransposer;
        private CinemachineComposer composer;
        private GameObject cameraHolder;
        private VRRig currentTarget;
        private Camera spectatorCamera;
        private RenderTexture renderTexture;
        private AudioListener audioListener;
        
        // Public settings (can be exposed to UI)
        public CameraMode CurrentMode { get; private set; } = CameraMode.ThirdPerson;
        public TrackingTarget TrackingTarget { get; private set; } = TrackingTarget.Body;
        public RotationMode RotationTracking { get; private set; } = RotationMode.SmoothFollow;
        
        // Configuration parameters
        public float fieldOfView = 60f;
        public float cameraHeight = 1.5f;
        public float cameraDistance = -3f;
        public float followSmoothness = 0.1f;
        public float rotationSmoothness = 0.1f;
        public float lookAheadTime = 0.1f;
        public Vector3 thirdPersonOffset = new Vector3(0, 1.5f, -3f);
        public Vector3 firstPersonOffset = new Vector3(0, 0.14f, 0);
        
        private bool isActive = false;
        
        public enum CameraMode
        {
            ThirdPerson,
            FirstPerson,
            FreeCam,
            Orbit,
            ShoulderCam
        }
        
        public enum TrackingTarget
        {
            Head,
            Body,
            LeftHand,
            RightHand,
            CustomPosition
        }
        
        public enum RotationMode
        {
            DirectFollow,
            SmoothFollow,
            LookAtTarget,
            FreeRotation
        }
        
        public void Initialize(ManualLogSource logSource)
        {
            logger = logSource;
            SetupCameraSystem();
        }
        
        private void SetupCameraSystem()
        {
            // Create camera holder
            cameraHolder = new GameObject("CinemachineCameraHolder");
            cameraHolder.transform.parent = transform;
            
            // Create Cinemachine Virtual Camera
            virtualCamera = cameraHolder.AddComponent<CinemachineVirtualCamera>();
            virtualCamera.Priority = 100;
            virtualCamera.m_Lens.FieldOfView = fieldOfView;
            virtualCamera.m_Lens.NearClipPlane = 0.04f;
            
            // Add Framing Transposer for 3rd person
            framingTransposer = virtualCamera.AddCinemachineComponent<CinemachineFramingTransposer>();
            framingTransposer.m_ScreenX = 0.5f;
            framingTransposer.m_ScreenY = 0.5f;
            framingTransposer.m_CameraDistance = Mathf.Abs(cameraDistance);
            framingTransposer.m_DeadZoneWidth = 0.1f;
            framingTransposer.m_DeadZoneHeight = 0.1f;
            framingTransposer.m_SoftZoneWidth = 0.8f;
            framingTransposer.m_SoftZoneHeight = 0.8f;
            framingTransposer.m_Damping = new Vector3(followSmoothness, followSmoothness, followSmoothness);
            framingTransposer.m_LookaheadTime = lookAheadTime;
            
            // Create spectator camera GameObject
            GameObject spectatorObj = new GameObject("SpectatorCamera");
            spectatorCamera = spectatorObj.AddComponent<Camera>();
            spectatorCamera.cullingMask = Camera.main.cullingMask;
            spectatorCamera.depth = Camera.main.depth + 1;
            spectatorObj.tag = "Untagged";
            
            // Create render texture
            renderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
            renderTexture.antiAliasing = 4;
            renderTexture.filterMode = FilterMode.Trilinear;
            spectatorCamera.targetTexture = renderTexture;
            
            // Add audio listener (disabled when not active)
            audioListener = spectatorObj.AddComponent<AudioListener>();
            audioListener.enabled = false;
            
            // Set the virtual camera to use our spectator camera
            virtualCamera.gameObject.AddComponent<CinemachineBrain>();
            var brain = spectatorObj.AddComponent<CinemachineBrain>();
            brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, 0.5f);
            
            logger.LogInfo("Cinemachine camera system initialized!");
        }
        
        public void StartSpectating(VRRig target = null)
        {
            if (target == null)
            {
                currentTarget = GorillaTagger.Instance.offlineVRRig;
            }
            else
            {
                currentTarget = target;
            }
            
            if (currentTarget == null)
            {
                logger.LogError("No target available for spectating!");
                return;
            }
            
            isActive = true;
            ConfigureForCurrentMode();
            
            // Enable audio listener
            if (audioListener != null)
            {
                audioListener.enabled = true;
            }
            
            logger.LogInfo($"Started spectating {currentTarget.gameObject.name} in {CurrentMode} mode");
        }
        
        public void StopSpectating()
        {
            isActive = false;
            
            // Clear follow target
            virtualCamera.Follow = null;
            virtualCamera.LookAt = null;
            
            // Disable audio listener
            if (audioListener != null)
            {
                audioListener.enabled = false;
            }
            
            logger.LogInfo("Stopped spectating");
        }
        
        public void ToggleCamera()
        {
            if (isActive)
            {
                StopSpectating();
            }
            else
            {
                StartSpectating();
            }
        }
        
        public void SetCameraMode(CameraMode mode)
        {
            CurrentMode = mode;
            if (isActive)
            {
                ConfigureForCurrentMode();
            }
        }
        
        public void SetTrackingTarget(TrackingTarget target)
        {
            TrackingTarget = target;
            if (isActive)
            {
                UpdateTrackingTarget();
            }
        }
        
        public void SetRotationMode(RotationMode mode)
        {
            RotationTracking = mode;
            if (isActive)
            {
                ConfigureRotation();
            }
        }
        
        private void ConfigureForCurrentMode()
        {
            switch (CurrentMode)
            {
                case CameraMode.FirstPerson:
                    ConfigureFirstPerson();
                    break;
                case CameraMode.ThirdPerson:
                    ConfigureThirdPerson();
                    break;
                case CameraMode.FreeCam:
                    ConfigureFreeCam();
                    break;
                case CameraMode.Orbit:
                    ConfigureOrbit();
                    break;
                case CameraMode.ShoulderCam:
                    ConfigureShoulderCam();
                    break;
            }
            
            UpdateTrackingTarget();
            ConfigureRotation();
        }
        
        private void ConfigureFirstPerson()
        {
            // Remove transposer for first person
            var transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                Destroy(transposer);
            }
            
            // Add DoNothing transposer (just follows)
            var doNothing = virtualCamera.AddCinemachineComponent<CinemachineTransposer>();
            doNothing.m_BindingMode = CinemachineTransposer.BindingMode.LockToTargetWithWorldUp;
            
            // Configure for head tracking
            virtualCamera.m_Lens.FieldOfView = fieldOfView;
            UpdateTrackingTarget();
        }
        
        private void ConfigureThirdPerson()
        {
            // Ensure we have a framing transposer
            if (framingTransposer == null)
            {
                framingTransposer = virtualCamera.AddCinemachineComponent<CinemachineFramingTransposer>();
            }
            
            framingTransposer.m_CameraDistance = Mathf.Abs(cameraDistance);
            framingTransposer.m_Damping = new Vector3(followSmoothness, followSmoothness, followSmoothness);
            framingTransposer.m_LookaheadTime = lookAheadTime;
            
            virtualCamera.m_Lens.FieldOfView = fieldOfView;
        }
        
        private void ConfigureFreeCam()
        {
            // For free cam, we might want different behavior
            var transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                transposer.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
            }
        }
        
        private void ConfigureOrbit()
        {
            // Add orbital transposer
            var orbital = virtualCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
            if (orbital == null)
            {
                orbital = virtualCamera.AddCinemachineComponent<CinemachineOrbitalTransposer>();
            }
            
            orbital.m_Heading.m_Bias = 0;
            orbital.m_Heading.m_HeadingBias = 0;
            orbital.m_Heading.m_VerticalAxis.m_MaxSpeed = 2f;
            orbital.m_Heading.m_HorizontalAxis.m_MaxSpeed = 2f;
            orbital.m_Radius = Mathf.Abs(cameraDistance);
            orbital.m_HeightOffset = cameraHeight;
        }
        
        private void ConfigureShoulderCam()
        {
            // Shoulder camera (slightly offset)
            var transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                transposer.m_FollowOffset = new Vector3(0.5f, cameraHeight, cameraDistance);
            }
        }
        
        private void UpdateTrackingTarget()
        {
            if (currentTarget == null) return;
            
            Transform targetTransform = null;
            
            switch (TrackingTarget)
            {
                case TrackingTarget.Head:
                    targetTransform = currentTarget.headMesh.transform;
                    break;
                case TrackingTarget.Body:
                    targetTransform = currentTarget.transform;
                    break;
                case TrackingTarget.LeftHand:
                    targetTransform = currentTarget.leftHandTransform;
                    break;
                case TrackingTarget.RightHand:
                    targetTransform = currentTarget.rightHandTransform;
                    break;
                case TrackingTarget.CustomPosition:
                    // Create a custom transform if needed
                    if (!customTarget)
                    {
                        customTarget = new GameObject("CustomTarget").transform;
                        customTarget.parent = currentTarget.transform;
                    }
                    targetTransform = customTarget;
                    break;
            }
            
            if (targetTransform != null)
            {
                virtualCamera.Follow = targetTransform;
                virtualCamera.LookAt = targetTransform;
            }
        }
        
        private Transform customTarget;
        
        private void ConfigureRotation()
        {
            var composer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
            if (composer == null)
            {
                composer = virtualCamera.AddCinemachineComponent<CinemachineComposer>();
            }
            
            switch (RotationTracking)
            {
                case RotationMode.DirectFollow:
                    composer.m_HorizontalDamping = 0;
                    composer.m_VerticalDamping = 0;
                    break;
                case RotationMode.SmoothFollow:
                    composer.m_HorizontalDamping = rotationSmoothness;
                    composer.m_VerticalDamping = rotationSmoothness;
                    break;
                case RotationMode.LookAtTarget:
                    // Look at a specific point (could be head or hands)
                    break;
                case RotationMode.FreeRotation:
                    // For free look
                    break;
            }
        }
        
        // Advanced feature: Picture-in-Picture mode
        public void EnablePIPMode(float pipSize = 0.2f, ScreenCorner corner = ScreenCorner.TopRight)
        {
            // Create a second camera for PIP
            GameObject pipCameraObj = new GameObject("PIPCamera");
            Camera pipCamera = pipCameraObj.AddComponent<Camera>();
            pipCamera.targetTexture = renderTexture; // Can share or use separate RT
            
            // Configure viewport rect based on corner
            Rect viewport = new Rect();
            switch (corner)
            {
                case ScreenCorner.TopRight:
                    viewport = new Rect(1 - pipSize, 1 - pipSize, pipSize, pipSize);
                    break;
                case ScreenCorner.TopLeft:
                    viewport = new Rect(0, 1 - pipSize, pipSize, pipSize);
                    break;
                case ScreenCorner.BottomRight:
                    viewport = new Rect(1 - pipSize, 0, pipSize, pipSize);
                    break;
                case ScreenCorner.BottomLeft:
                    viewport = new Rect(0, 0, pipSize, pipSize);
                    break;
            }
            
            pipCamera.rect = viewport;
            pipCamera.depth = spectatorCamera.depth + 1;
        }
        
        public enum ScreenCorner
        {
            TopRight,
            TopLeft,
            BottomRight,
            BottomLeft
        }
        
        // UI Integration Methods (for mod menus)
        public void UpdateSettings(float fov, float height, float distance, float followSmooth, float rotateSmooth)
        {
            fieldOfView = fov;
            cameraHeight = height;
            cameraDistance = distance;
            followSmoothness = followSmooth;
            rotationSmoothness = rotateSmooth;
            
            // Apply updates if camera is active
            if (isActive)
            {
                ConfigureForCurrentMode();
            }
        }
        
        // Save/Load configuration
        public void SaveConfiguration(string configName)
        {
            // Implement configuration saving
            // Could use PlayerPrefs or a config file
        }
        
        public void LoadConfiguration(string configName)
        {
            // Implement configuration loading
        }
        
        // Hotkey system
        private void Update()
        {
            if (!isActive) return;
            
            HandleHotkeys();
            UpdateDynamicOffsets();
        }
        
        private void HandleHotkeys()
        {
            // Example hotkeys (customize as needed)
            if (Input.GetKeyDown(KeyCode.Alpha1))
                SetCameraMode(CameraMode.FirstPerson);
            if (Input.GetKeyDown(KeyCode.Alpha2))
                SetCameraMode(CameraMode.ThirdPerson);
            if (Input.GetKeyDown(KeyCode.Alpha3))
                SetCameraMode(CameraMode.Orbit);
            if (Input.GetKeyDown(KeyCode.Equals))
                cameraDistance += 0.5f;
            if (Input.GetKeyDown(KeyCode.Minus))
                cameraDistance -= 0.5f;
            if (Input.GetKeyDown(KeyCode.PageUp))
                cameraHeight += 0.1f;
            if (Input.GetKeyDown(KeyCode.PageDown))
                cameraHeight -= 0.1f;
        }
        
        private void UpdateDynamicOffsets()
        {
            // Dynamic adjustment based on target speed
            if (currentTarget != null && framingTransposer != null)
            {
                float speed = currentTarget.GetComponent<Rigidbody>()?.velocity.magnitude ?? 0f;
                
                // Adjust follow distance based on speed
                float dynamicDistance = Mathf.Abs(cameraDistance) * (1 + speed * 0.1f);
                framingTransposer.m_CameraDistance = Mathf.Clamp(dynamicDistance, 2f, 10f);
                
                // Adjust lookahead based on speed
                framingTransposer.m_LookaheadTime = Mathf.Clamp(speed * 0.05f, 0.05f, 0.3f);
            }
        }
        
        // Clean up
        private void OnDestroy()
        {
            StopSpectating();
            
            if (renderTexture != null)
            {
                renderTexture.Release();
                Destroy(renderTexture);
            }
            
            logger.LogInfo("Cinematic camera system destroyed");
        }
    }
    
    // Optional: Extension for multiple camera support
    public class MultiCameraController : MonoBehaviour
    {
        private List<CinematicCameraController> cameras = new List<CinematicCameraController>();
        private int currentCameraIndex = 0;
        
        public void AddCamera(VRRig target)
        {
            GameObject newCamObj = new GameObject($"Camera_{cameras.Count}");
            var controller = newCamObj.AddComponent<CinematicCameraController>();
            cameras.Add(controller);
            
            if (cameras.Count == 1)
            {
                cameras[0].StartSpectating(target);
            }
        }
        
        public void SwitchToNextCamera()
        {
            if (cameras.Count == 0) return;
            
            cameras[currentCameraIndex].StopSpectating();
            currentCameraIndex = (currentCameraIndex + 1) % cameras.Count;
            cameras[currentCameraIndex].StartSpectating();
        }
    }
}
