using UnityEngine;

public class Cell
{
    public int ID { get; set; }
    public Transform Transform { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }
    public bool SnakeNewCell { get; set; }

    public Cell(GameObject cellGameObject, Vector2 vector2, GameObject categoryWorld, Sprite sprite)
    {
        Transform = cellGameObject.transform;
        Transform.position = vector2;
        Transform.parent = categoryWorld.transform;

        SpriteRenderer = cellGameObject.AddComponent<SpriteRenderer>();
        SpriteRenderer.sprite = sprite;

        cellGameObject.name = vector2.x + "_" + vector2.y;
    }

    public void DestroyCell()
    {
        GameObject.Destroy(Transform.gameObject);
    }
}
