using UnityEngine;

public static class UserInterfaceContent
{
    public static Object TextPrefab => Resources.Load("Text");
    public static Object InputPrefab => Resources.Load("InputField");
    public static Object SliderPrefab => Resources.Load("SliderGroup");
    public static Object TogglePrefab => Resources.Load("ToggleGroup");
}
