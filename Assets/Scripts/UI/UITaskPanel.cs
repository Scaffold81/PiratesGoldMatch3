using Game.Common;
using Game.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UITaskPanel : MonoBehaviour
    {
        [SerializeField]
        private Image _image;
        [SerializeField]
        private TMP_Text _text;
        public void Init(NodeType nodeType, string text)
        {
            LoadSprite(nodeType);
            _text.text = text;
        }
        public async void LoadSprite(NodeType nodeType)
        {
            var sprite = await AdressablesLoader.LoadSpriteAsync(nodeType.ToString());
            if (sprite == null) return;

            _image.sprite = sprite;
        }
    }
}
