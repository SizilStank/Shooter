using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _tripleShotPowerUp;
    [SerializeField] private GameObject _speedPowerUpPrefab;
    [SerializeField] private GameObject _AmmoPrefab;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject _ringPowerUp;
    [SerializeField] private float _waitTime = 3f;

    [SerializeField] private GameObject[] _powerUps;

    [SerializeField] private bool _stopSpawning;

    public void StartSpawn()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnTriplePowerUpRoutine());
        //StartCoroutine(SpawnRareRingPowerUp());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        while (_stopSpawning == false)
        {
            Vector3 spawnPosRando = new Vector3(Random.Range(-9.21f, 9.21f), 8, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, spawnPosRando, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(_waitTime);
        }
    }

    IEnumerator SpawnTriplePowerUpRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        while (_stopSpawning == false)
        {
            Vector3 spawnPosRando = new Vector3(Random.Range(-9.21f, 9.21f), 8, 0);
            int randompowerUpSpawn = Random.Range(0, 6);
            Instantiate(_powerUps[randompowerUpSpawn], spawnPosRando, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(3f, 10f));
        }
    }

    //IEnumerator SpawnRareRingPowerUp()
    //{
    //    yield return new WaitForSeconds(20f);
    //    Vector3 spawnPosRando = new Vector3(Random.Range(-9.21f, 9.21f), 8, 0);
    //    Instantiate(_ringPowerUp, spawnPosRando, Quaternion.identity);
    //    yield return new WaitForSeconds(20f);
    //    Instantiate(_ringPowerUp, spawnPosRando, Quaternion.identity);
    //}

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }


}
