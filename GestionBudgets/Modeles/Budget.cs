using System;
using System.Collections.Generic;

namespace GestionBudgets
{
public class Budget
    {
        private int idBudget;
        private string libelle;
        private decimal montantTotal;
        private DateTime dateDebut;
        private DateTime dateFin;
        private StatutBudget statut;

        public int IdBudget { get { return idBudget; } }
        public string Libelle { get { return libelle; } set { libelle = value; } }
        public decimal MontantTotal { get { return montantTotal; } }
        public StatutBudget Statut { get { return statut; } set { statut = value; } }
        public DateTime DateDebut { get { return dateDebut; } }
        public DateTime DateFin { get { return dateFin; } }
        public Gestionnaire Gestionnaire { get; set; }
        public List<LigneBudgetaire> Lignes { get; set; }
        public List<Prevision> Previsions { get; set; }
        public List<Validation> Validations { get; set; }
        public List<Rapport> Rapports { get; set; }

        public Budget(int id, string libelle, DateTime debut, DateTime fin)
        {
            idBudget = id;
            this.libelle = libelle;
            dateDebut = debut;
            dateFin = fin;
            statut = StatutBudget.BROUILLON;
            Lignes = new List<LigneBudgetaire>();
            Previsions = new List<Prevision>();
            Validations = new List<Validation>();
            Rapports = new List<Rapport>();
        }

        public void ajouterLigne(LigneBudgetaire ligne)
        {
            ligne.Budget = this;
            Lignes.Add(ligne);
            calculerTotal();
        }

        public decimal calculerTotal()
        {
            montantTotal = 0;
            foreach (LigneBudgetaire ligne in Lignes)
                montantTotal += ligne.MontantPrevu;
            return montantTotal;
        }

        public void soumettreValidation()
        {
            statut = StatutBudget.EN_ATTENTE;
        }

        public void modifier(string libelle)
        {
            this.libelle = libelle;
        }
    }
}
