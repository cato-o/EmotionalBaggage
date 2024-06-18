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
    private Vector3 lastObstaclePosition;
    private GameObject prevTile;
    private List<GameObject> currentTiles;
    private List<GameObject> currentObstacles;

    

    private void Start(){
        currentTiles = new List<GameObject>();
        currentObstacles = new List<GameObject>();
        obstacleSpawnFrequency = initialObstacleSpawnFrequency;

        Random.InitState(System.DateTime.Now.Millisecond);

        for (int i = 0; i < tileStartCount; ++i) {
            SpawnTile(startingTile.GetComponent<Tile>());
        }

        SpawnTile(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>());
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
        currentTiles.Add(prevTile);

        if (spawnObstacle) SpawnObstacle();

        if (tile.type == TileType.STRAIGHT) {
            currentTileLocation += Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size, currentTileDirection);
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

        Vector3 tilePlacementScale;
        tilePlacementScale = Vector3.Scale((prevTile.GetComponent<Renderer>().bounds.size - (Vector3.one * 2)) + (Vector3.one * startingTile.GetComponent<BoxCollider>().size.z / 2), currentTileDirection);

        currentTileLocation += tilePlacementScale;

        int currentPathLength = Random.Range(minimumStraightTiles, maximumStraightTiles);

        for (int i = 0; i < currentPathLength; ++i) {
            SpawnTile(startingTile.GetComponent<Tile>(), (i == 0)? false : true);
        }

       // SecondTileSpawn = 

        SpawnTile(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>(), false);


    }

    //public bool Clicked() {
      //  return true;
    //}

    private void SpawnObstacle(){
         if (Random.value > obstacleSpawnFrequency) return;

        GameObject obstaclePrefab = SelectRandomGameObjectFromList(obstacles);
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