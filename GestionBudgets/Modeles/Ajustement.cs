using System;
using System.Collections.Generic;

namespace GestionBudgets
{
public class Ajustement
    {
        private int idAjustement;
        private DateTime dateAjustement;
        private decimal ancienMontant;
        private decimal nouveauMontant;
        private string motif;

        public LigneBudgetaire Ligne { get; set; }
        public Ecart Ecart { get; set; }

        public int IdAjustement => idAjustement;
        public DateTime DateAjustement => dateAjustement;
        public decimal AncienMontant => ancienMontant;
        public decimal NouveauMontant => nouveauMontant;
        public string Motif => motif;

        private Ajustement()
        {
        }

        public Ajustement(int id, LigneBudgetaire ligne, Ecart ecart, decimal nouveau, string motif)
        {
            idAjustement = id;
            dateAjustement = DateTime.Now;
            Ligne = ligne;
            Ecart = ecart;
            ancienMontant = ligne.MontantPrevu;
            nouveauMontant = nouveau;
            this.motif = motif;
            ligne.Ajustements.Add(this);
            ecart.Ajustements.Add(this);
        }

        public void appliquer()
        {
            Ligne.MontantPrevu = nouveauMontant;
            if (Ligne.Budget != null)
                Ligne.Budget.calculerTotal();
        }

        public void annuler()
        {
            Ligne.MontantPrevu = ancienMontant;
            if (Ligne.Budget != null)
                Ligne.Budget.calculerTotal();
        }
    }
}

