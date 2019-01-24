using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour {


    public List<Transform> rooms = new List<Transform>();
    public GameObject skeletonPrefab;

    public Vector3 GetRandomPositionInRoom(int roomIndex)
    {
        if (roomIndex < 0 || roomIndex >= rooms.Count)
        {
            return Vector3.zero; Debug.LogError("Room index out of range '" + roomIndex + "'");
        }

        return new Vector3(Random.Range(rooms[roomIndex].position.x - rooms[roomIndex].localScale.x / 2, rooms[roomIndex].position.x + rooms[roomIndex].localScale.x / 2),
                            Random.Range(rooms[roomIndex].position.y - rooms[roomIndex].localScale.y / 2, rooms[roomIndex].position.y + rooms[roomIndex].localScale.y / 2),
                            Random.Range(rooms[roomIndex].position.z - rooms[roomIndex].localScale.z / 2, rooms[roomIndex].position.z + rooms[roomIndex].localScale.z / 2));
    }
    public Vector3 GetRoomPosition(int roomIndex)
    {
        if (roomIndex < 0 || roomIndex >= rooms.Count)
        {
            return Vector3.zero; Debug.LogError("Room index out of range '" + roomIndex + "'");
        }

        return rooms[roomIndex].position;
    }
    public float MeasureLargestRoomAxis(int roomIndex)
    {
        if (roomIndex < 0 || roomIndex >= rooms.Count)
        {
            return 0.0f; Debug.LogError("Room index out of range '" + roomIndex + "'");
        }

        return Mathf.Max(rooms[roomIndex].localScale.x, Mathf.Max(rooms[roomIndex].localScale.y, rooms[roomIndex].localScale.z));
    }
    public int GetClosestRoom(Vector3 pos)
    {
        int closest = -1;
        float distance = float.MaxValue;
        for(int i = 0; i < rooms.Count; ++i)
        {
            float newDistance = Vector3.Distance(pos, rooms[i].position);
            if (newDistance < distance)
            {
                closest = i;
                distance = newDistance;
            }
        }

        return closest;
    }
    public void SpawnEnemy(int room = -1)
    {
        if(room == -1)
        {
            room = Random.Range(0, rooms.Count);
        }
        
        Vector3 spawnPosition = GetRandomPositionInRoom(room);
        UnityEngine.AI.NavMeshHit hit;
        UnityEngine.AI.NavMesh.SamplePosition(spawnPosition, out hit, MeasureLargestRoomAxis(room), 1);
        spawnPosition = hit.position;

        Instantiate(skeletonPrefab, spawnPosition, Quaternion.identity);
    }
    // Use this for initialization
    void Start ()
    {
        StartCoroutine(SpawnHorde(hordeSize: 20, room: -1));
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator SpawnHorde(int room = -1, int hordeSize = 5)
    {
        while(hordeSize > 0)
        {
            SpawnEnemy(room);
            hordeSize--;
            yield return new WaitForSeconds(0.25f);
        }
    }
}
