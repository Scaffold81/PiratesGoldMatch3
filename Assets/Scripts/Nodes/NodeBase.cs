﻿using DG.Tweening;
using Game.Enums;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Game.Gameplay.Nodes
{
    public class NodeBase : MonoBehaviour, IPointerDownHandler
    {
        private MachTreeBase _machTreeView;

        [SerializeField]
        private NodeType _nodeType;
        [SerializeField]
        public NodeAbility _nodeAbility;
        [SerializeField]
        private NodeReward _nodeReward;

        [SerializeField]
        private Image _image;
        [SerializeField]
        private Image _imageBackground;
        [SerializeField]
        private Vector2 _position;
        private float _initialScale;
        private float _duration=0.5f;
        private bool _isActive;

        public NodeType NodeType { get => _nodeType; set => _nodeType = value; }
        public Vector2 Position { get => _position; set => _position = value; }
        public Image Image { get => _image; set => _image = value; }
        public Image ImageBackground { get => _imageBackground; set => _imageBackground = value; }
        public NodeReward NodeReward { get => _nodeReward; private set => _nodeReward = value; }

        public void Init(NodeType type, MachTreeBase machTreeView,NodeReward nodeReward)
        {
            _initialScale = transform.localScale.x;
            _nodeReward = nodeReward;
            _machTreeView = machTreeView;

            if (_nodeType != NodeType.Hidden)
            {
                _nodeType = type;
                StartCoroutine(LoadSprite());
            }
            else
            {
                Hide();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (NodeType == NodeType.Empty || NodeType == NodeType.Hidden) return;
            _machTreeView.SetSelectedNode(this);
            StopScaleAnimation();
        }

        public void Rename()
        {
            name = Position.x + "/" + Position.y;
        }

        public void Hide()
        {
            Image.enabled = false;
            ImageBackground.enabled = false;
        }

        public void Show()
        {
            if (_nodeType == NodeType.Empty)
                Image.enabled = false;
            else
                Image.enabled = true;
        }

        public void LoadNewSprite()
        {
            StartCoroutine(LoadSprite());
        }

        private IEnumerator LoadSprite()
        {
            var handle = Addressables.LoadAssetAsync<Sprite>("cut/" + _nodeType.ToString() + ".png");
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var sprite = handle.Result;
                Image.sprite = sprite;
                Image.enabled = true;
            }
            else
            {
                Debug.LogError("Failed to load Addressable: " + handle.DebugName);
            }

            Addressables.Release(handle);
        }

        public void SetNodeEmpty()
        {
            if (_nodeAbility != null)
            {
                _nodeAbility.ActivateAbility(this);
                _nodeAbility=null;
            }
                NodeType = NodeType.Empty;

                if (Image.enabled == true)
                    Image.enabled = false;

            
        }

        public void SetNodeAbility(NodeAbility ability)
        {
            _nodeAbility = ability;
        }

        public void SetNodeReaward() 
        { 
            _machTreeView.Reward(this); 
        }
        
        public void HightlightOn()
        {
            StartScaleAnimation();
            _isActive = true;
        }
        public void HightlightOff()
        {
            StopScaleAnimation();
        }

        private void StartScaleAnimation()
        {
            transform.DOScale(_initialScale+0.1f, _duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                transform.DOScale(_initialScale, _duration)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        if(_isActive)
                            StartScaleAnimation(); // Рекурсивный вызов для создания постоянного эффекта
                    });
            });
        }

        public void StopScaleAnimation()
        {
            _isActive=false;
            transform.DOScale(_initialScale, _duration); // Возвращаемся к начальному масштабу
           
        }
    }
}