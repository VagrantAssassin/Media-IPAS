using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuizControllerTextOnly : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private QuizSetTextOnly quizSet;

    [Header("Panels")]
    [SerializeField] private GameObject panelStart;
    [SerializeField] private GameObject panelQuiz;
    [SerializeField] private GameObject panelResult;

    [Header("Start UI")]
    [SerializeField] private TMP_InputField inputName;
    [SerializeField] private TMP_InputField inputClass;
    [SerializeField] private Button buttonStart;
    [SerializeField] private TMP_Text textWarning;

    [Header("Quiz UI")]
    [SerializeField] private TMP_Text textQuestion;
    [SerializeField] private TMP_Text textQuestionNumber;   // nomor soal (atas kiri)
    [SerializeField] private Button[] answerButtons;        // size 4
    [SerializeField] private TMP_Text[] answerButtonTexts;  // size 4

    [Header("Result UI")]
    [SerializeField] private TMP_Text textIdentity;
    [SerializeField] private TMP_Text textScore;

    [Header("Exit Button")]
    [SerializeField] private Button exitButton;
    [SerializeField] private string exitTargetSceneName = "Materi";

    [Header("Shuffle")]
    [SerializeField] private bool shuffleQuestions = true;
    [SerializeField] private bool shuffleChoices = true;

    private string playerName;
    private string playerClass;

    private List<int> questionOrder = new();
    private int currentQuestionIdx;
    private int correctCount;

    private int currentCorrectButtonIndex;

    private void Awake()
    {
        if (buttonStart != null)
        {
            buttonStart.onClick.RemoveAllListeners();
            buttonStart.onClick.AddListener(OnClickStart);
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(OnClickExit);
        }

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int captured = i;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnClickAnswer(captured));
        }

        ShowPanelStart();
    }

    private void ShowPanelStart()
    {
        if (panelStart) panelStart.SetActive(true);
        if (panelQuiz) panelQuiz.SetActive(false);
        if (panelResult) panelResult.SetActive(false);
        if (textWarning) textWarning.text = "";
        SetExitButtonVisible(true);
    }

    private void ShowPanelQuiz()
    {
        if (panelStart) panelStart.SetActive(false);
        if (panelQuiz) panelQuiz.SetActive(true);
        if (panelResult) panelResult.SetActive(false);
        SetExitButtonVisible(false);
    }

    private void ShowPanelResult()
    {
        if (panelStart) panelStart.SetActive(false);
        if (panelQuiz) panelQuiz.SetActive(false);
        if (panelResult) panelResult.SetActive(true);
        SetExitButtonVisible(true);
    }

    private void SetExitButtonVisible(bool visible)
    {
        if (exitButton != null)
            exitButton.gameObject.SetActive(visible);
    }

    private void OnClickExit()
    {
        if (!string.IsNullOrEmpty(exitTargetSceneName))
        {
            SceneManager.LoadScene(exitTargetSceneName);
            return;
        }

        Application.Quit();
    }

    private void OnClickStart()
    {
        if (quizSet == null || quizSet.questions == null || quizSet.questions.Count == 0)
        {
            if (textWarning) textWarning.text = "Quiz belum memiliki soal (QuizSet kosong).";
            return;
        }

        playerName = inputName != null ? inputName.text.Trim() : "";
        playerClass = inputClass != null ? inputClass.text.Trim() : "";

        if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(playerClass))
        {
            if (textWarning) textWarning.text = "Nama lengkap dan kelas wajib diisi.";
            return;
        }

        correctCount = 0;
        currentQuestionIdx = 0;

        BuildQuestionOrder();
        ShowPanelQuiz();
        ShowQuestion();
    }

    private void BuildQuestionOrder()
    {
        questionOrder.Clear();
        for (int i = 0; i < quizSet.questions.Count; i++)
            questionOrder.Add(i);

        if (shuffleQuestions)
            Shuffle(questionOrder);
    }

    private void ShowQuestion()
    {
        if (currentQuestionIdx >= questionOrder.Count)
        {
            FinishQuiz();
            return;
        }

        if (textQuestionNumber)
            textQuestionNumber.text = $"Soal {currentQuestionIdx + 1} / {questionOrder.Count}";

        int qIndex = questionOrder[currentQuestionIdx];
        var q = quizSet.questions[qIndex];

        if (textQuestion) textQuestion.text = q.questionText ?? "";

        // choices original order: 0=A,1=B,2=C,3=D
        string[] choices = { q.choiceA, q.choiceB, q.choiceC, q.choiceD };

        List<int> choiceOrder = new() { 0, 1, 2, 3 };
        if (shuffleChoices)
            Shuffle(choiceOrder);

        currentCorrectButtonIndex = choiceOrder.IndexOf(q.correctChoiceIndex);

        for (int btn = 0; btn < 4; btn++)
        {
            int choiceIdx = choiceOrder[btn];
            string label = choices[choiceIdx] ?? "";

            if (answerButtonTexts != null && btn < answerButtonTexts.Length && answerButtonTexts[btn] != null)
                answerButtonTexts[btn].text = label;

            if (answerButtons != null && btn < answerButtons.Length && answerButtons[btn] != null)
                answerButtons[btn].interactable = true;
        }
    }

    private void OnClickAnswer(int buttonIndex)
    {
        // disable all to prevent double-click spam
        for (int i = 0; i < 4; i++)
            if (answerButtons[i] != null) answerButtons[i].interactable = false;

        if (buttonIndex == currentCorrectButtonIndex)
            correctCount++;

        currentQuestionIdx++;
        ShowQuestion();
    }

    private void FinishQuiz()
    {
        ShowPanelResult();

        if (textIdentity)
            textIdentity.text = $"Nama: {playerName}\nKelas: {playerClass}";

        if (textScore)
            textScore.text = $"Hasil: {correctCount} / {questionOrder.Count} benar";
    }

    private static void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}