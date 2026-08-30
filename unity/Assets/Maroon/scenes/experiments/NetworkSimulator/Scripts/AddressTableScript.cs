using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class AddressTableScript : MonoBehaviour {
        [SerializeField]
        private AddressTableRow rowPrefab;
        [SerializeField]
        private Transform tableBody;

        private readonly List<AddressTableRow> tableRows = new List<AddressTableRow>();

        public void SetRows(IEnumerable<(string, string)> rows) {
            Clear();
            foreach(var row in rows.OrderBy(r => r.Item1)) {
                AddRow(row.Item1, row.Item2);
            }
        }

        private void AddRow(string text1, string text2) {
            var row = Instantiate(rowPrefab, tableBody);
            row.SetText(text1, text2);
            tableRows.Add(row);
        }

        public void Clear() {
            foreach(var row in tableRows) {
                Destroy(row.gameObject);
            }
            tableRows.Clear();
        }

        public AddressTableRow GetRow(string text) {
            return tableRows.FirstOrDefault(r => r.Text1 == text);
        }

        public void HighlightRow(string text) {
            HideHighlight();
            var row = GetRow(text);
            if(row != null) {
                row.SetFontStyleBold();
            }
        }

        public void HideHighlight() {
            foreach(var row in tableRows) {
                row.SetFontStyleNormal();
            }
        }
    }
}
