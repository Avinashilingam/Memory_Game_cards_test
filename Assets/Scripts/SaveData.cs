using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SaveData
{
    public int score;
    public int rows;
    public int cols;
    public List<int> cardIDs;        // shuffled deck
    public List<int> matchedIndices; // which cards are matched
}