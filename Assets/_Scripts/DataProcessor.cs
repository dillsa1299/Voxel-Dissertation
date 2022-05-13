using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DataProcessor : MonoBehaviour
{
    public string filePath = "/Sphere/Sphere_FC_1";

    private List<string> Times = new List<string>();
    private List<string> FrameTimes = new List<string>();
    private List<string> TriangleCounts = new List<string>();

    public float[] NewTimes;
    private float[] NewFrameTimes;
    private float[] NewTriangleCounts;
    private float[] sortedFrameTimes;

    public float step = 0.1f;


    // Start is called before the first frame update
    private void Start()
    {
        int arraySize = Mathf.RoundToInt(60/step);
        NewTimes = new float[arraySize];
        NewFrameTimes = new float[arraySize];
        NewTriangleCounts = new float[arraySize];
    }
    
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            LoadFile();
        }
    }

    private void LoadFile()
    {
        Times.Clear();
        FrameTimes.Clear();
        TriangleCounts.Clear();

        using(var reader = new StreamReader(Application.dataPath + "/_Performance Records" + filePath + ".csv"))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                Times.Add(values[0]);
                FrameTimes.Add(values[1]);
                TriangleCounts.Add(values[2]);
            }
        }
        int length = Times.Count;
        float pointer = step;
        int counter = 0;

        for (int i = 0; i < 60; i++)
        {
            //Debug.Log(i);
            NewTimes[i] = 0;
            NewFrameTimes[i] = 0;
            NewTriangleCounts[i] = 0;
        }
        //length = 65449;
        for (int i = 1; i < length; i++)
        {
            int pos = Mathf.RoundToInt(pointer*(1/step))-1;
            float currentTime = float.Parse(Times[i]);
            if ((currentTime > pointer-step) && (currentTime < pointer))
            {
                counter++;
                NewTimes[pos] = (Mathf.Round(pointer * Mathf.Pow(10, 1))-1)/10;
                NewFrameTimes[pos] = (NewFrameTimes[pos] + float.Parse(FrameTimes[i]))/counter;
                NewTriangleCounts[pos] = float.Parse(TriangleCounts[i]);//(NewTriangleCounts[pos] + float.Parse(TriangleCounts[i]))/counter;
            }
            else
            {
                pointer += step;
                counter = 1;
                if (pos > 599)
                {
                    pos = 599;
                }
                //Debug.Log(pos);
                NewTimes[pos] = (Mathf.Round(pointer * Mathf.Pow(10, 1))-1)/10;
                NewFrameTimes[pos] = (NewFrameTimes[pos])/counter;
                NewTriangleCounts[pos] = (NewTriangleCounts[pos] + float.Parse(FrameTimes[i]))/counter;
            }
        }
        NewTimes[(Mathf.RoundToInt(60/step))-1] = 60;

        sortedFrameTimes = NewFrameTimes;
        Array.Sort(sortedFrameTimes);
        float fullSum = 0;
        float ninetyFifth = 0;
        float ninetyNinth = 0;
        for (int i = 0; i < sortedFrameTimes.Length; i++)
        {
            fullSum += sortedFrameTimes[i]*1000;
        }
        for (int i = Mathf.RoundToInt(sortedFrameTimes.Length*0.95f); i < sortedFrameTimes.Length; i++)
        {
            ninetyFifth += sortedFrameTimes[i]*1000;
        }
        for (int i = Mathf.RoundToInt(sortedFrameTimes.Length*0.99f); i < sortedFrameTimes.Length; i++)
        {
            ninetyNinth += sortedFrameTimes[i]*1000;
        }

        string folderPath = Application.dataPath + "/_Performance Records";
        string path = Application.dataPath + "/_Performance Records" + filePath + "_Processed.csv";
        string stringToWrite = "Time (s),Frame Time (ms),Triangle Count," 
            + (fullSum/sortedFrameTimes.Length).ToString() + ","
            + (ninetyFifth/(sortedFrameTimes.Length*0.05)).ToString() + ","
            + (ninetyNinth/(sortedFrameTimes.Length*0.01)).ToString() + "\n";;
        for(int i = 0; i < NewTimes.Length; i++)
        {
            stringToWrite = stringToWrite +
                (NewTimes[i]).ToString() + "," +
                (NewFrameTimes[i]*1000).ToString() + "," +
                (NewTriangleCounts[i]).ToString() + "\n";
        }

        if(!AssetDatabase.IsValidFolder(folderPath)) Directory.CreateDirectory(folderPath);
        File.WriteAllText(path, stringToWrite);

        Debug.Log("Saved Results to:\n" + path); 
    }
}
