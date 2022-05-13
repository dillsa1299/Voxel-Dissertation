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

    public GameObject player;
    public GameObject playerCamera;
    public float pathSpeed = 0.1f;
    public int pathRadius = 64;
    public int pathHeight = 32;
    public float cameraAngle = 45f;
    public float pathTimer;
    private Vector3 pathCenter;
    public float pathAngle = 0f;
    private Vector3 pathStartPos;

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
            if (pathTimer > 60f) //1 minute of recording
            {
                SaveResults();
                pathTimer = 0;
                pathAngle = 0;
                recording = false;
            }
            pathCenter = new Vector3(0,pathHeight,0);
            recordingText.text = "Recording Performance";
            times.Add(pathTimer);
            frameTimes.Add(Time.unscaledDeltaTime);
            triCounts.Add(UnityEditor.UnityStats.triangles);

            FollowPath();

            if(Input.GetKeyDown(KeyCode.R))
            {
                SaveResults();
                recording = false;
            }
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                recording = true;
                pathTimer = 0;
                pathAngle = 0;
                FollowPath();
                pathStartPos = player.transform.position;
            }
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

    void FollowPath() //For moving camera whilst recording benchmark
     {
        pathTimer += Time.deltaTime;
        float x = -Mathf.Cos(pathTimer*pathSpeed) * pathRadius;
        float z = Mathf.Sin(pathTimer*pathSpeed) * pathRadius;
        Vector3 pos = new Vector3(x, 0, z);
        player.transform.position = pos + pathCenter;
        pathAngle = ((pathTimer * pathSpeed) * pathRadius);
        playerCamera.transform.eulerAngles = new Vector3(cameraAngle,(pathAngle),0);
     }

    private void SaveResults()
    {
        string folderPath = Application.dataPath + "/_Performance Records";
        string path = Application.dataPath + "/_Performance Records/" + filename + ".csv";

        string stringToWrite = "Time (s),Frame Time (ms),Triangle Count\n";
        for(int i = 0; i < times.Count; i++)
        {
            stringToWrite = stringToWrite +
                (times[i]).ToString() + "," +
                (frameTimes[i]*1000).ToString() + "," +
                ((float)triCounts[i]).ToString() + "\n";
        }

        if(!AssetDatabase.IsValidFolder(folderPath)) Directory.CreateDirectory(folderPath);
        File.WriteAllText(path, stringToWrite);

        Debug.Log("Saved Results to:\n" + path);        
    }
}
