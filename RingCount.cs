using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingCount : MonoBehaviour
{
   
     
    TMPro.TMP_Text text;
    int count;

    private void Awake()
    {
        text = GetComponent<TMPro.TMP_Text>();
    }

    void OnEnable() => Ring.OnCollected += OnCollectibleCollected;
    void OnDisable() => Ring.OnCollected -= OnCollectibleCollected;
    private void Start() => UpdateCount();
    void OnCollectibleCollected()
    {
      count++;
        UpdateCount();
    }

    void UpdateCount()
    {
        text.text = $"Rings: {count}";
    }
}
