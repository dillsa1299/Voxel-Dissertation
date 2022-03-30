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
    public BiomeGenerator biome;

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

    public bool cullFaces = true;

    public int chunkRenderDistance = 2;

    private GameObject[] chunks;

    private ChunkRenderer[] chunkRenderers;

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
        //InvokeRepeating("UpdateChunks", 10f, 10f);
        //Too expensive, takes approx 1 sec per update. Need to see how to optimise
    }

    public void ToggleCulling()
    {
        cullFaces = !cullFaces;
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
            biome.noiseScale = float.Parse(menuPerlinNoise.text);
        }
        if(menuWaterLevel.text != "")
        {
            biome.waterHeight = int.Parse(menuWaterLevel.text);
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
            biome.noiseScale = float.Parse(optionsPerlinNoise.text);
        }
        if(optionsWaterLevel.text != "")
        {
            biome.waterHeight = int.Parse(optionsWaterLevel.text);
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

    void UpdateChunks()
    {
        if(gameMode == GameMode.Gameplay)
        {
            Vector3 playerPos = player.transform.position;
            playerPos.y = 0;
            int renderDistance = chunkRenderDistance * world.chunkSize;
            foreach(ChunkRenderer chunk in chunkRenderers)
            {
                chunk.UpdateChunk(playerPos, renderDistance);
            }
            
        }
    }

    void GetChunks()
    {
        chunks = GameObject.FindGameObjectsWithTag("Chunk");
        ChunkRenderer[] temp = new ChunkRenderer[chunks.Length];
        int i = 0;
        foreach(GameObject chunk in chunks)
        {
            temp[i] = chunk.GetComponent<ChunkRenderer>();
            i++;
        }
        chunkRenderers = temp;
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
        menuPerlinNoise.placeholder.GetComponent<Text>().text = biome.noiseScale.ToString();
        menuWaterLevel.placeholder.GetComponent<Text>().text = biome.waterHeight.ToString();
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
        optionsPerlinNoise.placeholder.GetComponent<Text>().text = biome.noiseScale.ToString();
        optionsWaterLevel.placeholder.GetComponent<Text>().text = biome.waterHeight.ToString();
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
        GetChunks();
        UpdateChunks();
    }
}
