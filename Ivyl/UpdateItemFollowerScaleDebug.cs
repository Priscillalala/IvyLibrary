using UnityEngine;
using RoR2;

namespace IvyLibrary
{
    public class UpdateItemFollowerScaleDebug : MonoBehaviour
    {
        private ItemFollower itemFollower;

        public void Awake()
        {
            itemFollower = base.GetComponent<ItemFollower>();
        }

        public void Update()
        {
            if (itemFollower && itemFollower.followerInstance)
            {
                itemFollower.followerInstance.transform.localScale = itemFollower.transform.localScale;
            }
        }
    }
}
