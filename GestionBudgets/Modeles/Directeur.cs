using System;
using System.Collections.Generic;

namespace GestionBudgets
{
public class Directeur : Utilisateur
    {
        public List<Validation> Validations { get; set; }
        public List<Rapport> Rapports { get; set; }
        public TableauDeBord TableauDeBord { get; set; }

        public Directeur(int id, string nom, string email, string hash) : base(id, nom, email, hash)
        {
            Validations = new List<Validation>();
            Rapports = new List<Rapport>();
            TableauDeBord = new TableauDeBord();
        }

        public Validation validerBudget(int id, Budget budget, bool approuve)
        {
            Validation validation = new Validation(id, budget, this);
            if (approuve)
                validation.approuver();
            else
                validation.rejeter();
            Validations.Add(validation);
            budget.Validations.Add(validation);
            return validation;
        }

        public Rapport genererRapport(int id, Budget budget, string periode)
        {
            Rapport rapport = new Rapport(id, budget, periode);
            rapport.generer();
            Rapports.Add(rapport);
            budget.Rapports.Add(rapport);
            return rapport;
        }

        public TableauDeBord consulterTableauDeBord(Budget budget, string periode)
        {
            TableauDeBord.filtrerParPeriode(periode);
            TableauDeBord.afficherIndicateurs(budget);
            return TableauDeBord;
        }
    }
}
