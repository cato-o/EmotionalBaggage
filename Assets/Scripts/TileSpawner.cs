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
    private List<GameObject> turnTiles;
    [SerializeField]
    private List<GameObject> obstacles;

    [SerializeField]
    private float obstacleSpawnFrequency;
    [SerializeField]
    private float minimumObstacleSpawnDistance;
    private float maximumObstacleFrequency = 1f;
    private float ObstacleSpawnIncreaseRate = 0.01f;
    private float initialObstacleSpawnFrequency = 0.5f;

    private float rampSpawnFrequency = 0.2f;

    //three floor definitons
    private float floor1YValue = 0f;
    private float upRampOneToTwoYValue = 1.11f;
    private float floor2YValue = 2.215f;
    private float upRampTwoToThreeYValue = 3.33f;
    private float floor3YValue = 4.445f;

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

    

    private void Start(){
        currentTiles = new List<GameObject>();
        liarTiles = new List<GameObject>();
        currentObstacles = new List<GameObject>();
        obstacleSpawnFrequency = initialObstacleSpawnFrequency;

        Random.InitState(System.DateTime.Now.Millisecond);

        for (int i = 0; i < tileStartCount; ++i) {
            SpawnTile(startingTile.GetComponent<Tile>());
        }

        Tile firstTurn = SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>();
        SpawnTile(firstTurn, false);
        if (firstTurn.type == TileType.LEFT) {
            currentTileDirection = Vector3.left;
        } else if (firstTurn.type == TileType.RIGHT) {
            currentTileDirection = Vector3.right;
        }

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
        
        currentTileLocation = savedTileLocation;

    }

    public void Update(){
        if (obstacleSpawnFrequency < maximumObstacleFrequency)
            {
                obstacleSpawnFrequency += Time.deltaTime * ObstacleSpawnIncreaseRate;
            }
    }
    
    private void SpawnTile(Tile tile, bool spawnObstacle = false) {

        Quaternion newTileRotation = tile.gameObject.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);

        prevTile = GameObject.Instantiate(tile.gameObject, currentTileLocation, newTileRotation);
        if (decoy) {
            liarTiles.Add(prevTile);
        }
        else {
            currentTiles.Add(prevTile);
        }

        if (spawnObstacle) SpawnObstacle();

        if (tile.type == TileType.STRAIGHT) {
            currentTileLocation += Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size, currentTileDirection);
        }
        
    }

    private void iHateLiars() {
        Debug.Log("deleting the decoys");
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
        Debug.Log("direction: " + direction);
        currentTileDirection = direction;
        DeletePreviousTiles();
        iHateLiars();

        Vector3 tilePlacementScale;
        tilePlacementScale = Vector3.Scale((prevTile.GetComponent<Renderer>().bounds.size - (Vector3.one * 2)) + (Vector3.one * startingTile.GetComponent<BoxCollider>().size.z / 2), currentTileDirection);

        currentTileLocation += tilePlacementScale;


        int currentPathLength = Random.Range(minimumStraightTiles, maximumStraightTiles);

        for (int i = 0; i < currentPathLength; ++i) {
            SpawnTile(startingTile.GetComponent<Tile>(), (i == 0)? false : true);
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
        if (Random.value > obstacleSpawnFrequency) return;

        GameObject obstaclePrefab = SelectRandomGameObjectFromList(obstacles);

        // Check if the last spawned obstacle is the same as the current obstacle prefab
        if (currentObstacles.Count > 0) {
            GameObject lastObstacle = currentObstacles[currentObstacles.Count - 1];
            if (lastObstacle.CompareTag(obstaclePrefab.tag) && lastObstacle.name == "AnimatorObstacle") {
                Debug.Log("Skipping spawn of consecutive same obstacle: " + obstaclePrefab.name);
                return;
            }
        }

        Quaternion newObjectRotation = obstaclePrefab.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);
        Vector3 obstacleSpawnPosition = currentTileLocation;

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
}

}