// HtmlAgilityPack V1.0 - Simon Mourier <simon underscore mourier at hotmail dot com>

namespace HtmlAgilityPack
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a combined list and collection of HTML nodes.
    /// </summary>
    public class HtmlAttributeCollection : IEnumerable<HtmlAttribute>
    {
        private HtmlNode _ownernode;
        private List<HtmlAttribute> _items;
        internal Dictionary<string, HtmlAttribute> _names;

        internal HtmlAttributeCollection(HtmlNode ownernode)
        {
            _ownernode = ownernode;

            _items = new List<HtmlAttribute>();
            _names = new Dictionary<string, HtmlAttribute>();
        }

        #region Properties

        /// <summary>
        /// Gets a given attribute from the list using its name.
        /// </summary>
        public HtmlAttribute this[string name]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException("name");
                }
                HtmlAttribute value;
                return _names.TryGetValue(name.ToLower(), out value) ? value : null;
            }
            set
            {
                Add(value);
            }
        }

        /// <summary>
        /// Gets the attribute at the specified index.
        /// </summary>
        public HtmlAttribute this[int index]
        {
            get { return _items[index]; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the number of elements actually contained in the list.
        /// </summary>
        public int Count
        {
            get { return _items.Count; }
        }

        /// <summary>
        /// Checks for existance of attribute with given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            return _names.ContainsKey(name.ToLower());
        }

        /// <summary>
        /// Retrieves the index for the given attribute name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Index of attribute or -1 if not found.</returns>
        public int IndexOf(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            name = name.ToLower();

            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Name == name)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Inserts the specified attribute as the last attribute in the collection.
        /// </summary>
        /// <param name="item">The attribute to insert. May not be null.</param>
        /// <returns>The appended attribute.</returns>
        public HtmlAttribute Add(HtmlAttribute item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            _ownernode.SetChanged();

            HtmlAttribute attribute;

            if (_names.TryGetValue(item.Name, out attribute))
            {
                // Update attribute value;
                attribute.Value = item.Value;

                // Return existing attribute.
                return attribute;
            }

            item._ownernode = _ownernode;

            // Add new attibute to collection.
            _items.Add(item);
            _names.Add(item.Name, item);

            return item;
        }

        /// <summary>
        /// Creates and inserts a new attribute as the last attribute in the collection.
        /// </summary>
        /// <param name="name">The name of the attribute to insert.</param>
        /// <returns>The appended attribute.</returns>
        public HtmlAttribute Add(string name)
        {
            return Add(_ownernode._ownerdocument.CreateAttribute(name));
        }

        /// <summary>
        /// Creates and inserts a new attribute as the last attribute in the collection.
        /// </summary>
        /// <param name="name">The name of the attribute to insert.</param>
        /// <param name="value">The value of the attribute to insert.</param>
        /// <returns>The appended attribute.</returns>
        public HtmlAttribute Add(string name, string value)
        {
            return Add(_ownernode._ownerdocument.CreateAttribute(name, value));
        }

        /// <summary>
        /// Removes a given attribute from the list.
        /// </summary>
        /// <param name="item">The attribute to remove. May not be null.</param>
        public void Remove(HtmlAttribute item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            Remove(item.Name);
        }

        /// <summary>
        /// Removes an attribute from the list, using its name. If there are more than one attributes with this name, they will all be removed.
        /// </summary>
        /// <param name="name">The attribute's name. May not be null.</param>
        public void Remove(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            name = name.ToLower();
            
            HtmlAttribute attribute;

            if (_names.TryGetValue(name, out attribute))
            {
                _names.Remove(name);
                _items.Remove(attribute);
            }
        }

        /// <summary>
        /// Removes all attributes from the collection.
        /// </summary>
        public void Clear()
        {
            _names.Clear();
            _items.Clear();

            _ownernode.SetChanged();
        }

        /// <summary>
        /// Returns all attributes with specified name. Handles case insentivity
        /// </summary>
        /// <param name="name">Name of the attribute</param>
        /// <returns></returns>
        public IEnumerable<HtmlAttribute> AttributesWithName(string name)
        {
            name = name.ToLower();
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Name.Equals(name))
                {
                    yield return _items[i];
                }
            }
        }

        #endregion

        /// <summary>
        /// Get list enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<HtmlAttribute> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <summary>
        /// Get explicit non-generic list enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}