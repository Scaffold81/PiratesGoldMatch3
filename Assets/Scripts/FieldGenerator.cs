using UnityEngine;
using UnityEngine.UI;

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
        private NodeBase _nodesPrefabs;
        [SerializeField]
        private Image _nodesBackgroundPrefabs;
        [SerializeField]
        private bool _generate = false;

        [SerializeField]
        private GridLayoutGroup _gridLayoutGroup;
        [SerializeField]
        private GridLayoutGroup _gridLayoutGroupBackground;
        private void Awake()
        {
            if (Application.isPlaying)
            {
                Destroy(this);
            }
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
            var panel = GetComponent<RectTransform>();
            var cellSyze = panel.rect.width * 1.25f / _columns;
            _gridLayoutGroup.constraintCount = _columns;
            _gridLayoutGroup.cellSize = new Vector2(cellSyze, cellSyze);
            _gridLayoutGroup.spacing = -Vector2.one * cellSyze / 3.7f;

            _gridLayoutGroupBackground.constraintCount = _gridLayoutGroup.constraintCount;
            _gridLayoutGroupBackground.cellSize = _gridLayoutGroup.cellSize;
            _gridLayoutGroupBackground.spacing = _gridLayoutGroup.spacing;

            for (int y = 0; y < _rows; y++)
            {
                for (int x = 0; x < _columns; x++)
                {
                    var newElement = Instantiate(_nodesPrefabs);
                    newElement.transform.SetParent(transform); // Для организации поля в иерархии
                    newElement.name = x.ToString() + "/" + y.ToString();
                    newElement.Position = new Vector2(x, y);
                    newElement.GetComponent<RectTransform>().localScale = Vector3.one * 0.8f;

                    var background = Instantiate(_nodesBackgroundPrefabs);
                    newElement.ImageBackground = background;
                    background.transform.SetParent(_gridLayoutGroupBackground.transform);
                    background.transform.localScale = Vector3.one * 0.8f;
                }
            }
            _generate = false;
        }
    }
}

