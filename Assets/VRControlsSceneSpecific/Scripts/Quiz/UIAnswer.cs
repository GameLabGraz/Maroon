using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIAnswer : UISelectable
{

    public void playCorrectAnimation(float duration)
    {
        StartCoroutine(blink(Color.green, Color.white, 1f, duration));   
    }

    public void playWrongAnimation(float duration)
    {
        StartCoroutine(blink(Color.red, Color.white, 1f, duration));
    }

    IEnumerator blink(Color a, Color b, float period, float maxDuration)
    {
        float duration = 0f;

        Color color;
        while (duration < maxDuration)
        {
            float t;
            if ((int) (duration / period) % 2 == 0)
                t = (duration / period) % 1;
            else
                t = 1f - (duration / period) % 1;

            color = Color.Lerp(a, b, t);
            button.image.color = color;
            duration += Time.deltaTime;
            yield return null;
        }
    }
}
