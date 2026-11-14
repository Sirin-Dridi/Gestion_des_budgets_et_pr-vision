using System;
using System.Collections.Generic;

namespace GestionBudgets
{
public class ServicePrevision
    {
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
