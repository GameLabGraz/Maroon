using System.Reflection;
using UnityEngine;
using Valve.VR.InteractionSystem;
using GameLabGraz.VRInteraction;

public class UnscaledSliderDriver : MonoBehaviour
{
    [Header("Hands")]
    public Hand leftHand;
    public Hand rightHand;

    [Header("Sliders")]
    public LinearDrive[] sliders;
    public float enterRadius = 0.10f;
    public float exitRadius = 0.14f;

    private LinearDrive currentL, currentR;

    static MethodInfo miBegin, miEnd, miHoverUpdate;
    static bool reflReady, reflFailed;

    void Awake() => CacheReflection();

    void Update()
    {
        if (Mathf.Approximately(Time.timeScale, 1f))
        {
            return;
        }

        DriveHand(leftHand,  ref currentL);
        DriveHand(rightHand, ref currentR);
    }

    void DriveHand(Hand hand, ref LinearDrive current)
    {
        LinearDrive best = current;
        float bestDist = float.PositiveInfinity;
        if (current != null && TryDistance(hand, current, out float dCur))
        {
            if (dCur > exitRadius)
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
            foreach (var d in sliders)
            {
                if (!TryDistance(hand, d, out float d2)) continue;
                if (d2 <= enterRadius && d2 < bestDist)
                {
                    best = d; bestDist = d2;
                }
            }
        }

        foreach (var d in sliders)
        {
            if (d == best)
            {
                if (current != best)
                {
                    if (current != null) CallEnd(current, hand);
                    CallBegin(best, hand);
                    current = best;
                }
                CallHoverUpdate(best, hand);
            }
            else
            {
                if (current != null && d == current)
                {
                    CallEnd(d, hand);
                    if (current == d)
                    {
                        current = null;
                    }
                }
            }
        }
    }

    bool TryDistance(Hand hand, LinearDrive drive, out float dist)
    {
        dist = float.PositiveInfinity;
        if (!hand || !drive)
        {
            return false;
        }

        Vector3 handPos = hand.transform.position;

        Collider col = drive.GetComponent<Collider>();
        if (!col) col = drive.GetComponentInChildren<Collider>();

        if (col != null)
        {
            Vector3 p = col.ClosestPoint(handPos);
            dist = Vector3.Distance(handPos, p);
            return true;
        }
        else
        {
            dist = Vector3.Distance(handPos, drive.transform.position);
            return true;
        }
    }

    void CacheReflection()
    {
        if (reflReady || reflFailed) return;
        try
        {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            miBegin       = typeof(Interactable).GetMethod("OnHandHoverBegin", flags);
            miEnd         = typeof(Interactable).GetMethod("OnHandHoverEnd",   flags);
            miHoverUpdate = typeof(LinearDrive) .GetMethod("HandHoverUpdate",  flags);
            reflReady = (miBegin != null && miEnd != null && miHoverUpdate != null);
            reflFailed = !reflReady;
        }
        catch { reflFailed = true; }
    }

    void CallBegin(LinearDrive d, Hand h)
    {
        if (reflReady)
        {
            try
            {
                miBegin.Invoke(d, new object[] { h });
            }
            catch { }
        }
    }
    void CallHoverUpdate(LinearDrive d, Hand h)
    {
        if (reflReady)
        {
            try
            {
                miHoverUpdate.Invoke(d, new object[] { h });
            }
            catch { }
        }
    }
    void CallEnd(LinearDrive d, Hand h)
    {
        if (reflReady)
        {
            try
            {
                miEnd.Invoke(d, new object[] { h });
            }
            catch { }
        }
    }
}
