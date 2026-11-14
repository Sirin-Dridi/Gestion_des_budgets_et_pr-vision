using System;
using System.Collections.Generic;

namespace GestionBudgets
{
class Program
    {
        static void Main(string[] args)
        {
            Gestionnaire gestionnaire = new Gestionnaire(1, "sirin", "amine@email.com", "exempleHash");
            Directeur directeur = new Directeur(2, "Sonia", "sonia@email.com", "exempleHash");

            Budget budget = gestionnaire.creerBudget(1, "Budget annuel", new DateTime(2026, 1, 1), new DateTime(2026, 12, 31));
            LigneBudgetaire ligne = new LigneBudgetaire(1, "Matériel", 10000);
            budget.ajouterLigne(ligne);

            budget.soumettreValidation();
            directeur.validerBudget(1, budget, true);

            Transaction transaction = new Transaction(1, 13000, "Achat de matériel", TypeTransaction.DEPENSE);
            gestionnaire.saisirTransaction(ligne, transaction);

            Ecart ecart = ligne.calculerEcart();
            Console.WriteLine("Budget : " + budget.Libelle);
            Console.WriteLine("Statut : " + budget.Statut);
            Console.WriteLine("Écart : " + ecart.Difference + " DT");

            ServiceAlerte service = new ServiceAlerte();
            if (service.verifierSeuil(ecart, 10))
            {
                Alerte alerte = service.creerAlerte(1, ecart);
                service.notifier(alerte, gestionnaire);
                Console.WriteLine(alerte.Message);
            }

            Console.ReadKey();
        }
    }
}
