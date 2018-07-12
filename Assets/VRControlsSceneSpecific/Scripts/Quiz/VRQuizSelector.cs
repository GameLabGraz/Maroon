using UnityEngine;
using System.Collections.Generic;

public class VRQuizSelector : UIPanel
{
    public RectTransform listTransform;
    public UISelectable dummySelectable;
    List<UISelectable> selectables = new List<UISelectable>();

    void add(Quiz quiz)
    {
        UISelectable selectable = Instantiate(dummySelectable);
        selectable.transform.SetParent(listTransform);
        selectable.transform.localScale = Vector3.one;
        selectable.transform.localPosition = Vector3.zero;
        selectable.transform.localEulerAngles = Vector3.zero;
        selectable.init(quiz.Name, () =>
        {
            VRQuizController.Instance.openQuiz(quiz);
        });
        selectables.Add(selectable);
    }

    public void add(List<Quiz> quizzes)
    {
        foreach (Quiz quiz in quizzes)
        {
            add(quiz);
        }
    }
}
