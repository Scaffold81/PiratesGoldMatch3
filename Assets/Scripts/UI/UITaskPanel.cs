using Game.Common;
using Game.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UITaskPanel : MonoBehaviour
    {
        protected private NodeType _type;
        [SerializeField]
        private Image _image;
        [SerializeField]
        private TMP_Text _textCount;
        public void Init(NodeType nodeType, string text)
        {
            _type = nodeType;
            _textCount.text = text;

            LoadSprite(nodeType);
        }

        public async void LoadSprite(NodeType nodeType)
        {
            var sprite = await AdressablesLoader.LoadSpriteAsync(nodeType.ToString());
            if (sprite == null) return;
            if(_image!=null)
                _image.sprite = sprite;
        }
    }
}
