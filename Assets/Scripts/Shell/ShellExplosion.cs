using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_MaxDamage = 100f;                  
    public float m_ExplosionForce = 1000f;            
    public float m_MaxLifeTime = 2f;            //爆炸时间      
    public float m_ExplosionRadius = 5f;            //冲击范围  


    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime);
    }


    private void OnTriggerEnter(Collider other)  //istrigger is called
    {
        // Find all the tanks in an area around the shell and damage them.
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);
        //收集shell到攻击范围内的所有在m_Tankmask上的Collider

        for(int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigibody = colliders[i].GetComponent<Rigidbody>();
            if (!targetRigibody)
            {
                continue;
            }

            targetRigibody.AddExplosionForce(m_ExplosionForce,transform.position,m_ExplosionRadius);

            TankHealth targetHealth = targetRigibody.GetComponent<TankHealth>();
            if (!targetHealth)
            {
                continue;
            }
            float damage = CalculateDamage(targetRigibody.position);
            targetHealth.TakeDamage(damage);
        }

        m_ExplosionParticles.transform.parent = null;
        m_ExplosionParticles.Play();

        m_ExplosionAudio.Play();

        Destroy(m_ExplosionParticles.gameObject, 1.0f); //m_ExplosionParticles.main.duration);
      
        Destroy(gameObject);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.

        Vector3 explosionToTarget = targetPosition - transform.position;

        float explosionDistance = explosionToTarget.magnitude;

        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

        float damage = relativeDistance * m_MaxDamage;

        damage = Mathf.Max(0f, damage); //有可能为负

        return damage;
    }
}