using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Akgul_Ilhan_cal
{
    public partial class MainWindow : Window
    {
        // Mémorise le premier nombre saisi
        private double _valeurStockee = 0;

        // Mémorise l'opérateur choisi (+, -, *, /, ^)
        private string _operateurEnCours = "";

        // Indique si on doit repartir d'un affichage vide au prochain chiffre
        private bool _nouvelleSaisie = true;

        // Indique si le panneau scientifique est affiché
        private bool _modeScientifique = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        // ================== CLAVIER ==================

        // Chiffres et opérateurs (marche aussi avec le pavé numérique et en AZERTY)
        private void Window_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string t = e.Text;

            if (t.Length == 1 && char.IsDigit(t[0]))
            {
                AjouterChiffre(t);
                e.Handled = true;
            }
            else if (t == "+" || t == "-" || t == "*" || t == "/" || t == "^")
            {
                ChoisirOperateur(t);
                e.Handled = true;
            }
            else if (t == "=")
            {
                Egale();
                e.Handled = true;
            }
        }

        // Touches spéciales
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    Egale();
                    e.Handled = true;
                    break;

                case Key.Escape:
                    //Tout Effacer
                    Effacer();
                    e.Handled = true;
                    break;

                case Key.Back:
                    //Retour en arriere
                    Supprimer();
                    e.Handled = true;
                    break;

                case Key.Space:
                    // Empêche de re-cliquer sur le dernier bouton qui a le focus
                    e.Handled = true;
                    break;
            }
        }

        // ================== BOUTONS ==================

        private void BTN_Chiffre_Click(object sender, RoutedEventArgs e)
        {
            AjouterChiffre((sender as Button)?.Content.ToString());
        }

        private void BTN_Operateur_Click(object sender, RoutedEventArgs e)
        {
            ChoisirOperateur((sender as Button)?.Content.ToString());
        }

        private void BTN_Egale_Click(object sender, RoutedEventArgs e)
        {
            Egale();
        }

        private void BTN_CLR_Click(object sender, RoutedEventArgs e)
        {
            Effacer();
        }

        // Bouton S : affiche / cache le panneau scientifique (la fenêtre s'agrandit vers la droite)
        private void BTN_Calculette_Scientifique(object sender, RoutedEventArgs e)
        {
            _modeScientifique = !_modeScientifique;

            if (_modeScientifique)
            {
                COL_Scientifique.Width = new GridLength(300);
                PANEL_Scientifique.Visibility = Visibility.Visible;
                Width = 720;
            }
            else
            {
                PANEL_Scientifique.Visibility = Visibility.Collapsed;
                COL_Scientifique.Width = new GridLength(0);
                Width = 420;
            }
        }

        // Boutons du panneau scientifique (sin, cos, √, xʸ, ...)
        private void BTN_Fonction_Click(object sender, RoutedEventArgs e)
        {
            string fonction = (sender as Button)?.Tag?.ToString();

            // xʸ a besoin de deux nombres : on le traite comme un opérateur
            if (fonction == "^")
            {
                ChoisirOperateur("^");
                return;
            }

            AppliquerFonction(fonction);
        }

        // ================== LOGIQUE ==================

        private void AjouterChiffre(string chiffre)
        {
            if (_nouvelleSaisie || TB_Display.Text == "0")
            {
                TB_Display.Text = chiffre;
                _nouvelleSaisie = false;
            }
            else
            {
                TB_Display.Text += chiffre;
            }
        }

        private void ChoisirOperateur(string operateur)
        {
            // Si un calcul est déjà en cours, on l'exécute d'abord (calcul en chaîne)
            if (!string.IsNullOrEmpty(_operateurEnCours) && !_nouvelleSaisie)
            {
                CalculerResultat();
            }

            _valeurStockee = double.Parse(TB_Display.Text);
            _operateurEnCours = operateur;
            _nouvelleSaisie = true;
        }

        private void Egale()
        {
            CalculerResultat();
            _operateurEnCours = "";
            _nouvelleSaisie = true;
        }

        private void Effacer()
        {
            TB_Display.Text = "0";
            _valeurStockee = 0;
            _operateurEnCours = "";
            _nouvelleSaisie = true;
        }

        private void Supprimer()
        {
            // Après un résultat ou un opérateur, on ne supprime rien
            if (_nouvelleSaisie)
                return;

            // Il ne reste qu'un caractère (ou "-5" qui deviendrait "-") : on remet 0
            if (TB_Display.Text.Length <= 1 ||
                (TB_Display.Text.Length == 2 && TB_Display.Text.StartsWith("-")))
            {
                TB_Display.Text = "0";
                _nouvelleSaisie = true;
            }
            else
            {
                // Retire le dernier caractère : 12 devient 1
                TB_Display.Text = TB_Display.Text.Substring(0, TB_Display.Text.Length - 1);
            }
        }

        // Applique une fonction scientifique à la valeur affichée
        private void AppliquerFonction(string fonction)
        {
            double x = double.Parse(TB_Display.Text);
            double r;

            switch (fonction)
            {
                // Trigonométrie en degrés
                case "sin": r = Math.Sin(x * Math.PI / 180); break;
                case "cos": r = Math.Cos(x * Math.PI / 180); break;
                case "tan": r = Math.Tan(x * Math.PI / 180); break;
                case "sqrt": r = Math.Sqrt(x); break;
                case "carre": r = x * x; break;
                case "log": r = Math.Log10(x); break;
                case "ln": r = Math.Log(x); break;
                case "inv": r = 1 / x; break;
                case "pi": r = Math.PI; break;
                case "neg": r = -x; break;
                case "fact":
                    // Factorielle : entiers de 0 à 170 seulement
                    if (x < 0 || x != Math.Floor(x) || x > 170)
                    {
                        r = double.NaN;
                        break;
                    }
                    r = 1;
                    for (int i = 2; i <= (int)x; i++)
                        r *= i;
                    break;
                default:
                    return;
            }

            if (double.IsNaN(r) || double.IsInfinity(r))
            {
                MessageBox.Show("Opération impossible.");
                Effacer();
                return;
            }

            // Arrondi pour éviter 1,2246E-16 à la place de 0 (ex : sin 180)
            TB_Display.Text = Math.Round(r, 10).ToString();

            // Pour ±, on laisse l'utilisateur continuer à taper son nombre
            if (fonction != "neg")
                _nouvelleSaisie = true;
        }

        // Effectue le calcul entre la valeur stockée et la valeur affichée
        private void CalculerResultat()
        {
            if (string.IsNullOrEmpty(_operateurEnCours))
                return;

            double valeurActuelle = double.Parse(TB_Display.Text);
            double resultat = 0;

            switch (_operateurEnCours)
            {
                case "+":
                    resultat = _valeurStockee + valeurActuelle;
                    break;
                case "-":
                    resultat = _valeurStockee - valeurActuelle;
                    break;
                case "*":
                    resultat = _valeurStockee * valeurActuelle;
                    break;
                case "/":
                    if (valeurActuelle == 0)
                    {
                        MessageBox.Show("Division par zéro impossible.");
                        TB_Display.Text = "0";
                        _valeurStockee = 0;
                        return;
                    }
                    resultat = _valeurStockee / valeurActuelle;
                    break;
                case "^":
                    resultat = Math.Pow(_valeurStockee, valeurActuelle);
                    break;
            }

            if (double.IsNaN(resultat) || double.IsInfinity(resultat))
            {
                MessageBox.Show("Opération impossible.");
                TB_Display.Text = "0";
                _valeurStockee = 0;
                return;
            }

            TB_Display.Text = resultat.ToString();
            _valeurStockee = resultat;
        }
    }
}