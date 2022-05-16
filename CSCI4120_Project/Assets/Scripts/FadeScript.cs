using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour
{
    public Image panel;

    float _time = 0f;
    float _fadeTime = 3f;

    public void Fade()
    {
        StartCoroutine(FadeFlow());
    }

    IEnumerator FadeFlow()
    {
        panel.gameObject.SetActive(true);
        Color alpha = panel.color;

        _time = 0f;
        while (alpha.a < 1f)
        {
            _time += Time.deltaTime / _fadeTime;
            alpha.a = Mathf.Lerp(0, 1, _time);
            panel.color = alpha;
            yield return null;
        }

        _time = 0f;
        yield return new WaitForSeconds(3f);

        while (alpha.a > 0f)
        {
            _time += Time.deltaTime / _fadeTime;
            alpha.a = Mathf.Lerp(1, 0, _time);
            panel.color = alpha;
            yield return null;
        }
        panel.gameObject.SetActive(false);
        yield return null;
    }
}
