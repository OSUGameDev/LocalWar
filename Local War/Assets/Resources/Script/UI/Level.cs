using UnityEngine;
using UnityEditor;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;

/// <summary>
/// For Internal Use
/// </summary>
public class LevelInfoCollection
{
    [XmlArray]
    public LevelInfo[] levels;

    public LevelInfoCollection()
    {
        levels = null;
    }

    public LevelInfoCollection(LevelInfo[] levels)
    {
        this.levels = levels;
    }

    /// <summary>
    /// Default name of the levelCollection
    /// </summary>
    public const string DefaultFileCacheName = "Levels.xml";

    /// <summary>
    /// Location of the default level collection in the editor
    /// </summary>
    public const string DefaultEditorFileCache = "Assets\\Editor\\" + DefaultFileCacheName;

    /// <summary>
    /// Location of teh default level collection in the player
    /// </summary>
    public static string DefaultPlayerFileCache
    {
        get {
            return Path.Combine(Application.dataPath, DefaultFileCacheName);
        }
    }

    /// <summary>
    /// Get the path to the default level cache
    /// </summary>
    public static string DefaultLevelCache
    {
        get {
#if UNITY_EDITOR
            return DefaultEditorFileCache;
#else
            return DefaultPlayerFileCache;
#endif
        }
    }

    /// <summary>
    /// Get the Levels in the detault cache
    /// </summary>
    public static LevelInfo[] DefaultLevels
    {
        get {
            return LoadLevels(DefaultLevelCache);
        }
        set {
            SaveLevels(value, DefaultLevelCache);
        }
    }

    /// <summary>
    /// Internal Xml serializer
    /// </summary>
    private static XmlSerializer serializer = new XmlSerializer(typeof(LevelInfoCollection), new[] { typeof(LevelInfo), typeof(LevelInfo[]) });

    /// <summary>
    /// Deserialize LevelInfo from a path
    /// </summary>
    /// <param name="path">File to deserialize</param>
    /// <returns>Contents of the file</returns>
    public static LevelInfo[] LoadLevels(string path)
    {
        try
        {

            using (var fp = new StreamReader(path, System.Text.Encoding.UTF8))
            {
                var levelCollection = (LevelInfoCollection)serializer.Deserialize(fp);
                return levelCollection.levels;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Could not open " + path + e.Message);
            return new LevelInfo[0];
        }
    }

    /// <summary>
    /// Serialize LevelInfo to a path
    /// </summary>
    /// <param name="values">Values to serialize</param>
    /// <param name="path">File to serialize to</param>
    public static void SaveLevels(LevelInfo[] values, string path)
    {
        using (var fp = new StreamWriter(path, false, System.Text.Encoding.UTF8))
        {
            serializer.Serialize(fp, new LevelInfoCollection(values));
        }
    }
}

/// <summary>
/// Information needed for the main menu Level Loading
/// </summary>
public class LevelInfo
{
    /// <summary>
    /// Name of the level
    /// </summary>
    [XmlAttribute]
    public string Name;

    /// <summary>
    /// Path to the level
    /// </summary>
    [XmlElement]
    public string path;

#if UNITY_EDITOR
    public static implicit operator LevelInfo(SceneAsset item)
    {
        var toReturn = new LevelInfo();
        toReturn.Name = item.name;
        toReturn.path = AssetDatabase.GetAssetPath(item);
        return toReturn;
    }
#endif
}