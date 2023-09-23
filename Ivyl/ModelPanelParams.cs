using UnityEngine;

namespace Ivyl
{
    public struct ModelPanelParams
    {
        public Quaternion modelRotation;
        public float minDistance;
        public float maxDistance;
        public Transform focusPoint;
        public Transform cameraPosition;
        public ModelPanelParams(Vector3 modelRotation, float minDistance, float maxDistance, Transform focusPoint = null, Transform cameraPosition = null)
            : this(Quaternion.Euler(modelRotation), minDistance, maxDistance, focusPoint, cameraPosition) { }
        public ModelPanelParams(Quaternion modelRotation, float minDistance, float maxDistance, Transform focusPoint = null, Transform cameraPosition = null)
        {
            this.modelRotation = modelRotation;
            this.minDistance = minDistance;
            this.maxDistance = maxDistance;
            this.focusPoint = focusPoint;
            this.cameraPosition = cameraPosition;
        }
    }
}