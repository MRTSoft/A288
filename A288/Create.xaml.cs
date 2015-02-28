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

namespace A288
{
    //! \brief Class that handles interaction logic for %Create Quiz window
    //! 
    //! This class contains all the function called by the controls on the 'Create' window. 
    //! To keep thinks clean it also contains generic functions for performing operations like loading or saving questions.

    public partial class Create : Window
    {
        //TODO [OPTIONAL] Add graphical elements.
        //TODO [OPTIONAL] Add a marking system to the test (max points or smth like that).

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        //                                          M E M B E R S
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

        private List<qData> QUIZ;//!< Contains the questions.
        private int curentQ;//!< The curent question that is being edited. \note The first question is refered as 1 but is stored in \ref QUIZ[0]



        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        //                                          C O N S T R U C T O R
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -


        //! \brief The default constructor. 
        //! Default constructor that initializes the WPF window's content.
        //! It initialises the components and creates a blank quiz.
        public Create()
        {
            InitializeComponent();
            QUIZ = new List<qData>(1);
            QUIZ.Add(new qData());
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
        /// Displays an OpenFileDialog and creates a quiz from an existing XML file.
        /// </summary>
        /// \note None of the parameters is used.
        /// <param name="sender">The bLoadXml Button.</param>
        /// <param name="e">The Click events args</param>
        private void bLoad_Xml(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "eXtensible Markup Language files |*.xml; *.XML | All files | *.*";
            dlg.FilterIndex = 0;
            Nullable<bool> ok = dlg.ShowDialog();
            if (ok == true)
            {
                string file = dlg.FileName;
                qData.XmlToObjects(ref QUIZ, file);
                removeInvalid();
                if (QUIZ.Count() > 0)
                    loadQuestion(1);
                curentQ = 1;
                nTime.Text = qData.MaxTime.ToString();
                nQuestions.Text = qData.UsableQuestions.ToString();
                tabWrapper.SelectedIndex++;
            }//Else do nothing
           
        }//bLoad_Xml

        /// <summary>
        /// Displays an OpenFileDialog and creates a quiz from an existing TXT file.
        /// </summary>
        /// \note None of the parameters is used.
        /// <param name="sender">The bLoadTxt Button</param>
        /// <param name="e">The Click event args.</param>
        private void bLoad_Txt(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Plain text files |*.txt; | All files | *.*";
            Nullable<bool> ok = dlg.ShowDialog();
            if (ok == true)
            {
                string file = dlg.FileName;
                qData.TxtToObjects(ref QUIZ, file);
                removeInvalid();
                if (QUIZ.Count() > 0)
                    loadQuestion(1);
                curentQ = 1;
                nTime.Text = qData.MaxTime.ToString();
                nQuestions.Text = qData.UsableQuestions.ToString();
                tabWrapper.SelectedIndex++;
            }
        }

        /// <summary>
        /// Advances to the next tab.
        /// </summary>
        /// \note None of the parameters is used.
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="e">The event args.</param>
        private void bNext_Tab(object sender, RoutedEventArgs e)
        {
            tabWrapper.SelectedIndex = tabWrapper.SelectedIndex + 1;
        }


        // 2.Question Control
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        /// <summary>
        /// Trigers the loading of a specific question (selected as the new index).
        /// </summary>
        /// \note None of the parameters is used.
        /// <param name="sender">The cbNumber ComboBox.</param>
        /// <param name="e">The event args.</param>
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            saveCurQ(true);
            int curent = cbNumber.SelectedIndex;
            loadQuestion(curent + 1);
        }//ComboBox_SelectionChanged

        /// <summary>
        /// Advances to the next question.
        /// </summary>
        /// \note None of the parameters is used.
        /// <param name="sender">The bQNext Button</param>
        /// <param name="e">The event args.</param>
        private void bQNext_Click(object sender, RoutedEventArgs e)
        {
            if (saveCurQ())
                loadQuestion(curentQ + 1);
        }//bQNext_Click

        /// <summary>
        /// Goes back to the previous question.
        /// </summary>
        /// \note None of the parameters is used.
        /// <param name="sender">The bQPrev Button.</param>
        /// <param name="e">The event args.</param>
        private void bQPrev_Click(object sender, RoutedEventArgs e)
        {
            string er = "";
            if (QUIZ[curentQ - 1].Validate(ref er) != true)
            {
                QUIZ.RemoveAt(curentQ - 1);
            }
            else saveCurQ();
            loadQuestion(curentQ - 1);
        }

        /// <summary>
        /// Delete the current question.
        /// </summary>
        /// \note None of the parameters is used.
        /// <param name="sender">The bDelete Button.</param>
        /// <param name="e">The event args.</param>
        private void bDelete_Click(object sender, RoutedEventArgs e)
        {
            if (QUIZ.Count < 1) return;
            if (QUIZ.Count == 1) QUIZ[0] = new qData();
            else
            {
                QUIZ.RemoveAt(curentQ - 1);
            }
            if (curentQ > QUIZ.Count) curentQ = QUIZ.Count;
            loadQuestion(curentQ);
        }

        // 3. Quiz Options
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        /// <summary>
        /// Transforms a textBox control into a basic numeric control.
        /// </summary>
        /// <param name="sender">The textBox that raised the event.</param>
        /// <param name="e">The event args.</param>
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
                        if (usable > QUIZ.Count) usable = QUIZ.Count;
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
        /// Opens an SaveFileDialog and saves the quiz as an XML file
        /// </summary>
        /// After a save location is chosen this calls the XmlToFile method.
        /// <param name="sender">The bSave Button.</param>
        /// <param name="e">The event args.</param>
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
                qData.XmlToFile(QUIZ, file);
            }
        }//bSave_Xml

        /// <summary>
        /// Opens an SaveFileDialog and saves the quiz as a TXT file
        /// </summary>
        /// After a save location is chosen this calls the TxtToFile method.
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
                qData.TxtToFile(QUIZ, file);
            }
        }


        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        //                                  M E T H O D S ( - II - Misc. functions)
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

        /// <summary>
        /// Loads a specific question on the WPF controls.
        /// </summary>
        /// \note This function calls refreshCombolist();
        /// <param name="nr">The index of the question + 1.</param>
        private void loadQuestion(int nr)// index starts with 1
        {
            if (nr > QUIZ.Count())//we need a new Question
            {
                QUIZ.Add(new qData());
            }

            lqNr.Content = "Question " + nr.ToString() + " of " + (QUIZ.Count()).ToString();
            tbText.Text = QUIZ[nr - 1].Text;
            tbA1.Text = QUIZ[nr - 1].Vars[0]; cb1.IsChecked = QUIZ[nr - 1].Ans[0];
            tbA2.Text = QUIZ[nr - 1].Vars[1]; cb2.IsChecked = QUIZ[nr - 1].Ans[1];
            tbA3.Text = QUIZ[nr - 1].Vars[2]; cb3.IsChecked = QUIZ[nr - 1].Ans[2];
            tbA4.Text = QUIZ[nr - 1].Vars[3]; cb4.IsChecked = QUIZ[nr - 1].Ans[3];
            tbA5.Text = QUIZ[nr - 1].Vars[4]; cb5.IsChecked = QUIZ[nr - 1].Ans[4];
            bQPrev.Content = "Question " + (nr - 1).ToString();
            bQNext.Content = "Question " + (nr + 1).ToString();
            if (nr - 1 <= 0) bQPrev.Visibility = System.Windows.Visibility.Hidden;
            else bQPrev.Visibility = System.Windows.Visibility.Visible;

            curentQ = nr;
            refreshComboList();
        }

        /// <summary>
        /// Truncate a string so that it contains only numerical characters.
        /// </summary>
        /// <param name="text">The text to be truncated</param>
        /// <returns>A string containing only numerical characters or an empty string.</returns>
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
        /// Keeps the ComboList in sync with the questions from the quiz.
        /// </summary>
        private void refreshComboList()
        {
            cbNumber.SelectionChanged -= comboBox_SelectionChanged;
            cbNumber.Items.Clear();
            for (int i = 0; i < QUIZ.Count; i++)
            {
                Label lbl = new Label();
                lbl.FontSize = 20.0d;
                lbl.Content = (i + 1).ToString();
                lbl.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                lbl.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                cbNumber.Items.Add(lbl);
            }
            cbNumber.SelectedIndex = curentQ - 1;
            cbNumber.SelectionChanged += comboBox_SelectionChanged;
        }

        /// <summary>
        /// Removes all the invalid question by calling Validate on each of them.
        /// </summary>
        /// Also it calls the refreshComboList method.
        /// \note This function doesn't throw errors.
        private void removeInvalid()
        {
            List<qData> save = new List<qData>();
            string er = "";
            foreach (qData q in QUIZ)
            {
                if (q.Validate(ref er))
                    save.Add(q);
            }
            QUIZ.Clear();
            QUIZ.AddRange(save);
            refreshComboList();
        }

        /// <summary>
        /// Saves or updates the curent question into the QUIZ list.
        /// </summary>
        /// This function can also display errors if the question is invalid.
        /// <param name="supressError">If this is set to 'true' then no errors will be shown to the user.</param>
        /// <returns>'true' if the question was saved/updated with succes and 'false' otherwise.</returns>
        private bool saveCurQ(bool supressError=false)
        {
            int i = curentQ - 1;
            qData q = new qData(tbText.Text, tbA1.Text, (bool)cb1.IsChecked, tbA2.Text, (bool)cb2.IsChecked, tbA3.Text, (bool)cb3.IsChecked, tbA4.Text, (bool)cb4.IsChecked, tbA5.Text, (bool)cb5.IsChecked);
            string er = "";
            if (q.Validate(ref er) == true)
            {
                try
                {
                    QUIZ[curentQ - 1] = q;
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
