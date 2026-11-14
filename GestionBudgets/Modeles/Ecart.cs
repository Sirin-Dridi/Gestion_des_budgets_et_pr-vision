using System;
using System.Collections.Generic;

namespace GestionBudgets
{
public class Ecart
    {
        private int idEcart;
        private decimal montantPrevu;
        private decimal montantRealise;
        private decimal difference;
        private decimal pourcentage;

        public LigneBudgetaire Ligne { get; set; }
        public List<Alerte> Alertes { get; set; }
        public List<Ajustement> Ajustements { get; set; }
        public decimal Difference { get { return difference; } }
        public decimal Pourcentage { get { return pourcentage; } }

        public Ecart(int id, decimal prevu, decimal realise)
        {
            idEcart = id;
            montantPrevu = prevu;
            montantRealise = realise;
            Alertes = new List<Alerte>();
            Ajustements = new List<Ajustement>();
            calculerDifference();
            calculerPourcentage();
        }

        public decimal calculerDifference()
        {
            difference = montantRealise - montantPrevu;
            return difference;
        }

        public decimal calculerPourcentage()
        {
            if (montantPrevu == 0)
                pourcentage = 0;
            else
                pourcentage = (difference / montantPrevu) * 100;
            return pourcentage;
        }
    }
}
