using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoSceneManager : MonoBehaviour {

    public GameObject[] _objects; //The list of all Gameobjects the scene can show

    private int _index = 0;

    void Start()
    {
        foreach(GameObject go in _objects) //Disable every particlesystem
        {
            go.SetActive(false);
        }
        _objects[_index].SetActive(true);
    }

    /// <summary>
    /// Is emmitted whenever the next button in the Demo scene is pressed
    /// </summary>
    public void OnNextButtonPressed()
    {
        _objects[_index].SetActive(false); //Deactivate current Gameobjects
        _index++;
        if (_index > _objects.Length - 1)  //When the effect index is above the last array index
        {
            _index = 0; //Set index to first element
        }
        _objects[_index].SetActive(true); //Activate next Gameobjects
    }

    /// <summary>
    /// Is emmitted whenever the prev button in the Demo scene is pressed
    /// </summary>
    public void OnPrevButtonPressed()
    {
        _objects[_index].SetActive(false); //Deactivate current Gameobjects
        _index--;
        if (_index < 0)  //When the effect index is less than the last array index
        {
            _index = _objects.Length - 1; //set index to last element
        }
        _objects[_index].SetActive(true); //Activate next Gameobjects
    }
}
