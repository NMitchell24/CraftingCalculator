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
using CraftingCalculator.Recipes;
using CraftingCalculator.Ingredients;

namespace CraftingCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // For Testing Recipe Calculations with nested complex recipes.           
            MindArc mindArc = new MindArc();
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<IngredientType, int> ingredient in mindArc.GetIngredients())
            {
               sb.AppendLine(ingredient.Key.GetDisplayName() + ": " + ingredient.Value.ToString());
            }
 
            MessageBox.Show(sb.ToString());
        }
    }
}
