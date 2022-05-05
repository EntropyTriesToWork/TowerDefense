using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Entropy.TD.Map;

namespace Entropy.TD
{
    public class ExpansionArrow : MonoBehaviour
    {
        public Direction direction;
        private Tween _tween;
        public Vector3 position;
        public float mapSize;
        public void Initialize(Direction direction, Vector3 position, float mapSize)
        {
            this.position = position;
            this.direction = direction;
            this.mapSize = mapSize;
            transform.localScale = new Vector3(mapSize, 0.5f, mapSize);
        }

        public void OnMouseDown()
        {
            MapManager.Instance.AddNewGridMap(direction, position);
        }
        public void OnMouseEnter()
        {
            if(_tween != null) { return; }
            _tween = transform.DOScale(new Vector3(mapSize * 0.95f, 0.5f, mapSize * 0.95f), 0.3f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }
        public void OnMouseExit()
        {
            if(_tween == null) { return; }
            _tween.Kill();
            _tween = null;
            transform.DOScale(new Vector3(mapSize, 0.5f, mapSize), 0.1f);
        }
        public void OnDisable()
        {
            _tween.Kill();
        }
        public void OnDestroy()
        {
            _tween.Kill();
        }
    }
}