using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace Game.Gameplay.Nodes.Generator
{
    [ExecuteInEditMode]
    public class FieldGenerator : MonoBehaviour
    {
        [SerializeField]
        private int _rows = 10;
        [SerializeField]
        private int _columns = 7;
        [SerializeField]
        private RectTransform _backGroundField;

        [SerializeField]
        private NodeBase _nodesPrefabs;
        [SerializeField]
        private Image _nodesBackgroundPrefabs;

        private List<Transform> _nodes = new List<Transform>();
        [SerializeField]
        private bool _generate = false;

        private void Awake()
        {
            Destroy(this);
        }

        private void Update()
        {
            if (_generate)
            {
                GenerateField();
            }
        }

        public void GenerateField()
        {
            _nodes.Clear();
            _nodes.AddRange(_backGroundField.GetComponentsInChildren<Transform>());
            _nodes.AddRange(transform.GetComponentsInChildren<Transform>());

            if (_nodes.Count > 0)
            {
                foreach (var node in _nodes)
                {
                    if(node.gameObject!= _backGroundField.gameObject&& node.gameObject != transform.gameObject)
                        DestroyImmediate(node.gameObject);
                }
                _nodes.Clear();
            }


            var panel = GetComponent<RectTransform>();
            var cellSize = panel.rect.width / _columns; // ������������ ������ ����� ������

            float startX = -(panel.rect.width / 2) + (cellSize / 2); // ��������� ������� �� ��� X
            float startY = panel.rect.height / 2 - (cellSize / 2); // ��������� ������� �� ��� Y

            for (int y = 0; y < _rows; y++)
            {
                for (int x = 0; x < _columns; x++)
                {
                    var newElement = Instantiate(_nodesPrefabs);
                    newElement.transform.SetParent(transform);
                    newElement.name = x.ToString() + "/" + y.ToString();

                    // ������������ ������� ��� ������� ������
                    var posX = startX + x * cellSize;
                    var posY = startY - y * cellSize;
                    newElement.Position = new Vector2(x, y);
                    var rect = newElement.GetComponent<RectTransform>();
                    rect.anchoredPosition = new Vector2(posX, posY);
                    rect.transform.localScale = Vector3.one * cellSize * 2f / 100; // ������������� ������ ������

                    var background = Instantiate(_nodesBackgroundPrefabs);
                    newElement.ImageBackground = background;
                    background.transform.position = rect.transform.position;
                    background.transform.SetParent(_backGroundField); // ������� ����������� �� GridLayoutGroup
                    background.transform.localScale = Vector3.one * cellSize / 100; // ������ ��������������� ������� ����
                }
            }
            _generate = false;
        }
    }
}

