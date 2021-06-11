using System.Collections;
using UnityEngine;

public class SlideBlock : MonoBehaviour
{
    #region Variables

        public event System.Action<SlideBlock> onBlockPressed;
        public event System.Action onFinishedMoving;

        public Vector2Int coord;
        [SerializeField] private Vector2Int startingCoord;

    #endregion

    public void Init(Vector2Int startingCoord, Texture2D image)
    {
        this.startingCoord = startingCoord;
        coord = startingCoord;

        GetComponent<MeshRenderer>().material.shader = Shader.Find("Unlit/MichaelUnlit");
        GetComponent<MeshRenderer>().material.mainTexture = image;
    }

    public void MoveToPosition(Vector2 target, float duration)
    {

    }

    private void OnMouseDown()
    {
        if (onBlockPressed != null)
        {
            onBlockPressed(this);
        }
    }

    public IEnumerator AnimateMove(Vector2 target, float duration)
    {
        Vector2 initialPosition = transform.position;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime / duration;
            transform.position = Vector2.Lerp(initialPosition, target, percent);
            yield return null;
        }

        if (onFinishedMoving != null)
        {
            onFinishedMoving();
        }
    }

    public bool IsAtStartingCoord()
    {
        return coord == startingCoord;
    }
}