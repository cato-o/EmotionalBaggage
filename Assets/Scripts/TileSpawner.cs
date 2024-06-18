 using System.Collections.Generic;
using UnityEngine;

namespace EmotionalBaggage {
public class TileSpawner : MonoBehaviour
{
    [SerializeField]
    private int tileStartCount = 10;
    [SerializeField]
    private int minimumStraightTiles = 3;
    [SerializeField]
    private int maximumStraightTiles = 10;
    [SerializeField]
    private GameObject startingTile;
    [SerializeField]
    private GameObject startingRamp;
    [SerializeField]
    private List<GameObject> turnTiles;
    [SerializeField]
    private List<GameObject> obstacles;
    [SerializeField]
    private List<GameObject> forwardTiles;

    [SerializeField]
    private float obstacleSpawnFrequency;
    [SerializeField]
    private float minimumObstacleSpawnDistance;

    [SerializeField]
    private float rampSpawnFrequency = 0.2f;

    //three floor definitons
    private float floor1YValue = 0f;
    private float floor2YValue = 2.215f;
    private float floor3YValue = 4.445f;
    private float currentFloor = 1;

    private Vector3 currentTileLocation = Vector3.zero;
    private Vector3 currentTileDirection = Vector3.forward;
    private Vector3 savedTileLocation = Vector3.zero;
    private Vector3 savedTilePlacementScale = Vector3.zero;
    private Vector3 lastObstaclePosition;
    private GameObject prevTile;
    private List<GameObject> currentTiles;
    private List<GameObject> currentObstacles;
    private List<GameObject> liarTiles;
    private bool decoy = false;
    private bool isStart = false;
    private TileType lastTileType;

    

    private void Start(){
        isStart = true;
        currentTiles = new List<GameObject>();
        liarTiles = new List<GameObject>();
        currentObstacles = new List<GameObject>();

        Random.InitState(System.DateTime.Now.Millisecond);

        //this sets up the starting stretch of tile
        for (int i = 0; i < tileStartCount - 2; ++i) {
            SpawnTile(startingTile.GetComponent<Tile>());
        }

        SpawnTile(startingRamp.GetComponent<Tile>());

        for (int i = 0; i < 2; ++i) {
            SpawnTile(startingTile.GetComponent<Tile>());
        }
    

        // first turn tile spawn
        Tile firstTurn = SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>();
        SpawnTile(firstTurn, false);
        if (firstTurn.type == TileType.LEFT) {
            currentTileDirection = Vector3.left;
        } else if (firstTurn.type == TileType.RIGHT) {
            currentTileDirection = Vector3.right;
        }

        // saves the tile location before it's modified for the decoy tiles
        savedTileLocation = currentTileLocation;

        Vector3 tilePlacementScale;
        tilePlacementScale = Vector3.Scale((prevTile.GetComponent<Renderer>().bounds.size - (Vector3.one * 2)) + (Vector3.one * startingTile.GetComponent<BoxCollider>().size.z / 2), currentTileDirection);
        currentTileLocation += tilePlacementScale;

        // spawn decoy tiles
        for (int i = 0; i < 15; ++i) {
            decoy = true;
            SpawnTile(startingTile.GetComponent<Tile>());
            decoy = false;
        }
        
        // set back save
        currentTileLocation = savedTileLocation;
        isStart = false;

    }

    private void SpawnTile(Tile tile, bool spawnObstacle = false) {

        // don't spawn up ramp if on floor 3
        if ((tile.type == TileType.UPRAMP && currentFloor == 3) ||
            (tile.type == TileType.DOWNRAMP && currentFloor == 1)) {
            return;
        }

        
        if (tile.type == TileType.STRAIGHT && !isStart) {
            spawnObstacle = Random.value < obstacleSpawnFrequency;
        }

        Quaternion newTileRotation = tile.gameObject.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);
        
        if (tile.type == TileType.DOWNRAMP) {
            currentTileLocation.y -= 2.215f;
        }

        prevTile = GameObject.Instantiate(tile.gameObject, currentTileLocation, newTileRotation);

        if (decoy) {
            liarTiles.Add(prevTile);
        }
        else {
            currentTiles.Add(prevTile);
            if (spawnObstacle) SpawnObstacle();
        }


        if (tile.type == TileType.STRAIGHT) {
            currentTileLocation += Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size, currentTileDirection);
        }
        
        if (tile.type == TileType.UPRAMP) {
            currentTileLocation += Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size, currentTileDirection);
            if (currentFloor == 1) {
                currentTileLocation.y = floor2YValue;
                currentFloor = 2;
            } else {
                currentTileLocation.y = floor3YValue;
                currentFloor = 3;
            }
        }

        if (tile.type == TileType.DOWNRAMP) {
            currentTileLocation += Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size, currentTileDirection);
            if (currentFloor == 3) {
                currentTileLocation.y = floor2YValue;
                currentFloor = 2;
            } else {
                currentTileLocation.y = floor1YValue;
                currentFloor = 1;
            }
            
        }

        
    }

    private void iHateLiars() {
        // don't mind the silly name, deletes the decoys
         while (liarTiles.Count != 0) {
            GameObject tile = liarTiles[0];
            liarTiles.RemoveAt(0);
            Destroy(tile);
        }
    }

    private void DeletePreviousTiles() {   
        while (currentTiles.Count != 1) {
            GameObject tile = currentTiles[0];
            currentTiles.RemoveAt(0);
            Destroy(tile);
        }

        while (currentObstacles.Count != 0) {
            GameObject obstacle = currentObstacles[0];
            currentObstacles.RemoveAt(0);
            Destroy(obstacle);
        }
    }

    public void AddNewDirection(Vector3 direction) {
        currentTileDirection = direction;
        DeletePreviousTiles();
        iHateLiars();

        Vector3 tilePlacementScale;
        tilePlacementScale = Vector3.Scale((prevTile.GetComponent<Renderer>().bounds.size - (Vector3.one * 2)) + (Vector3.one * startingTile.GetComponent<BoxCollider>().size.z / 2), currentTileDirection);

        currentTileLocation += tilePlacementScale;

        SpawnTile(startingTile.GetComponent<Tile>()); // starting tile after turn to avoid any weird mesh overlaps

        int currentPathLength = Random.Range(minimumStraightTiles, maximumStraightTiles);

        for (int i = 0; i < currentPathLength; ++i) {
            SpawnTile(SelectRandomForwardTile(forwardTiles).GetComponent<Tile>());
        }

        Tile theTurn = SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>();
        SpawnTile(theTurn, false);
        if (theTurn.type == TileType.LEFT) {
            currentTileDirection = Quaternion.Euler(0, -90, 0) * currentTileDirection;
        } else if (theTurn.type == TileType.RIGHT) {
            currentTileDirection = Quaternion.Euler(0, 90, 0) * currentTileDirection;

        }

        savedTilePlacementScale = tilePlacementScale;
        savedTileLocation = currentTileLocation;

        tilePlacementScale = Vector3.Scale((prevTile.GetComponent<Renderer>().bounds.size - (Vector3.one * 2)) + (Vector3.one * startingTile.GetComponent<BoxCollider>().size.z / 2), currentTileDirection);
        currentTileLocation += tilePlacementScale;

        // spawn decoy tiles
        for (int i = 0; i < 15; ++i) {
            decoy = true;
            SpawnTile(startingTile.GetComponent<Tile>());
            decoy = false;
        }

        tilePlacementScale = savedTilePlacementScale;
        currentTileLocation = savedTileLocation;
    }


    private void SpawnObstacle() {

        GameObject obstaclePrefab = SelectRandomGameObjectFromList(obstacles);

        // Check if the last spawned obstacle is the same as the current obstacle prefab
        if (currentObstacles.Count > 0) {
            GameObject lastObstacle = currentObstacles[currentObstacles.Count - 1];
            if ((lastObstacle.CompareTag(obstaclePrefab.tag)) && (obstaclePrefab.tag == "AnimatorObstacle")) {
                return;
            }
        }

        Quaternion newObjectRotation = obstaclePrefab.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);
        Vector3 obstacleSpawnPosition = currentTileLocation;


        if (obstaclePrefab.tag == "Dodger") {
            float xOffset;
            if (Random.value < 0.5f) {
                xOffset = Random.Range(-1.0f, -0.4f);
            } else {
                xOffset = Random.Range(0.4f, 1.0f);
            }

            obstacleSpawnPosition.x += xOffset;
        }

        if (Vector3.Distance(obstacleSpawnPosition, lastObstaclePosition) >= minimumObstacleSpawnDistance) {
            GameObject obstacle = Instantiate(obstaclePrefab, obstacleSpawnPosition, newObjectRotation);
            currentObstacles.Add(obstacle);
            lastObstaclePosition = obstacleSpawnPosition; 
        }
    }
    private GameObject SelectRandomGameObjectFromList(List<GameObject> list) {
        if (list.Count == 0) return null;

        return list[Random.Range(0, list.Count)];

    }

    private GameObject SelectRandomForwardTile(List<GameObject> list){
        if (Random.value < rampSpawnFrequency)
        {
            return (Random.value < 0.5f) ? forwardTiles[1] : forwardTiles[2]; // randomly pick a ramp
        }
        else 
        {
            return forwardTiles[0]; // pick a straight
        }
    }
        
}
}
