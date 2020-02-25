using System.Collections.Generic;
using Antares.Evaluation.LearningContent;
using UnityEngine;

namespace Maroon.Assessment.Content
{
    public class AssessmentTableCell : AssessmentDivision { }

    public class AssessmentTableRow : AssessmentContent
    {
        protected  List<AssessmentTableCell> Cells = new List<AssessmentTableCell>();

        public override void LoadContent(Node content)
        {
            if (!(content is Antares.Evaluation.LearningContent.Row row))
                return;

            foreach (var cell in row.Cells)
            {
                var cellObject = Object.Instantiate(Resources.Load(PanelPrefab), transform, false) as GameObject;
                var assessmentTableCell = cellObject?.AddComponent<AssessmentTableCell>();
                if(assessmentTableCell == null) continue;

                assessmentTableCell.ObjectId = ObjectId;
                assessmentTableCell.LoadContent(cell);

                Cells.Add(assessmentTableCell);
            }
        }
    }

    public class AssessmentTable : AssessmentContent
    {
        protected List<AssessmentTableRow> Rows = new List<AssessmentTableRow>();

        public override void LoadContent(Node content)
        {
            if (!(content is Antares.Evaluation.LearningContent.Table table))
                return;

            foreach (var row in table.Rows)
            {
                var rowObject = Object.Instantiate(Resources.Load(TableRowPrefab), transform, false) as GameObject;
                var assessmentTableRow = rowObject?.AddComponent<AssessmentTableRow>();
                if(assessmentTableRow == null) continue;

                assessmentTableRow.ObjectId = ObjectId;
                assessmentTableRow.LoadContent(row);
                
                Rows.Add(assessmentTableRow);
            }
        }
    }
}
