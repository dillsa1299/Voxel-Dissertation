using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    This script controls the states of the game
*/

public class MainController : MonoBehaviour
{

    
    public GameObject player;
    public Vector3Int playerChunkPos;
    private Vector3Int currentChunkCenterPos = Vector3Int.zero; //Camera starts at (0,0)

    public World world;
    public BiomeGenerator biome;

    public bool isPlaying = false;
    public bool cullFaces = true;
    public float chunkUpdateTime = 1;


    //UI References
    public GameObject menuUI;
    public GameObject optionsUI;
    public GameObject statsHUD;
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

    public void ToggleCulling()
    {
        cullFaces = !cullFaces;
    }

    public void StartChunkUpdates()
    {
        SetCurrentChunkCoordinates();
        StopAllCoroutines();
        StartCoroutine(CheckIfLoadNextChunk());
    }

    IEnumerator CheckIfLoadNextChunk()
    {
        yield return new WaitForSeconds(chunkUpdateTime);
        if (
            Mathf.Abs(currentChunkCenterPos.x - player.transform.position.x) > world.chunkSize ||
            Mathf.Abs(currentChunkCenterPos.z - player.transform.position.z) > world.chunkSize ||
            (Mathf.Abs(playerChunkPos.y - player.transform.position.y) > world.chunkHeight)
            )
        {
            world.LoadNextChunks(player);
            StartCoroutine(CheckIfLoadNextChunk());
        }
        else
        {
            StartCoroutine(CheckIfLoadNextChunk());
        }
    }

    private void SetCurrentChunkCoordinates()
    {
        playerChunkPos = WorldDataHelper.ChunkPositionFromVoxelCoords(world, Vector3Int.RoundToInt(player.transform.position));
        currentChunkCenterPos.x = playerChunkPos.x + world.chunkSize / 2;
        currentChunkCenterPos.z = playerChunkPos.z + world.chunkSize / 2;
    }

    /*
        State controllers below
    */
    void Update()
    {
        switch(gameMode)
        {
            case GameMode.MainMenu:
                break;
            case GameMode.Options:
                UpdateOptions();
                break;
            case GameMode.Gameplay:
                UpdateGameplay();
                break;
        }
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
        StartChunkUpdates();
    }

    //Sets variables to values in input boxes
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
}
