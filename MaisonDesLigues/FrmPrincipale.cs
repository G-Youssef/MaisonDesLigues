﻿using System;
using System.Data;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Configuration;
using System.Collections.ObjectModel;
using ComposantNuite;
using ComposantPPE;
using BaseDeDonnees;
namespace MaisonDesLigues
{
    public partial class FrmPrincipale : Form
    {
        private const int CS_NOCLOSE = 0x0200;
        /// <summary>
        /// Desactive La croix rouge du winform
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_NOCLOSE;
                return cp;
            }
        }

        /// <summary>
        /// constructeur du formulaire
        /// </summary>
        public FrmPrincipale()
        {
            InitializeComponent();
        }
        private Bdd UneConnexion;
        private String TitreApplication;
        private String IdStatutSelectionne = "";
        private ComposantPPE.ComposantPPE UnComposantVac;
        /// <summary>
        /// création et ouverture d'une connexion vers la base de données sur le chargement du formulaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmPrincipale_Load(object sender, EventArgs e)
        {
            UneConnexion = ((FrmLogin)Owner).UneConnexion;
            TitreApplication = ((FrmLogin)Owner).TitreApplication;
            this.Text = TitreApplication;
        }
        /// <summary>
        /// gestion de l'événement click du bouton quitter.
        /// Demande de confirmation avant de quitetr l'application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmdQuitter_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Voulez-vous quitter l'application ?", ConfigurationManager.AppSettings["TitreApplication"], MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                UneConnexion.FermerConnexion();
                Application.Exit();
            }
        }

        private void RadTypeParticipant_Changed(object sender, EventArgs e)
        {
            switch (((RadioButton)sender).Name)
            {
                case "RadBenevole":
                    this.GererInscriptionBenevole();
                    break;
                case "RadLicencie":
                    //this.GererInscriptionLicencie();
                    break;
                case "RadIntervenant":
                    this.GererInscriptionIntervenant();
                    break;

                default:
                    throw new Exception("Erreur interne à l'application");
            }
        }

        /// <summary>     
        /// procédure permettant d'afficher l'interface de saisie du complément d'inscription d'un intervenant.
        /// </summary>
        private void GererInscriptionIntervenant()
        {

            GrpBenevole.Visible = false;
            GrpIntervenant.Visible = true;
            PanFonctionIntervenant.Visible = true;
            GrpIntervenant.Left = 23;
            GrpIntervenant.Top = 264;
            Utilitaire.CreerDesControles(this, UneConnexion, "VSTATUT01", "Rad_", PanFonctionIntervenant, "RadioButton", this.rdbStatutIntervenant_StateChanged);
            Utilitaire.RemplirComboBox(UneConnexion, CmbAtelierIntervenant, "VATELIER01");

            CmbAtelierIntervenant.Text = "Choisir";

        }

        /// <summary>     
        /// procédure permettant d'afficher l'interface de saisie des disponibilités des bénévoles.
        /// </summary>
        private void GererInscriptionBenevole()
        {

            GrpBenevole.Visible = true;
            GrpBenevole.Left = 23;
            GrpBenevole.Top = 264;
            GrpIntervenant.Visible = false;

            Utilitaire.CreerDesControles(this, UneConnexion, "VDATEBENEVOLAT01", "ChkDateB_", PanelDispoBenevole, "CheckBox", this.rdbStatutIntervenant_StateChanged);
            // on va tester si le controle à placer est de type CheckBox afin de lui placer un événement checked_changed
            // Ceci afin de désactiver les boutons si aucune case à cocher du container n'est cochée
            foreach (Control UnControle in PanelDispoBenevole.Controls)
            {
                if (UnControle.GetType().Name == "CheckBox")
                {
                    CheckBox UneCheckBox = (CheckBox)UnControle;
                    UneCheckBox.CheckedChanged += new System.EventHandler(this.ChkDateBenevole_CheckedChanged);
                }
            }


        }
        /// <summary>
        /// permet d'appeler la méthode VerifBtnEnregistreIntervenant qui déterminera le statu du bouton BtnEnregistrerIntervenant
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbStatutIntervenant_StateChanged(object sender, EventArgs e)
        {
            // stocke dans un membre de niveau form l'identifiant du statut sélectionné (voir règle de nommage des noms des controles : prefixe_Id)
            this.IdStatutSelectionne = ((RadioButton)sender).Name.Split('_')[1];
            BtnEnregistrerIntervenant.Enabled = VerifBtnEnregistreIntervenant();
        }
        /// <summary>
        /// Permet d'intercepter le click sur le bouton d'enregistrement d'un bénévole.
        /// Cetteméthode va appeler la méthode InscrireBenevole de la Bdd, après avoir mis en forme certains paramètres à envoyer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEnregistreBenevole_Click(object sender, EventArgs e)
        {
            Collection<Int16> IdDatesSelectionnees = new Collection<Int16>();
            Int64? NumeroLicence;
            if (TxtLicenceBenevole.MaskCompleted)
            {
                NumeroLicence = System.Convert.ToInt64(TxtLicenceBenevole.Text);
            }
            else
            {
                NumeroLicence = null;
            }


            foreach (Control UnControle in PanelDispoBenevole.Controls)
            {
                if (UnControle.GetType().Name == "CheckBox" && ((CheckBox)UnControle).Checked)
                {
                    /* Un name de controle est toujours formé come ceci : xxx_Id où id représente l'id dans la table
                     * Donc on splite la chaine et on récupére le deuxième élément qui correspond à l'id de l'élément sélectionné.
                     * on rajoute cet id dans la collection des id des dates sélectionnées
                        
                    */
                    IdDatesSelectionnees.Add(System.Convert.ToInt16((UnControle.Name.Split('_'))[1]));
                }
            }
            UneConnexion.InscrireBenevole(TxtNom.Text, TxtPrenom.Text, TxtAdr1.Text, TxtAdr2.Text != "" ? TxtAdr2.Text : null, TxtCp.Text, TxtVille.Text, txtTel.MaskCompleted ? txtTel.Text : null, TxtMail.Text != "" ? TxtMail.Text : null, System.Convert.ToDateTime(TxtDateNaissance.Text), NumeroLicence, IdDatesSelectionnees);

        }
        /// <summary>
        /// Cetet méthode teste les données saisies afin d'activer ou désactiver le bouton d'enregistrement d'un bénévole
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChkDateBenevole_CheckedChanged(object sender, EventArgs e)
        {
            BtnEnregistreBenevole.Enabled = (TxtLicenceBenevole.Text == "" || TxtLicenceBenevole.MaskCompleted) && TxtDateNaissance.MaskCompleted && Utilitaire.CompteChecked(PanelDispoBenevole) > 0;
        }
        /// <summary>
        /// Méthode qui permet d'afficher ou masquer le controle panel permettant la saisie des nuités d'un intervenant.
        /// S'il faut rendre visible le panel, on teste si les nuités possibles ont été chargés dans ce panel. Si non, on les charges 
        /// On charge ici autant de contrôles ResaNuit qu'il y a de nuits possibles
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RdbNuiteIntervenant_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Name == "RdbNuiteIntervenantOui")
            {
                PanNuiteIntervenant.Visible = true;
                if (PanNuiteIntervenant.Controls.Count == 0) // on charge les nuites possibles possibles et on les affiche
                {
                    //DataTable LesDateNuites = UneConnexion.ObtenirDonnesOracle("VDATENUITE01");
                    //foreach(Dat
                    Dictionary<Int16, String> LesNuites = UneConnexion.ObtenirDatesNuites();
                    int i = 0;
                    foreach (KeyValuePair<Int16, String> UneNuite in LesNuites)
                    {
                        ComposantNuite.ResaNuite unResaNuit = new ResaNuite(UneConnexion.ObtenirDonnesOracle("VHOTEL01"), (UneConnexion.ObtenirDonnesOracle("VCATEGORIECHAMBRE01")), UneNuite.Value, UneNuite.Key);
                        unResaNuit.Left = 5;
                        unResaNuit.Top = 5 + (24 * i++);
                        unResaNuit.Visible = true;
                        //unResaNuit.click += new System.EventHandler(ComposantNuite_StateChanged);
                        PanNuiteIntervenant.Controls.Add(unResaNuit);
                    }


                }

            }

            else
            {
                PanNuiteIntervenant.Visible = false;

            }
            BtnEnregistrerIntervenant.Enabled = VerifBtnEnregistreIntervenant();

        }

        /// <summary>
        /// Cette procédure va appeler la procédure .... qui aura pour but d'enregistrer les éléments 
        /// de l'inscription d'un intervenant, avec éventuellment les nuités à prendre en compte        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEnregistrerIntervenant_Click(object sender, EventArgs e)
        {
            try
            {
                if (RdbNuiteIntervenantOui.Checked)
                {
                    // inscription avec les nuitées
                    Collection<Int16> NuitsSelectionnes = new Collection<Int16>();
                    Collection<String> HotelsSelectionnes = new Collection<String>();
                    Collection<String> CategoriesSelectionnees = new Collection<string>();
                    foreach (Control UnControle in PanNuiteIntervenant.Controls)
                    {
                        if (UnControle.GetType().Name == "ResaNuite" && ((ResaNuite)UnControle).GetNuitSelectionnee())
                        {
                            // la nuité a été cochée, il faut donc envoyer l'hotel et la type de chambre à la procédure de la base qui va enregistrer le contenu hébergement 
                            //ContenuUnHebergement UnContenuUnHebergement= new ContenuUnHebergement();
                            CategoriesSelectionnees.Add(((ResaNuite)UnControle).GetTypeChambreSelectionnee());
                            HotelsSelectionnes.Add(((ResaNuite)UnControle).GetHotelSelectionne());
                            NuitsSelectionnes.Add(((ResaNuite)UnControle).IdNuite);
                        }

                    }
                    if (NuitsSelectionnes.Count == 0)
                    {
                        MessageBox.Show("Si vous avez sélectionné que l'intervenant avait des nuités\n in faut qu'au moins une nuit soit sélectionnée");
                    }
                    else
                    {
                        UneConnexion.InscrireIntervenant(TxtNom.Text, TxtPrenom.Text, TxtAdr1.Text, TxtAdr2.Text != "" ? TxtAdr2.Text : null, TxtCp.Text, TxtVille.Text, txtTel.MaskCompleted ? txtTel.Text : null, TxtMail.Text != "" ? TxtMail.Text : null, System.Convert.ToInt16(CmbAtelierIntervenant.SelectedValue), this.IdStatutSelectionne, CategoriesSelectionnees, HotelsSelectionnes, NuitsSelectionnes);
                        MessageBox.Show("Inscription intervenant effectuée");
                    }
                }
                else
                { // inscription sans les nuitées
                    UneConnexion.InscrireIntervenant(TxtNom.Text, TxtPrenom.Text, TxtAdr1.Text, TxtAdr2.Text != "" ? TxtAdr2.Text : null, TxtCp.Text, TxtVille.Text, txtTel.MaskCompleted ? txtTel.Text : null, TxtMail.Text != "" ? TxtMail.Text : null, System.Convert.ToInt16(CmbAtelierIntervenant.SelectedValue), this.IdStatutSelectionne);
                    MessageBox.Show("Inscription intervenant effectuée");

                }


            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        /// <summary>
        /// Méthode privée testant le contrôle combo et la variable IdStatutSelectionne qui contient une valeur
        /// Cette méthode permetra ensuite de définir l'état du bouton BtnEnregistrerIntervenant
        /// </summary>
        /// <returns></returns>
        private Boolean VerifBtnEnregistreIntervenant()
        {
            return CmbAtelierIntervenant.Text != "Choisir" && this.IdStatutSelectionne.Length > 0;
        }
        /// <summary>
        /// Méthode permettant de définir le statut activé/désactivé du bouton BtnEnregistrerIntervenant
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmbAtelierIntervenant_TextChanged(object sender, EventArgs e)
        {
            BtnEnregistrerIntervenant.Enabled = VerifBtnEnregistreIntervenant();
        }



        //*************************************************Debut changement du code************************************************//

        /// <summary>
        /// procedure privée permettant de faire appel à des procédure lorsque l'utilisateur check l'un des bouton radio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radGestionTypeTable_changed(object sender, EventArgs e)
        {
            switch (((RadioButton)sender).Name)
            {
                case "radAtelier":
                    this.GererAjoutAtelier();
                    break;
                case "radTheme":
                    if (radTheme.Checked == true)
                    {
                        this.GererAjoutTheme();
                    }
                    break;
                case "radVacation":
                    if (radVacation.Checked == true)
                    {
                        this.GererAjoutVacation();
                    }
                    break;
                case "radVacModif":
                    if (radVacModif.Checked == true)
                    {
                        this.GererModifVacation();
                    }
                    break;
                default:
                    throw new Exception("Erreur interne à l'application");
            }
        }

        /// <summary>
        /// procédure permettant d'afficher l'interface de saisie de Modifier une Vacation
        /// </summary>
        private void GererModifVacation()
        {
            //On test si le composant "information Vacation" existe,alors on le supprime.
            if (this.grpModifVacation.Controls.Contains(UnComposantVac))
            {
                this.grpModifVacation.Controls.Remove(UnComposantVac);
            }

            CmbVacationModif.Visible = false;
            Lb_ComposVaca.Visible = false;
            LbNumVac.Visible = false;
            PanComposantVac.Visible = false;
            grpAjoutAtelier.Visible = false;
            grpAjoutTheme.Visible = false;
            grpAjoutVacation.Visible = false;
            grpModifVacation.Visible = true;
            grpModifVacation.Left = 95;
            grpModifVacation.Top = 77;
            BtnModifVacation.Enabled = false;

            Utilitaire.RemplirComboBox(UneConnexion, UneConnexion.ObtenirDonnesOracleAtVac(), CmbAtVacUpdate, "libelle", "id");
            CmbAtVacUpdate.Text = "Choisir un Atelier dans cette liste";

            ////Test sur la liste déroulante Atelier afin de savoir si celle-ci est vide.
            if (CmbAtVacUpdate.Items.Count == 0)
            {
                radVacModif.Checked = false;
                grpModifVacation.Visible = false;

                MessageBox.Show("Il n'existe pas d'atelier possédant des vacations !");

            }
        }

        /// <summary>
        /// procédure permettant d'afficher l'interface de saisie de Ajouter un Atelier
        /// </summary>
        private void GererAjoutAtelier()
        {

            PanComposantVac.Visible = false;
            grpModifVacation.Visible = false;
            grpAjoutAtelier.Visible = true;
            grpAjoutTheme.Visible = false;
            grpAjoutVacation.Visible = false;
            grpAjoutAtelier.Left = 179;
            grpAjoutAtelier.Top = 75;


        }

        /// <summary>
        /// procédure publique permettant d'afficher l'interface de saisie de Ajouter un Theme.
        /// </summary>
        private void GererAjoutTheme()
        {

            TxtLibelleTheme_0.Text = "";
            PanComposantVac.Visible = false;
            grpAjoutAtelier.Visible = false;
            grpAjoutTheme.Visible = true;
            grpAjoutVacation.Visible = false;
            grpModifVacation.Visible = false;
            grpAjoutTheme.Left = 168;
            grpAjoutTheme.Top = 75;


            Utilitaire.RemplirComboBox(UneConnexion, CmbAtelierTheme, "VATELIER01");
            CmbAtelierTheme.Text = "Choisir un Atelier dans cette liste";

            //Test sur la liste déroulante Atelier afin de savoir si celle-ci est vide.
            if (CmbAtelierTheme.Items.Count == 0)
            {
                radTheme.Checked = false;
                grpAjoutTheme.Visible = false;
                MessageBox.Show("Atention il n'existe pas d'atelier pour le moment, vous ne pouvez créer un theme !");

            }


        }

        /// <summary>
        /// procédure publique permettant d'afficher l'interface de saisie de Ajouter une Vacation.
        /// </summary>
        private void GererAjoutVacation()
        {
            PanComposantVac.Visible = true;
            grpAjoutAtelier.Visible = false;
            grpAjoutTheme.Visible = false;
            grpAjoutVacation.Visible = true;
            grpModifVacation.Visible = false;
            grpAjoutVacation.Left = 139;
            grpAjoutVacation.Top = 75;
            PanComposantVac.Left = 85;
            PanComposantVac.Top = 79;

            //On remplit notre combobox d'atelier avec les données passée en paramètres ci dessous
            Utilitaire.RemplirComboBox(UneConnexion, CmbAtVacInsert, "VATELIER01");
            CmbAtVacInsert.Text = "Choisir un Atelier dans cette liste";

            //Test sur la liste déroulante Atelier afin de savoir si celle-ci est vide.
            if (CmbAtVacInsert.Items.Count == 0)
            {
                radVacation.Checked = false;
                grpAjoutVacation.Visible = false;
                PanComposantVac.Visible = false;
                MessageBox.Show("Atention il n'existe pas d'atelier pour le moment, vous ne pouvez créer une vacation !");

            }
        }

        /// <summary>
        /// gestion de l'événement click du bouton quitter.
        /// Demande de confirmation avant de quitetr l'application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmdQuitter2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Voulez-vous quitter l'application ?", ConfigurationManager.AppSettings["TitreApplication"], MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                UneConnexion.FermerConnexion();
                Application.Exit();
            }
        }


        /// <summary>
        /// Cette procédure va appeler la procédure .... qui aura pour but d'enregistrer les éléments 
        /// de l'ajout d'un Atelier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAjoutAtelier_Click(object sender, EventArgs e)
        {
            try
            {// Concatenation de la date de la vacation avec l'heure de debut ou de fin de vacation recuperer du composant
                String DateHeureDeb = CompoVacAtelier.GetDate() + " " + Convert.ToString(CompoVacAtelier.GetHeureDeb());
                String DateHeureFin = CompoVacAtelier.GetDate() + " " + Convert.ToString(CompoVacAtelier.GetHeureFin());

                UneConnexion.creerAtelier(TxtLibelleAtelier.Text, Convert.ToInt32(NupAtPlaceMax.Value), TxtThemeAtelier.Text, Convert.ToDateTime(DateHeureDeb), Convert.ToDateTime(DateHeureFin));


                MessageBox.Show("L'Atelier " + TxtLibelleAtelier.Text + " a bien été créé !");
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }

        }

        /// <summary>
        /// Cette procédure va appeler la procédure .... qui aura pour but d'enregistrer les éléments 
        /// de l'ajout d'un Theme
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAjoutTheme_Click(object sender, EventArgs e)
        {
            try
            {
                //On parcours tous les contrôles situés dans le panel Theme
                //foreach (Control UnControle in PanTheme.Controls)
                //{
                //On test si le controle est un textbox ou MaskedTextBox
                //if (UnControle.GetType().Name == "TextBox")
                //{
                //    if (UnControle.Text == "")
                //    {
                //        MessageBox.Show("un ou plusieurs champs n'ont pas été remplis,Celle-ci n'ont pas été ajouté");
                //    }
                //else
                //{

                //On Ajoute plusieurs themes en même temps
                UneConnexion.creerTheme(Convert.ToInt16(CmbAtelierTheme.SelectedValue), TxtLibelleTheme_0.Text);
                CmbAtelierTheme.Text = "Choisir un Atelier dans cette liste";
                TxtLibelleTheme_0.Text = "";
                MessageBox.Show("Le(s) Theme(s) a/ont bien été créé(s) !");
                //}
                //if (UnControle.Name.StartsWith("TxtLibelTheme_"))
                //{
                //    UnControle.Dispose();
                //}
                //}
                //}

                //BtnAutreThemeAtelier.Enabled = true;
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        /// <summary>
        /// Cette procédure va appeler la procédure .... qui aura pour but d'enregistrer les éléments 
        /// de l'ajout d'une Vacation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAjoutVacation_Click(object sender, EventArgs e)
        {
            try
            {
             

                        // Concatenation de la date de la vacation avec l'heure de debut ou de fin de vacation recuperer du composant
                        String DateHeureDeb = (composantPPE3.GetDate() + " " + composantPPE3.GetHeureDeb());
                        String DateHeureFin = (composantPPE3.GetDate() + " " + composantPPE3.GetHeureFin());

                        UneConnexion.creerVacation(Convert.ToInt32(CmbAtVacInsert.SelectedValue), DateHeureDeb, DateHeureFin);
                        MessageBox.Show("La Vacation a bien été créé !");
                    
                

               

            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        /// <summary>
        /// Cette procédure va appeler la procédure .... qui aura pour but d'enregistrer les éléments 
        /// de la modification d'une Vacation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnModifVacation_Click(object sender, EventArgs e)
        {
            try
            {
                // Concatenation de la date de la vacation avec l'heure de debut ou de fin de vacation recuperer du composant
                String DateHeureDeb = UnComposantVac.GetDate() + " " + Convert.ToString(UnComposantVac.GetHeureDeb());
                String DateHeureFin = UnComposantVac.GetDate() + " " + Convert.ToString(UnComposantVac.GetHeureFin());

                UneConnexion.MajVacation(Convert.ToDateTime(DateHeureDeb), Convert.ToDateTime(DateHeureFin), Convert.ToInt16(CmbVacationModif.SelectedValue));

                //On met le groupbox modifVacation en false et ainsi l'utilisateur retourne à l'etape où il doit checker un bouton radio
                //grpModifVacation.Visible = false;
                //radVacModif.Checked = false;
                LbNumVac.Visible = false;
                CmbVacationModif.Visible = false;
                Lb_ComposVaca.Visible = false;
                BtnModifVacation.Enabled = false;
                this.grpModifVacation.Controls.Remove(UnComposantVac);
                CmbAtVacUpdate.Text = "Choisir un Atelier dans cette liste";
                MessageBox.Show("La Vacation " + CmbVacationModif.SelectedValue + " a bien été mise à jour au " + UnComposantVac.GetDate() + " de " + UnComposantVac.GetHeureDeb() + " à " + UnComposantVac.GetHeureFin());
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        /// <summary>
        /// procédure privée permettant à la séléction d'un atelier dans la combobox CmbAtVacUpdate de charger la combobox CmbVacation Modif
        /// Ainsi que de remplir celle-ci.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmbAtVacUpdate_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //On test si le composant "information Vacation" existe,alors on le supprime.
            if (this.grpModifVacation.Controls.Contains(UnComposantVac))
            {
                this.grpModifVacation.Controls.Remove(UnComposantVac);
            }
            LbNumVac.Visible = true;
            CmbVacationModif.Visible = true;
            Lb_ComposVaca.Visible = false;
            BtnModifVacation.Enabled = false;


            Utilitaire.RemplirComboBox(UneConnexion, UneConnexion.ObtenirDonnesOracleNumVac(Convert.ToInt16(CmbAtVacUpdate.SelectedValue)), CmbVacationModif, "numero", "numero");
            CmbVacationModif.Text = "Choisir une Vacation dans cette liste";
        }

        /// <summary>
        /// procédure privée permettant à la séléction d'un numero de vacation de charger les informations correspondant à celle-ci afin de les modifier
        /// et Activer le bouton modifier la vacation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmbVacationModif_SelectionChangeCommitted(object sender, EventArgs e)
        {

            //On test si le composant "information Vacation" existe,alors on le supprime.
            if (this.grpModifVacation.Controls.Contains(UnComposantVac))
            {
                this.grpModifVacation.Controls.Remove(UnComposantVac);
            }

            //On crée un nouveau objet composant utilisateur "Informations Vacations" qu'on instancie.
            UnComposantVac = new ComposantPPE.ComposantPPE(UneConnexion.HeureVac(Convert.ToInt16(CmbVacationModif.SelectedValue), "heuredebutvacation"), UneConnexion.HeureVac(Convert.ToInt16(CmbVacationModif.SelectedValue), "heuredebutvacation"), UneConnexion.HeureVac(Convert.ToInt16(CmbVacationModif.SelectedValue), "heurefinvacation"));
            //On ajoute ce composant comme controle du groupbox "grpModifVcation" en le plaçant grâce aux coordonnées left et top
            grpModifVacation.Controls.Add(UnComposantVac);
            UnComposantVac.Left = 174;
            UnComposantVac.Top = 137;
            UnComposantVac.Visible = true;
            Lb_ComposVaca.Visible = true;
            BtnModifVacation.Enabled = true;

        }





        /// <summary>
        /// Méthode privée testant les contrôle combobox et MaskedTextBox lorsque on clique sur l'un des radio bouton(Atelier,Vacation,Theme) 
        /// Cette méthode permetra ensuite de définir l'état des Bouton Ajouter des différentes tables Vacation, Atelier, Theme
        /// </summary>
        /// <returns>True ou false</returns>
        private Boolean VerifBtnAjouter()
        {
            Boolean TestVide = false;

            if (radAtelier.Checked == true)
            {
                TestVide = (TxtLibelleAtelier.Text != "" && NupAtPlaceMax.Value != 0 && TxtThemeAtelier.Text != "");
            }
            else if (radTheme.Checked == true)
            {


                TestVide = (CmbAtelierTheme.Text != "Choisir un Atelier dans cette liste" && TxtLibelleTheme_0.Text != "");




            }
            else if (radVacation.Checked == true)
            {
                TestVide = CmbAtVacInsert.Text != "Choisir un Atelier dans cette liste";
            }

            return TestVide;
        }

        /// <summary>
        /// Méthode permettant de définir le statut activé/désactivé du bouton BtnAjoutVacation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmbAtVacInsert_TextChanged(object sender, EventArgs e)
        {
            BtnAjoutVacation.Enabled = VerifBtnAjouter();
        }

        /// <summary>
        /// Méthode permettant de définir le statut activé/désactivé du bouton BtnAjoutTheme
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmbAtelierTheme_TextChanged(object sender, EventArgs e)
        {
            BtnAjoutTheme.Enabled = VerifBtnAjouter();
        }

        /// <summary>
        /// Méthode permettant de définir le statut activé/désactivé du bouton BtnAjoutTheme
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtLibelleTheme_TextChanged_1(object sender, EventArgs e)
        {
            BtnAjoutTheme.Enabled = VerifBtnAjouter();
        }

        /// <summary>
        /// Méthode permettant de définir le statut activé/désactivé du bouton BtnAjoutAtelier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtLibelleAtelier_TextChanged(object sender, EventArgs e)
        {
            BtnAjoutAtelier.Enabled = VerifBtnAjouter();
        }



        /// <summary>
        /// Méthode qui permet d'afficher ou masquer le controle GroupBox permettant la saisie des modifications d'une vacation.
        /// S'il faut rendre visible le groupbox, on teste si les ateliers possèdant des vacations ont été chargés dans la combobox CmbAtVacUpdate dans ce groupbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NupAtPlaceMax_ValueChanged(object sender, EventArgs e)
        {

            BtnAjoutAtelier.Enabled = VerifBtnAjouter();
        }





        /// <summary>
        /// Méthode qui permet d'afficher ou masquer le controle GroupBox permettant la saisie des modifications d'une vacation.
        /// S'il faut rendre visible le groupbox, on teste si les ateliers possèdant des vacations ont été chargés dans la combobox CmbAtVacUpdate dans ce groupbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radModifVacation_CheckedChanged(object sender, EventArgs e)
        {
            if (CmbAtVacUpdate.Items.Count == 0)
            {
                grpModifVacation.Visible = false;
                MessageBox.Show("Il n'y a aucune Vacation de créer ! ");
            }

            grpModifVacation.Visible = true;
            CmbAtVacUpdate.Text = "Choisir un Atelier dans cette liste";
        }



        /// <summary>
        /// Méthode permettant de définir le statut activé/désactivé du bouton BtnAjoutAtelier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtThemeAtelier_TextChanged(object sender, EventArgs e)
        {
            BtnAjoutAtelier.Enabled = VerifBtnAjouter();
        }




    }
}
