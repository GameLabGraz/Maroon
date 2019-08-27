using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Maroon.Util;
using Object = UnityEngine.Object;
using LearningContent = Antares.Evaluation.LearningContent;

public static class AssessmentUIExtensions
{
  private const string ResourceFolder = "CustomUI/";
  private const string PanelPrefab = ResourceFolder + "Panel";
  private const string TextPrefab = ResourceFolder + "Text";
  private const string TablePrefab = ResourceFolder + "Table";
  private const string TableRowPrefab = ResourceFolder + "TableRow";
  private const string InputPrefab = ResourceFolder + "Input";
  private const string ButtonPrefab = ResourceFolder + "Button";
  private const string ImagePrefab = ResourceFolder + "Image";
  private const string DropDownPrefab = ResourceFolder + "DropDown";
  private const string SeperatorPrefab = ResourceFolder + "Seperator";

  private static void SetLayout(this GameObject obj, LearningContent.LayoutMode mode)
  {
    switch (mode)
    {
      case LearningContent.LayoutMode.Rows:
        var verticalLayout = obj.AddComponent<VerticalLayoutGroup>();
        verticalLayout.childControlHeight = verticalLayout.childControlWidth = true;
        verticalLayout.padding = new RectOffset(10, 10, 10, 10);
        verticalLayout.spacing = 10;
        break;
      case LearningContent.LayoutMode.Columns:
        var horizontalLayout = obj.AddComponent<HorizontalLayoutGroup>();
        horizontalLayout.childControlHeight = horizontalLayout.childControlWidth = true;
        horizontalLayout.spacing = 10;
        break;
      case LearningContent.LayoutMode.Coordinates:
      default:
        Debug.LogWarning($"Unable to handle layout mode: {mode}");
        break;
    }
  }

  public static void LoadUIElement(this LearningContent.Division div, Transform parent)
  {
    var panelObject = Object.Instantiate(Resources.Load(PanelPrefab), parent, false) as GameObject;
    if (panelObject == null)
      return;

    panelObject.SetLayout(div.Layout);

    foreach (var item in div.Items)
    {
      switch (item)
      {
        case LearningContent.Table table:
          table.LoadUIElement(panelObject.transform);
          break;
        case LearningContent.Text text:
          text.LoadUIElement(panelObject.transform);
          break;
        case LearningContent.Selection selection:
          selection.LoadUIElement(panelObject.transform);
          break;
        case LearningContent.Input input:
          input.LoadUIElement(panelObject.transform);
          break;
        case LearningContent.Button button:
          button.LoadUIElement(panelObject.transform);
          break;
        case LearningContent.Image image:
          image.LoadUIElement(panelObject.transform);
          break;
      }
    }
  }

  public static void LoadUIElement(this LearningContent.Text text, Transform parent)
  {
    var textObject = Object.Instantiate(Resources.Load(TextPrefab), parent, false) as GameObject;
    if (textObject == null)
      return;

    var textComponent = textObject.GetComponent<UnityEngine.UI.Text>();
    textComponent.text = text.Content;
    textComponent.resizeTextForBestFit = true;

    switch (text.Type)
    {
      case LearningContent.TextType.Heading:
        textComponent.fontSize = textComponent.resizeTextMaxSize = 36;
        textComponent.fontStyle = FontStyle.Bold;
        break;
      case LearningContent.TextType.Subheading:
        textComponent.fontSize = textComponent.resizeTextMaxSize = 28;
        textComponent.fontStyle = FontStyle.Bold;
        break;
      case LearningContent.TextType.Text:
        textComponent.fontSize = textComponent.resizeTextMaxSize = 20;
        break;
      default:
        Debug.LogWarning($"Unable to handle text type: {text.Type}");
        break;
    }
  }

  public static void LoadUIElement(this LearningContent.Input input, Transform parent)
  {
    var inputObject = Object.Instantiate(Resources.Load(InputPrefab), parent, false) as GameObject;
    if (inputObject == null)
      return;

    inputObject.GetComponent<UnityEngine.UI.InputField>().lineType = InputField.LineType.MultiLineNewline;
  }

  public static void LoadUIElement(this LearningContent.Button button, Transform parent)
  {
    var buttonObject = Object.Instantiate(Resources.Load(ButtonPrefab), parent, false) as GameObject;
    if(buttonObject == null)
      return;

    var buttonComponent = buttonObject.GetComponent<UnityEngine.UI.Button>();
    buttonComponent.gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = button.Caption;
  }

  public static async void LoadUIElement(this LearningContent.Image image, Transform parent)
  {
    var imageObject = Object.Instantiate(Resources.Load(ImagePrefab), parent, false) as GameObject;
    if (imageObject == null)
      return;

    var imageComponent = imageObject.GetComponent<UnityEngine.UI.Image>();

    // Test Image
    imageComponent.sprite = await RemoteTexture.GetSprite("https://github.com/GamesResearchTUG/Maroon/blob/master/Assets/Images/logo.png?raw=true");
  }

  public static void LoadUIElement(this LearningContent.Selection selection, Transform parent)
  {
    var dropDownObject = Object.Instantiate(Resources.Load(DropDownPrefab), parent, false) as GameObject;
    if (dropDownObject == null)
      return;

    var dropDownComponent = dropDownObject.GetComponent<Dropdown>();
    dropDownComponent.AddOptions(
      selection.Options.Select(option => new Dropdown.OptionData(option.Label)).ToList());
  }

  public static void LoadUIElement(this LearningContent.Table table, Transform parent)
  {
    var tableOject = Object.Instantiate(Resources.Load(TablePrefab), parent, false) as GameObject;
    if (tableOject == null)
      return;

    foreach (var row in table.Rows)
    {
      var rowObject = Object.Instantiate(Resources.Load(TableRowPrefab), tableOject.transform, false) as GameObject;
      if(rowObject == null)
        return;

      foreach (var cell in row.Cells)
      {
        cell.LoadUIElement(rowObject.transform);
      }
    }
  }
}
