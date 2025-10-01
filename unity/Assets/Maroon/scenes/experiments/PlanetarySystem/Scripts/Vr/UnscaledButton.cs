using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Valve.VR.InteractionSystem;
using GameLabGraz.VRInteraction; // VRHoverButton

public class UnscaledButtons : MonoBehaviour
{

    //short explanation: searches for the nearest button 
    //(with enter and exit radius because idk why it didn't work else), 
    //and only the nearest button will be targeted because of issues when they are close together
    //
    public Hand leftHand;
    public Hand rightHand;

    public VRHoverButton[] buttons;
    public float enterRadius = 0.10f;

    public float exitRadius = 0.14f;

    VRHoverButton currentL, currentR;

    static MethodInfo miBegin, miUpdate, miEnd;
    static bool reflReady, reflFailed;


    void Update()
    {
        if (Mathf.Approximately(Time.timeScale, 1f))
        {
            return;
        }

        DriveHand(leftHand,  ref currentL);
        DriveHand(rightHand, ref currentR);
    }

    void DriveHand(Hand hand, ref VRHoverButton current)
    {
        if (!hand) //DSD damaged me that I requestion everything
        {
            return;
        }
        VRHoverButton best = current;
        float bestDist = float.PositiveInfinity;

        if (current != null && TryDistance(hand, current, out float dCur))
        {
            if (dCur <= exitRadius)
            {
                best = current;
            }
            else
            {
                best = null;
            }
            bestDist = dCur;
        }
        else
        {
            current = null;
        }

        if (best == null)
        {
            foreach (var btn in buttons)
            {
                if (!btn) continue;
                if (!TryDistance(hand, btn, out float d)) continue;
                if (d <= enterRadius && d < bestDist)
                {
                    best = btn;
                    bestDist = d;
                }
            }
        }

        foreach (var btn in buttons)
        {
            if (!btn) continue;

            if (btn == best)
            {
                if (current != best)
                {
                    if (current != null) CallEnd(current, hand);
                    CallBegin(best, hand);
                    current = best;
                }
                CallUpdate(best, hand);
            }
            else
            {
                if (current != null && btn == current)
                {
                    CallEnd(btn, hand);
                    if (current == btn) current = null;
                }
            }
        }
    }

    bool TryDistance(Hand hand, VRHoverButton btn, out float dist)
    {
        dist = float.PositiveInfinity;
        if (!btn)
        {
            return false;
        }

        Vector3 handPos = hand.transform.position;

        if (btn.TryGetComponent<Collider>(out var col) && col)
        {
            var p = col.ClosestPoint(handPos);
            dist = Vector3.Distance(handPos, p);
        }
        return true;
    }

    void Awake()
    {
        if (reflReady || reflFailed)
        {
            return;
        }

        try
        {
            var tInter = typeof(Interactable);
            var tHov = typeof(VRHoverButton);
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;

            miBegin = tInter.GetMethod("OnHandHoverBegin", flags);
            miEnd = tInter.GetMethod("OnHandHoverEnd", flags);
            miUpdate = tHov.GetMethod("HandHoverUpdate", flags);

            reflReady = (miBegin != null && miEnd != null && miUpdate != null);
            reflFailed = !reflReady;
        }
        catch
        {
            reflFailed = true;
        }
    }

    void CallBegin(VRHoverButton b, Hand h)
    {
        if (reflReady)
        {
            try
            {
                miBegin.Invoke(b, new object[] { h });
            }//so that it doesn't crash if it doesnt work lol
            catch { }
        }
    }
    void CallUpdate(VRHoverButton b, Hand h)
    {
        if (reflReady)
        {
            try
            {
                miUpdate.Invoke(b, new object[] { h });
            }
            catch { }
        }
    }
    void CallEnd(VRHoverButton b, Hand h)
    {
        if (reflReady)
        {
            try
            {
                miEnd.Invoke(b, new object[] { h });
            }
            catch { }
        }
    }
}
