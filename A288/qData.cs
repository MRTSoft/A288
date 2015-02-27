//! \file qData.cs 
//! \brief Contains the definition of question-related object classes
//! In this file we define the qData, qWrapper and qDataDouble classes

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;


namespace A288
{
    /// <summary>
    /// Provides the structure for organizing the questions and support for serializing and deserializing data.
    /// </summary>
    /// Also the class serves as a base for qWrapper and qDataDouble classes.
    public class qData 
    {
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        //                                          M E M B E R S
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        private string text = ""; ///< The text of the question.
        private string[] vars = new string[5]; ///< The posible answers.
        private bool[] ans = new bool[5]; ///< Marks if a answer is correct.
        static private int maxTime;//< The time allowed for the quiz in minutes (0 for unlimited time).
        static private int usableQuestions;//< The number of questions to be used (0 te use all of them).

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        //                                     P R O P E R T I E S
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

        /// <summary>
        /// The text of the question.
        /// </summary>
        [XmlElement("Text")]
        public string Text
        {
            get { return this.text; }
            set { this.text = value; }
        }

        /// <summary>
        /// Array containing true if an answer is correct or false otherwise.
        /// </summary>
        [XmlArray("Answers")]
        public bool[] Ans
        {
            get
            {
                return this.ans;
            }
            set
            {
                this.ans = value;
            }
        }

        /// <summary>
        /// Array containing the posible answers to the question.
        /// </summary>
        [XmlArray("Variants")]
        public string[] Vars
        {
            get
            {
                return this.vars;
            }
            set
            {
                this.vars = value;
            }
        }

        /// <summary>
        /// The maximum time alocated for the quizz.
        /// </summary>
        ///
        ///  Allows the user to set a maximum time for the quizz.
        /// \note For unlimited time use 0.
        static public int MaxTime
        {
            get { return qData.maxTime; }
            set { qData.maxTime = value; }
        }

        /// <summary>
        /// The number of questions to be used in a single quiz.
        /// </summary>
        /// 
        /// \note If this is set to 0 then we will use all of them.
        static public int UsableQuestions
        {
            get { return qData.usableQuestions; }
            set { qData.usableQuestions = value; }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        //                                          C O N S T R U C T O R S
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

        /// \brief The default constructor
        /// 
        /// Creates an empty qData object. It should be used only to create placeholders
        public qData()
        {
            //Default constructor. Do nothing.
        }

        /// <summary>
        /// Creates an qData object with all the fields filled in. The fields are grouped as arrays.
        /// </summary>
        /// <param name="t">The text of the question.</param>
        /// <param name="v">The possible answers.</param>
        /// <param name="a">The correct answers.</param>
        public qData(string t, string[] v, bool[] a)
        {
            text = t;
            for (int i=0; i<5; i++)
            {
                vars[i] = v[i];
                ans[i] = a[i];
            }
        }

        /// <summary>
        /// Creates an qData object with all the fields filled in. The fields are transmited as variables.
        /// </summary>
        /// <param name="t">The text of the question.</param>
        /// <param name="v1">The first answer.</param>
        /// <param name="a1">Tells if the first answer is correct.</param>
        /// <param name="v2">The second answer.</param>
        /// <param name="a2">Tells if the second answer is correct.</param>
        /// <param name="v3">The third answer.</param>
        /// <param name="a3">Tells if the third answer is correct.</param>
        /// <param name="v4">The forth answer.</param>
        /// <param name="a4">Tells if the forth answer is correct.</param>
        /// <param name="v5">The fifth answer.</param>
        /// <param name="a5">Tells if the fifth answer is correct.</param>
        public qData(
            string t, 
            string v1, 
            bool a1, 
            string v2 = "", 
            bool a2 = false, 
            string v3 = "", 
            bool a3 = false, 
            string v4 = "", 
            bool a4 = false, 
            string v5 = "", 
            bool a5 = false)
        {
            text = t;
            ans[0] = a1; ans[1] = a2; ans[2] = a3; ans[3] = a4; ans[4] = a5;
            vars[0] = v1; vars[1] = v2; vars[2] = v3; vars[3] = v4; vars[4] = v5;
        }

        

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        //                                        M E T H O D S
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

        /// <summary>
        /// Returns true if the question is in a valid state or false otherwise.
        /// </summary>
        /// 
        /// The Question is considered valid if:
        /// 1. The text isn't empty;
        /// 2. A least one of the answers is true.
        /// <param name="error">An error string returned if the question is invalid.</param>
        /// <returns>The valid state of the question.</returns>
        public bool Validate(ref string error)
        {
            if (text.Length == 0)
            {
                error = "The question Text can't be empty! If needed use some spaces.";
                return false;
            }
            bool ca = false;
            for (int i = 0; i < 5; i++)
            {
                ca = ca || ans[i];
            }
            if (ca == false)
            {
                error += "\n At least one answer must be true!";
                return false;
            }
            error = "";
            return true;//Valid Question
        }

        /// <summary>
        /// Turns a qData object into it's string representation.
        /// </summary>
        /// 
        /// The string follows the pattern:<br/>
        /// Question text &lt;ENTER&gt;<br/>
        /// #ans[0]&lt;ENTER&gt;<br/>
        /// #ans[1]&lt;ENTER&gt;<br/>
        /// #ans[2]&lt;ENTER&gt;<br/>
        /// #ans[3]&lt;ENTER&gt;<br/>
        /// #ans[4]&lt;ENTER&gt;<br/>
        /// 
        /// \note The # before ans[i] marks the answer as true. If the answer is false the # will not appear.
        /// 
        /// <returns>The string representing the qData object that follows the pattern used for .txt files.</returns>
        public override string ToString()
        {
            string s = this.Text.Replace("\n", " ") + "\n";
            for (int i = 0; i < 5; i++)
            {
                if (ans[i]) s += "#";
                s += vars[i] + "\n";
            }
            //s += "\n";
            return s;
        }

        /// <summary>
        /// Serialize a List of qData objects and saves them to a file.
        /// </summary>
        /// <param name="questions">An List&lt;qData&gt; object that contains the questions to be saved. </param>
        /// <param name="file">The file name with complete path.</param>
        static public void XmlToFile( List<qData> questions, string file)
        {
            qWrapper bar = new qWrapper(questions, qData.MaxTime, qData.UsableQuestions);
            XmlSerializer serializer = new XmlSerializer(typeof(qWrapper));
            TextWriter wr = new StreamWriter(file);
            serializer.Serialize(wr, bar); 
            wr.Close();
        }

        /// <summary>
        /// Parse an XML file to a List of qData objects.
        /// </summary>
        /// <param name="questions">The List where the objects should be saved.</param>
        /// <param name="file">Full file path of the XML file.</param>
        static public void XmlToObjects(ref List<qData> questions, string file)
        {
            //throw new NotImplementedException("Needs to be modified!");
            qWrapper bar = new qWrapper();
            XmlSerializer deserializer = new XmlSerializer(typeof(qWrapper));
            TextReader tr = new StreamReader(file);
            bar = (qWrapper)deserializer.Deserialize(tr);
            tr.Close();
            questions.AddRange(bar.Questions);
            qData.MaxTime = bar.MaxTime;
            qData.UsableQuestions = bar.UsedQuestions;
        }

        /// <summary>
        /// Reads a List of questions from a .txt file that follows a specific pattern.
        /// </summary>
        /// 
        /// The file <strong>must</strong> have the following structure:<br/>
        /// Question 1 text<br/>
        /// #1st answer<br/>
        /// #2dn answer<br/>
        /// #3rd answer<br/>
        /// #4th answer<br/>
        /// #5th answer<br/>
        /// Question 2 text<br/>
        /// ...
        /// 
        /// \note TXT files don't support the MaxTime and UsableQuestions properties.
        /// 
        /// \note Between questions should't be empty lines
        /// The quuestion text can span on a single line
        /// The corect answer/answers begin with an # and ends at the end of the line.
        /// /note if the line begins with ' #' a space followed by a # the answer is considered false.
        /// The false answers begin with a normal character.
        /// /note The # in front of correct answers is not displayed.
        /// <param name="QUIZZ">The List where the questions will be saved</param>
        /// <param name="file"></param>
        public static void TxtToObjects(ref List<qData> QUIZZ, string file)
        {
            QUIZZ.Clear();//remove all items
            //TODO add suport for adding questions - DONE; Just load more tests

            StreamReader tr = new StreamReader(file);                        
            string text, v1, v2, v3, v4, v5;
            string er = "";
            try
            {               
                while (tr.EndOfStream == false)
                {
                    text = tr.ReadLine();//read the Text of the question
                    v1 = tr.ReadLine();
                    v2 = tr.ReadLine();
                    v3 = tr.ReadLine();
                    v4 = tr.ReadLine();
                    v5 = tr.ReadLine();
                    bool a1, a2, a3, a4, a5;
                    a1 = v1.Contains("#"); if (a1) { v1 = v1.Substring(1); }
                    a2 = v2.Contains("#"); if (a2) { v2 = v2.Substring(1); }
                    a3 = v3.Contains("#"); if (a3) { v3 = v3.Substring(1); }
                    a4 = v4.Contains("#"); if (a4) { v4 = v4.Substring(1); }
                    a5 = v5.Contains("#"); if (a5) { v5 = v5.Substring(1); }
                    qData q = new qData(text, v1, a1, v2, a2, v3, a3, v4, a4, v5, a5);
                    if (q.Validate(ref er) == true) QUIZZ.Add(q);
                }
            }// end try block
            catch
            {
                throw;
            }     
            finally
            {
                tr.Close();
            }
            
        }// end read file

        /// <summary>
        /// Writes the questions to an .txt file using the same syntax as readFile
        /// </summary>
        /// <param name="Quizz">The List of qData objects to be written in the file.</param>
        /// <param name="file">The file name with full path.</param>
        public static void TxtToFile(List<qData> Quizz, string file)
        {
            StreamWriter sr = new StreamWriter(file);
            try
            {
                foreach (qData q in Quizz)
                {
                    sr.Write(q.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sr.Close();
            }
            sr.Close();
        }

    }// qData

    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    // - - - - - - - -                                 q W R A P P E R                             - - - - - - - - - - -
    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -


    /// <summary>
    /// Wrapper class used to serialize a quiz. Aditional quiz metadata can be added via this class
    /// </summary>
    public class qWrapper
    {
        /// <summary>
        /// The list of questions.
        /// </summary>
        /// Each question is a qData item.
        [XmlArray("Questions")]
        public List<qData> Questions
        { get; set; }

        /// <summary>
        /// The time (in minutes) used to solve all the used questions.
        /// </summary>
        ///  The time is expressed in minutes and it's a integer.
        ///  /note If this is 0 then the quiz will not be timed.
        [XmlElement("MaxTime")]
        public int MaxTime
        { get; set; }

        /// <summary>
        /// The number of questions used in a quiz.
        /// </summary>
        /// Note that this must be smaller than the total number of questions.
        /// \note If this is set to 0 then all the questions will be used.
        [XmlElement("UsedQuestions")]
        public int UsedQuestions
        { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// This is required by the XmlParser
        public qWrapper()
        {
            this.Questions = new List<qData>();
            this.MaxTime = this.UsedQuestions = 0;
        }

        /// <summary>
        /// Creates a new qWrapper object
        /// </summary>
        /// <param name="Q">The list of questions for the quiz.</param>
        /// <param name="mTime">The time for the quiz.</param>
        /// <param name="qNr">The number of questions used in a quiz.</param>
        public qWrapper (List<qData> Q = null, int mTime = 0, int qNr = 0)
        {
            this.Questions = new List<qData>();
            if (Q != null) this.Questions.AddRange(Q);
            this.MaxTime = mTime;
            this.UsedQuestions = qNr;
        }
    }

    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    // - - - - - - - -                           q D a t a D o u b l e                             - - - - - - - - - - -
    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

    /// <summary>
    /// An extension of the qData class that supports saving two set of answers: the actual question answers and the user answers
    /// </summary>
    public class qDataDouble : qData
    {
        bool right;//< After Evaluate is called this variable show if the user has answered correctly
        /// <summary>
        /// After Evaluate is called this variable show if the user has answered correctly
        /// </summary>
        /// \note This property is read-only.
        public bool Right
        {
            get { return this.right; }
        }

        public bool[] UserAnswers//< Remembers the answers chosed by the user
        {
            get; set;
        }//UserAnswers
       
        /// <summary>
        /// Default constructor. reates a qDataDouble object.
        /// </summary>
        /// \note By default we asume that the user has not answered correctly
        public qDataDouble()
        {
            this.UserAnswers = new bool[] { false, false, false, false, false };
            this.right = false;
        }//qDataDouble
        
        /// <summary>
        /// Creates a new qDataDouble object using a qData object as source for the data.
        /// </summary>
        /// <param name="q">A qData object that represents the question</param>
        public qDataDouble(qData q)
        {
            this.Ans = q.Ans;
            this.Text = q.Text;
            this.Vars = q.Vars;
            this.UserAnswers = new bool[]{false, false, false, false,false};
        }

        /// <summary>
        /// Shuffle the answers of a question.
        /// </summary>
        /// \note The UserAnswers is keep in sync.
        public void shuffleAnswers()
        {
            int rSeed = DateTime.Now.Millisecond;
            System.Random rand = new Random(rSeed);
            for (int n = 5, p = 0; n>0; n--)
            {
                p = rand.Next(0, n - 1);
                bool aux = this.Ans[n - 1];
                this.Ans[n - 1] = this.Ans[p];
                this.Ans[p] = aux;
                aux = this.UserAnswers[n-1];
                this.UserAnswers[n - 1] = this.UserAnswers[p];
                this.UserAnswers[p] = aux;
                string sAux = this.Vars[n - 1];
                this.Vars[n - 1] = this.Vars[p];
                this.Vars[p] = sAux;
            }
        }
        
        /// <summary>
        /// This function compares the user answers with the question answers and sets the Right property of the object accordingly.
        /// </summary>
        public void Evaluate()
        {
            this.right = true;
            for (int i=0; i<5; i++)
            {
                if (this.Ans[i] != this.UserAnswers[i]) this.right = this.right && false;
                if (this.Ans[i]) this.Vars[i] = "[TRUE]: " + this.Vars[i];
                else this.Vars[i] = "[FALSE]: " + this.Vars[i];
            }
        }//Evaluate
    }// qDataDouble
}// MedQ
