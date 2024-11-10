using UnityEngine;
using UnityEngine.UI;

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
    private bool _generate = false;

    [SerializeField]
    private GridLayoutGroup _gridLayoutGroup;
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
        var cellSyze = panel.rect.width * 1.5f / _columns;
        _gridLayoutGroup.constraintCount = _columns;
        _gridLayoutGroup.constraintCount = _columns;
        _gridLayoutGroup.cellSize = new Vector2(cellSyze, cellSyze);
        _gridLayoutGroup.spacing = -Vector2.one * cellSyze / 3f;

        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _columns; x++)
            {
                var newElement = Instantiate(_nodesPrefabs);
                newElement.transform.SetParent(transform); // Для организации поля в иерархии
                newElement.name = x.ToString() + "/" + y.ToString();
                newElement.Position = new Vector2(x, y);
                newElement.GetComponent<RectTransform>().localScale = Vector3.one * 0.8f;

            }
        }
        _generate = false;
    }
}

