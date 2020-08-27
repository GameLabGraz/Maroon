using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class DrawGraph : MonoBehaviour, IResetObject
{


    private AcidTitration acidTitrationScript;
    private Dictionary<double, double> result = new Dictionary<double, double>();
    private Dictionary<double, double> equivalenzPoint = new Dictionary<double, double>();

    private RectTransform rect;
    private float height;
    private float width;

    private float maxMl = 200.0f;
    private float ph = 14.0f;

    private int fluidRestrictionSpeed = 10;

    GameObject lineRenderer;
    LineRenderer[] equivalenzLine;
    LineRenderer line;

    private int counter = 0;

    public bool permitted = false;
    public GameObject burette;
    OpenBurette buretteScript;
    double equivalenzPointKey;

    public GameObject pipet;
    private ShowFluid showFluidScript;

    private double volumeAddedPh = 0f;
    private double volumeAddedMl = 0f;
    
    
    // Use this for initialization
    void Start()
    {
        acidTitrationScript = GameObject.Find("TitrationController").GetComponent<AcidTitration>();
        //result = acidTitrationScript.getResultDictonary();
        lineRenderer = GameObject.Find("LineRenderer");
        line = lineRenderer.GetComponent<LineRenderer>();
        buretteScript = burette.GetComponent<OpenBurette>();
        showFluidScript = pipet.GetComponent<ShowFluid>();
        rect = GetComponent<RectTransform>();
        height = rect.rect.height;
        width = rect.rect.width;
    }

    public void InitLine()
    {
        result = acidTitrationScript.getResultDictionary();
        equivalenzPoint = acidTitrationScript.getEquivalenzPointDictionary();
        equivalenzLine = GetComponentsInChildren<LineRenderer>();
        equivalenzLine[1].positionCount = 3;

        if (equivalenzPoint.Count > 0)
        {
            int temp = (int)(equivalenzPoint.Keys.First() * 10);

            if ((equivalenzPoint.Keys.First() * 10) % 1 != 0)
                temp += 1;

            equivalenzPointKey = temp / 10.00;
        }

        if (result.Count > 0)
        {
            // show the start concentration of the analyte
            //volumeAddedScript.changeVolumeAddedPanel(result.Keys.First().ToString("F2"), result.Values.First().ToString("F2"));
        }
    }

    public void ResetObject()
    {
        line.positionCount = 0;

        if (equivalenzLine != null)
            equivalenzLine[1].positionCount = 0;

        counter = 0;

        volumeAddedMl = 0f;
        volumeAddedPh = 0f;

        result.Clear();
        equivalenzPoint.Clear();
    }

    public IEnumerator DrawLine(/*ShowVolumeAdded volumeAddedScript,*/)
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
                line.positionCount = counter + 1;
                line.SetPosition(counter, new Vector3(tmpMl, tmpPh, 0));
                counter++;

                //volumeAddedScript.changeVolumeAddedPanel(entry.Key.ToString("F2"), entry.Value.ToString("F2"));
                volumeAddedMl = entry.Key;
                volumeAddedPh = entry.Value;

                showFluidScript.determineAnalytColor((float)entry.Value);

                if (equivalenzPoint.Count > 0)
                {
                    if (entry.Key.Equals(equivalenzPointKey))
                    {
                        // Equivalence point horizontal line
                        foreach (KeyValuePair<double, double> entryEqu in equivalenzPoint)
                        {
                            float tmpMl_ = ((float)entryEqu.Key / maxMl) * width;
                            float tmpPh_ = ((float)entryEqu.Value / ph) * height;

                            float equivalenzlinewidth = (10 / maxMl) * width;

                            equivalenzLine[1].SetPosition(0, new Vector3(tmpMl_ - equivalenzlinewidth, tmpPh_, 0));
                            equivalenzLine[1].SetPosition(1, new Vector3(tmpMl_, tmpPh_, 0));
                            equivalenzLine[1].SetPosition(2, new Vector3(tmpMl_ + equivalenzlinewidth, tmpPh_, 0));
                            break;
                        }
                    }
                }
            }
        }
    }

    public void setStreamspeed(float value)
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

    public void getVolumeAddedPh(MessageArgs args)
    {
        args.value = (float)volumeAddedPh;
    }

    public void getVolumeAddedMl(MessageArgs args)
    {
        args.value = (float)volumeAddedMl;
    }
}


