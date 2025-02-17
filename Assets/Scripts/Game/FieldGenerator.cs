using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Game.Enums;

namespace Game.Gameplay.Nodes.Generator
{
    [ExecuteInEditMode]
    public class FieldGenerator : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _backGroundField;

        [SerializeField]
        private NodeBase _nodesPrefabs;
        [SerializeField]
        private Image _nodesBackgroundPrefabs;

        private List<Transform> _nodes = new List<Transform>();

        public void GenerateField(NodeType[,] nodeField)
        {
            _nodes.Clear();
            _nodes.AddRange(_backGroundField.GetComponentsInChildren<Transform>());
            _nodes.AddRange(transform.GetComponentsInChildren<Transform>());
            
            var columns = nodeField.GetLength(1);
            var rows = nodeField.GetLength(0);

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

            var cellSize = panel.rect.width / columns; // Рассчитываем размер одной ячейки

            float startX = -(panel.rect.width / 2) + (cellSize / 2); // Начальная позиция по оси X
            float startY = panel.rect.height / 2 - (cellSize / 2); // Начальная позиция по оси Y

           
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    var newElement = Instantiate(_nodesPrefabs);
                    newElement.transform.SetParent(transform);
                    newElement.name = x.ToString() + "/" + y.ToString();
                    newElement.NodeType = nodeField[y,x];
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
                }
            }
        }
    }
}

