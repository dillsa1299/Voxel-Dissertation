using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIMetrics : MonoBehaviour
{
    [SerializeField] private Text fpsText;
    [SerializeField] private Text tricountText;
    [SerializeField] private Text recordingText;
    [SerializeField] private float hudRefreshRate = 0.5f;

    private float timer;

    private List<float> times = new List<float>();
    private List<float> frameTimes = new List<float>();
    private List<int> triCounts = new List<int>();

    private bool recording = false;

    public string filename = "test";
 
    private void Update()
    {        
        if (recording)
        {
            recordingText.text = "Recording Performance";
            times.Add(Time.time);
            frameTimes.Add(Time.unscaledDeltaTime);
            triCounts.Add(UnityEditor.UnityStats.triangles);

            if(Input.GetKeyDown(KeyCode.R))
            {
                SaveResults();
                recording = false;
            }
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.R)) recording = true;
            recordingText.text = "";
        }

        if (Time.unscaledTime > timer)
        {
            int fps = (int)(1f / Time.unscaledDeltaTime); //Time.unscaledDeltaTime;
            fpsText.text = "FPS: " + fps + "  ";
            tricountText.text = "Tri Count: " + (UnityEditor.UnityStats.triangles) + "  ";
            timer = Time.unscaledTime + hudRefreshRate;
        }
    }

    private void SaveResults()
    {
        string folderPath = Application.dataPath + "/_Performance Records";
        string path = Application.dataPath + "/_Performance Records/" + filename + ".csv";

        string stringToWrite = "Time (s),Frame Time (ms),Triangle Count (100,000's)\n";
        for(int i = 0; i < times.Count; i++)
        {
            stringToWrite = stringToWrite +
                (times[i]-times[0]).ToString() + "," +
                (frameTimes[i]*1000).ToString() + "," +
                ((float)triCounts[i]/100000).ToString() + "\n";
        }

        if(!AssetDatabase.IsValidFolder(folderPath)) Directory.CreateDirectory(folderPath);
        File.WriteAllText(path, stringToWrite);

        Debug.Log("Saved Results to:\n" + path);        
    }
}
