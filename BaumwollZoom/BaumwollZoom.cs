using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICities;
using UnityEngine;

namespace BaumwollZoom
{
    public class BaumwollZoomMod : IUserMod
    {
        public string Name
        {
            get { return "BaumwollZoom Zoom to Cursor"; }
        }

        public string Description
        {
            get { return "Modifies the camera behaviour such that scrolling zooms in or out while keeping the cursor in the same place."; }
        }
    }


    public class BaumwollZoomLoader : LoadingExtensionBase
    {
        BaumwollZoomBehaviour instance;

        public override void OnLevelUnloading()
        {
            if (instance != null)
            {
                MonoBehaviour.Destroy(instance);
            }
            base.OnLevelUnloading();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            instance = GameObject.FindObjectOfType<CameraController>().gameObject.AddComponent<BaumwollZoomBehaviour>();
            base.OnLevelLoaded(mode);
        }
    }


    public class BaumwollZoomBehaviour : MonoBehaviour
    {
        private CameraController cameraController;

        private static RayCaster raycaster = new RayCaster();

        private void Start()
        {
            cameraController = GameObject.FindObjectOfType<CameraController>();
            cameraController.m_maxTiltDistance = 1000000f;
        }

        private float frameInitialCurrentSize;

        private RayCaster.RaycastInput frameInitialMouseRaycastInput;

        private void Update()
        {
            frameInitialCurrentSize = cameraController.m_currentSize;
            frameInitialMouseRaycastInput = new RayCaster.RaycastInput(
                Camera.main.ScreenPointToRay(Input.mousePosition),
                Camera.main.farClipPlane
            );
        }

        private void LateUpdate()
        {
            if (frameInitialCurrentSize == 0f || cameraController.m_currentSize == 0f || frameInitialCurrentSize == cameraController.m_currentSize)
            {
                return;
            }
            RayCaster.RaycastOutput output;
            if (raycaster.CastRay(frameInitialMouseRaycastInput, out output))
            {
                var fractionTowardsCursor = 1f - cameraController.m_currentSize / frameInitialCurrentSize;
                var correction = fractionTowardsCursor * (output.m_hitPos - cameraController.m_currentPosition);
                correction.y = 0f;
                cameraController.transform.position += correction;
                cameraController.m_targetPosition += correction;
                cameraController.m_currentPosition += correction;
            }
        }
    }

    class RayCaster : ToolBase
    {
        public bool CastRay(RaycastInput input, out RaycastOutput output)
        {
            return RayCast(input, out output);
        }
    }
}
