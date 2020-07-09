using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitScript : MonoBehaviour {
    
    public void ExitGame() {
    	Debug.Log("exited");
	Application.Quit();
    }

}
