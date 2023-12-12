#region

using System;
using System.Collections.Generic;
using Entity.Scripts.Senses;
using UnityEngine;

#endregion

public class EntityAudition : MonoBehaviour
{
    [Serializable]
    public class AudibleHeard
    {
        public Audible audible;
        public float timeLeftToForget = 5f;

        public string GetAllegiance()
        {
            return audible.GetAllegiance();
        }
    }

    public List<AudibleHeard> heardAudibles = new();

    private void Update()
    {
        int i = 0;
        foreach (AudibleHeard ah in heardAudibles)
        {
            ah.timeLeftToForget -= Time.deltaTime;
        }

        heardAudibles.RemoveAll(ah => ah.timeLeftToForget <= 0f);

        heardAudibles.Sort((ah1, ah2) =>
            Vector3.Distance(ah1.audible.transform.position, transform.position)
            < Vector3.Distance(ah2.audible.transform.position, transform.position)
                ? 1
                : 0);
    }

    public void NotifyAudible(Audible audible)
    {
        AudibleHeard existingAudibleHeard = heardAudibles.Find(ah => ah.audible == audible);

        if (existingAudibleHeard != null)
        {
            existingAudibleHeard.timeLeftToForget = 5f;
        }
        else
        {
            AudibleHeard audibleHeard = existingAudibleHeard != null ? existingAudibleHeard : new AudibleHeard();
            audibleHeard.audible = audible;
            heardAudibles.Add(audibleHeard);
        }
    }

    public Transform GetBestTarget()
    {
        return heardAudibles.Count > 0 ? heardAudibles[0].audible.transform : null;
    }
}