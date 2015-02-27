//! \file Practice.xaml.cs
//! \brief Defines and implements the Practice class
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace A288
{
    /// <summary>
    /// The Practice class defines a form that is used to take a test.
    /// </summary>
    /// //! This class contains all the function called by the controls on the 'Practice' window. 
    //! To keep thinks clean it also contains generic functions for performing operations like loading or saving questions.
    public partial class Practice : Window
    {
        //TODO Create sections for the methods
        //TODO Write documentation for this
        //TODO [OPTIONAL] Add graphical elements

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        //                                          M E M B E R S
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

        private List<qDataDouble> QUIZ = new List<qDataDouble>();//< The list with the questions for the quiz
        private int cQ = 0;//< The current question (using 1 base index)
        System.Windows.Threading.DispatcherTimer mainTimer = new System.Windows.Threading.DispatcherTimer();//<The main timer.
        TimeSpan remainingTime;//< The remaining time for the quiz. When this reaches 0 the test will end.

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        //                                          C O N S T R U C T O R
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        /// <summary>
        /// The default constructor. It creates and display a new Practice WPF.
        /// </summary>
        public Practice()
        {
            InitializeComponent();
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        //                                  M E T H O D S ( - I -  Event handlers)
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

        /// <summary>
        /// Triggers after the window was initialized. Loads a quiz from an XML file.
        /// </summary>
        /// Opens a OpenFileDialog and selects a quiz. Then calls initialQuestionLoad(). <br/>
        /// Finally starts the timer and loads the first question.
        /// <param name="sender">The Practice window.</param>
        /// <param name="e">The event args.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "eXtensible Markup Language |*.xml;*.XML|All files|*.*";
            dlg.FilterIndex = 0;
            Nullable<bool> ok = dlg.ShowDialog();
            if (ok == true)
            {
                try
                {
                    string file = dlg.FileName;
                    initialQuestionLoad(file);
                    cQ = 1;
                    if (cQ <= QUIZ.Count)
                        loadQuestion(cQ);
                    //Start the test and bind the timer
                    mainTimer.Interval = new TimeSpan(0, 0, 1);
                    mainTimer.Tick += new EventHandler(timerTick);
                    if (remainingTime.Minutes != 0) mainTimer.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("You need to open a test (.xml file)!", "Fatal error", MessageBoxButton.OK, MessageBoxImage.Stop);
                this.Close();
            }
        }//Window_Loaded

        /// <summary>
        /// The timer Event Hadler. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerTick(Object sender, EventArgs e)
        {
            lTime.Content = remainingTime.Hours.ToString("00") + ":" + remainingTime.Minutes.ToString("00") + ":" + remainingTime.Seconds.ToString("00");
            remainingTime = remainingTime.Subtract(new TimeSpan(0, 0, 1));
            if (remainingTime.TotalSeconds < 0.2d) endTheQuizz();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void genericClick(object sender, MouseButtonEventArgs e)
        {
            Label l = sender as Label;
            string id = l.Name;
            id = id.Substring(1);
            try
            {
                int v = int.Parse(id);
                QUIZ[cQ - 1].UserAnswers[v - 1] = !QUIZ[cQ - 1].UserAnswers[v - 1];
                setButtonState(l, QUIZ[cQ - 1].UserAnswers[v - 1]);
            }
            catch { return; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void genericClickEnded(object sender, MouseButtonEventArgs e)
        {
            if (QUIZ[cQ - 1].Right) MessageBox.Show("You have answered correctly at this question,", "Correct", MessageBoxButton.OK, MessageBoxImage.Information);
            else MessageBox.Show("You haven't answered correctly at this question.", "Wrong!", MessageBoxButton.OK, MessageBoxImage.Stop);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bNext_Click(object sender, RoutedEventArgs e)
        {
            loadQuestion(cQ + 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bPrev_Click(object sender, RoutedEventArgs e)
        {
            loadQuestion(cQ - 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lSubmit_MouseEnter(object sender, MouseEventArgs e) { lSubmit.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xE6, 0x3C, 0x00)); } /* #FF E6 3C 00 */
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lSubmit_MouseLeave(object sender, MouseEventArgs e) { lSubmit.Background = new SolidColorBrush(Colors.Red); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lSubmit_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show("Are you sure you want to submit the questions? \nThis will end the current quiz.", "Are you sure?", MessageBoxButton.OKCancel);
            if (dr == MessageBoxResult.Cancel) return;
            endTheQuizz();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lStats_Click(object sender, MouseButtonEventArgs e)
        {
            string mbText = "";
            int right = 0;
            foreach (qDataDouble q in QUIZ) if (q.Right) right++;
            double percent = (double)right / (double)QUIZ.Count;
            mbText += "Test finished!" + Environment.NewLine;
            mbText += "Your score: " + percent.ToString("##0.00%") + ", meaning " + right.ToString() + " of " + QUIZ.Count + Environment.NewLine;
            mbText += "Do you wish review your answers?" + Environment.NewLine;
            mbText += "[YES] - Compare your answers(highlited)" + Environment.NewLine + "      with the correct answers (marked [TRUE])." + Environment.NewLine;
            mbText += "[NO] - Quit and exit.";
            MessageBoxResult dr = MessageBox.Show(mbText, "Results", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.Yes);
            if (dr == MessageBoxResult.No) this.Close();
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        //                                  M E T H O D S ( - II - Misc. functions)
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

        /// <summary>
        /// Load all the questions into the QUIZ list
        /// </summary>
        /// <param name="file"></param>
        private void initialQuestionLoad(string file)
        {
            List<qData> fetch = new List<qData>();
            qData.XmlToObjects(ref fetch, file);
            remainingTime = new TimeSpan(0, qData.MaxTime, 0);
            {
                List<qData> save = new List<qData>();
                string er = "";
                foreach (qData q in fetch)
                {
                    if (q.Validate(ref er)) save.Add(q);
                }
                fetch.Clear();
                fetch.AddRange(save);
            }

            Random r = new Random(DateTime.Now.Millisecond);
            for (int i = fetch.Count, p = 0; i > 0; i--)
            {
                p = r.Next(0, i - 1);
                qData aux = fetch[p];
                fetch[p] = fetch[i - 1];
                fetch[i - 1] = aux;
            }
            QUIZ = new List<qDataDouble>();
            if (qData.UsableQuestions != 0)
            {
                for (int i = 0; i < qData.UsableQuestions; i++)
                {
                    QUIZ.Add(new qDataDouble(fetch[i]));
                }
            }
            else
            {
                foreach (qData q in fetch) QUIZ.Add(new qDataDouble(q));
            }
            
        }// initialQuestionLoad

        /// <summary>
        /// Load a question in the form.
        /// </summary>
        /// <param name="nr">The index (starting from 1) of the question</param>
        private void loadQuestion(int nr)
        {
            if (nr < 1) return;
            if (nr > QUIZ.Count)
            {
                MessageBoxResult r = MessageBox.Show("Do you wish to submit your quiz?", "Last question reached", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (r == MessageBoxResult.Yes) endTheQuizz();
                return;
            }
            
            qDataDouble q = QUIZ[nr - 1];
            tbQ.Text = q.Text;
            resetButtons();
            q.shuffleAnswers();
            string[] var = q.Vars;
            Label[] btns = new Label[] { a1, a2, a3, a4, a5 };
            for (int i = 0; i < 5; i++) btns[i].Content = var[i];
            //a1.Content = var[0];            a2.Content = var[1];            a3.Content = var[2];            a4.Content = var[3];            a5.Content = var[4];
            bool[] uAns = q.UserAnswers;
            loadUserAnswers(uAns);
            cQ = nr;
            lHeader.Content = "Question " + cQ.ToString() + " of " + QUIZ.Count.ToString();
        }// loadQuestion
        
        /// <summary>
        /// Set the state of the answer buttons to match the user's answers
        /// </summary>
        /// <param name="uAns"></param>
        private void loadUserAnswers(bool[] uAns)
        {
            Label[] btns = new Label[] { a1, a2, a3, a4, a5 };
            for (int i = 0; i < 5; i++)
            {
                setButtonState(btns[i], uAns[i]);
            }
        }

        /// <summary>
        /// Set all the answer buttons to un-checked state.
        /// </summary>
        private void resetButtons()
        {
            Label[] btns = new Label[] { a1, a2, a3, a4, a5 };
            foreach(Label l in btns)
            {
                setButtonState(l, false);
            }
        }

        /// <summary>
        /// Set an answer button to a checked or un-checked state.
        /// </summary>
        /// <param name="l">The answer button who's state we'll change.</param>
        /// <param name="state">The state of the button that will be set.</param>
        private void setButtonState(Label l, bool state)
        {
            Label neutralStyle = new Label();
            neutralStyle.Background = new SolidColorBrush(Color.FromArgb(0x7F, 0xFF, 0xFF, 0xFF));//#FF000000
            neutralStyle.Foreground = Brushes.Black;//#FF000000
            Label ckStyle = new Label();
            ckStyle.Background = new SolidColorBrush(Color.FromArgb(0x7F, 0x00, 0x00, 0x00));//#FF00000000
            ckStyle.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));//#FFFFFFFFFF
            switch (state)
            {
                case true://checked
                    {
                        l.Foreground = ckStyle.Foreground;
                        l.Background = ckStyle.Background;
                    }break;
                case false://un-checked
                    {
                        l.Background = neutralStyle.Background;
                        l.Foreground = neutralStyle.Foreground;
                    }break;
                default:
                    {
                        l.Background = neutralStyle.Background;
                        l.Foreground = neutralStyle.Foreground;
                    }break;
            }//switch
        }//setButtonState
            
        /// <summary>
        /// End the quizz by evaluating the questions, calculation the score and un-binding the answer buttons
        /// </summary>
        private void endTheQuizz()
        {
            mainTimer.Stop();// First we stop the timer.
            TimeSpan completed = new TimeSpan(0, qDataDouble.MaxTime, 0);//We display the finished time
            completed = completed.Subtract(remainingTime);
            lTime.Content = completed.Hours.ToString("00") + ":" + completed.Minutes.ToString("00") + completed.Seconds.ToString("00");
            //inQuiz = false;
            foreach( qDataDouble q in QUIZ)// Evaluate the questions
            {
                q.Evaluate();
            }
            //We re-bind the controls
            lSubmit.MouseDown -= lSubmit_Click;// .. of the submit button
            lSubmit.MouseDown += lStats_Click;
            lSubmit.Content = "STATS";
            Label[] btns = new Label[] { a1, a2, a3, a4, a5 };// .. of the answers
            foreach(Label l in btns)
            {
                l.MouseDown -= genericClick;
                l.MouseDown += genericClickEnded;
            }
            lStats_Click(lSubmit, new MouseButtonEventArgs(Mouse.PrimaryDevice ,0,MouseButton.Left, Stylus.CurrentStylusDevice));
            loadQuestion(cQ);
        }//endTheQuizz
    }
}
