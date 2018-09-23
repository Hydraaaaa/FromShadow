using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumberTesting : MonoBehaviour
{
    [SerializeField] DamageNumber damageNumberPrefab = null;
    [SerializeField] Transform damageNumberParent = null;

    [SerializeField] int minDamage = 1000;
    [SerializeField] int maxDamage = 9999;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Semicolon))
        {
            DamageNumber _damageNumber = Instantiate(damageNumberPrefab, damageNumberParent);

            _damageNumber.Number = Random.Range(minDamage, maxDamage);
        }
    }
}
