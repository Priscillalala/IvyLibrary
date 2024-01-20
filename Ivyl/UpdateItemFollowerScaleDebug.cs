using UnityEngine;
using RoR2;

namespace IvyLibrary
{
    /// <summary>
    /// A component to update the scale of an item follower while creating item display rules.
    /// </summary>
    /// <remarks>
    /// Should not be included in releases.
    /// </remarks>
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
