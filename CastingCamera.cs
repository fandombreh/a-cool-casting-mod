using Unity.Cinemachine;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;


namespace z3roCastingMod.Librarys
{
    public class CastingCamera : MonoBehaviour
    {
        public float FieldOfView = 60;
        public float CameraHeight = 1.5f;
        public float CameraDistance = -2f;
        public float MotionSmoothing = 0.6f;
        public float RotationSmoothing = 0.6f;
        public float NearClip = 0.04f;

        public VRRig Target;
        public static ViewMode viewMode;
        public ParentType parentType;
        public RotationMode rotationMode ;

        private GameObject CameraHolder;
        private GameObject RotationGuide;
        private AudioListener tester;
        public bool StartedCasting;
        public bool UseMotionSmoothing;
        public bool UseRotationSmoothing;
        public bool UseRotationGuide = false;
        Camera test;
        public GameObject testGO;
        public static RenderTexture Render;

        public enum ViewMode
        {
            ThirdPerson,
            FirstPerson,
        }
        public enum ParentType
        {
            Body,
            Head,
        }
        public enum RotationMode
        {
            Body,Head
        }
        
        void Update()
        {
            if (StartedCasting)
            {
               if (testGO != null)
                {
                    testGO.SetActive(true);
                }
                if (CameraHolder == null)
                {
                    CameraHolder = new GameObject("CamHolder");
                }
                if(RotationGuide == null)
                {
                    RotationGuide = new GameObject("rot guide");
                }
                if(CameraHolder != null)
                {
                    var camera = test;
                    var cameraGO = testGO;
                    camera.fieldOfView = FieldOfView;
                    camera.nearClipPlane = NearClip;
                    

                    if(viewMode == ViewMode.ThirdPerson)
                    {
                        RotationGuide.transform.position = cameraGO.transform.position;
                        RotationGuide.transform.LookAt(Target.transform.position);
                        cameraGO.transform.position = UseMotionSmoothing ? Vector3.Lerp(cameraGO.transform.position, CameraHolder.transform.position, 1f - MotionSmoothing) : CameraHolder.transform.position;
                        if (UseRotationGuide == false)
                        {
                            cameraGO.transform.rotation = UseRotationSmoothing ? Quaternion.Lerp(cameraGO.transform.rotation, CameraHolder.transform.rotation, 1f - RotationSmoothing) : CameraHolder.transform.rotation;
                        }
                        else
                        {
                            cameraGO.transform.rotation = UseRotationSmoothing ? Quaternion.Lerp(cameraGO.transform.rotation, RotationGuide.transform.rotation, 1f - RotationSmoothing) : RotationGuide.transform.rotation;
                        }
                        if (parentType == ParentType.Body)
                        {
                            CameraHolder.transform.parent = Target.transform;
                            CameraHolder.transform.localPosition = new Vector3(0, CameraHeight, CameraDistance);
                            if (rotationMode == RotationMode.Body)
                            {
                                CameraHolder.transform.LookAt(Target.transform.position);
                            }
                            if (rotationMode == RotationMode.Head)
                            {
                                CameraHolder.transform.rotation = Target.headMesh.transform.rotation;
                            }
                        }
                        if (parentType == ParentType.Head)
                        {
                            CameraHolder.transform.parent = Target.headMesh.transform;
                            CameraHolder.transform.localPosition = new Vector3(0, CameraHeight, CameraDistance);
                            if (rotationMode == RotationMode.Body)
                            {
                                CameraHolder.transform.LookAt(Target.transform.position);
                            }
                            if (rotationMode == RotationMode.Head)
                            {
                                CameraHolder.transform.rotation = Target.headMesh.transform.rotation;
                            }
                        }
                    }
                    
                }
            }
        }
        void LateUpdate()
        {
            if (CameraHolder != null)
            {
                var camera = test;
                var cameraGO = testGO;
                camera.cameraType = CameraType.Preview;
                camera.fieldOfView = FieldOfView;
                if (viewMode == ViewMode.FirstPerson)
                {
                    cameraGO.transform.position = CameraHolder.transform.position;
                    cameraGO.transform.rotation = UseRotationSmoothing ? Quaternion.Lerp(cameraGO.transform.rotation, CameraHolder.transform.rotation, 1f - RotationSmoothing) : CameraHolder.transform.rotation;
                    CameraHolder.transform.parent = Target.headMesh.transform;
                    CameraHolder.transform.localPosition = new Vector3(0, 0.14f, 0);
                    CameraHolder.transform.localRotation = Quaternion.Euler(Vector3.zero);
                }
            }

           
        }
        public void InstantiateCamera()
        {
            StartedCasting = true;
            if (Target == null)
            {
                Target = GorillaTagger.Instance.offlineVRRig;
            }
            
            if(this.gameObject.GetComponent<Camera>() != null && test == null)
            {
                testGO = new GameObject("CameraGO");
                test = testGO.AddComponent<Camera>();
                test.cameraType = CameraType.Preview;
                test.cullingMask = Camera.main.cullingMask;
                testGO.tag = "Untagged";
                if (Render == null)
                {
                    Render = new RenderTexture(Screen.width, Screen.height, 24);
                    test.targetTexture = Render;
                }
            }
            if (tester == null)
            {
                tester = testGO.AddComponent<AudioListener>();
            }
            if (tester != null)
            {
                tester.enabled = true;
            }
        }
        public void TerminateCamera(CinemachineVirtualCamera cinemachine = null)
        {
            if(tester != null)
            {
                tester.enabled = false;
            }
            StartedCasting = false;
            if (cinemachine != null)
            {
                cinemachine.enabled = true;
            }
        }
    }
}
