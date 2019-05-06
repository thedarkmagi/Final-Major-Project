using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        
    }
    public struct chunkSize
    {
        public int nPointsInputted;
        public int Xsize, Ysize; 
        // should probably include camera presets for this
    }

    #region references 
    private VoronoiGeneration generator;

    public Slider islandThreshHold;
    public Dropdown sizeDropdown;
    #endregion

    private GenerationSettings generationSettings;
    private chunkSize small, medium, large;
    private List<chunkSize> sizeList = new List<chunkSize>();
    private int selectedSizeIndex;
    // Start is called before the first frame update
    void Start()
    {
        generator = FindObjectOfType<VoronoiGeneration>();
        generationSettings.islandThreshHold = 0.5f;
        generationSettings.useImagePool = true;
        generationSettings.useElevationSystem = false;
        generationSettings.nChunks = 1;
        generationSettings.selectedImagePoolIndex = 0;
        selectedSizeIndex = 0;
        #region size values initialtion
        small.nPointsInputted = 1000;
        small.Xsize = 250;
        small.Ysize = 250;

        medium.nPointsInputted = 5000;
        medium.Xsize = 1250;
        medium.Ysize = 1250;

        large.nPointsInputted = 10000;
        large.Xsize = 2500;
        large.Ysize = 2500;

        sizeList.Add(small);
        sizeList.Add(medium);
        sizeList.Add(large);
        selectedSizeIndex = 0;
        #endregion

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
    }
    public void setUseElevation(bool input)
    {
        generationSettings.useElevationSystem = input;
    }
    public void startGeneration()
    {
        //this is simply a check to stop me accidently trying to generate a larger size as it'll take at least 1hour 40min to finish generating
        if(generationSettings.useElevationSystem)
        {
            selectedSizeIndex = 0;
        }

        generationSettings.chunkSize = sizeList[selectedSizeIndex];
        generator.SetGenerationSettings(generationSettings);
        generator.StartGeneration();
    }
}
