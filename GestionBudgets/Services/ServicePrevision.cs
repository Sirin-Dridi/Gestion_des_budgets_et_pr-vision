using System;
using System.Collections.Generic;

namespace GestionBudgets
{
public class ServicePrevision
    {
        public decimal? ProjeterDepenses(Budget budget, LigneBudgetaire ligne, DateTime au)
        {
            DateTime debut = budget.DateDebut.Date;
            DateTime fin = budget.DateFin.Date;
            DateTime observation = au.Date < fin ? au.Date : fin;
            if (observation < debut)
                return null;

            var depenses = ligne.Transactions.Where(t => t.Type == TypeTransaction.DEPENSE
                && t.DateTransaction.Date >= debut && t.DateTransaction.Date <= observation).ToList();
            if (depenses.Count == 0 && observation < fin)
                return null;

            int joursEcoules = (observation - debut).Days + 1;
            int duree = (fin - debut).Days + 1;
            return Math.Round(depenses.Sum(t => t.Montant) / joursEcoules * duree, 2);
        }

        public decimal calculerPrevision(Prevision prevision)
        {
            return prevision.calculer();
        }

        public void selectionnerMethode(Prevision prevision, MethodePrevision methode)
        {
            prevision.Methode = methode;
        }
    }
}

