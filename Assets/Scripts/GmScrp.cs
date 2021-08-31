using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GmScrp : MonoBehaviour
{
    public QuestionList[] questions; // Массив вопросов
    public Text[] answersText; // Массив ответов
    public Image qImage; // Сюда выводится вопрос
    public GameObject HeadPanel;
    public Button[] answerBttns = new Button[4];
    public Sprite[] TFIcons = new Sprite[3];
    public Image TFIcon;
    public Text TFText;
    public Text EndText;

    List<object> qList; // Список вопросов (для редактирования, чтоб исключить повторения)
    QuestionList crntQ;
    int randQ;

    public void OnClickPlay()
    {
        qList = new List<object>(questions); // Записываем все вопросы
        questionGenerate(); // Вызываем генерацию вопросов
        if (!HeadPanel.GetComponent<Animator>().enabled)    // Включаем анимацию панели:
            HeadPanel.GetComponent<Animator>().enabled = true; // Первый раз
        else
            HeadPanel.GetComponent<Animator>().SetTrigger("In"); // После проигрыша
    }

    void questionGenerate()
    {
        if (qList.Count > 0)
        {
            randQ = Random.Range(0, qList.Count); // Рандомный индекс вопроса
            crntQ = qList[randQ] as QuestionList; // Выбираем рандомно текущий вопрос из списка
            qImage.sprite = crntQ.question;    // Выводим текущий вопрос в специальное поле

            qImage.gameObject.GetComponent<Animator>().SetTrigger("In");

            List<string> answers = new List<string>(crntQ.answers); // Создаём список всех ответов на вопросы
            for (int i = 0; i < crntQ.answers.Length; i++) // Вывод ответов
            {
                int rand = Random.Range(0, answers.Count); // Рандомим ответы
                answersText[i].text = answers[rand]; // Помещаем ответы в формы
                answers.RemoveAt(rand); // Не создаём дубликатов
            }
            StartCoroutine(animBttns());
        }
        else
        {
            EndText.text = "ВЫ ПРОШЛИ ИГРУ!";
        }
    }

    IEnumerator animBttns()
    {
        for (int i = 0; i < answerBttns.Length; i++)
            answerBttns[i].gameObject.SetActive(false);

        for (int i = 0; i < answerBttns.Length; i++)
            answerBttns[i].interactable = false;
        int a = 0;
        while (a < answerBttns.Length)
        {
            yield return new WaitForSeconds(1);
            if (!answerBttns[a].gameObject.activeSelf)
                answerBttns[a].gameObject.SetActive(true);
            else
                answerBttns[a].gameObject.GetComponent<Animator>().SetTrigger("In");
            a++;
        }
        yield return new WaitForSeconds(1);
        for (int i = 0; i < answerBttns.Length; i++)
            answerBttns[i].interactable = true;
        yield break;
    }

    IEnumerator TrueOrFalse(bool check)
    {
        for (int i = 0; i < answerBttns.Length; i++)
            answerBttns[i].gameObject.GetComponent<Animator>().SetTrigger("Out");
        qImage.gameObject.GetComponent<Animator>().SetTrigger("Out");
        yield return new WaitForSeconds(0.5f);
        
        if (!TFIcon.gameObject.activeSelf)
            TFIcon.gameObject.SetActive(true);
        else
            TFIcon.gameObject.GetComponent<Animator>().SetTrigger("In");

        if (check)
        {
            TFIcon.sprite = TFIcons[0];
            yield return new WaitForSeconds(0.4f);
            TFText.text = "ПРАВИЛЬНЫЙ ОТВЕТ!";

            yield return new WaitForSeconds(1);
            TFIcon.gameObject.GetComponent<Animator>().SetTrigger("Out");
            qList.RemoveAt(randQ); //Удаляем из списка вопрос, который уже был.
            yield return new WaitForSeconds(1);
            questionGenerate();
            yield break;
        }
        else
        {
            TFIcon.sprite = TFIcons[1];
            yield return new WaitForSeconds(0.4f);
            TFText.text = "НЕПРАВИЛЬНЫЙ ОТВЕТ!";

            yield return new WaitForSeconds(1);
            TFIcon.gameObject.GetComponent<Animator>().SetTrigger("Out");
            HeadPanel.GetComponent<Animator>().SetTrigger("Out");
            yield break;
        }
 
    }

    public void AnswerBttns(int index)
    {
        if (answersText[index].text.ToString() == crntQ.answers[0])
            StartCoroutine(TrueOrFalse(true));
        else
            StartCoroutine(TrueOrFalse(false));
    }
}

[System.Serializable]
public class QuestionList
{
    public Sprite question; // Вопрос
    public string[] answers = new string[4]; // Массив ответов
}