using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Crosstales.FB;
using System.IO;

public class MenuGenerationInterface : MonoBehaviour
{
    public struct GenerationSettings
    {
        public chunkSize chunkSize;
        public float islandThreshHold;
        public int nChunks;
        public int selectedImagePoolIndex;
        public bool useImagePool;
        public bool useElevationSystem;
        public bool usePerlin;
        public bool useCustomImage;
        public bool useElevationSlow;
        public bool useRivers;
    }
    public struct chunkSize
    {
        public int nPointsInputted;
        public int Xsize, Ysize;
        // should probably include camera presets for this
        public int cameraYPos;
    }
    public struct cameraSettings
    {
        public int yPos;
        // 248 is what it used to be for the small size. 
    }

    #region references 
    private VoronoiGeneration generator;

    public Slider islandThreshHold;
    public Dropdown sizeDropdown;
    public GameObject imagePoolDropdown;
    public Toggle perlinToggle;
    #endregion

    private GenerationSettings generationSettings;
    private chunkSize small, medium, large;
    private List<chunkSize> sizeList = new List<chunkSize>();
    private int selectedSizeIndex;
    // Start is called before the first frame update
    void Start()
    {
        generator = FindObjectOfType<VoronoiGeneration>();
        resetValues();
        #region size values initialtion
        small.nPointsInputted = 1000;
        small.Xsize = 250;
        small.Ysize = 250;
        small.cameraYPos = 248;

        medium.nPointsInputted = 5000;
        medium.Xsize = 1250;
        medium.Ysize = 1250;
        medium.cameraYPos = 750;

        large.nPointsInputted = 10000;
        large.Xsize = 2500;
        large.Ysize = 2500;
        large.cameraYPos = 1500;

        sizeList.Add(small);
        sizeList.Add(medium);
        sizeList.Add(large);
        selectedSizeIndex = 0;
        #endregion
        
    }

    public void resetValues()
    {
        generationSettings.islandThreshHold = 0.5f;
        generationSettings.useImagePool = true;
        generationSettings.useElevationSystem = false;
        generationSettings.useElevationSlow = false;
        generationSettings.useRivers = false;
        generationSettings.usePerlin = false;
        generationSettings.useCustomImage = false;
        generationSettings.nChunks = 1;
        generationSettings.selectedImagePoolIndex = 0;
        selectedSizeIndex = 0;
    }
    public void OnEnable()
    {
        //resetValues();
    }

    public void setIslandThreshold(float input)
    {
        generationSettings.islandThreshHold = input;
    }
    public void setNChunks(float input)
    {
        generationSettings.nChunks =Mathf.FloorToInt(input);
    }
    public void setSelectedSize()
    {
        selectedSizeIndex = sizeDropdown.value;
    }
    public void setSelectedImagePool(int input)
    {
        generationSettings.selectedImagePoolIndex = input;
    }
    public void setUseImagePool(bool input)
    {
        generationSettings.useImagePool = input;
        imagePoolDropdown.SetActive(input);
        if (input)
        {
            perlinToggle.isOn = false;
        }
    }
    public void setUseElevation(bool input)
    {
        generationSettings.useElevationSystem = input;
    }
    public void setUseElevationSlow(bool input)
    {
        generationSettings.useElevationSlow = input;
    }
    public void setUseRivers(bool input)
    {
        generationSettings.useRivers = input;
    }
    public void setUsePerlin(bool input)
    {
        generationSettings.usePerlin = input;
        if(generationSettings.useImagePool && input)
        {
            //generationSettings.useImagePool = false;
            imagePoolDropdown.GetComponentInParent<Toggle>().isOn = false;
        }
    }
    public void exit()
    {
        Application.Quit();
    }
    public void startGeneration()
    {
        //this is simply a check to stop me accidently trying to generate a larger size as it'll take at least 1hour 40min to finish generating
        if(generationSettings.useElevationSystem)
        {
            selectedSizeIndex = 0;
        }

        generationSettings.chunkSize = sizeList[selectedSizeIndex];
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, generationSettings.chunkSize.cameraYPos, Camera.main.transform.position.z);
        generator.SetGenerationSettings(generationSettings);
        generator.StartGeneration();
        gameObject.SetActive(false);
    }


    public void selectFile()
    {
        string path = FileBrowser.OpenSingleFile();
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(path))
        {
            fileData = File.ReadAllBytes(path);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); 

            generator.customImage = tex;
            generationSettings.useCustomImage = true;
        }
        
    }
}
