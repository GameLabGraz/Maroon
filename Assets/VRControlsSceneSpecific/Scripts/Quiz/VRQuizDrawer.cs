using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class VRQuizDrawer : UIPanel
{
    public UIAnswer dummyUIAnswer;
    public Text questionText;
    public RectTransform answerParent;
    Quiz currentQuiz;
    List<UIAnswer> uiAnswers = new List<UIAnswer>();
    Question currentQuestion;
    int currentQuestionIndex = 0;


    public override void open()
    {
        base.open();
        currentQuestionIndex = 0;
        currentQuiz = VRQuizController.Instance.quizController.SelectedQuiz;
        currentQuestion = null;
        openNextQuestion();
    }

    void reset()
    {
        while (uiAnswers.Count > 0)
        {
            Destroy(uiAnswers[0].gameObject);
            uiAnswers.RemoveAt(0);
        }
        questionText.text = "";
    }

    void openNextQuestion()
    {
        reset();
        if (currentQuestionIndex >= currentQuiz.Questions.Count)
        {
            VRQuizController.Instance.openQuizSelection();
            return;
        }

        currentQuestion = currentQuiz.Questions[currentQuestionIndex++];
        questionText.text = currentQuestion.Text;

        foreach (Answer answer in currentQuestion.Answers)
        {
            addAnswerButton(answer);
        }
    }

    void addAnswerButton(Answer answer)
    {
        UIAnswer uiAnswer = Instantiate(dummyUIAnswer);
        uiAnswer.init(answer.Text, () =>
        {
            onAnswerPress(uiAnswer, answer.Correct);
        });
        uiAnswer.transform.SetParent(answerParent);
        uiAnswer.transform.localScale = Vector3.one;
        uiAnswer.transform.localEulerAngles = Vector3.zero;
        uiAnswer.transform.localPosition = Vector3.zero;

        uiAnswers.Add(uiAnswer);
    }

    void onAnswerPress(UIAnswer uiAnswer, bool correctAnswer)
    {
        if (currentQuestion.Type == Question.QuestionType.SingleAnswer)
        {
            float animationDuration = 2f;
            if (correctAnswer)
                uiAnswer.playCorrectAnimation(animationDuration);
            else
            {
                uiAnswer.playWrongAnimation(animationDuration);
                int correctIndex = currentQuestion.Answers.FindIndex(a => a.Correct);
                uiAnswers[correctIndex].playCorrectAnimation(animationDuration);
            }

            Invoke("openNextQuestion", animationDuration);
        }
        else
        {
            //TODO add multiple answer question handling
            Debug.LogError("Multiple answer quizzes not implemented");
        }
    }
}
