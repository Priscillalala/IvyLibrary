using UnityEngine;

namespace IvyLibrary
{
    public struct ItemDisplayTransform
    {
        public string childName;
        public Vector3? localPos;
        public Vector3? localAngles;
        public Vector3? localScale;

        public ItemDisplayTransform(string childName, Vector3 localPos, Vector3 localAngles, Vector3 localScale)
        {
            this.childName = childName;
            this.localPos = localPos;
            this.localAngles = localAngles;
            this.localScale = localScale;
        }
    }
}