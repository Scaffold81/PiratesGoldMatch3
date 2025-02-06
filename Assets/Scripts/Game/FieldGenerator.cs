using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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

        private List<GameObject> _nodes = new List<GameObject>();
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
            if (_nodes.Count > 0)
            {
                foreach (var node in _nodes)
                {
                    DestroyImmediate(node.gameObject);
                }
                _nodes.Clear();
            }


            var panel = GetComponent<RectTransform>();
            var cellSize = panel.rect.width / _columns; // Рассчитываем размер одной ячейки

            float startX = -(panel.rect.width / 2) + (cellSize / 2); // Начальная позиция по оси X
            float startY = panel.rect.height / 2 - (cellSize / 2); // Начальная позиция по оси Y

            for (int y = 0; y < _rows; y++)
            {
                for (int x = 0; x < _columns; x++)
                {
                    var newElement = Instantiate(_nodesPrefabs);
                    newElement.transform.SetParent(transform);
                    newElement.name = x.ToString() + "/" + y.ToString();

                    // Рассчитываем позицию для текущей ячейки
                    var posX = startX + x * cellSize;
                    var posY = startY - y * cellSize;
                    newElement.Position = new Vector2(x, y);
                    var rect = newElement.GetComponent<RectTransform>();
                    rect.anchoredPosition = new Vector2(posX, posY);
                    rect.transform.localScale = Vector3.one * cellSize * 2f / 100; // Устанавливаем размер ячейки

                    var background = Instantiate(_nodesBackgroundPrefabs);
                    newElement.ImageBackground = background;
                    background.transform.position = rect.transform.position;
                    background.transform.SetParent(_backGroundField); // Убираем зависимость от GridLayoutGroup
                    background.transform.localScale = Vector3.one * cellSize / 100; // Пример масштабирования заднего фона
                    _nodes.Add(background.gameObject);
                    _nodes.Add(newElement.gameObject);
                }
            }
            _generate = false;
        }
    }
}

