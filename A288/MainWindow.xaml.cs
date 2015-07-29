/// \file MainWindow.xaml.cs
/// \brief Contains the MainWindow class implementation
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace A288
{
    /// <summary>
    /// Entry Point of the App.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Default constructor that initializes the WPF window's content.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Creates a new Create object and displays the form for creating a new quiz.
        /// </summary>
        /// \note The MainWindow form is hidden while the other form is displayed.
        /// 
        /// <param name="sender">The object that raised the event (in our case the create Button).</param>
        /// <param name="e">The event args.</param>
        private void create_Click(object sender, RoutedEventArgs e)
        {
            Create f = new Create();
            try
            {
                this.Hide();
                Nullable<bool> b = f.ShowDialog();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error!" + ex.Message, "An error occured!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Show();
            }
            
        }//create_Click

        /// <summary>
        /// Creates a new Practice object and displays the form for attempting a quiz.
        /// </summary>
        /// <param name="sender">The object that raised the event (in our case the start Button).</param>
        /// <param name="e">The event args.</param>
        private void start_Click(object sender, RoutedEventArgs e)
        {
            Practice f = new Practice();
            try
            {
                this.Hide();
                Nullable<bool> b = f.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error!" + ex.Message, "An error occurred!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Show();
                MessageBox.Show("Hello!");
            }//finally
        }//start_Click
    }//MainWindow
}//namespace
