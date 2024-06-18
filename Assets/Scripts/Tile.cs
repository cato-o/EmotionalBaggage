using UnityEngine;


namespace EmotionalBaggage {

public enum TileType {
    STRAIGHT,
    UPRAMP,
    DOWNRAMP,
    LEFT,
    RIGHT
}

/// <summary>
/// Defines the attributes of a tile.
/// </summary>
public class Tile : MonoBehaviour
{
    public TileType type;
    public Transform pivot;
}

}