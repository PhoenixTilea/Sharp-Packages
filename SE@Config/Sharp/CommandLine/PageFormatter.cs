// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using SE;

namespace SE.Config
{
    /// <summary>
    /// AutoConfigAttribute page pretty printing
    /// </summary>
    public class PageFormatter
    {
        class ArgumentComparer : IComparer<string[]>
        {
            bool IsBasicCharacter(char c)
            {
                return ((c >= 'A' && c <= 'Z') ||
                        (c >= 'a' && c <= 'z'));
            }
            public int Compare(string[] x, string[] y)
            {
                if (x[0][0] == '[' && y[0][0] == '[') return string.Compare(x[0], y[0]);
                else if (x[0][0] == '[') return -1;
                else if (y[0][0] == '[') return 1;
                else
                {
                    if (IsBasicCharacter(x[0][1]) && IsBasicCharacter(y[0][1])) return string.Compare(x[0], y[0]);
                    else if (IsBasicCharacter(x[0][1])) return -1;
                    else if (IsBasicCharacter(y[0][1])) return 1;
                    else return string.Compare(x[0], y[0]);
                }
            }
        }
        private static ArgumentComparer comparer = new ArgumentComparer();

        protected string separator = " | ";
        /// <summary>
        /// The separator char used for printing
        /// </summary>
        public string Separator
        {
            get { return separator; }
            set { separator = value; }
        }

        protected List<string> columns = new List<string>();
        /// <summary>
        /// The columns contained in this manual page
        /// </summary>
        public List<string> Columns
        {
            get{ return columns; }
        }
        
        protected List<string[]> rows = new List<string[]>();
        /// <summary>
        /// The rows of this manual page
        /// </summary>
        public List<string[]> Rows
        {
            get{ return rows; }
        }

        /// <summary>
        /// Creates a new manual page instance with the provided columns
        /// </summary>
        public PageFormatter(params string[] columns)
        { 
            this.columns.AddRange(columns);
        }

        /// <summary>
        /// Adds new columns to the manual page
        /// </summary>
        /// <param name="names"></param>
        public void AddColumn(IEnumerable<string> names)
        {
            foreach (string name in names)
                Columns.Add(name);
        }
        /// <summary>
        /// Adds an entire row to the manual page
        /// </summary>
        /// <param name="values"></param>
        public void AddRow(params string[] values)
        {
            if(values == null)
                return;

            while (columns.Count < values.Length)
                columns.Add(string.Empty);

            Rows.Add(values);
        }

        /// <summary>
        /// Adds a collection of rows to the manual page
        /// </summary>
        /// <param name="list"></param>
        public void AddRows(List<string[]> list, bool allowDuplicates = false)
        {
            foreach (string[] row in list)
                if (allowDuplicates || !Rows.Any(p => p.SequenceEqual(row)))
                    AddRow(row);
        }

        List<int> GetColumnLengths()
        {
            return Columns
                .Select((t, i) => Rows.Select(x => (x.Length <= i) ? string.Empty : x[i])
                    .Union(new[] { Columns[i] })
                    .Where(x => x != null)
                    .Select(x => x.ToString().Length).Max())
                .ToList();
        }

        /// <summary>
        /// Sorts the rows by content
        /// </summary>
        public void Sort()
        {
            rows.Sort(comparer);
        }

        public override string ToString()
        {
            if (columns.Count == 0)
                return string.Empty;

            StringBuilder result = new StringBuilder();

            List<int> columnLengths = GetColumnLengths();
            string format = Enumerable.Range(0, columns.Count).Select(i => "{" + i + ",-" + columnLengths[i] + "}" + separator).Aggregate((s, a) => s + a);
            format = format.Trim().Trim(separator.ToCharArray());

            int maxRowLength = Math.Max(0, rows.Any() ? rows.Max(row => (row.Length < columns.Count) ? row[0].Length : string.Format(format, row).Length) : 0);
            string columnHeaders = string.Format(format, Columns.ToArray());

            List<string> results = rows.Select(row => (row.Length < columns.Count) ? row[0] : string.Format(format, row)).ToList();
            if (columnHeaders.Trim().Length > columnLengths.Count)
            {
                int longestLine = Math.Max(maxRowLength, columnHeaders.Length);
                string divider = " " + string.Join("", Enumerable.Repeat("-", longestLine - 1)) + " ";
                result.AppendLine(columnHeaders);
                result.AppendLine(divider);
            }

            foreach (string row in results)
                result.AppendLine(row.Trim());

            return result.ToString().Trim(Environment.NewLine.ToCharArray());
        }
    }
}
