using System.Collections.Generic;

namespace WordamentSolver
{
    /// <summary>
    /// Simple Trie using char nodes.
    /// </summary>
    class Trie
    {
        private class Node
        {
            /// <summary>
            /// Character for this node.
            /// </summary>
            public char Character { get; set; }

            /// <summary>
            /// Indicates if this node forms a complete word.
            /// </summary>
            public bool IsWord { get; set; }

            /// <summary>
            /// Child nodes.
            /// </summary>
            public Dictionary<char, Node> Children = new Dictionary<char, Node>();

            /// <summary>
            /// Create a node with the supplied character.
            /// </summary>
            /// <param name="character">Character for this node.</param>
            public Node(char character)
            {
                Character = character;
            }
        }

        private Node root = new Node(' '); // Dummy root node

        /// <summary>
        /// Create an empty Trie.
        /// </summary>
        public Trie() { }

        /// <summary>
        /// Create a Trie using the supplied words.
        /// </summary>
        /// <param name="words">Initial words to populate the Trie.</param>
        public Trie(IEnumerable<string> words)
        {
            foreach (string word in words)
                Add(word);
        }

        /// <summary>
        /// Add a word to the Trie.
        /// </summary>
        /// <param name="word">Word to add.</param>
        public void Add(string word)
        {
            Node node = root;
            foreach (char character in word)
            {
                Node childNode;
                if (!node.Children.TryGetValue(character, out childNode))
                {
                    childNode = new Node(character);
                    node.Children.Add(character, childNode);
                }
                node = childNode;
            }
            // As node will point to the last Node, we indicate that this is a word
            node.IsWord = true;
        }

        /// <summary>
        /// Indicates if the Trie contains the specified word and if the word is a prefix to another word.
        /// </summary>
        /// <param name="word">Word to search for.</param>
        /// <param name="isPrefix">True if the specified word is a prefix; otherwise false.</param>
        /// <returns>True if the Trie contains the specified word; otherwise false.</returns>
        public bool Contains(string word, out bool isPrefix)
        {
            Node node = root;
            foreach (char character in word)
            {
                if (!node.Children.TryGetValue(character, out node))
                {
                    isPrefix = false;
                    return false;
                }
            }
            isPrefix = true;
            return node.IsWord;
        }
    }
}
