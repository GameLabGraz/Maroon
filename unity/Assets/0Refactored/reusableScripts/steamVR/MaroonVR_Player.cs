using UnityEngine;
using Valve.VR.InteractionSystem;

namespace MaroonVR
{
    public class MaroonVR_Player : Player
    {
        [SerializeField]
        protected GameObject snapTurnGameObject;

        [Header("Maroon VR Specific")] 
        [Tooltip("The user can turn the player via the controllers.")]
        public bool allowSnapTurns = true;

        [Header("Skeleton Settings")] 
        public bool showController = true;
        public bool animateWithController = false;

        // Start is called before the first frame update
        protected void Start()
        {
            if (snapTurnGameObject)
            {
                snapTurnGameObject.SetActive(allowSnapTurns);
            }

            foreach (var hand in instance.hands)
            {
                if (hand == null) continue;
                
                if (showController) hand.ShowController(true);
                else hand.HideController(true);
                
                hand.SetSkeletonRangeOfMotion(animateWithController? 
                    Valve.VR.EVRSkeletalMotionRange.WithController :
                    Valve.VR.EVRSkeletalMotionRange.WithoutController);
            }
        }
    }
}
