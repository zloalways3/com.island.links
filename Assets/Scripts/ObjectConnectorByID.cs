using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Linq;

public class ObjectConnectorByID : MonoBehaviour
{
    public bool IsLineCompleted { get; set; }

    private LineRenderer lineRenderer;
    private Vector3 startPosition;
    private GameObject startObject;
    private bool isDrawing;

    [SerializeField] private ObjectConnectorByID _otherObjectConnectorByID;

    // Список объектов, к которым привязана линия
    private List<GameObject> connectedObjects = new List<GameObject>();

    // Массив всех объектов, к которым будет привязываться линия
    [SerializeField] private GameObject[] gridObjects;

    // ID, по которым будем соединять объекты
    public int objectID;

    // Ссылка на ObjectConnectorManager для управления списком линий
    private ObjectConnectorManager manager;


    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        if (gridObjects == null || gridObjects.Length == 0)
        {
            gridObjects = GameObject.FindGameObjectsWithTag("Connectable");
        }
        manager = FindObjectOfType<ObjectConnectorManager>();
    }

    void Update()
    {
#if UNITY_EDITOR
        // Начало касания
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0; // Устанавливаем Z в 0 для 2D
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject && CheckObjectID(hit.collider.gameObject))
            {
                //startPosition = GetObjectCenter(hit.collider.gameObject); // Привязка к центру объекта
                startPosition = GetObjectCenter(hit.point); // Привязка к центру объекта
                startObject = hit.collider.gameObject;

                ObjectIDComponent startIDComponent = startObject.GetComponent<ObjectIDComponent>();
                if (_otherObjectConnectorByID.IsLineCompleted)
                {
                    _otherObjectConnectorByID.ResetLine();
                }

                lineRenderer.positionCount = 1;
                lineRenderer.SetPosition(0, startPosition);
                isDrawing = true;

                // Устанавливаем флаг, что линия привязана к стартовому объекту
                SetObjectConnected(startObject, true);
                connectedObjects.Add(startObject); // Добавляем в список связанных объектов

                // Устанавливаем цвет линии на основе стартового объекта

                if (startIDComponent != null)
                {
                    lineRenderer.startColor = startIDComponent.lineColor;
                    lineRenderer.endColor = startIDComponent.lineColor;
                }

                // Добавляем линию в список manager
                if (manager != null)
                {
                    manager.AddLine(lineRenderer);
                }
            }
        }

        // Рисование линии
        if (isDrawing && Input.GetMouseButton(0))
        {
            Vector3 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentPos.z = 0;  // Устанавливаем Z в 0 для 2D

            currentPos = GetObjectCenter(currentPos);

            // Проверка пересечения с другим объектом
            RaycastHit2D hit = Physics2D.Raycast(currentPos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject != startObject)
            {
                var hitObject = hit.collider.gameObject;
                ObjectIDComponent idComponent = hitObject.GetComponent<ObjectIDComponent>();

                if (idComponent != null)
                {
                    // Прерываем линию, если она пересекается с объектом, у которого уже есть привязанная линия
                    if (idComponent.IsConnected)
                    {
                        ResetLine(); // Сбрасываем линию и флаги
                        return;
                    }

                    if (idComponent.ID == objectID)
                    {
                        // Привязка к новому объекту с совпадающим ID
                        //Vector3 hitObjectPosition = GetObjectCenter(hitObject); // Привязка к центру объекта
                        Vector3 hitObjectPosition = GetObjectCenter(hit.point); // Привязка к центру объекта

                        // Обновляем LineRenderer, добавляя новую точку
                        lineRenderer.positionCount += 1;
                        lineRenderer.SetPosition(lineRenderer.positionCount - 1, hitObjectPosition);

                        // Устанавливаем флаг, что линия привязана к этому объекту
                        SetObjectConnected(hitObject, true);
                        connectedObjects.Add(hitObject); // Добавляем в список связанных объектов

                        // Линия завершается сразу после пересечения с объектом с совпадающим ID
                        FinishLine();

                        IsLineCompleted = true;
                        return;
                    }
                    else if (idComponent.ID == 0)
                    {
                        // Привязка к новому объекту с ID == 0 (линия продолжается)
                        //Vector3 hitObjectPosition = GetObjectCenter(hitObject); // Привязка к центру объекта
                        Vector3 hitObjectPosition = GetObjectCenter(hit.point); // Привязка к центру объекта

                        // Обновляем LineRenderer, добавляя новую точку
                        lineRenderer.positionCount += 1;
                        lineRenderer.SetPosition(lineRenderer.positionCount - 1, hitObjectPosition);

                        // Устанавливаем флаг, что линия привязана к этому объекту
                        SetObjectConnected(hitObject, true);
                        connectedObjects.Add(hitObject); // Добавляем в список связанных объектов

                        // Обновляем начальную позицию для продолжения линии
                        startPosition = hitObjectPosition;
                        startObject = hitObject;
                    }
                    else
                    {
                        // Прерываем линию, так как ID не совпадает и не равен 0
                        ResetLine(); // Сбрасываем линию и флаги
                        return;
                    }
                }
            }
            else
            {
                // Обновляем текущую последнюю точку линии
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, currentPos);
            }
        }

        // Завершение касания
        if (Input.GetMouseButtonUp(0) && isDrawing)
        {
            if (connectedObjects.Count != 0)
            {
                ResetLine();
            }
            isDrawing = false;
        }

#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            touchPos.z = 0; // Устанавливаем Z в 0 для 2D

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);
                    if (hit.collider != null && hit.collider.gameObject == gameObject && CheckObjectID(hit.collider.gameObject))
                    {
                        startPosition = GetObjectCenter(hit.point);
                        startObject = hit.collider.gameObject;

                        ObjectIDComponent startIDComponent = startObject.GetComponent<ObjectIDComponent>();
                        if (_otherObjectConnectorByID.IsLineCompleted)
                        {
                            _otherObjectConnectorByID.ResetLine();
                        }

                        lineRenderer.positionCount = 1;
                        lineRenderer.SetPosition(0, startPosition);
                        isDrawing = true;

                        SetObjectConnected(startObject, true);
                        connectedObjects.Add(startObject);

                        if (startIDComponent != null)
                        {
                            lineRenderer.startColor = startIDComponent.lineColor;
                            lineRenderer.endColor = startIDComponent.lineColor;
                        }

                        if (manager != null)
                        {
                            manager.AddLine(lineRenderer);
                        }
                    }
                    break;

                case TouchPhase.Moved:
                    if (isDrawing)
                    {
                        Vector3 currentPos = Camera.main.ScreenToWorldPoint(touch.position);
                        currentPos.z = 0;

                        currentPos = GetObjectCenter(currentPos);

                        RaycastHit2D hitMove = Physics2D.Raycast(currentPos, Vector2.zero);

                        if (hitMove.collider != null && hitMove.collider.gameObject != startObject)
                        {
                            var hitObject = hitMove.collider.gameObject;
                            ObjectIDComponent idComponent = hitObject.GetComponent<ObjectIDComponent>();

                            if (idComponent != null)
                            {
                                if (idComponent.IsConnected)
                                {
                                    ResetLine();
                                    return;
                                }

                                if (idComponent.ID == objectID)
                                {
                                    Vector3 hitObjectPosition = GetObjectCenter(hitMove.point);

                                    lineRenderer.positionCount += 1;
                                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, hitObjectPosition);

                                    SetObjectConnected(hitObject, true);
                                    connectedObjects.Add(hitObject);

                                    FinishLine();
                                    IsLineCompleted = true;
                                    return;
                                }
                                else if (idComponent.ID == 0)
                                {
                                    Vector3 hitObjectPosition = GetObjectCenter(hitMove.point);

                                    lineRenderer.positionCount += 1;
                                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, hitObjectPosition);

                                    SetObjectConnected(hitObject, true);
                                    connectedObjects.Add(hitObject);

                                    startPosition = hitObjectPosition;
                                    startObject = hitObject;
                                }
                                else
                                {
                                    ResetLine();
                                    return;
                                }
                            }
                        }
                        else
                        {
                            lineRenderer.SetPosition(lineRenderer.positionCount - 1, currentPos);
                        }
                    }
                    break;

                case TouchPhase.Ended:
                    if (isDrawing)
                    {
                        if (connectedObjects.Count != 0)
                        {
                            ResetLine();
                        }
                        isDrawing = false;
                    }
                    break;
            }
        }
#endif
    }

    private Vector3 GetObjectCenter(Vector3 position)
    {
        GameObject closestObject = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject gridObject in gridObjects)
        {
            float distance = Vector3.Distance(position, gridObject.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = gridObject;
            }
        }

        if (closestObject != null)
        {
            return closestObject.transform.position;
        }

        return position;
    }

    private bool CheckObjectID(GameObject obj)
    {
        ObjectIDComponent idComponent = obj.GetComponent<ObjectIDComponent>();
        return idComponent != null && (idComponent.ID == objectID || idComponent.ID == 0);
    }

    private Vector3 GetConstrainedPosition(Vector3 currentPos)
    {
        Vector3 constrainedPos = currentPos;
        Vector3 direction = currentPos - startPosition;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            constrainedPos.y = startPosition.y;
        }
        else
        {
            constrainedPos.x = startPosition.x;
        }

        return constrainedPos;
    }

    private void FinishLine()
    {
        Debug.Log("FinishLine()");
        isDrawing = false;
    }

    private void SetObjectConnected(GameObject obj, bool connected)
    {
        ObjectIDComponent idComponent = obj.GetComponent<ObjectIDComponent>();
        if (idComponent != null)
        {
            idComponent.IsConnected = connected;
        }
    }

    public void ResetLine()
    {
        if (manager != null)
        {
            manager.RemoveLine(lineRenderer);
        }

        isDrawing = false;
        lineRenderer.positionCount = 0;

        foreach (GameObject obj in connectedObjects)
        {
            SetObjectConnected(obj, false);
        }

        connectedObjects.Clear();
    }
}
