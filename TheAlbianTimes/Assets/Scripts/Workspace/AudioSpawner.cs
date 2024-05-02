using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Workspace
{
    public class AudioSpawner : MonoBehaviour
    {
        private const string OFFICE_SOUND = "Office";

        private const int OFFICE_AUDIOS_COUNT = 13;
        private const int AUDIO_SPAWNERS_COUNT = 3;
        
        private const float MIN_TIME_TO_SPAWN = 22f;
        private const float MAX_TIME_TO_SPAWN = 80f;
        private const float MIN_X_POSITION = 20f;
        private const float MAX_X_POSITION = 30f;
        private const float MIN_Y_POSITION = 20f;
        private const float MAX_Y_POSITION = 30f;

        private Vector2[] _audioSpawners = new Vector2[AUDIO_SPAWNERS_COUNT];

        private float[] _speeds = new [] { 1f, -2f, 1.3f, 2.7f };
        private float[] _currentTimers = new float[AUDIO_SPAWNERS_COUNT];
        private float[] _spawnTimers = new float[AUDIO_SPAWNERS_COUNT];
        private float[] _normalizeValues = new float[AUDIO_SPAWNERS_COUNT];

        private string[] _audios = new string[OFFICE_AUDIOS_COUNT];
        
        private string[] _usedAudios = new string[OFFICE_AUDIOS_COUNT];

        private float _leftMinXPosition;
        private float _leftMaxXPosition;
        private float _rightMinXPosition;
        private float _rightMaxXPosition;

        private void Start()
        {
            for (int i = 0; i < _audios.Length; i++)
            {
                _audios[i] = OFFICE_SOUND + i;
            }

            for (int i = 0; i < _normalizeValues.Length; i++)
            {
                _normalizeValues[i] = Random.Range(0f, 360f);
            }
            
            Vector2 position = gameObject.transform.position;
            
            for (int i = 0; i < _audioSpawners.Length; i++)
            {
                float normalizeValue = _normalizeValues[i];
                _audioSpawners[i] = new Vector2(Mathf.Sin(normalizeValue) * Random.Range(MIN_X_POSITION, MAX_X_POSITION) + position.x, 
                    Mathf.Cos(normalizeValue + 180) * Random.Range(MIN_Y_POSITION, MAX_Y_POSITION) + position.y);
            }
            
            for (int i = 0; i < _spawnTimers.Length; i++)
            {
                _spawnTimers[i] = GetNewAudioSpawnTimer();
            }
        }

        void Update()
        {
            Vector2 position = gameObject.transform.position;
            
            for (int i = 0; i < _audioSpawners.Length; i++)
            {
                float deltaTime = Time.deltaTime;
                
                _normalizeValues[i] += deltaTime * _speeds[i];
                
                _audioSpawners[i] = new Vector2(Mathf.Sin(_normalizeValues[i]) * Random.Range(MIN_X_POSITION, MAX_X_POSITION) + position.x, 
                    Mathf.Cos(_normalizeValues[i] + 180) * Random.Range(MIN_Y_POSITION, MAX_Y_POSITION) + position.y);

                _currentTimers[i] += deltaTime;

                if (_currentTimers[i] < _spawnTimers[i])
                {
                    continue;
                }
                _currentTimers[i] = 0;
                _spawnTimers[i] = GetNewAudioSpawnTimer();
                AudioManager.Instance.Play3DRandomSound(_audios, 10, 100, 0.2f, 0.4f, 0.7f, 1f, _audioSpawners[i]);
            }
        }

        private float GetNewAudioSpawnTimer()
        {
            return Random.Range(MIN_TIME_TO_SPAWN, MAX_TIME_TO_SPAWN);;
        }
    }
}
