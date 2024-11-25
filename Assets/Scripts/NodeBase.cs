using System;
using System.Collections;
using TMPro;
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
    private Image _imageBackground;
    [SerializeField]
    private Vector2 _position;
    [SerializeField]
    private TMP_Text _positionText;

    [SerializeField]
    private MachTreeView _machTreeView;

    public NodeType NodeType { get => _nodeType; set => _nodeType = value; }
    public Vector2 Position { get => _position; set => _position = value; }
    public Image Image { get => _image; set => _image = value; }
    public Image ImageBackground { get => _imageBackground; set => _imageBackground = value; }
    public TMP_Text PositionText { get => _positionText; set => _positionText = value; }

    public void Init(NodeType type, MachTreeView machTreeView)
    {
        _machTreeView= machTreeView;
        PositionText.gameObject.SetActive(false);
       
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
        if(NodeType == NodeType.Ready|| NodeType == NodeType.Hidden)return;
        _machTreeView.SetSelectedNode(this);
    }

    public void Rename()
    {
        name = Position.x+"/"+Position.y;
    }

    public void Hide()
    {
        Image.enabled = false;
        ImageBackground.enabled = false;
    }

    public void Show(Vector2 position)
    {
        if(_nodeType == NodeType.Ready)
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
        var handle = Addressables.LoadAssetAsync<Sprite>("cut/" + _nodeType.ToString()+".png");
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

    public void DestroyNode()
    {
        NodeType = NodeType.Ready;
        Image.enabled = false;
    }
}