using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMetrics : MonoBehaviour
{
    [SerializeField] private Text _fpsText;
    [SerializeField] private Text _tricountText;
    [SerializeField] private float _hudRefreshRate = 0.2f;

    private float _timer;
 
    private void Update()
    {
        if (Time.unscaledTime > _timer)
        {
            int fps = (int)(1f / Time.unscaledDeltaTime);
            _fpsText.text = "FPS: " + fps + "  ";
            _tricountText.text = "Tri Count: " + (UnityEditor.UnityStats.triangles) + "  ";
            _timer = Time.unscaledTime + _hudRefreshRate;
        }
    }
}
