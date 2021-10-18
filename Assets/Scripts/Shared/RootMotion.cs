using UnityEngine;

public class RootMotion : MonoBehaviour
{
    //Lerp
    private Vector2 start;
    private Vector2 end;

    public void SetToZero()
    {
        Vector3 pos = new Vector3(0, 0, 0);
        transform.localPosition = pos;
    }

    public void DoUpdate(float progress)
    {
        Vector2 nextPos = Vector2.Lerp(start, end, progress);

        Vector3 pos = transform.localPosition;
        pos.Set(nextPos.x, nextPos.y, 0);
        transform.localPosition = pos;
    }

    public float SinglePointLerp(Vector2 endingVector, float speed)
    {
        start = transform.localPosition;
        end = endingVector;
        return TimeToComplete(speed);
    }

    //Returns the time in second to complete the lerp
    public float StartLerp(Vector2 StartingVector, Vector2 EndingVector, float speed)
    {
        start = StartingVector;
        end = EndingVector;
        DoUpdate(0);
        return TimeToComplete(speed);
    }

    private float TimeToComplete(float speed)
    {
        return Vector2.Distance(start, end) / speed;
    }
}
