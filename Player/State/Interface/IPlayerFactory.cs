using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerFactory
{
    PlayerController Spawn(Vector3 spawnPosition);
}

public class LocalPlayerFactory : IPlayerFactory
{
    private readonly PlayerController _playerPrefab;

    public LocalPlayerFactory(PlayerController playerPrefab)
    {
        _playerPrefab = playerPrefab;
    }

    public PlayerController Spawn(Vector3 spawnPosition)
    {
        return UnityEngine.Object.Instantiate(
            _playerPrefab,
            spawnPosition,
            Quaternion.identity
        );
    }
}

//public class NetworkPlayerFactory : IPlayerFactory
//{
//    private readonly PlayerController _playerPrefab;

//    public NetworkPlayerFactory(PlayerController playerPrefab)
//    {
//        _playerPrefab = playerPrefab;
//    }

//    public PlayerController Spawn(Vector3 spawnPosition, PlayerData data)
//    {
//        // Mirror의 경우, 스폰/데이터 초기화 로직은 서버 콜백에서 처리
//        NetworkServer.Spawn(_playerPrefab.gameObject);
//        // 실제 Init(data)는 OnServerAddPlayer 콜백 안에서
//        return _playerPrefab;
//    }
//}
