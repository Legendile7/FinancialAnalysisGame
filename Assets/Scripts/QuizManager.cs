using LootLocker.Requests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class QuizQuestion
{
    public string question;
    public string[] choices;
    public int correctIndex;
    public string difficulty;
}

[Serializable]
public class QuizData
{
    public List<QuizQuestion> questions;
}

public class QuizManager : MonoBehaviour
{
    public int currentSkillScore = 0;
    private const int maxSkillScore = 100; // Hard cap at 100
    private QuizData quizData;
    private QuizQuestion currentQuestion;
    public TMP_Text questionText;
    public Button[] answerButtons;

    private List<QuizQuestion> askedQuestions = new List<QuizQuestion>();

    public GameObject correctNotification;
    public GameObject incorrectNotification;

    public int questionCount = 0;
    public TMP_Text questionCounter;
    public TMP_Text difficultyText;

    public int totalCorrect = 0;

    int[] correctAnswers = new int[3];
    int[] incorrectAnswers = new int[3];

    public GameObject quizPanel;
    public GameObject quizEndPanel;

    public TMP_Text scoreText;
    public TMP_Text XPGain;
    public TMP_Text gemsGain;

    int xpGain = 0;
    int gemsGainAmount = 0;

    void Start()
    {
        LoadQuizData();
        string progressionKey = "skill";

        LootLockerSDKManager.GetPlayerProgression(progressionKey, response =>
        {
            if (!response.success)
            {
                Debug.Log("Failed: " + response.errorData);
                LootLockerSDKManager.RegisterPlayerProgression(progressionKey, (registerResponse) =>
                {
                    if (!registerResponse.success)
                    {
                        Debug.Log("Error Registering progression!");
                        Debug.Log(registerResponse.errorData.ToString());
                        return;
                    }
                    Debug.Log("Progression Registered successfully!");
                });
            }

            Debug.Log($"The player's skill is {response.points} points");
            currentSkillScore = Mathf.Min((int)response.points, maxSkillScore);
            LoadNextQuestion();
        });
    }

    public void EndQuiz()
    {
        quizPanel.SetActive(false);
        quizEndPanel.SetActive(true);

        scoreText.text = $"Score: {totalCorrect}/5 ({totalCorrect * 20}%)";
        xpGain = totalCorrect * 10;
        gemsGainAmount = totalCorrect * 2;
        XPGain.text = "+" + xpGain;
        gemsGain.text = "+" + gemsGainAmount;

        questionCount = 0;
        totalCorrect = 0;
        askedQuestions.Clear();

        // Reset answer counts
        for (int i = 0; i < 3; i++)
        {
            correctAnswers[i] = 0;
            incorrectAnswers[i] = 0;
        }
    }

    public void RestartQuiz()
    {
        quizEndPanel.SetActive(false);
        quizPanel.SetActive(true);
        LoadNextQuestion();
    }

    void LoadQuizData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "quiz_questions.json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            quizData = JsonUtility.FromJson<QuizData>(json);
        }
        else
        {
            Debug.LogError("Quiz data file not found!");
        }
    }

    public void SkillUp(int[] correctAnswers, int[] incorrectAnswers)
    {
        int pointsToAdd = correctAnswers[0] - incorrectAnswers[0] +
                          correctAnswers[1] * 2 - incorrectAnswers[1] * 2 +
                          correctAnswers[2] * 3 - incorrectAnswers[2] * 3;
        Debug.Log($"Adding {pointsToAdd} points to player's skill");
        string progressionKey = "skill";
        ulong amountOfPoints = 0;
        if (pointsToAdd >= 0)
        {
            amountOfPoints = (ulong)pointsToAdd;
            Debug.Log($"Adding {amountOfPoints} points to player's skill");
            LootLockerSDKManager.AddPointsToPlayerProgression(progressionKey, amountOfPoints, response =>
            {
                if (!response.success)
                {
                    Debug.Log("Failed: " + response.errorData);
                    return;
                }

                Debug.Log(response.success);

                currentSkillScore = Mathf.Min((int)response.points, maxSkillScore);
                Debug.Log($"The player's skill is now {currentSkillScore} points");
            });
        }
        else
        {
            amountOfPoints = (ulong)Mathf.Abs(pointsToAdd);
            Debug.Log($"Subtracting {amountOfPoints} points to player's skill");
            LootLockerSDKManager.SubtractPointsFromPlayerProgression(progressionKey, amountOfPoints, response =>
            {
                if (!response.success)
                {
                    Debug.Log("Failed: " + response.errorData);
                    return;
                }

                Debug.Log(response.success);

                currentSkillScore = Mathf.Min((int)response.points, maxSkillScore);
                Debug.Log($"The player's skill is now {currentSkillScore} points");
            });
        }

        EndQuiz();
    }

    public string GetNextQuestionDifficulty()
    {
        float normalizedSS = Mathf.Clamp(currentSkillScore, 0, maxSkillScore) / (float)maxSkillScore;

        // Probability for each difficulty based on the skill score
        float P_easy = Mathf.Max(0, 1 - normalizedSS*1.5f);    // Easy decreases as skill increases
        float P_medium = Mathf.Min(1, normalizedSS); // Medium increases as skill increases
        float P_hard = Mathf.Min(1, normalizedSS * 0.5f);   // Hard increases, but is capped at a lower rate than medium

        // Normalize the probabilities to sum to 1
        float total = P_easy + P_medium + P_hard;
        P_easy /= total;
        P_medium /= total;
        P_hard /= total;

        // Randomly select a difficulty based on the calculated probabilities
        float rand = UnityEngine.Random.value;

        if (rand < P_easy) return "Easy";
        else if (rand < P_easy + P_medium) return "Medium";
        else return "Hard";
    }


    public void LoadNextQuestion()
    {
        if (questionCount >= 5)
        {
            Debug.Log("Quiz completed!");
            SkillUp(correctAnswers, incorrectAnswers);
            quizPanel.SetActive(false);
            quizEndPanel.SetActive(true);
            return;
        }

        // Find a new question that hasn't been asked yet
        string difficulty = GetNextQuestionDifficulty();
        difficultyText.text = difficulty;
        List<QuizQuestion> filteredQuestions = quizData.questions.FindAll(q => q.difficulty == difficulty);
        filteredQuestions = filteredQuestions.Where(q => !askedQuestions.Contains(q)).ToList();

        if (filteredQuestions.Count > 0)
        {
            // Pick a random question from the remaining unasked questions
            currentQuestion = filteredQuestions[UnityEngine.Random.Range(0, filteredQuestions.Count)];

            // Mark the question as asked
            askedQuestions.Add(currentQuestion);

            // Update the question and choices UI
            questionText.text = currentQuestion.question;

            List<int> indices = new List<int>();
            for (int i = 0; i < currentQuestion.choices.Length; i++)
            {
                indices.Add(i);
            }
            indices = indices.OrderBy(x => UnityEngine.Random.value).ToList();

            for (int i = 0; i < answerButtons.Length; i++)
            {
                if (i < currentQuestion.choices.Length)
                {
                    answerButtons[i].gameObject.SetActive(true);
                    int randomizedIndex = indices[i];
                    answerButtons[i].GetComponentInChildren<TMP_Text>().text = currentQuestion.choices[randomizedIndex];
                    answerButtons[i].onClick.RemoveAllListeners();
                    answerButtons[i].onClick.AddListener(() => SubmitAnswer(randomizedIndex));
                }
                else
                {
                    answerButtons[i].gameObject.SetActive(false);
                }
            }

            questionCount++;
            questionCounter.text = $"Question {questionCount}/5";
        }
        else
        {
            Debug.LogError("No remaining questions found for difficulty: " + difficulty);
        }
    }

    public void SubmitAnswer(int selectedIndex)
    {
        int difficultyIndex = currentQuestion.difficulty == "Easy" ? 0 : (currentQuestion.difficulty == "Medium" ? 1 : 2);


        // Check if the selected answer is correct
        if (selectedIndex == currentQuestion.correctIndex)
        {
            correctAnswers[difficultyIndex] += 1;
            correctNotification.SetActive(true); // Show correct notification
            correctNotification.GetComponentInChildren<TMP_Text>().text = "Correct!";
            totalCorrect++;
        }
        else
        {
            incorrectAnswers[difficultyIndex] += 1;
            incorrectNotification.SetActive(true); // Show incorrect notification
            incorrectNotification.GetComponentInChildren<TMP_Text>().text = "Incorrect!";
        }

        // Load the next question
        LoadNextQuestion();
    }

}
