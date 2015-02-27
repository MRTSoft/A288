/// \file Create.xaml.cs
/// \brief Contains the Create class definition

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.IO;

namespace MedQ
{
    //! \brief Class that handles interaction logic for %Create Quizz window
    //! 
    //! This class contains all the function called by the controls on the 'Create' window. 
    //! To keep thinks clean it also contains generic functions for performing operations like loading or saving questions.

    public partial class Create : Window
    {
        //TODO Write documentation for Create class
        //TODO Put the functions in propper order
        //TODO [OPTIONAL] Add Graphical elements
        //TODO [OPTIONAL] Add points to the test

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        //                                          M E M B E R S
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

        private List<qData> QUIZZ;//!< Contains the questions.
        private int curentQ;//!< The curent question that is being edited. \note The first question is refered as 1 but is stored in \ref QUIZZ[0]



        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        //                                          C O N S T R U C T O R
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -


        //! \brief The default constructor. 
        //! Default constructor that initializes the WPF window's content.
        //! It initialises the components and creates a blank quizz.
        public Create()
        {
            InitializeComponent();
            QUIZZ = new List<qData>(1);
            QUIZZ.Add(new qData());
            curentQ = 1;
            nQuestions.Text = "0";
            nTime.Text = "0";
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        //                                  M E T H O D S ( - I -  Event handlers)
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 


        // 1. Quiz Start
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bLoad_Xml(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "eXtensible Markup Language files |*.xml; *.XML | All files | *.*";
            dlg.FilterIndex = 0;
            Nullable<bool> ok = dlg.ShowDialog();
            if (ok == true)
            {
                string file = dlg.FileName;
                qData.XmlToObjects(ref QUIZZ, file);
                removeInvalid();
                if (QUIZZ.Count() > 0)
                    loadQuestion(1);
                curentQ = 1;
                nTime.Text = qData.MaxTime.ToString();
                nQuestions.Text = qData.UsableQuestions.ToString();
                tabWrapper.SelectedIndex++;
            }//Else do nothing
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bLoad_Txt(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Plain text files |*.txt; | All files | *.*";
            Nullable<bool> ok = dlg.ShowDialog();
            if (ok == true)
            {
                string file = dlg.FileName;
                qData.TxtToObjects(ref QUIZZ, file);
                removeInvalid();
                if (QUIZZ.Count() > 0)
                    loadQuestion(1);
                curentQ = 1;
                nTime.Text = qData.MaxTime.ToString();
                nQuestions.Text = qData.UsableQuestions.ToString();
                tabWrapper.SelectedIndex++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bNext_Tab(object sender, RoutedEventArgs e)
        {
            tabWrapper.SelectedIndex = tabWrapper.SelectedIndex + 1;
        }


        // 2.Question Control
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            saveCurQ(true);
            int curent = cbNumber.SelectedIndex;
            loadQuestion(curent + 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bQNext_Click(object sender, RoutedEventArgs e)
        {
            if (saveCurQ())
                loadQuestion(curentQ + 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bQPrev_Click(object sender, RoutedEventArgs e)
        {
            string er = "";
            if (QUIZZ[curentQ - 1].Validate(ref er) != true)
            {
                QUIZZ.RemoveAt(curentQ - 1);
            }
            else saveCurQ();
            loadQuestion(curentQ - 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bDelete_Click(object sender, RoutedEventArgs e)
        {
            if (QUIZZ.Count < 1) return;
            if (QUIZZ.Count == 1) QUIZZ[0] = new qData();
            else
            {
                QUIZZ.RemoveAt(curentQ - 1);
            }
            if (curentQ > QUIZZ.Count) curentQ = QUIZZ.Count;
            loadQuestion(curentQ);
        }

        // 3. Quiz Options
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tBoxNumberTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender.Equals(nTime))//If the time is modified
            {
                if (nTime.Text.Length < 1) return;// Remove non-numeric chars
                nTime.TextChanged -= tBoxNumberTextChanged;
                nTime.Text = parseText(nTime.Text);
                nTime.Select(nTime.Text.Length, 0);
                try // Convert the value to a number
                {
                    int min = 0;
                    if (int.TryParse(nTime.Text, out min))
                    {
                        qData.MaxTime = min;
                    }
                    else qData.MaxTime = 0;
                }
                catch
                {
                    qData.MaxTime = 0;
                }
                if (nTime.Text == "0" || nTime.Text == "") nTime.Text = "Unlimited (0)"; // Display custom value for "" or "0"
                nTime.TextChanged += tBoxNumberTextChanged;
            }
            else if (sender.Equals(nQuestions))// If the usable questions number is modified
            {
                if (nQuestions.Text.Length < 1) return;// Remove non-numeric chars
                nQuestions.TextChanged -= tBoxNumberTextChanged;
                nQuestions.Text = parseText(nQuestions.Text);
                nQuestions.Select(nQuestions.Text.Length, 0);
                try // Convert the value to a number
                {
                    int usable = 0;
                    if (int.TryParse(nQuestions.Text, out usable))
                    {
                        if (usable > QUIZZ.Count) usable = QUIZZ.Count;
                        qData.UsableQuestions = usable;
                        nQuestions.Text = usable.ToString();
                    }
                    else qData.UsableQuestions = 0;
                }
                catch { qData.UsableQuestions = 0; }
                if (nQuestions.Text == "0" || nQuestions.Text == "") nQuestions.Text = "Use all (0)";// Display custom value for "" or "0"
                nQuestions.TextChanged += tBoxNumberTextChanged;
            }//if
        }//TextBox

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bSave_Xml(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Filter = "eXtensible Markup Language files (.xml)|*.xml; *.XML | All files | *.*";
            dlg.FilterIndex = 0;
            Nullable<bool> ok = dlg.ShowDialog();
            if (ok == true)
            {
                saveCurQ();
                removeInvalid();
                string file = dlg.FileName;
                qData.XmlToFile(QUIZZ, file);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bSave_Txt(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Filter = "Plain Text files |*.txt; *.TXT| All files | *.*";
            dlg.FilterIndex = 0;
            Nullable<bool> ok = dlg.ShowDialog();
            if (ok == true)
            {
                saveCurQ();
                removeInvalid();
                string file = dlg.FileName;
                //qData.XmlToFile(QUIZZ, file);
                qData.TxtToFile(QUIZZ, file);
            }
        }


        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        //                                  M E T H O D S ( - II - Misc. functions)
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nr"></param>
        private void loadQuestion(int nr)// index starts with 1
        {
            if (nr > QUIZZ.Count())//we need a new Question
            {
                QUIZZ.Add(new qData());
            }

            lqNr.Content = "Question " + nr.ToString() + " of " + (QUIZZ.Count()).ToString();
            tbText.Text = QUIZZ[nr - 1].Text;
            tbA1.Text = QUIZZ[nr - 1].Vars[0]; cb1.IsChecked = QUIZZ[nr - 1].Ans[0];
            tbA2.Text = QUIZZ[nr - 1].Vars[1]; cb2.IsChecked = QUIZZ[nr - 1].Ans[1];
            tbA3.Text = QUIZZ[nr - 1].Vars[2]; cb3.IsChecked = QUIZZ[nr - 1].Ans[2];
            tbA4.Text = QUIZZ[nr - 1].Vars[3]; cb4.IsChecked = QUIZZ[nr - 1].Ans[3];
            tbA5.Text = QUIZZ[nr - 1].Vars[4]; cb5.IsChecked = QUIZZ[nr - 1].Ans[4];
            bQPrev.Content = "Question " + (nr - 1).ToString();
            bQNext.Content = "Question " + (nr + 1).ToString();
            if (nr - 1 <= 0) bQPrev.Visibility = System.Windows.Visibility.Hidden;
            else bQPrev.Visibility = System.Windows.Visibility.Visible;

            curentQ = nr;
            refreshComboList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string parseText(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != '0' && text[i] != '1' && text[i] != '2' && text[i] != '3' && text[i] != '4' && text[i] != '5' && text[i] != '6' && text[i] != '7' && text[i] != '8' && text[i] != '9')
                {
                    return text.Substring(0, i);
                }
            }
            return text;
        }

        /// <summary>
        /// 
        /// </summary>
        private void refreshComboList()
        {
            cbNumber.SelectionChanged -= ComboBox_SelectionChanged;
            cbNumber.Items.Clear();
            for (int i = 0; i < QUIZZ.Count; i++)
            {
                Label lbl = new Label();
                lbl.FontSize = 20.0d;
                lbl.Content = (i + 1).ToString();
                lbl.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                lbl.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                cbNumber.Items.Add(lbl);
            }
            cbNumber.SelectedIndex = curentQ - 1;
            cbNumber.SelectionChanged += ComboBox_SelectionChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        private void removeInvalid()
        {
            List<qData> save = new List<qData>();
            string er = "";
            foreach (qData q in QUIZZ)
            {
                if (q.Validate(ref er))
                    save.Add(q);
            }
            QUIZZ.Clear();
            QUIZZ.AddRange(save);
            refreshComboList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supressError"></param>
        /// <returns></returns>
        private bool saveCurQ(bool supressError=false)
        {
            int i = curentQ - 1;
            qData q = new qData(tbText.Text, tbA1.Text, (bool)cb1.IsChecked, tbA2.Text, (bool)cb2.IsChecked, tbA3.Text, (bool)cb3.IsChecked, tbA4.Text, (bool)cb4.IsChecked, tbA5.Text, (bool)cb5.IsChecked);
            string er = "";
            if (q.Validate(ref er) == true)
            {
                try
                {
                    QUIZZ[curentQ - 1] = q;
                    return true;
                }
                catch (Exception ex)
                {
                    if (!supressError) MessageBox.Show(ex.Message);
                    return false;
                }
            }
            else
            {
                if (!supressError) MessageBox.Show(er);
                return false;
            }
        }//saveCurQ        
    }//end class
}//end namespace
