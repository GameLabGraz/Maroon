using GEAR.Localization.Text;
using UnityEngine;

public class PC_ErrorDisplay : MonoBehaviour
{
    public GameObject ErrorText;
    public float DisplayTime = 1f;
    private float _currentTime = -1f;
    
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!(_currentTime > 0f)) return;
        _currentTime -= Time.deltaTime;
            
        if(_currentTime < 0f)
            gameObject.SetActive(false);
    }

    
    public void ShowError(string localizationCode)
    {
        ErrorText.GetComponent<LocalizedTMP>().Key = localizationCode;
        ErrorText.GetComponent<LocalizedTMP>().UpdateLocalizedText();

        gameObject.SetActive(true);
        _currentTime = DisplayTime;
    }
}
