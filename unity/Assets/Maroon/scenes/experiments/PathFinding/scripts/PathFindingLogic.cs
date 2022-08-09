using GEAR.Localization.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public class PathFindingLogic : MonoBehaviour
{
    [SerializeField] Maze _maze;
    [SerializeField] LocalizedTMP _algoDescription;

    private PathFindingAlgorithm[] algos =
    {
        new BreadthFirstSearch(),
        new DepthFirstSearch(),
        new AStarPathFinding(),
        new DijkstraPathFinding()
    };

    protected void Start()
    {
        _maze.Init(8, algos[0]);
    }

    protected void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                MazeElement hitElement;
                if ((hitElement = hit.transform.parent.GetComponent<MazeElement>()) != null)
                {
                    _maze.InspectElement(hitElement.X, hitElement.Y);
                }
                else
                {
                    _maze.InspectElement(-1, -1);
                }
            }
        }
    }

    public void SetMazeSize(float size)
    {
        _maze.SetMazeSize((int)size);
    }

    public void SetSpeed(float speed)
    {
        _maze.SetSpeed(speed);
    }
    public void SetPathfindingAlgo(int algoIndex)
    {
        _maze.SetAlgorithm(algos[algoIndex]);
        _algoDescription.Key = algos[algoIndex].Name;
    }
}
