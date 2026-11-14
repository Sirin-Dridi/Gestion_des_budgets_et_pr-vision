using System;
using System.Collections.Generic;

namespace GestionBudgets
{
public class Validation
    {
        private int idValidation;
        private DateTime dateDecision;
        private string decision;
        private string commentaire;

        public Budget Budget { get; set; }
        public Directeur Directeur { get; set; }
        public string Decision { get { return decision; } }

        public Validation(int id, Budget budget, Directeur directeur)
        {
            idValidation = id;
            Budget = budget;
            Directeur = directeur;
            decision = "EN_ATTENTE";
            commentaire = "";
        }

        public void approuver()
        {
            decision = "APPROUVE";
            dateDecision = DateTime.Today;
            Budget.Statut = StatutBudget.VALIDE;
        }

        public void rejeter()
        {
            decision = "REJETE";
            dateDecision = DateTime.Today;
            Budget.Statut = StatutBudget.REJETE;
        }
    }
}
