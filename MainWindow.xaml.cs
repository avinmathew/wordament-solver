using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace WordamentSolver
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Stores relationships of adjacent boxes.
        /// </summary>
        private static Dictionary<int, List<int>> _adjacent = new Dictionary<int, List<int>>();

        /// <summary>
        /// Complete lexicon of words.
        /// </summary>
        private static HashSet<string> _allWords = ReadWords("lexicon.txt");

        static MainWindow()
        {
            // Setup adjacent box relationships
            _adjacent.Add(0, new List<int> { 1, 4, 5 });
            _adjacent.Add(1, new List<int> { 0, 2, 4, 5, 6 });
            _adjacent.Add(2, new List<int> { 1, 3, 5, 6, 7 });
            _adjacent.Add(3, new List<int> { 2, 6, 7 });
            _adjacent.Add(4, new List<int> { 0, 1, 5, 8, 9 });
            _adjacent.Add(5, new List<int> { 0, 1, 2, 4, 6, 8, 9, 10});
            _adjacent.Add(6, new List<int> { 1, 2, 3, 5, 7, 9, 10, 11 });
            _adjacent.Add(7, new List<int> { 2, 3, 6, 10, 11 });
            _adjacent.Add(8, new List<int> { 4, 5, 9, 12, 13 });
            _adjacent.Add(9, new List<int> { 4, 5, 6, 8, 10, 12, 13, 14 });
            _adjacent.Add(10, new List<int> { 5, 6, 7, 9, 11, 13, 14, 15 });
            _adjacent.Add(11, new List<int> { 6, 7, 10, 14, 15});
            _adjacent.Add(12, new List<int> { 8, 9, 13 });
            _adjacent.Add(13, new List<int> { 8, 9, 10, 12, 14});
            _adjacent.Add(14, new List<int> { 9, 10, 11, 13, 15 });
            _adjacent.Add(15, new List<int> { 10, 11, 14 });
        }

        /// <summary>
        /// Read lexicon into HashSet.
        /// </summary>
        /// <param name="path">Path of file (assume Zyzzyva format).</param>
        /// <returns>Set of words from file.</returns>
        private static HashSet<string> ReadWords(string path)
        {
            HashSet<string> words = new HashSet<string>();
            using (var reader = File.OpenText(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string word = line.Split(' ')[0];
                    if (word.Length >= MIN_WORD_LENGTH && word.Length <= 16)
                    {
                        words.Add(word);
                    }
                }
            }
            return words;
        }

        /// <summary>
        /// Minimum word length.
        /// </summary>
        private const int MIN_WORD_LENGTH = 3;

        /// <summary>
        /// TextBoxes ordered left to right and top to bottom.
        /// </summary>
        private TextBox[] _textboxes;

        public MainWindow()
        {
            InitializeComponent();

            _textboxes = new TextBox[16] { txt0, txt1, txt2, txt3, txt4, txt5, txt6, txt7, txt8, txt9, txt10, txt11, txt12, txt13, txt14, txt15 };
        }

        /// <summary>
        /// Recursively find words starting from the specified box.
        /// </summary>
        /// <param name="box">Starting box.</param>
        /// <param name="prefix">Prefix for the current word.</param>
        /// <param name="visited">Indicates which boxes have already been visited for this path.</param>
        /// <param name="lexicon">Lexicon of words to search.</param>
        /// <param name="foundWords">List in which to add found words.</param>
        private void FindWords(int box, string prefix, bool[] visited, Trie lexicon, List<string> foundWords)
        {
            visited[box] = true;
            string possibleWord = prefix + _textboxes[box].Text;

            bool isPrefix;
            bool isWord = lexicon.Contains(possibleWord, out isPrefix);

            if (!isPrefix) // No possible words based on this path, so abort
                return;

            if (possibleWord.Length >= MIN_WORD_LENGTH && isWord)
            {
                foundWords.Add(possibleWord);
            }

            // Visit children that haven't already been visited
            foreach (int child in _adjacent[box].Where(x => !visited[x]))
            {
                bool[] newVisited = new bool[16];
                visited.CopyTo(newVisited, 0);
                FindWords(child, possibleWord, newVisited, lexicon, foundWords);
            }
        }

        /// <summary>
        /// Solve the puzzle and output the found words to the results pane.
        /// </summary>
        private void Solve()
        {
            // Get distinct characters entered into TextBoxes
            HashSet<char> enteredCharacters = new HashSet<char>(_textboxes.SelectMany(textBox => textBox.Text).Distinct());

            // Filter words by the entered characters
            var filteredWords = _allWords.Where(word => word.ToCharArray().Distinct().All(character => enteredCharacters.Contains(character)));
            Trie filteredLexicon = new Trie(filteredWords);

            List<string> foundWords = new List<string>();
            for (int i = 0; i < 16; i++)
            {
                bool[] visited = new bool[16];
                FindWords(i, string.Empty, visited, filteredLexicon, foundWords);
            }

            var words = foundWords.Distinct().OrderByDescending(w => w.Length);
            txtResults.Text = string.Format("Found {0} words:" + Environment.NewLine, words.Count());
            txtResults.Text += string.Join(Environment.NewLine, words);
        }

        /// <summary>
        /// Solve button handler. Invokes Solve function.
        /// </summary>
        private void Solve_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            Solve();
            Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Clear button handler. Removes text from textboxes and results pane.
        /// </summary>
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            foreach (TextBox textBox in _textboxes)
            {
                textBox.Text = string.Empty;
                txtResults.Text = string.Empty;
            }
        }
    }
}
