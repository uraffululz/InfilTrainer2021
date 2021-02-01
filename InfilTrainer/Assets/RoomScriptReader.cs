using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScriptReader : MonoBehaviour {

	[SerializeField] RoomScriptable roomScript;

	public bool demandsConnection;
	public GameObject[] connectionPoints;


    void Awake()
    {
		demandsConnection = roomScript.demandsAdditionalConnection;
        //connections = roomScript.connectionPoints;
    }
	
}
