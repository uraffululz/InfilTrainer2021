using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRoomScriptable", menuName = "Scriptables/RoomScriptable", order = 0)]
public class RoomScriptable : ScriptableObject{

	public bool demandsAdditionalConnection; //One of this room's connection points MUST be a doorway
	//public bool[] hasConnectionOnSide; //Length of this array should be FOUR, corresponding (in order) with possible LOWER, LEFT, UPPER, and RIGHT side connections
	//public GameObject[] connectionPoints;

	//TREASURE SPAWNING VARIABLES
	public bool hasPickups;
	public bool hasFloorTreasure;
	public bool hasWallTreasure;

	//LASER SPAWNING VARIABLES
	public int laserDensity;
  
}
