//
//Author: Tobias St�ckl
//
using UnityEngine;

public class ToggleUIElement : MonoBehaviour
{
    [SerializeField]
    private GameObject UiElement;
    private bool _isActive = true;


    public void ToggleElement()
    {
        _isActive = !_isActive;
        UiElement.SetActive(_isActive);
    }
}