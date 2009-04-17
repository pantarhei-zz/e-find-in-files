using System;
using System.Globalization;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Win32;

namespace FindInFiles
{
    /// <summary>
    /// Represents values previously entered into a ComboBox and
    /// persisted to the registry.
    /// </summary>
    class ComboBoxHistory
    {
        private const int HISTORY_LENGTH = 10;

        private readonly string key;
        private readonly ComboBox combobox;
        private readonly List<string> history = new List<string>();

        /// <summary>
        /// Creates a new ComboBoxHistory instance for the given key name,
        /// associated with the given combo box.
        /// </summary>
        public ComboBoxHistory(string key, ComboBox box)
        {
            this.key = key;
            combobox = box;
        }

        public void Load(RegistryKey regkey)
        {
            history.Clear();
            PushBack(regkey.GetValue(key) as string);

            for (int i = 1; i < HISTORY_LENGTH; i++)
                PushBack(regkey.GetValue(key + i.ToString(CultureInfo.InvariantCulture)) as string);

            SetComboBox(combobox);
        }

        public void Grab()
        {
            PushFront(combobox.Text);
        }

        private void PushFront(string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            history.Remove(value);
            history.Insert(0, value);
            Trim();
        }

        private void PushBack(string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            history.Add(value);
            Trim();
        }

        private void Trim()
        {
            while (history.Count > HISTORY_LENGTH)
                history.RemoveAt(HISTORY_LENGTH);
        }

        public void Save(RegistryKey regkey)
        {
            Trim();
            if (history.Count == 0)
                return;

            regkey.SetValue(key, history[0]);
            for (int i = 1; i < history.Count; i++)
                regkey.SetValue(key + i.ToString(CultureInfo.InvariantCulture), history[i]);
        }

        private void SetComboBox(ComboBox box)
        {
            box.BeginUpdate();
            box.Items.Clear();

            if (history.Count != 0)
            {
                foreach (var o in history)
                    box.Items.Add(o);

                box.Text = history[0];
            }

            box.EndUpdate();
        }
    }
}
