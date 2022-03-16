using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    This script controls the states of the game
*/

public class MainController : MonoBehaviour
{

    public GameObject menuUI;
    public GameObject optionsUI;
    public GameObject statsHUD;
    public GameObject player;
    private PlayerMovement playerMovement;
    public Camera mainCam;
    public bool isPlaying = false;

    public World world;

    public InputField menuWorldSize;
    public InputField menuChunkSize;
    public InputField menuChunkHeight;
    public InputField menuPerlinNoise;
    public InputField menuWaterLevel;
    public InputField optionsWorldSize;
    public InputField optionsChunkSize;
    public InputField optionsChunkHeight;
    public InputField optionsPerlinNoise;
    public InputField optionsWaterLevel;


    enum GameMode
    {
        
        MainMenu,
        Options,
        Gameplay
    }

    GameMode gameMode = GameMode.MainMenu;

    // Start is called before the first frame update
    void Start()
    {
        StartMainMenu();
    }

    public void ResetCameraPos()
    {
        Vector3 tpPos = new Vector3((world.chunkSize*world.worldSizeInChunks)/2, world.chunkHeight*1.5f,(world.chunkSize*world.worldSizeInChunks)/2);
        player.GetComponent<PlayerMovement>().teleportPlayer(tpPos); //TELEPORT TO MIDDLE OF MAP
        mainCam.GetComponent<MouseLook>().xRotation = 45f;
    }

    public void setSizesMenu()
    {
        if(menuWorldSize.text != "")
        {
            world.worldSizeInChunks = int.Parse(menuWorldSize.text);
        }
        if(menuChunkSize.text != "")
        {
            world.chunkSize = int.Parse(menuChunkSize.text);
        }
        if(menuChunkHeight.text != "")
        {
            world.chunkHeight = int.Parse(menuChunkHeight.text);
        }
        if(menuPerlinNoise.text != "")
        {
            world.noiseScale = float.Parse(menuPerlinNoise.text);
        }
        if(menuWaterLevel.text != "")
        {
            world.waterHeight = int.Parse(menuWaterLevel.text);
        }
    }

    public void setSizesOptions()
    {
        if(optionsWorldSize.text != "")
        {
            world.worldSizeInChunks = int.Parse(optionsWorldSize.text);
        }
        if(optionsChunkSize.text != "")
        {
            world.chunkSize = int.Parse(optionsChunkSize.text);
        }
        if(optionsChunkHeight.text != "")
        {
            world.chunkHeight = int.Parse(optionsChunkHeight.text);
        }
        if(optionsPerlinNoise.text != "")
        {
            world.noiseScale = float.Parse(optionsPerlinNoise.text);
        }
        if(optionsWaterLevel.text != "")
        {
            world.waterHeight = int.Parse(optionsWaterLevel.text);
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch(gameMode)
        {
            case GameMode.MainMenu:
                UpdateMainMenu();
                break;
            case GameMode.Options:
                UpdateOptions();
                break;
            case GameMode.Gameplay:
                UpdateGameplay();
                break;
        }
    }

    void UpdateMainMenu()
    {

    }

    void UpdateOptions()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            StartGameplay();
        }
    }

    void UpdateGameplay()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            StartOptions();
        }
    }

    public void StartMainMenu()
    {
        gameMode                        = GameMode.MainMenu;
        menuUI.gameObject.SetActive(true);
        optionsUI.gameObject.SetActive(false);
        statsHUD.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None; //Unlocks Cursor
        isPlaying = false;

        menuWorldSize.placeholder.GetComponent<Text>().text = world.worldSizeInChunks.ToString();
        menuChunkSize.placeholder.GetComponent<Text>().text = world.chunkSize.ToString();
        menuChunkHeight.placeholder.GetComponent<Text>().text = world.chunkHeight.ToString();
        menuPerlinNoise.placeholder.GetComponent<Text>().text = world.noiseScale.ToString();
        menuWaterLevel.placeholder.GetComponent<Text>().text = world.waterHeight.ToString();
    }

    public void StartOptions()
    {
        gameMode                        = GameMode.Options;
        menuUI.gameObject.SetActive(false);
        optionsUI.gameObject.SetActive(true);
        statsHUD.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None; //Unlocks Cursor
        isPlaying = false;

        optionsWorldSize.placeholder.GetComponent<Text>().text = world.worldSizeInChunks.ToString();
        optionsChunkSize.placeholder.GetComponent<Text>().text = world.chunkSize.ToString();
        optionsChunkHeight.placeholder.GetComponent<Text>().text = world.chunkHeight.ToString();
        optionsPerlinNoise.placeholder.GetComponent<Text>().text = world.noiseScale.ToString();
        optionsWaterLevel.placeholder.GetComponent<Text>().text = world.waterHeight.ToString();
        optionsWorldSize.text = "";
        optionsChunkSize.text = "";
        optionsChunkHeight.text = "";
        optionsPerlinNoise.text = "";
        optionsWaterLevel.text = "";
    }
    public void StartGameplay()
    {
        gameMode                        = GameMode.Gameplay;
        menuUI.gameObject.SetActive(false);
        optionsUI.gameObject.SetActive(false);
        statsHUD.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked; //Locks Cursor
        isPlaying = true;
    }
}
