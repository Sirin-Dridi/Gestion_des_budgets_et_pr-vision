using System;
using System.Collections.Generic;

namespace GestionBudgets
{
public class ServiceAlerte
    {
        public bool verifierSeuil(Ecart ecart, decimal seuil)
        {
            return ecart.Pourcentage > seuil;
        }

        public Alerte creerAlerte(int id, Ecart ecart)
        {
            Alerte alerte = new Alerte(id, "Dépassement du budget", "Attention");
            alerte.Ecart = ecart;
            ecart.Alertes.Add(alerte);
            return alerte;
        }

        public void notifier(Alerte alerte, Utilisateur utilisateur)
        {
            alerte.envoyer(utilisateur);
        }
    }
}
