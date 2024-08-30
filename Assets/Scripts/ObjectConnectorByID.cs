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

    // ������ ��������, � ������� ��������� �����
    private List<GameObject> connectedObjects = new List<GameObject>();

    // ������ ���� ��������, � ������� ����� ������������� �����
    [SerializeField] private GameObject[] gridObjects;

    // ID, �� ������� ����� ��������� �������
    public int objectID;

    // ������ �� ObjectConnectorManager ��� ���������� ������� �����
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
        // ������ �������
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0; // ������������� Z � 0 ��� 2D
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject && CheckObjectID(hit.collider.gameObject))
            {
                //startPosition = GetObjectCenter(hit.collider.gameObject); // �������� � ������ �������
                startPosition = GetObjectCenter(hit.point); // �������� � ������ �������
                startObject = hit.collider.gameObject;

                ObjectIDComponent startIDComponent = startObject.GetComponent<ObjectIDComponent>();
                if (_otherObjectConnectorByID.IsLineCompleted)
                {
                    _otherObjectConnectorByID.ResetLine();
                }

                lineRenderer.positionCount = 1;
                lineRenderer.SetPosition(0, startPosition);
                isDrawing = true;

                // ������������� ����, ��� ����� ��������� � ���������� �������
                SetObjectConnected(startObject, true);
                connectedObjects.Add(startObject); // ��������� � ������ ��������� ��������

                // ������������� ���� ����� �� ������ ���������� �������

                if (startIDComponent != null)
                {
                    lineRenderer.startColor = startIDComponent.lineColor;
                    lineRenderer.endColor = startIDComponent.lineColor;
                }

                // ��������� ����� � ������ manager
                if (manager != null)
                {
                    manager.AddLine(lineRenderer);
                }
            }
        }

        // ��������� �����
        if (isDrawing && Input.GetMouseButton(0))
        {
            Vector3 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentPos.z = 0;  // ������������� Z � 0 ��� 2D

            currentPos = GetObjectCenter(currentPos);

            // �������� ����������� � ������ ��������
            RaycastHit2D hit = Physics2D.Raycast(currentPos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject != startObject)
            {
                var hitObject = hit.collider.gameObject;
                ObjectIDComponent idComponent = hitObject.GetComponent<ObjectIDComponent>();

                if (idComponent != null)
                {
                    // ��������� �����, ���� ��� ������������ � ��������, � �������� ��� ���� ����������� �����
                    if (idComponent.IsConnected)
                    {
                        ResetLine(); // ���������� ����� � �����
                        return;
                    }

                    if (idComponent.ID == objectID)
                    {
                        // �������� � ������ ������� � ����������� ID
                        //Vector3 hitObjectPosition = GetObjectCenter(hitObject); // �������� � ������ �������
                        Vector3 hitObjectPosition = GetObjectCenter(hit.point); // �������� � ������ �������

                        // ��������� LineRenderer, �������� ����� �����
                        lineRenderer.positionCount += 1;
                        lineRenderer.SetPosition(lineRenderer.positionCount - 1, hitObjectPosition);

                        // ������������� ����, ��� ����� ��������� � ����� �������
                        SetObjectConnected(hitObject, true);
                        connectedObjects.Add(hitObject); // ��������� � ������ ��������� ��������

                        // ����� ����������� ����� ����� ����������� � �������� � ����������� ID
                        FinishLine();

                        IsLineCompleted = true;
                        return;
                    }
                    else if (idComponent.ID == 0)
                    {
                        // �������� � ������ ������� � ID == 0 (����� ������������)
                        //Vector3 hitObjectPosition = GetObjectCenter(hitObject); // �������� � ������ �������
                        Vector3 hitObjectPosition = GetObjectCenter(hit.point); // �������� � ������ �������

                        // ��������� LineRenderer, �������� ����� �����
                        lineRenderer.positionCount += 1;
                        lineRenderer.SetPosition(lineRenderer.positionCount - 1, hitObjectPosition);

                        // ������������� ����, ��� ����� ��������� � ����� �������
                        SetObjectConnected(hitObject, true);
                        connectedObjects.Add(hitObject); // ��������� � ������ ��������� ��������

                        // ��������� ��������� ������� ��� ����������� �����
                        startPosition = hitObjectPosition;
                        startObject = hitObject;
                    }
                    else
                    {
                        // ��������� �����, ��� ��� ID �� ��������� � �� ����� 0
                        ResetLine(); // ���������� ����� � �����
                        return;
                    }
                }
            }
            else
            {
                // ��������� ������� ��������� ����� �����
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, currentPos);
            }
        }

        // ���������� �������
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
            touchPos.z = 0; // ������������� Z � 0 ��� 2D

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
