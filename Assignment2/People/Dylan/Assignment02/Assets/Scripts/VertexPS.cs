using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexPS : MonoBehaviour
{
    [SerializeField]
    SkinnedMeshRenderer skin;
    Mesh mesh;


    ParticleSystem ps1;
    ParticleSystem.MainModule ps1Main;
    ParticleSystemRenderer ps1Ren;
    ParticleSystem.EmissionModule ps1Emi;
    public Material mat1;
    int max; // number of particles to spawn

    // Use this for initialization
    void Start()
    {
        mesh = (Mesh)Instantiate(skin.sharedMesh);
        skin.sharedMesh = mesh;

        max = mesh.vertices.Length;

        // initialize particle system
        gameObject.AddComponent<ParticleSystem>();
        ps1 = this.GetComponent<ParticleSystem>();
        ps1Main = ps1.main;

        ps1Main.loop = true;
        ps1Main.startLifetime = 10;
        ps1Main.maxParticles = max;
        ps1Main.simulationSpace = ParticleSystemSimulationSpace.World;
        ps1Main.startSpeed = 0f;
        ps1Main.startSize = 0.02f;

        ps1Ren = this.GetComponent<ParticleSystemRenderer>();
        ps1Ren.material = mat1;

        ps1Emi = ps1.emission;
        ps1Emi.enabled = true;
        ps1Emi.rateOverTime = max;
        skin.enabled = false;

    }


    void LateUpdate()
    {
        mesh = new Mesh();
        skin.BakeMesh(mesh);

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps1.particleCount];
        int count = ps1.GetParticles(particles);
        for (int i = 0; i < particles.Length; i++)
        {
                particles[i].position = transform.TransformPoint(mesh.vertices[i]);
        }
        ps1.SetParticles(particles, count);
    }
}
