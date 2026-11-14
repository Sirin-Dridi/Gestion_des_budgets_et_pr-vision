using System;
using System.Collections.Generic;

namespace GestionBudgets
{
public class Alerte
    {
        private int idAlerte;
        private string message;
        private DateTime dateEnvoi;
        private string niveau;
        private bool lue;

        public Ecart Ecart { get; set; }
        public Utilisateur Destinataire { get; set; }
        public string Message { get { return message; } }

        public Alerte(int id, string message, string niveau)
        {
            idAlerte = id;
            this.message = message;
            this.niveau = niveau;
            lue = false;
        }

        public void envoyer(Utilisateur destinataire)
        {
            Destinataire = destinataire;
            dateEnvoi = DateTime.Now;
            destinataire.Alertes.Add(this);
        }

        public void marquerLue()
        {
            lue = true;
        }
    }
}
