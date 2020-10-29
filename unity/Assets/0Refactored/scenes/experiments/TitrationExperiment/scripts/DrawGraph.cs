using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DrawGraph : MonoBehaviour, IResetObject
{

    private AcidTitration acidTitrationScript;
    private Dictionary<double, double> result = new Dictionary<double, double>();
    private Dictionary<double, double> equivalenzPoint = new Dictionary<double, double>();

    private RectTransform rect;
    private float height;
    private float width;

    private readonly float maxMl = 100.0f;
    private readonly float ph = 14.0f;

    private int fluidRestrictionSpeed = 10;
    private int counter = 0;

    private LineRenderer[] lineRenderers;
    private LineRenderer equivalenzLine;
    private LineRenderer titrationCurveLine;
    private LineRenderer axisLine;

    [SerializeField] private GameObject burette;

    private OpenBurette buretteScript;
    private double equivalenzPointKey;
    private ShowFluid showFluidScript;

    // Display Panel values
    private double volumeAddedPh = 0f;
    private double volumeAddedMl = 0f;
    
 
    void Start()
    {
        acidTitrationScript = GameObject.Find("TitrationController").GetComponent<AcidTitration>();

        buretteScript = burette.GetComponent<OpenBurette>();
        showFluidScript = ShowFluid.Instance;
        rect = GetComponent<RectTransform>();
        height = rect.rect.height;
        width = rect.rect.width;

        lineRenderers = GetComponentsInChildren<LineRenderer>();
        titrationCurveLine = lineRenderers[0];
        equivalenzLine = lineRenderers[1];
        axisLine = lineRenderers[2];

        DrawAxisLines();
    }

    public void Initialise()
    {
        result = acidTitrationScript.GetResultDictionary();
        equivalenzPoint = acidTitrationScript.GetEquivalenzPointDictionary();
        equivalenzLine.positionCount = 3;


        if (equivalenzPoint.Count > 0)
        {
            int temp = (int)(equivalenzPoint.Keys.First() * 10);

            if ((equivalenzPoint.Keys.First() * 10) % 1 != 0)
                temp += 1;

            equivalenzPointKey = temp / 10.00;
        }
    }

    public void ResetObject()
    {
        titrationCurveLine.positionCount = 0;

        if (equivalenzLine != null)
            equivalenzLine.positionCount = 0;

        counter = 0;
        volumeAddedMl = 0f;
        volumeAddedPh = 0f;

        result.Clear();
        equivalenzPoint.Clear();
    }

    public IEnumerator DrawLine()
    {
        int prev_counter = 0;

        if (result.Count > 0)
        {
            // Titration curve
            foreach (KeyValuePair<double, double> entry in result)
            {
                if (!buretteScript.open)
                    yield return new WaitUntil(() => buretteScript.open);

                if (counter != 0)
                {
                    // Add the ml correctly: Possib. values to add [0.1 ml, 1 ml, 10 ml]
                    if (fluidRestrictionSpeed != 1)
                    {
                        if (counter % (fluidRestrictionSpeed + prev_counter) == 0)
                        {
                            prev_counter = counter;
                            yield return new WaitForSeconds(0.1f);
                        }
                    }
                    else
                    {
                        if (counter % (fluidRestrictionSpeed) == 0)
                        {
                            prev_counter = counter;
                            yield return new WaitForSeconds(0.1f);
                        }
                    }
                }

                float tmpMl = ((float)entry.Key / maxMl) * width;
                float tmpPh = ((float)entry.Value / ph) * height;

                // Renders line
                titrationCurveLine.positionCount = counter + 1;
                titrationCurveLine.SetPosition(counter, new Vector3(tmpMl, tmpPh, 0));
                counter++;

                volumeAddedMl = entry.Key;
                volumeAddedPh = entry.Value;

                showFluidScript.DetermineAnalyteColor((float)entry.Value);

                if (equivalenzPoint.Count > 0)
                {
                    if (entry.Key.Equals(equivalenzPointKey))
                    {
                        // Equivalence point horizontal line
                        foreach (KeyValuePair<double, double> entryEqu in equivalenzPoint)
                        {
                            float tmpMl_ = ((float)entryEqu.Key / maxMl) * width;
                            float tmpPh_ = ((float)entryEqu.Value / ph) * height;

                            float equivalenzlinewidth = (5 / maxMl) * width;

                            equivalenzLine.SetPosition(0, new Vector3(tmpMl_ - equivalenzlinewidth, tmpPh_, 0));
                            equivalenzLine.SetPosition(1, new Vector3(tmpMl_, tmpPh_, 0));
                            equivalenzLine.SetPosition(2, new Vector3(tmpMl_ + equivalenzlinewidth, tmpPh_, 0));
                            break;
                        }
                    }
                }
            }
        }
    }

    public void SetStreamspeed(float value)
    {
        switch (value)
        {
            case 0:
                fluidRestrictionSpeed = 1;
                break;
            case 1:
                fluidRestrictionSpeed = 10;
                break;
            case 2:
                fluidRestrictionSpeed = 100;
                break;
            default:
                break;
        }
    }

    public void GetVolumeAddedPh(MessageArgs args)
    {
        args.value = (float)volumeAddedPh;
    }

    public void GetVolumeAddedMl(MessageArgs args)
    {
        args.value = (float)volumeAddedMl;
    }

    public void DrawAxisLines()
    {
        float heightHalf = height / 2;

        int numberOfPosForTick = 4;
        float tickSpacing = width / 10; // 10ml
        float tickSpacingBig = width / 2; // 50ml
        float counterForTicks = width;
        int tmpCounter = 0;

        List<int> tickHeightSmall = new List<int>() {0, 5, -2, 0};
        List<int> tickHeightBig = new List<int>() {0, 10, -10, 0};
        
        // x-Axis
        while(counterForTicks > 0)
        {
            axisLine.positionCount += numberOfPosForTick;
            for(int i = 0; i < numberOfPosForTick; i++)
            {
                if (counterForTicks % tickSpacingBig == 0 && counterForTicks != 0)
                {
                    axisLine.SetPosition(tmpCounter + i, new Vector3(counterForTicks, tickHeightBig[i], 0));
                }
                else
                {
                    axisLine.SetPosition(tmpCounter + i, new Vector3(counterForTicks, tickHeightSmall[i], 0));
                }
            }
            counterForTicks -= tickSpacing;
            tmpCounter += numberOfPosForTick;
        }

        axisLine.positionCount += 1;
        axisLine.SetPosition(tmpCounter, new Vector3(0, 0, 0));
        tmpCounter++;
        
        tickSpacing = height / 14;
        counterForTicks = tickSpacing;

        // y-Axis
        while (counterForTicks <= height)
        {
            axisLine.positionCount += numberOfPosForTick;
            for (int i = 0; i < numberOfPosForTick; i++)
            {
                if (Mathf.Approximately(counterForTicks, heightHalf))
                {
                    axisLine.SetPosition(tmpCounter + i, new Vector3(tickHeightBig[i], counterForTicks, 0));
                }
                else
                {
                    axisLine.SetPosition(tmpCounter + i, new Vector3(tickHeightSmall[i], counterForTicks, 0));
                }
            }
            counterForTicks += tickSpacing;
            tmpCounter += numberOfPosForTick;
        }
    }
}


