using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBuilder : MonoBehaviour {

	int roomSpawnRound = 0;
	int minRooms = 2;
	int maxRooms = 3;

	GameObject roomParent;
	List<GameObject> confirmedRooms;
	List<GameObject> nextRoomSpawns;

	[SerializeField] GameObject[] entryRooms;
	[SerializeField] GameObject[] rooms;

	[SerializeField] GameObject wallPanel;
	[SerializeField] GameObject doorway;
	
	

	void Awake() {
		roomParent = new GameObject("RoomParent");
		confirmedRooms = new List<GameObject>();
		nextRoomSpawns = new List<GameObject>();

		//for (int i = 0; i <= maxRooms; i++) {
			//if (i == 0) {
				SpawnEntryRoom();
			//}
			//else if (i > 0 && i < minRooms) {
			//	SpawnAdditionalRooms(false);
			//}
			//else if (i >= minRooms && i < maxRooms) {
			//	SpawnAdditionalRooms(true);
			//}
			//else {
			//	SpawnFinalRooms();
			//}
		//}
	}


    void SpawnEntryRoom() {
		GameObject entry = Instantiate(entryRooms[Random.Range(0, entryRooms.Length)], Vector3.zero, Quaternion.identity);
		entry.transform.SetParent(roomParent.transform);
		confirmedRooms.Add(entry);

		//if (entry.GetComponent<RoomScriptReader>().demandsConnection) {
			ForceFirstCurrentRoomConnectionRandomly(entry.GetComponent<RoomScriptReader>().connectionPoints);
		//}

		EndRoomSpawnRound();
	}


	void SpawnAdditionalRooms(bool minRoomsReached) {//If the minimum number of rooms HAS NOT been reached, they DEMAND additional connection(s) (thus, FALSE). Later rooms are optional (thus, TRUE).
		if (!minRoomsReached) {
			EvaluateRoomSpawn(true, false);
		}
		else {
			EvaluateRoomSpawn(false, false);
		}

		EndRoomSpawnRound();
	}

	private void EvaluateRoomSpawn (bool forceRoomToHaveConnection, bool wallOffAllConnections) {//Up to minRooms, force connections. After, don't force connections. At maxRooms, wall off all final connections.
		List<GameObject> currentRoomSpawnList = new List<GameObject>(nextRoomSpawns);
		List<GameObject> roomsToSpawn = new List<GameObject>(rooms);

		for (int i = 0; i < currentRoomSpawnList.Count; i++) {
			if (roomsToSpawn.Count > 0) {
				int randomRoom = Random.Range(0, roomsToSpawn.Count);
				GameObject nextRoom = Instantiate(roomsToSpawn[randomRoom], currentRoomSpawnList[i].transform.position, Quaternion.LookRotation(currentRoomSpawnList[i].transform.up, Vector3.up));
				if (CheckRoomBounds(nextRoom.GetComponents<BoxCollider>())) {
					nextRoom.transform.SetParent(roomParent.transform);
					confirmedRooms.Add(nextRoom);
					nextRoomSpawns.RemoveAt(0);

					if (forceRoomToHaveConnection) {
						ForceFirstCurrentRoomConnectionRandomly(nextRoom.GetComponent<RoomScriptReader>().connectionPoints);
					}
					else if (wallOffAllConnections) {
						foreach (GameObject point in nextRoom.GetComponent<RoomScriptReader>().connectionPoints) {
							Instantiate(wallPanel, point.transform.position, point.transform.rotation, point.transform);
						}
					}
					else {
						DecideCurrentRoomConnections(nextRoom.GetComponent<RoomScriptReader>().connectionPoints);
					}
				}
				else {
					roomsToSpawn.RemoveAt(randomRoom);
					Destroy(nextRoom);
					i--;
				}
			}
			else {
				print("Ran out of rooms to attempt spawning. Placing backup wall.");
				Destroy(currentRoomSpawnList[i].transform.GetChild(0).gameObject);
				Instantiate(wallPanel, currentRoomSpawnList[i].transform.position, currentRoomSpawnList[i].transform.rotation/*, currentRoomSpawnList[i].transform*/);
			}
		}
	}

	void SpawnFinalRooms() {//If the maximum number of rooms has been reached, they have NO CONNECTIONS and ONLY SPAWN WALL PANELS
		EvaluateRoomSpawn(false, true);
	}


	void ForceFirstCurrentRoomConnectionRandomly(GameObject[] pointsArray) {
		List<GameObject> pointsList = new List<GameObject>(pointsArray);

		//for (int i = 0; i < pointsArray.Length; i++) {
		//	pointsList.Add(pointsArray[i]);
		//}

		GameObject pointChosen = pointsList[Random.Range(0, pointsList.Count)];

		Instantiate(doorway, pointChosen.transform.position, pointChosen.transform.rotation, pointChosen.transform);

		nextRoomSpawns.Add(pointChosen); //Add this point to the list of remaining room spawn points still to be attached

		pointsList.Remove(pointChosen);

		DecideCurrentRoomConnections(pointsList.ToArray());
	}


	void DecideCurrentRoomConnections(GameObject[] points) {
		foreach (GameObject point in points) {
			int wallOrDoor = Random.Range(0, 2);

			if (wallOrDoor == 0) {
				Instantiate(wallPanel, point.transform.position, point.transform.rotation, point.transform);
			}
			else {
				Instantiate(doorway, point.transform.position, point.transform.rotation, point.transform);
				nextRoomSpawns.Add(point); //Add this point to the list of remaining room spawn points still to be attached
			}
		}

		//EndRoomSpawnRound();
	}


	void EndRoomSpawnRound() {
		roomSpawnRound++;
		//print("Room Round: " + roomSpawnRound + " of " + maxRooms);

		if (roomSpawnRound > 0 && roomSpawnRound < minRooms) {
			SpawnAdditionalRooms(false);
		}
		else if (roomSpawnRound >= minRooms && roomSpawnRound < maxRooms) {
			SpawnAdditionalRooms(true);
		}
		else if (roomSpawnRound == maxRooms) {
			SpawnFinalRooms();
		}
	}


	bool CheckRoomBounds(BoxCollider[] newRoomBounds) {
		bool boundsConfirmed = true;

		foreach (BoxCollider boxCol in newRoomBounds) {
			foreach (GameObject room in confirmedRooms) {
				BoxCollider[] confirmedRoomBounds = room.GetComponents<BoxCollider>();

				foreach (BoxCollider confirmedCol in confirmedRoomBounds) {
					if (boxCol.bounds.Intersects(confirmedCol.bounds)) {
						boundsConfirmed = false;
					}
				}
			}
		}

		if (boundsConfirmed) {
			print("Room bounds clear");
			return true;
		}
		else {
			print("Room bounds occupied");
			return false;
		}
	}
}
