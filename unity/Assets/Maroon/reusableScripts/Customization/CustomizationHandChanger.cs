using System;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using Valve.VR.InteractionSystem.Sample;

public class CustomizationHandChanger : SkeletonUIOptions
{
    [Serializable]
    public struct HandModels
    {
        public GameObject rightHand;
        public GameObject leftHand;
    }

    public List<HandModels> handPrefabs = new List<HandModels>();

    public void OnChangeHandByIndex(int selectedIndex)
    {
        if (selectedIndex < 0 || selectedIndex >= handPrefabs.Count)
            return;

        var selectedModels = handPrefabs[selectedIndex];
        if(selectedModels.leftHand == null || selectedModels.rightHand == null)
            return;
        
        for (int handIndex = 0; handIndex < Player.instance.hands.Length; handIndex++)
        {
            var hand = Player.instance.hands[handIndex];
            if (hand != null)
            {
                if (hand.handType == SteamVR_Input_Sources.RightHand)
                    hand.SetRenderModel(selectedModels.rightHand);
                if (hand.handType == SteamVR_Input_Sources.LeftHand)
                    hand.SetRenderModel(selectedModels.leftHand);
            }
        }
    }
}
