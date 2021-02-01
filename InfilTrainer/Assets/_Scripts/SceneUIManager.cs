using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneUIManager : MonoBehaviour {

	[SerializeField] Text actionText;


    void Start() {
        
    }


    void Update() {
        
    }


	public void UpdateActionText(string updateText) {
		actionText.text = updateText;
	}
}
