using System;
using System.Collections.Generic;

namespace GestionBudgets
{
public class Administrateur : Utilisateur
    {
        public List<Utilisateur> Utilisateurs { get; set; }

        public Administrateur(int id, string nom, string email, string hash) : base(id, nom, email, hash)
        {
            Utilisateurs = new List<Utilisateur>();
        }

        public void creerUtilisateur(Utilisateur utilisateur)
        {
            Utilisateurs.Add(utilisateur);
        }

        public void modifierUtilisateur(Utilisateur utilisateur, string nom, string email)
        {
            utilisateur.Nom = nom;
            utilisateur.Email = email;
        }

        public void desactiverUtilisateur(Utilisateur utilisateur)
        {
            utilisateur.Actif = false;
        }
    }
}
