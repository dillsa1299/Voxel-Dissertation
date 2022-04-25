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
    private Vector3Int currentChunkCenterPos;

    public World world;
    public BiomeGenerator biome;
    public NoiseData noiseData;

    public bool isPlaying = false;
    public float chunkUpdateTime = 1f;


    //UI References
    public GameObject Menu;
    public GameObject Options;
    public GameObject PerlinOptions;
    public GameObject PerlinAdvancedOptions;
    public GameObject statsHUD;
    public InputField Options_RenderDistance;
    public InputField Options_ChunkHeight;
    public InputField PerlinOptions_OffsetX;
    public InputField PerlinOptions_OffsetY;
    public InputField PerlinOptions_WaterHeight;
    public InputField PerlinOptions_NoiseScale;
    public InputField PerlinAdvancedOptions_Persistance;
    public InputField PerlinAdvancedOptions_Redistribution;
    public InputField PerlinAdvancedOptions_Exponent;
    public InputField PerlinAdvancedOptions_NoiseZoom;
    public InputField PerlinAdvancedOptions_Octaves;

    enum GameMode
    {
        
        Menu,
        Options,
        PerlinOptions,
        PerlinAdvancedOptions,
        Gameplay
    }

    GameMode gameMode = GameMode.Menu;

    // Start is called before the first frame update
    void Start()
    {
        StartMenu();
        currentChunkCenterPos = Vector3Int.RoundToInt(player.transform.position);
    }

    public void ToggleCulling()
    {
        world.cullFaces = !world.cullFaces;
    }

    public void RegeneratePerlin()
    {
        world.RegeneratePerlin(Vector3Int.RoundToInt(player.transform.position));
    }

    public void RegenerateSpheres()
    {
        world.RegenerateSpheres(Vector3Int.RoundToInt(player.transform.position));
    }

    public void RegenerateGrid()
    {
        world.RegenerateGrid(Vector3Int.RoundToInt(player.transform.position));
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
            case GameMode.Menu:
                UpdateMenu();
                break;
            case GameMode.Options:
                UpdateOptions();
                break;
            case GameMode.PerlinOptions:
                UpdatePerlinOptions();
                break;
            case GameMode.PerlinAdvancedOptions:
                UpdatePerlinAdvancedOptions();
                break;
            case GameMode.Gameplay:
                UpdateGameplay();
                break;
        }
    }

    void UpdateMenu()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            StartGameplay();
        }
    }
    void UpdateOptions()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            StartMenu();
        }
    }
    void UpdatePerlinOptions()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            StartMenu();
        }
    }
    void UpdatePerlinAdvancedOptions()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            StartPerlinOptions();
        }
    }

    void UpdateGameplay()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            StartMenu();
        }
    }

    public void StartMenu()
    {
        gameMode                        = GameMode.Menu;
        Menu.gameObject.SetActive(true);
        Options.gameObject.SetActive(false);
        PerlinOptions.gameObject.SetActive(false);
        PerlinAdvancedOptions.gameObject.SetActive(false);
        statsHUD.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None; //Unlocks Cursor
        isPlaying = false;
    }

    public void StartOptions()
    {
        gameMode                        = GameMode.Options;
        Menu.gameObject.SetActive(false);
        Options.gameObject.SetActive(true);
        PerlinOptions.gameObject.SetActive(false);
        PerlinAdvancedOptions.gameObject.SetActive(false);
        statsHUD.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None; //Unlocks Cursor
        isPlaying = false;

        Options_RenderDistance.placeholder.GetComponent<Text>().text = world.chunkRenderDistance.ToString();
        Options_RenderDistance.text = "";
        Options_ChunkHeight.placeholder.GetComponent<Text>().text = world.chunkHeight.ToString();
        Options_ChunkHeight.text = "";
    }

    public void StartPerlinOptions()
    {
        gameMode                        = GameMode.PerlinOptions;
        Menu.gameObject.SetActive(false);
        Options.gameObject.SetActive(false);
        PerlinOptions.gameObject.SetActive(true);
        PerlinAdvancedOptions.gameObject.SetActive(false);
        statsHUD.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None; //Unlocks Cursor
        isPlaying = false;

        PerlinOptions_OffsetX.placeholder.GetComponent<Text>().text = world.terrainOffset.x.ToString();
        PerlinOptions_OffsetX.text = "";
        PerlinOptions_OffsetY.placeholder.GetComponent<Text>().text = world.terrainOffset.y.ToString();
        PerlinOptions_OffsetY.text = "";
        PerlinOptions_WaterHeight.placeholder.GetComponent<Text>().text = biome.waterLevel.ToString();
        PerlinOptions_WaterHeight.text = "";
        PerlinOptions_NoiseScale.placeholder.GetComponent<Text>().text = biome.noiseScale.ToString();
        PerlinOptions_NoiseScale.text = "";
    }

    public void StartPerlinAdvancedOptions()
    {
        gameMode                        = GameMode.PerlinAdvancedOptions;
        Menu.gameObject.SetActive(false);
        Options.gameObject.SetActive(false);
        PerlinOptions.gameObject.SetActive(false);
        PerlinAdvancedOptions.gameObject.SetActive(true);
        statsHUD.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None; //Unlocks Cursor
        isPlaying = false;

        PerlinAdvancedOptions_Persistance.placeholder.GetComponent<Text>().text = noiseData.persistance.ToString();
        PerlinAdvancedOptions_Persistance.text = "";
        PerlinAdvancedOptions_Redistribution.placeholder.GetComponent<Text>().text = noiseData.redistributionModifier.ToString();
        PerlinAdvancedOptions_Redistribution.text = "";
        PerlinAdvancedOptions_Exponent.placeholder.GetComponent<Text>().text = noiseData.exponent.ToString();
        PerlinAdvancedOptions_Exponent.text = "";
        PerlinAdvancedOptions_NoiseZoom.placeholder.GetComponent<Text>().text = noiseData.noiseZoom.ToString();
        PerlinAdvancedOptions_NoiseZoom.text = "";
        PerlinAdvancedOptions_Octaves.placeholder.GetComponent<Text>().text = noiseData.octaves.ToString();
        PerlinAdvancedOptions_Octaves.text = "";
    }

    public void StartGameplay()
    {
        gameMode                        = GameMode.Gameplay;
        Menu.gameObject.SetActive(false);
        Options.gameObject.SetActive(false);
        PerlinOptions.gameObject.SetActive(false);
        PerlinAdvancedOptions.gameObject.SetActive(false);
        statsHUD.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked; //Locks Cursor
        isPlaying = true;
        StartChunkUpdates();
    }

    //Sets variables to values in input boxes
    public void SetInputOptions()
    {
        if(Options_ChunkHeight.text != "")
        {
            world.chunkHeight = int.Parse(Options_ChunkHeight.text.ToString());
        }
        if(Options_RenderDistance.text != "")
        {
            world.chunkRenderDistance = int.Parse(Options_RenderDistance.text);
        }
    }
    public void SetInputPerlin()
    {
        if(PerlinOptions_OffsetX.text != "")
        {
            world.terrainOffset.x = int.Parse(PerlinOptions_OffsetX.text.ToString());
        }
        if(PerlinOptions_OffsetY.text != "")
        {
            world.terrainOffset.y = int.Parse(PerlinOptions_OffsetY.text.ToString());
        }
        if(PerlinOptions_WaterHeight.text != "")
        {
            biome.waterLevel = int.Parse(PerlinOptions_WaterHeight.text);
        }
        if(PerlinOptions_NoiseScale.text != "")
        {
            biome.noiseScale = float.Parse(PerlinOptions_NoiseScale.text);
        }
    }

    public void SetInputPerlinAdvanced()
    {
        if(PerlinAdvancedOptions_Persistance.text != "")
        {
            noiseData.persistance = float.Parse(PerlinAdvancedOptions_Persistance.text.ToString());
        }
        if(PerlinAdvancedOptions_Redistribution.text != "")
        {
            noiseData.redistributionModifier = float.Parse(PerlinAdvancedOptions_Redistribution.text.ToString());
        }
        if(PerlinAdvancedOptions_Exponent.text != "")
        {
            noiseData.exponent = float.Parse(PerlinAdvancedOptions_Exponent.text.ToString());
        }
        if(PerlinAdvancedOptions_NoiseZoom.text != "")
        {
            noiseData.noiseZoom = float.Parse(PerlinAdvancedOptions_NoiseZoom.text.ToString());
        }
        if(PerlinAdvancedOptions_Octaves.text != "")
        {
            noiseData.octaves = int.Parse(PerlinAdvancedOptions_Octaves.text.ToString());
        }
    }
}
