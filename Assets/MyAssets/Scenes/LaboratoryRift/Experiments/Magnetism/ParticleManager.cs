using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    private float cycleInterval = 0.01f;
    private List<ChargedParticle> _chargedParticles;
    private List<MovingChargedParticle> _movingChargedParticles;

    private void Start()
    {
        // get all particles
        _chargedParticles = new List<ChargedParticle>(FindObjectsOfType<ChargedParticle>()); // contains MovingChargedParticles as well -> interhitance
        _movingChargedParticles = new List<MovingChargedParticle>(FindObjectsOfType<MovingChargedParticle>());

        // start cycle for each moving particle
        foreach (var mcp in _movingChargedParticles)
            StartCoroutine(Cycle(mcp));
    }

    public IEnumerator Cycle(MovingChargedParticle mcp)
    {
        yield return new WaitForSeconds(Random.Range(0f, cycleInterval)); // prevent spike in processing - offset cycle per particle randomly
        
        while (true)
        {
            ApplyMagneticForce(mcp);
            yield return new WaitForSeconds(cycleInterval);
        }
    }

    public void ApplyMagneticForce(MovingChargedParticle mcp)
    {
        Vector3 newForce = Vector3.zero;

        foreach (var cp in _chargedParticles)
        {
            if (mcp == cp)
                continue;

            float distance = Vector3.Distance(mcp.transform.position, cp.gameObject.transform.position);
            float force = 1000 * mcp.charge * cp.charge / Mathf.Pow(distance, 2);

            Vector3 direction = Vector3.Normalize(mcp.transform.position - cp.transform.position);

            newForce += force * direction * cycleInterval; // shorter intervall -> weaker force

            if (float.IsNaN(newForce.x)) // handles case when two mcp's are on top of each other
                newForce = Vector3.zero;

            mcp.Rb.AddForce(newForce);
        }
    }
}