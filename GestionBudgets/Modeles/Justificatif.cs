using System;
using System.Collections.Generic;

namespace GestionBudgets
{
public class Justificatif
    {
        private int idJustificatif;
        private string nomFichier;
        private string cheminFichier;
        private DateTime dateAjout;

        public Transaction Transaction { get; set; }
        public Justificatif(int id, string nom, string chemin)
        {
            idJustificatif = id;
            nomFichier = nom;
            cheminFichier = chemin;
            dateAjout = DateTime.Today;
        }

        public void televerser(string chemin)
        {
            cheminFichier = chemin;
        }

        public string consulter()
        {
            return cheminFichier;
        }
    }
}
