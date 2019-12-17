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

            //Affichage de grille
            AffichageGrille(grille, grilleCouleur);


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

            return mot;
        }


        public static void AffichageGrille(char[,] grille, int[,] grilleCouleur)
        {
            for (int i = 0; i < grille.GetLength(0); i++)
            {
                Console.Write("\n");
                for (int l = 0; l < grille.GetLength(1); l++)
                {
                    Console.Write(".---");
                }
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
                        default: // pas de char, donc pas de couleur
                            Console.BackgroundColor = ConsoleColor.Black;
                            break;
                    }
                    
                    Console.Write("| " + grille[i, j] + " ");

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
