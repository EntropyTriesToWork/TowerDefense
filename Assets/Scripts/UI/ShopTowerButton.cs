using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Entropy.TD.Tower
{
    [RequireComponent(typeof(Button))]
    public class ShopTowerButton : MonoBehaviour
    {
        public TMP_Text costText;
        public Image towerSprite;
        Button _button;
        public TowerSO towerData;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }
        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        public void Initialize(TowerSO towerData, System.Action onPress = null)
        {
            _button = GetComponent<Button>();
            if (onPress != null) { _button.onClick.AddListener(() => onPress.Invoke()); }
            towerSprite.sprite = towerData.towerSprite;

            this.towerData = towerData;
            costText.text = "$" + towerData.buildCost.ToString(); ;
        }
        private void Update()
        {
            if (towerData != null)
            {
                if (GameManager.Instance.Money < towerData.buildCost) { _button.interactable = false; }
                else { _button.interactable = true; }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}