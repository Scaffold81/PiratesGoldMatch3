﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class NodeBase:MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private NodeType _nodeType;
    
    [SerializeField]
    private Image _image;
    [SerializeField]
    private Vector2 _position;
   
    [SerializeField]
    private MachTreeView _machTreeView;

    public NodeType NodeType { get => _nodeType; set => _nodeType = value; }
    public Vector2 Position { get => _position; set => _position = value; }
    
    public void Init(NodeType type, MachTreeView machTreeView)
    {
        _machTreeView= machTreeView;

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
        _machTreeView.SetSelectedNode(this);
    }

    internal void Rename()
    {
        name= Position.x+"/"+Position.y;
    }

    private void Hide()
    {
        _image.enabled = false;
    }

    private IEnumerator LoadSprite()
    {
        var handle = Addressables.LoadAssetAsync<Sprite>("cut/" + _nodeType.ToString()+".png");
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var sprite = handle.Result;
            _image.sprite = sprite;
            _image.enabled = true;
        }
        else
        {
            Debug.LogError("Failed to load Addressable: " + handle.DebugName);
        }

        Addressables.Release(handle);
    }
}