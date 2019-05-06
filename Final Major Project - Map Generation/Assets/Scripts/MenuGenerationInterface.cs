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




    public void setIslandThreshold()
    {
         generationSettings.islandThreshHold = islandThreshHold.value;
    }
    public void setSelectedSize()
    {
        selectedSizeIndex = sizeDropdown.value;
    }
    public void startGeneration()
    {
        generationSettings.chunkSize = sizeList[selectedSizeIndex];
        generator.SetGenerationSettings(generationSettings);
        generator.StartGeneration();
    }
}
