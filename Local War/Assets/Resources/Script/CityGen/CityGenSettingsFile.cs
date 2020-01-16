using UnityEngine;
using UnityEditor;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;


public class MapGenSettings
{
    /// <summary>
    /// Number of blocks in city
    /// </summary>
    public int citySize = 10;

    /// <summary>
    /// Number of building locations per block
    /// </summary>
    public int blockSize = 10;

    /// <summary>
    /// Min size of a building in X and Z dimension
    /// </summary>
    public int buildMinSize = 3;

    /// <summary>
    /// Max size of a building in X and Z dimension
    /// </summary>
    public int buildMaxSize = 5;

    /// <summary>
    /// Size of Roads
    /// </summary>
    public int roadSize = 2;

    /// <summary>
    /// Empty space around buildings, min 0 max .49
    /// </summary>
    public float buildingSpacing = .4f;

    /// <summary>
    /// Wall thickness
    /// </summary>
    public float buildingThickness = .05f;

    /// <summary>
    /// Mininum building height
    /// </summary>
    public float baseHeight = 2.5f;

    /// <summary>
    ///  Range above minimum building height
    /// </summary>
    public float baseRange = 3.5f;

    /// <summary>
    /// Adds a % chance of a building to be extraHeight taller
    /// </summary>
    public float extraTallBuildings = 0.02f;

    /// <summary>
    /// Extra height in case of building being taller
    /// </summary>
    public float extraHeight = 10.0f;

    /// <summary>
    /// Change to add noise to ground level, also raises ground slightly, not reccommended but fun
    /// </summary>
    public bool groundNoise = true;

    /// <summary>
    /// Adds noise to building height, also increases building height, dependent on citySize
    /// </summary>
    public bool buildingHeightNoise = true;

    /// <summary>
    /// Changes the frequency of the noise, lower number = smoother terrain
    /// </summary>
    public float refinement = 0.01f;

    /// <summary>
    /// Changes the magnitude, lower numbers = flatter terrain
    /// </summary>
    public float magnitude = 35f;

    public int xSize
    {
        get {
            return (blockSize * citySize) + (citySize * roadSize) + (roadSize);
        }
    }

    public int zSize
    {
        get {
            return (blockSize * citySize) + (citySize * roadSize) + (roadSize);
        }
    }

    public MapGenSettings()
    {
    }

    static MapGenSettings()
    {
        CustomSerializer.AddSerializer<MapGenSettings>();
    }
}
