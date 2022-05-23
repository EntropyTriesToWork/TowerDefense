using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

namespace Entropy.TD.Tower
{
    public class GameManager : MonoBehaviour
    {
        #region Variables
        public static GameManager Instance;

        [FoldoutGroup("Player")] public int startingLives = 20;
        [FoldoutGroup("Player")] public int startingMoney = 200;

        [FoldoutGroup("UI")] public TMP_Text livesText, moneyText, timeText, waveText, moneyChangeText;

        [FoldoutGroup("Path")] public List<Transform> _paths = new List<Transform>();
        [FoldoutGroup("Path")] public Transform endPath, startPath;

        [BoxGroup("Read Only")] [SerializeField] [ReadOnly] int _lives;
        [BoxGroup("Read Only")] [SerializeField] [ReadOnly] int _money;
        [BoxGroup("Read Only")] [SerializeField] [ReadOnly] float _time;
        [BoxGroup("Read Only")] [SerializeField] [ReadOnly] int _wave;
        #endregion

        #region Properties
        public int Lives
        {
            get => _lives;
            set
            {
                _lives = Mathf.Max(value, 0);
                UpdateLives();
            }
        }
        public int Money
        {
            get => _money;
            set
            {
                int diff = _money;
                _money = Mathf.Max(value, 0);
                UpdateMoney(value - diff);
            }
        }
        public float GameTime
        {
            get => _time;
            set
            {
                _time = value;
                UpdateTime();
            }
        }
        public int Wave
        {
            get => _wave;
            set
            {
                _wave = value;
                UpdateWave();
            }
        }
        #endregion

        #region Messages
        private void Awake()
        {
            if (Instance == null) { Instance = this; }
            else { Destroy(this); }
        }
        public void Update()
        {
            GameTime += UnityEngine.Time.deltaTime;
        }
        public void Start()
        {
            Lives = startingLives;
            Money = startingMoney;
            moneyChangeText.text = "";
        }
        #endregion

        #region UI
        public void UpdateLives()
        {
            livesText.text = _lives.ToString();
        }
        public void UpdateMoney(int difference)
        {
            moneyText.text = "$" + _money.ToString();
            if (difference > 0)
            {
                moneyChangeText.text = "+" + difference.ToString();
                moneyChangeText.color = Color.green;
            }
            else
            {
                moneyChangeText.text = difference.ToString();
                moneyChangeText.color = Color.red;
            }
        }
        public void UpdateTime()
        {
            timeText.text = Utils.FormatTimeToMinutes(_time);
        }
        public void UpdateWave()
        {
            waveText.text = "Wave " + Wave.ToString();
        }
        #endregion

        public Transform GetNextPathPoint(Transform lastWaypoint)
        {
            if (lastWaypoint == null) { return startPath; }
            if (!_paths.Contains(lastWaypoint)) { return endPath; }
            int nextPoint = _paths.IndexOf(lastWaypoint) + 1;
            if (_paths.Count > nextPoint)
            {
                return _paths[nextPoint];
            }
            return endPath;
        }
    }
}