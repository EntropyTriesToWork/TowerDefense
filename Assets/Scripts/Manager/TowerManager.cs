using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entropy.TD.Tower
{
    public class TowerManager : MonoBehaviour
    {
        public static TowerManager Instance;

        public CanvasGroup towerMenuCanvas;
        public Transform shopButtonParent;
        public ShopTowerButton shopButton;
        public TowerSO[] towers;

        public GameObject buildPlots;

        public LayerMask groundLayer, buildPlotLayer;

        List<ShopTowerButton> _shopButtons;

        GameObject _ghostTower;
        TowerSO _selectedTower;

        #region Messages
        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void Start()
        {
            CreateButtons();
            buildPlots.SetActive(false);
        }
        public void Update()
        {

            if (Input.GetMouseButtonDown(1))
            {
                CancelGhostTower();
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (_ghostTower != null && _selectedTower != null)
                {
                    if (GameManager.Instance.Money >= _selectedTower.buildCost)
                    {
                        ConfirmPosition();
                    }
                    else
                    {
                        Debug.Log("Not enough money!");
                        CancelGhostTower();
                    }
                }
            }
            if (_ghostTower != null)
            {
                RaycastHit hit = Utils.RaycastFromMouseUsingPerspectiveCamera(buildPlotLayer);
                if (hit.collider != null)
                {
                    _ghostTower.transform.position = hit.collider.transform.position;
                    return;
                }

                hit = Utils.RaycastFromMouseUsingPerspectiveCamera(groundLayer);
                if (hit.collider != null)
                {

                    _ghostTower.transform.position = hit.point;
                }
            }
        }
        #endregion

        #region Tower Creation
        public void CreateButtons()
        {
            if (_shopButtons == null) { _shopButtons = new List<ShopTowerButton>(); }
            if (_shopButtons.Count > 0)
            {
                for (int i = 0; i < _shopButtons.Count; i++)
                {
                    Destroy(_shopButtons[i].gameObject);
                }
                _shopButtons = new List<ShopTowerButton>();
            }

            for (int i = 0; i < towers.Length; i++)
            {
                ShopTowerButton obj = Instantiate(shopButton, shopButtonParent);
                _shopButtons.Add(obj);
                TowerSO towerSO = towers[i];
                obj.Initialize(towerSO, () => TryToPressTowerButton(towerSO));
            }
        }
        public void TryToPressTowerButton(TowerSO towerData)
        {
            if (GameManager.Instance.Money >= towerData.buildCost)
            {
                GameObject obj = Instantiate(towerData.towerPrefab, Vector3.zero, Quaternion.identity);
                _ghostTower = obj;
                _selectedTower = towerData;

                buildPlots.SetActive(true);
            }
            else { Debug.Log("Not enough money!"); }
        }
        public void CancelGhostTower()
        {
            Destroy(_ghostTower);
            _selectedTower = null;

            buildPlots.SetActive(false);
        }
        public void ConfirmPosition()
        {
            RaycastHit hit = Utils.RaycastFromMouseUsingPerspectiveCamera(buildPlotLayer);
            if (hit.collider != null)
            {
                GameManager.Instance.Money -= _selectedTower.buildCost;

                _ghostTower.transform.position = hit.transform.position;
                Destroy(hit.collider.gameObject);

                _ghostTower.GetComponent<Tower>().canAttack = true;

                _ghostTower = null;
                _selectedTower = null;
                buildPlots.SetActive(false);
            }
        }
        #endregion
    }
}