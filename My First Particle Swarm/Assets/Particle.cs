using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ParticleType
{
    Depth,
    Himmelblau
}
[System.Serializable]
public class ParticlePosition
{
    public Vector3 position;
    public float functionValue;
    public float FunctionValue { get => GetFunctionValue(); }
    public ParticleType pType = ParticleType.Depth;

    //public ParticlePosition(Vector3 position = new pos)
    //{
    //    this.position = position;
    //}

    private float GetFunctionValue()
    {
        if (pType == ParticleType.Depth)
        {
            if (Physics.Raycast(position, Vector3.down, hitInfo: out RaycastHit hit, 500))
            {
                functionValue = hit.distance;
                return functionValue;
            }
            else
            {
                return float.MinValue;
            } 
        }
        else
        {
            // f(x,y) = (x2 + y - 11)2 + (x + y2 - 7)2 [-6,6]
            float x = position.x;
            float y = position.z;
            float x2 = Mathf.Pow(x, 2);
            float y2 = Mathf.Pow(y, 2);
            functionValue = Mathf.Pow((x2 + y - 11),2) + Mathf.Pow((x + y2 - 7),2);
            return functionValue;
        }
    }

    
}

public class Particle : MonoBehaviour
{
    
    public static ParticlePosition bestTeamPosition = new ParticlePosition();
    public ParticleType pType = ParticleType.Depth;

    public Vector3 currentPosition;
    //public Vector3 currentDirection;
    public ParticlePosition bestPersonalPosition;
    public Vector3 currentVelocity;
    public Vector3 endDestination;
    public Vector3 startDestination;
    public float phi = 2.0f;
    public float omega = 0.1f;
    private float journeyLenght;
    public float DistanceToFloor { get => GetDistanceToGround(); }

    public void Awake()
    {
        currentPosition = this.transform.position;
        int randomDirection = Random.Range(1, 4);
        switch (randomDirection)
        {
            case 1:
                currentVelocity = this.transform.forward;
                break;
            case 2:
                currentVelocity = -this.transform.forward;
                break;
            case 3:
                currentVelocity = this.transform.right;
                break;
            case 4:
            default:
                currentVelocity = -this.transform.right;
                break;
        }
        bestTeamPosition.pType = pType;
        bestPersonalPosition.pType = pType;
        bestPersonalPosition.position = currentPosition;
        if (pType == ParticleType.Depth)
        {
            if (bestPersonalPosition.FunctionValue > bestTeamPosition.FunctionValue)
            {
                bestTeamPosition.position = currentPosition;
            } 
        }
        else
        {
            if (bestPersonalPosition.FunctionValue < bestTeamPosition.FunctionValue)
            {
                bestTeamPosition.position = currentPosition;
            }
        }
    }

    public void MoveUpdate()
    {
        omega *= 0.99f;
        GameManager.UpdateOmega();
        float rp, rg;
        rp = Random.value;
        rg = Random.value;
        currentVelocity = omega * currentVelocity +
                       (phi * rp) * (bestPersonalPosition.position - currentPosition) +
                       (phi * rg) * (bestTeamPosition.position - currentPosition);
        startDestination = currentPosition;
        endDestination = currentPosition + currentVelocity;
        if (pType == ParticleType.Depth)
        {
            endDestination.x = Mathf.Clamp(endDestination.x, 0, 480);
            endDestination.z = Mathf.Clamp(endDestination.z, 0, 480); 
        }
        else
        {
            endDestination.x = Mathf.Clamp(endDestination.x, -6, 6);
            endDestination.z = Mathf.Clamp(endDestination.z, -6, 6);
        }
        journeyLenght = Vector3.Distance(startDestination, endDestination);
    }

    public void Move(int amountOfMoves = 1)
    {
        StartCoroutine(MoveParticle(amountOfMoves));
    }

    IEnumerator MoveParticle(int amountOfMoves = 1)
    {

        for (int i = 0; i < amountOfMoves; i++)
        {
            float speed;
            this.name = $"M:{amountOfMoves - i-1}";
            MoveUpdate();
            float startTime = Time.time;
            if (pType == ParticleType.Depth)
            {
                speed = 500.0f; 
            }
            else
            {
                speed = 50.0f;
            }

            while (currentPosition != endDestination)
            {
                float distanceCovered = (Time.time - startTime) * speed;
                float fracJourney = distanceCovered / journeyLenght;
                currentPosition = Vector3.Lerp(startDestination, endDestination, fracJourney);
                this.transform.position = currentPosition;
                yield return null;
            }
            UpdateBests();
        }
    }

    public void UpdateBests()
    {
        currentPosition = this.transform.position;
        if (pType == ParticleType.Depth)
        {
            if (GetDistanceToGround() > bestPersonalPosition.FunctionValue)
            {
                bestPersonalPosition.position = currentPosition;
            }

            if (bestPersonalPosition.FunctionValue > bestTeamPosition.FunctionValue)
            {
                bestTeamPosition.position = bestPersonalPosition.position;
                Debug.DrawRay(bestTeamPosition.position, Vector3.down * 500, Color.blue, 5.0f);
                SetBestGO();
            } 
        }
        else
        {
            if (GetDistanceToGround() < bestPersonalPosition.FunctionValue)
            {
                bestPersonalPosition.position = currentPosition;
            }

            if (bestPersonalPosition.FunctionValue < bestTeamPosition.FunctionValue)
            {
                bestTeamPosition.position = bestPersonalPosition.position;
                Debug.DrawRay(bestTeamPosition.position, Vector3.down * 500, Color.blue, 5.0f);
                SetBestGO();
            }
        }
    }

    private static void SetBestGO()
    {
        GameObject.Find("BestDepthText").GetComponent<Text>().text = $"Best Depth = {bestTeamPosition.FunctionValue}";
        GameManager.lr.SetPosition(0, bestTeamPosition.position);
        GameManager.lr.SetPosition(1, bestTeamPosition.position + Vector3.down * 250);
        GameManager.bestPositionGO.transform.position = bestTeamPosition.position;
    }

    private float GetDistanceToGround()
    {
        if (pType == ParticleType.Depth)
        {
            Debug.DrawRay(currentPosition, Vector3.down * 500, Color.red, 0.5f);
            if (Physics.Raycast(currentPosition, Vector3.down, hitInfo: out RaycastHit hit, 500))
            {
                return hit.distance;
            }
            else
            {
                return -1;
            } 
        }
        else
        {
            Debug.DrawRay(currentPosition, Vector3.down * 500, Color.green, 0.5f);
            // f(x,y) = (x2 + y - 11)2 + (x + y2 - 7)2 [-6,6]
            float x = currentPosition.x;
            float y = currentPosition.z;
            float x2 = Mathf.Pow(x, 2);
            float y2 = Mathf.Pow(y, 2);
            float functionValue = Mathf.Pow((x2 + y - 11), 2) + Mathf.Pow((x + y2 - 7), 2);
            return functionValue;
        }
    }
}
