using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class DangerSpawner : NetworkBehaviour {
    private float _speedMultiplier = 2;
    private float _currentSpeed;

    private Vector3 _velocity;

    [SerializeField] private GameObject _dangerPrefab;
    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private float _moveDistance;

    private void OnEnable() {
        PlayerMovement.OnPlayerSpawned += PlayerSpawned;
    }

    void PlayerSpawned() {
        StartCoroutine(PrepareSpawn());
    }
    
    IEnumerator PrepareSpawn() {
        yield return new WaitForSeconds(3 / _speedMultiplier);
        Debug.Log("Preparing");
        int randomSpawn = Random.Range(0, _spawnPoints.Count);
        Spawn_ClientRpc(randomSpawn);
    }

    [ClientRpc]
    void Spawn_ClientRpc(int spawnPoint) {
        StartCoroutine(Spawn(_spawnPoints[spawnPoint]));
    }
    
    IEnumerator Spawn(Transform spawnPoint) {
        Debug.Log("Spawning");
        var danger = Instantiate(_dangerPrefab, spawnPoint.position, spawnPoint.rotation);

        Debug.Log("Moving up");
        Vector3 endPointStart = danger.transform.position + danger.transform.up * 2;
        while (Vector3.Distance(danger.transform.position, endPointStart) > 0.1f) {
            danger.transform.Translate(new Vector3(0, _speedMultiplier * Time.deltaTime));
            yield return null;
        }

        Debug.Log("Moving forward");
        Vector3 endPoint = danger.transform.position + danger.transform.forward * 10;
        while (Vector3.Distance(danger.transform.position, endPoint) > 0.1f) {
            danger.transform.Translate(new Vector3(0, 0, _speedMultiplier * Time.deltaTime));
            yield return null;
        }

        Debug.Log("Moving down");
        Vector3 endPointEnd = danger.transform.position - danger.transform.up * 2;
        while (Vector3.Distance(danger.transform.position, endPointEnd) > 0.1f) {
            danger.transform.Translate(new Vector3(0, -_speedMultiplier * Time.deltaTime));
            yield return null;
        }
        
        _speedMultiplier += 1;
        StartCoroutine(PrepareSpawn());


    }
}
