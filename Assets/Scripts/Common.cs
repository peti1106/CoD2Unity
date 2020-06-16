using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

public static class Utils
{
    public static string CoD2Path = "Assets\\Resources\\";

    public static string CoD2IwdPath = Application.streamingAssetsPath;

    public static Dictionary<string, byte[]> maps = new Dictionary<string, byte[]>();
    public static Dictionary<string, byte[]> materials = new Dictionary<string, byte[]>();
    public static Dictionary<string, byte[]> images = new Dictionary<string, byte[]>();
    public static Dictionary<string, byte[]> xmodel = new Dictionary<string, byte[]>();
    public static Dictionary<string, byte[]> xmodelparts = new Dictionary<string, byte[]>();
    public static Dictionary<string, byte[]> xmodelsurfs = new Dictionary<string, byte[]>();

    public static string ReadStringTerminated(this BinaryReader br, byte terminatingChar = 0x00)
    {
        char[] rawName = new char[64];

        int i = 0;
        for (i = 0; i < 64; i++)
        {
            if (br.PeekChar() == terminatingChar)
                break;

            rawName[i] = br.ReadChar();
        }

        return new string(rawName).Replace("\0", string.Empty).Trim();
    }

    public static string ReadStringLength(this BinaryReader br, uint length)
    {
        char[] rawName = new char[length];

        rawName = br.ReadChars((int)length);

        return new string(rawName).Replace("\0", string.Empty).Trim();
    }

    public static void ReadZipContentsToDictionaries()
    {
        string[] iwdNames = Directory.GetFiles(CoD2IwdPath, "*.iwd", SearchOption.TopDirectoryOnly);

        foreach (string iwdName in iwdNames)
        {
            Debug.Log(iwdName);
            using (ZipArchive archive = ZipFile.OpenRead(iwdName))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (!string.IsNullOrEmpty(entry.Name))
                    {
                        //Debug.Log(entry.FullName);
                        AddEntryToDictionaryByType(entry);
                    }
                }
            }
        }

        /*foreach (KeyValuePair<string, ZipArchiveEntry> entry in maps)
        {
            Debug.Log(entry.Key);
        }*/
    }

    private static string GetFolderName(string entryFullName)
    {
        int found = entryFullName.IndexOf("/");
        return (found > 0) ? entryFullName.Substring(0, found) : "";
    }

    private static void AddEntryToDictionaryByType(ZipArchiveEntry entry)
    {
        string folderName = GetFolderName(entry.FullName);

        switch (folderName)
        {
            case "maps":
                CreateNewOrUpdateExisting(maps, entry.Name, ReadStream(entry.Open()));
                break;
            case "materials":
                CreateNewOrUpdateExisting(materials, entry.Name, ReadStream(entry.Open()));
                break;
            case "images":
                CreateNewOrUpdateExisting(images, entry.Name, ReadStream(entry.Open()));
                break;
            case "xmodel":
                CreateNewOrUpdateExisting(xmodel, entry.Name, ReadStream(entry.Open()));
                break;
            case "xmodelparts":
                CreateNewOrUpdateExisting(xmodelparts, entry.Name, ReadStream(entry.Open()));
                break;
            case "xmodelsurfs":
                CreateNewOrUpdateExisting(xmodelsurfs, entry.Name, ReadStream(entry.Open()));
                break;
        }
    }

    public static void CreateNewOrUpdateExisting<TKey, TValue>(this IDictionary<TKey, TValue> map, TKey key, TValue value)
    {
        if (map.ContainsKey(key))
        {
            map[key] = value;
        }
        else
        {
            map.Add(key, value);
        }
    }

    private static byte[] ReadStream(Stream stream)
    {
        using (var ms = new MemoryStream())
        {
            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }
}