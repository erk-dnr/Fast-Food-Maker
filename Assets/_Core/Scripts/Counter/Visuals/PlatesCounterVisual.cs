using System;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour
{
    [SerializeField] PlatesCounter platesCounter;
    [SerializeField] Transform plateVisualPrefab;
    [SerializeField] float plateStackOffsetY = 0.1f;
    
    Transform _counterTopPoint;
    List<GameObject> _plateVisualsList;


    void Awake()
    {
        _plateVisualsList = new List<GameObject>();
        _counterTopPoint = platesCounter.GetCounterTopPoint();
    }

    void Start()
    {
        platesCounter.OnPlateSpawned += PlatesCounter_OnPlateSpawned;
        platesCounter.OnPlateRemoved += PlatesCounter_OnPlateRemoved;
    }

    void OnDestroy()
    {
        platesCounter.OnPlateSpawned -= PlatesCounter_OnPlateSpawned;
        platesCounter.OnPlateRemoved -= PlatesCounter_OnPlateRemoved;
    }

    void PlatesCounter_OnPlateSpawned(object sender, EventArgs e)
    {
        Transform plateVisualTransform = Instantiate(plateVisualPrefab, _counterTopPoint);
        
        plateVisualTransform.localPosition = new Vector3(0, plateStackOffsetY * _plateVisualsList.Count, 0);
        _plateVisualsList.Add(plateVisualTransform.gameObject);
    }

    void PlatesCounter_OnPlateRemoved(object sender, EventArgs e)
    {
        GameObject plateGameObject = _plateVisualsList[_plateVisualsList.Count - 1];
        _plateVisualsList.Remove(plateGameObject);
        Destroy(plateGameObject);
    }
}
