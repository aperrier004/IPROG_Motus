using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motus
{
    class Program
    {
        static void Main(string[] args)
        {
            // Affichage de début
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("          ~~~ MO-MO-MO-MOTUS ~~~          ");
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("\nBy Coline BINET et Alban PERRIER G1 1A ENSC");


            // MENU HERE -TODO
            // Nouvelle Partie

            // Statistiques

            // Options

            // Quitter

            // TODO : clear la console quand on change de "page"

            InstanciationPartie();
            Console.ReadKey();

        }
        
        

        public static void InstanciationPartie()
        {

            string fichierSource = "dico_fr.txt";
            string fichierCible = "dico_reduit.txt";

            int tailleMot = 0;

            
            // Demande de la taille du mot
            do
            {
                Console.WriteLine("Entrer une taille de mot à deviner entre 6 et 10");
                tailleMot = int.Parse(Console.ReadLine());
            }
            while (tailleMot < 5 && tailleMot > 11);

            // Création du fichier avec seulement des mots de la taille demandée
            reduireFichier(fichierSource, tailleMot, fichierCible);

            Console.WriteLine("Réduction terminée."); // A SUPPRIMER 

            Console.WriteLine("Entrer le nombre de tentatives autorisées");
            int nbTentatives = int.Parse(Console.ReadLine());

            Console.WriteLine("Souhaitez-vous mettre un temps limité pour proposer un mot? O/N");
            string temp = Console.ReadLine();
            temp = temp.ToUpper(); //Normalisation de la chaine

            int tempsTour = 0;

            if (temp.Equals("O"))
            {
                do
                {
                    Console.WriteLine("Entrer un temps compris entre 5 et 60 secondes : ");
                    tempsTour = int.Parse(Console.ReadLine());
                }
                while (tempsTour < 4 && tailleMot > 61);
            }

            char[,] grille = new char[nbTentatives, tailleMot];

            // Remplit la grille de caractères espaces pour la verification des cases
            for(int i = 0; i < grille.GetLength(0); i++)
            {
                for (int j = 0; j < grille.GetLength(1); j++)
                {
                    grille[i, j] = '.';
                }
            }

            // grille pour connaitre la couleur a afficher selon la validité de la lettre
            int [,] grilleCouleur = new int[nbTentatives, tailleMot];

            JouerPartie(grille, tempsTour, fichierCible, grilleCouleur);

        }

        public static void JouerPartie(char[,] grille, int tempsTour, string fichierCible, int[,] grilleCouleur)
        {
            //Générer le mot random du dico
            string motInitial = GenerateurMotAleatoire(fichierCible);
            Console.WriteLine(motInitial);
            // Ajout de la lettre connue (première lettre) du mot a la grille sur la 1ère ligne
            grille[0, 0] = motInitial[0];
            grilleCouleur[0, 0] = 1;

            //Affichage de grille
            AffichageGrille(grille, grilleCouleur);

            // Appel a jouer tour
            bool finPartie = false;
            bool partieGagne = false;

            while (!finPartie)
            {
                partieGagne= JouerTour(grille, tempsTour, grilleCouleur, motInitial);
                if(partieGagne)
                {
                    finPartie = partieGagne;
                }
                else if(!grille[grille.GetLength(0), 2].Equals('.'))
                {
                    finPartie = true;
                }
                AffichageGrille(grille, grilleCouleur);
            }

            // Affichage de fin
           FinDePartie(partieGagne, motInitial);


        }

        // Jouer un tour
        public static bool JouerTour(char[,] grille, int tempsTour, int[,] grilleCouleur, string motInitial)
        {
            bool PartieGagné = false;
            string motPropose = "";
            int tourActuel = 0;

            // Demande du mot
            while (motPropose.Length != grille.GetLength(1))
            {
                Console.WriteLine("\nEntrer votre proposition de mot");
                motPropose = Console.ReadLine();
                motPropose = motPropose.ToUpper();
            }

            // Pour savoir a quelle ligne / tentative on est et où il faut ajouter le mot dans la grille
            for(tourActuel = 0; tourActuel < grille.GetLength(0); tourActuel++)
            {
                if (grille[tourActuel, 2].Equals('.'))
                {
                    break;
                }
            }

            // rajouter le mot dans la grille
            for (int i = 0; i < grille.GetLength(1); i++)
            {
                    grille[tourActuel, i] = motPropose[i];
            }

            return VerificationMot(motInitial, motPropose, tourActuel, grilleCouleur);

            
            
        }

        // Check si le mot entré correspond au mot initial
        public static bool VerificationMot(string motInitial, string motPropose, int tourActuel, int [,] grilleCouleur)
        {

            bool gagne = false;
            // Check si le mot est parfaitement égal
            if (motInitial.Equals(motPropose))
            {
                // mettre grille couleur a 1
                for (int i = 0; i < grilleCouleur.GetLength(1); i++)
                {
                    grilleCouleur[tourActuel, i] = 1;
                    
                }
                gagne = true;
            }


            else
            {
                for (int i = 0; i < grilleCouleur.GetLength(1); i++)
                {
                    // Check si des lettres sont bien placées
                    if (motPropose[i].Equals(motInitial[i]))
                    {
                        grilleCouleur[tourActuel, i] = 1;
                    }
                    // Check si mal placée --> 
                    else
                    {
                        int j = i;
                        bool bonneLettre = false;
                        while((!bonneLettre) && (j < grilleCouleur.GetLength(1)))
                        {
                            // lettre présente
                            if (motPropose[i].Equals(motInitial[j]))
                            {
                                grilleCouleur[tourActuel, i] = 2;
                                bonneLettre = true;
                            }
                            j++;
                        }
                            
                    }
                    // Lettres pas présente
                    if (grilleCouleur[tourActuel, i] == 0)
                    {
                        grilleCouleur[tourActuel, i] = -1;
                    }
                }

            }

            return gagne;
            
        }


        // Retourne un mot aléatoire du dico réduit
        public static string GenerateurMotAleatoire(string fichierCible)
        {
            string mot = "";
            try
            {
                Console.WriteLine("Debut generation ");
                // Création d'une instance de StreamReader pour permettre la lecture de notre fichier cible 
                System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
                //Premier reader
                StreamReader monStreamReader = new StreamReader(fichierCible, encoding);
                //Deuxieme reader
                StreamReader monStreamReader2 = new StreamReader(fichierCible, encoding);

                mot = monStreamReader.ReadLine();
                

                int nbMots = 0;
                
                // Nb de mots du fichier
                while (mot != null)
                {
                    mot = monStreamReader.ReadLine();
                    
                    nbMots++;
                }

                // Choix d'un nombre aléatoire entre 1 et le nb de mots du fichiers
                Random rnd = new Random();
                int random = rnd.Next(1, nbMots);
                
                
                nbMots = 0;

                // Parcours du dico pour trouver le mot correspondant a la ligne aléatoire chosie
                while (nbMots != random)
                {
                    mot = monStreamReader2.ReadLine();
                    nbMots++;
                }
                
                // Fermeture du StreamReader (attention très important) 
                monStreamReader.Close();
                monStreamReader2.Close();


            }
            catch (Exception ex)
            {
                // Code exécuté en cas d'exception 
                Console.Write("Une erreur est survenue au cours de l'opération :");
                Console.WriteLine(ex.Message);
            }

            return mot.ToUpper();
        }


        // Affichage de fin
        public static void FinDePartie(bool partieGagne, string motInitial)
        {
            // Cas victorieux
            if(partieGagne)
            {
                Console.WriteLine("Bravo, vous avez gagné la partie!");
            }
            else //t'as perdu loser
            {
                Console.WriteLine("Tu feras mieux la prochaine fois!");
                Console.WriteLine("Le mot a deviné était : " + motInitial);
            }

            // Demande de rejouer
            Console.WriteLine("Souhaitez-vous rejouer? O/N");
            string rejouer = Console.ReadLine();

            if (rejouer.Equals("O"))
            {
                InstanciationPartie();
            }
            else
            {
                // revenir au menu
            }

        }

        public static void AffichageGrille(char[,] grille, int[,] grilleCouleur)
        {
            for (int i = 0; i < grille.GetLength(0); i++)
            {
                /*Console.Write("\n");
                for (int l = 0; l < grille.GetLength(1); l++)
                {
                    Console.Write(".---");
                }*/
                Console.Write("\n");
                for (int j = 0; j < grille.GetLength(1); j++)
                {
                    int caseSwitch = grilleCouleur[i,j];

                    switch (caseSwitch)
                    {
                        case 1: // lettre bien placée = rouge
                            Console.BackgroundColor = ConsoleColor.Red;
                            break;
                        case 2: // lettre présente mais mal placée = jaune
                            Console.BackgroundColor = ConsoleColor.Yellow;
                            break;
                        case -1: // lettre non présente = bleue
                            Console.BackgroundColor = ConsoleColor.Blue;
                            break;
                        case 0: // pas de char, donc pas de couleur
                            Console.BackgroundColor = ConsoleColor.Black;
                            break;
                    }
                    
                    Console.Write(grille[i, j] );

                }

            }
          
        }

        

        // Enleve tous les accents 
        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }


        // Création du dico correspondant au nb de lettres souhaité
        public static void reduireFichier(string fichier_source, int tailleMot, string fichier_cible)
        {
            try
            {
                // Création d'une instance de StreamReader pour permettre la lecture de notre fichier source 
                System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
                StreamReader monStreamReader = new StreamReader(fichier_source, encoding);

                // Création d'une instance de StreamWriter pour permettre l'ecriture de notre fichier cible
                StreamWriter monStreamWriter = File.CreateText(fichier_cible);
                
                string mot = monStreamReader.ReadLine();

                // Lecture de tous les mots du fichier (un par lignes) 
                while (mot != null)
                {
                    if (mot.Length == tailleMot)              // tous les mots de la taille donnée
                    {
                        mot = RemoveDiacritics(mot);        //... on convertit le mot sans accent
                        monStreamWriter.WriteLine(mot);   //... on écrit dans le fichier cible
                    }
                        
                    mot = monStreamReader.ReadLine();

                }
                // Fermeture du StreamReader (attention très important) 
                monStreamReader.Close();
                // Fermeture du StreamWriter (attention très important) 
                monStreamWriter.Close();
            }
            catch (Exception ex)
            {
                // Code exécuté en cas d'exception 
                Console.Write("Une erreur est survenue au cours de l'opération :");
                Console.WriteLine(ex.Message);
            }
        }



    }
}
