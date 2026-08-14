using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GEAR.Localization.Text;

public class BlockInfoPanel : MonoBehaviour
{
    public LocalizedTMP infoText;
    public ScrollRect scrollRect;

    public string generalInfoKey = "logicGateSimulator.info.general";
    public string deleteInfoKey = "logicGateSimulator.info.delete";

    void Start()
    {
        ShowGeneralInfo();
    }

    public void ShowGeneralInfo()
    {
        ShowInfo(generalInfoKey);
    }

    public void ShowDeleteInfo()
    {
        ShowInfo(deleteInfoKey);
    }

    public void ShowBlockInfo(GameObject blockPrefab)
    {
        if (blockPrefab == null)
        {
            ShowGeneralInfo();
            return;
        }

        string blockName = blockPrefab.name;
        string informationKey = generalInfoKey;

        if (blockName.StartsWith("AND"))
        {
            informationKey = "logicGateSimulator.info.and";
        }
        else if (blockName.StartsWith("OR"))
        {
            informationKey = "logicGateSimulator.info.or";
        }
        else if (blockName.StartsWith("NOT"))
        {
            informationKey = "logicGateSimulator.info.not";
        }
        else if (blockName.StartsWith("BUFFER"))
        {
            informationKey = "logicGateSimulator.info.buffer";
        }
        else if (blockName.StartsWith("NAND"))
        {
            informationKey = "logicGateSimulator.info.nand";
        }
        else if (blockName.StartsWith("NOR"))
        {
            informationKey = "logicGateSimulator.info.nor";
        }
        else if (blockName.StartsWith("XOR"))
        {
            informationKey = "logicGateSimulator.info.xor";
        }
        else if (blockName.StartsWith("XNOR"))
        {
            informationKey = "logicGateSimulator.info.xnor";
        }
        else if (blockName.StartsWith("Button"))
        {
            informationKey = "logicGateSimulator.info.button";
        }
        else if (blockName.StartsWith("Switch"))
        {
            informationKey = "logicGateSimulator.info.switch";
        }
        else if (blockName.StartsWith("1Block"))
        {
            informationKey = "logicGateSimulator.info.oneBlock";
        }
        else if (blockName.StartsWith("Splitter"))
        {
            informationKey = "logicGateSimulator.info.splitter";
        }
        else if (blockName.StartsWith("Blue"))
        {
            informationKey = "logicGateSimulator.info.blueLed";
        }
        else if (blockName.StartsWith("Red"))
        {
            informationKey = "logicGateSimulator.info.redLed";
        }
        else if (blockName.StartsWith("Green"))
        {
            informationKey = "logicGateSimulator.info.greenLed";
        }
        else if (blockName.StartsWith("White"))
        {
            informationKey = "logicGateSimulator.info.whiteLed";
        }
        else if (blockName.StartsWith("Yellow"))
        {
            informationKey = "logicGateSimulator.info.yellowLed";
        }

        ShowInfo(informationKey);
    }

    void ShowInfo(string informationKey)
    {
        if (infoText == null)
        {
            return;
        }

        infoText.Key = informationKey;
        Canvas.ForceUpdateCanvases();

        if (scrollRect != null)
        {
            scrollRect.StopMovement();
            scrollRect.verticalNormalizedPosition = 1f;
        }
    }
}