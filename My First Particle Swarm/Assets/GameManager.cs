using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{ 
    public static float omegaAverage;
    public static GameObject bestPositionGO;
    public static LineRenderer lr;
    public static Transform objectToFindStat;
    public static Text currentOmegaTextStatic;
    public static Transform objectToFind;
    private static List<Particle> particles;

    public GameObject bestPositionGOPrefab;
    public ParticlePosition swarmBest;
    public GameObject particleGO;
    public GameObject particleGOHimmelblau;
    public InputField inputFieldSpawnQTY;
    public InputField inputFieldMovementAmount;
    public InputField inputFieldOmega;
    public InputField inputFieldPhi;
    public Text currentOmegaText;
    private Gradient colorGradient1;
    public Toggle ptype;

    private void Start()
    {
        bestPositionGO = Instantiate(bestPositionGOPrefab, null);
        objectToFindStat = objectToFind;
        if (bestPositionGO != null)
        {
            bestPositionGO.name = "Best Position";
            bestPositionGO.AddComponent<LineRenderer>();
            lr = bestPositionGO.GetComponent<LineRenderer>();
            lr.SetPosition(0, Particle.bestTeamPosition.position);
            lr.SetPosition(1, Particle.bestTeamPosition.position + Vector3.down * 250);
            lr.startColor = Color.blue;
            lr.endColor = Color.yellow;
        }
        swarmBest = Particle.bestTeamPosition;
        currentOmegaTextStatic = currentOmegaText;
    }

    public void UpdateParticles()
    {
        foreach (Particle particle in FindObjectsOfType<Particle>())
        {
            particle.UpdateBests();
        }
    }

    public void MoveParticles()
    {
        if (int.TryParse(inputFieldMovementAmount.text, result: out int amountOfMoves))
        {
            foreach (Particle particle in FindObjectsOfType<Particle>())
            {
                particle.Move(amountOfMoves);
            } 
        }
        else
        {
            foreach (Particle particle in FindObjectsOfType<Particle>())
            {
                inputFieldMovementAmount.text = "1";
                particle.Move();
            }
        }
    }

    public void SpawnParticles()
    {
        if (int.TryParse(inputFieldSpawnQTY.text, result: out int qty))
        {
            for (int i = 0; i < qty; i++)
            {
                if (ptype.isOn)
                {
                    int x = Random.Range(0, 480);
                    int z = Random.Range(0, 480);
                    Vector3 position = new Vector3(x, particleGO.transform.position.y, z);
                    Instantiate(particleGO, position, particleGO.transform.rotation, null); 
                }
                else
                {

                    int x = Random.Range(-6, 6);
                    int z = Random.Range(-6, 6);
                    Vector3 position = new Vector3(x, particleGOHimmelblau.transform.position.y, z);
                    Instantiate(particleGOHimmelblau, position, particleGOHimmelblau.transform.rotation, null);
                }
            }
        }
        else
        {
            inputFieldSpawnQTY.text = "10";
            SpawnParticles();
        }
        UpdateParticles();
        SetOmegaPhi();
        GetListOfParticles();
    }

    public void RandomizeParticleLocation()
    {
        Vector3 position;
        Particle.bestTeamPosition = new ParticlePosition();

        foreach (Particle particle in FindObjectsOfType<Particle>())
        {
            if (ptype.isOn)
            {
                Particle.bestTeamPosition.pType = ParticleType.Depth;
                float x = Random.Range(0, 480);
                float z = Random.Range(0, 480);
                position = new Vector3(x, particleGO.transform.position.y, z);

            }
            else
            {
                Particle.bestTeamPosition.pType = ParticleType.Himmelblau;
                float x = Random.Range(-6.0f, 6.0f);
                float z = Random.Range(-6.0f, 6.0f);
                position = new Vector3(x, particleGOHimmelblau.transform.position.y, z);
            }
            particle.transform.position = position;
            particle.bestPersonalPosition.position = position;
            
        }
        UpdateParticles();
        SetOmegaPhi();
        GetListOfParticles();
    }

    public void SetOmegaPhi()
    {
        if (int.TryParse(inputFieldOmega.text,result: out int omega))
        {
            foreach (Particle particle in FindObjectsOfType<Particle>())
            {
                particle.omega = omega;
            }
        }

        if (int.TryParse(inputFieldPhi.text, result: out int phi))
        {
            foreach (Particle particle in FindObjectsOfType<Particle>())
            {
                particle.phi = phi;
            }
        }
    }

    public static void UpdateOmega()
    {
        omegaAverage = 0;
        foreach (Particle particle in particles)
        {
            omegaAverage += particle.omega;
        }
        omegaAverage /= particles.Count;
        currentOmegaTextStatic.text = $"Omega Average: {omegaAverage}";
    }

    public void GetListOfParticles()
    {
        particles = new List<Particle>();
        foreach (Particle particle in FindObjectsOfType<Particle>())
        {
            particles.Add(particle);
        }
    }

    public void DestroyAllParticles()
    {
        Particle[] particlesArr = FindObjectsOfType<Particle>();
        for (int i = 0; i < particlesArr.Length; i++)
        {
            Destroy(particlesArr[i].gameObject);
        }
    }
}// End Class
