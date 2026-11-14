using System;
using System.Collections.Generic;

namespace GestionBudgets
{
public abstract class Utilisateur
    {
        private int idUtilisateur;
        private string nom;
        private string email;
        private string motDePasseHash;
        private bool actif;

        public int IdUtilisateur { get { return idUtilisateur; } }
        public string Nom { get { return nom; } set { nom = value; } }
        public string Email { get { return email; } set { email = value; } }
        public bool Actif { get { return actif; } set { actif = value; } }
        public List<Alerte> Alertes { get; set; }

        public Utilisateur(int id, string nom, string email, string hash)
        {
            idUtilisateur = id;
            this.nom = nom;
            this.email = email;
            motDePasseHash = hash;
            actif = true;
            Alertes = new List<Alerte>();
        }

        public bool authentifier(string hash)
        {
            return actif && motDePasseHash == hash;
        }

        public void deconnecter()
        {
            Console.WriteLine("Déconnexion effectuée.");
        }
    }
}
