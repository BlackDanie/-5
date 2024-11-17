using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

// Абстрактний клас Project
public abstract class Project
{
    public string Title { get; set; }
    public int EstimatedHours { get; set; }

    public Project() { } // Для серіалізації

    public Project(string title, int estimatedHours)
    {
        Title = title;
        EstimatedHours = estimatedHours;
    }

    public abstract void DisplayInfo();
}

// Інтерфейси
public interface ITaskable
{
    void AddTask(string task);
}

public interface ICalculable
{
    decimal CalculateCost(decimal rate);
}

// Клас веб-розробки
public class WebDevelopmentProject : Project, ITaskable, ICalculable
{
    public List<string> Tasks { get; set; } = new List<string>();

    public WebDevelopmentProject() { } // Для серіалізації

    public WebDevelopmentProject(string title, int estimatedHours)
        : base(title, estimatedHours)
    {
    }

    public void AddTask(string task)
    {
        Tasks.Add(task);
    }

    public decimal CalculateCost(decimal rate)
    {
        return EstimatedHours * rate;
    }

    public override void DisplayInfo()
    {
        Console.WriteLine($"[Веб-розробка] Назва: {Title}, Годин: {EstimatedHours}, Завдань: {Tasks.Count}");
    }
}

// Клас мобільної розробки
public class MobileDevelopmentProject : Project, ITaskable, ICalculable
{
    public List<string> Tasks { get; set; } = new List<string>();

    public MobileDevelopmentProject() { } // Для серіалізації

    public MobileDevelopmentProject(string title, int estimatedHours)
        : base(title, estimatedHours)
    {
    }

    public void AddTask(string task)
    {
        Tasks.Add(task);
    }

    public decimal CalculateCost(decimal rate)
    {
        return EstimatedHours * rate;
    }

    public override void DisplayInfo()
    {
        Console.WriteLine($"[Мобільна розробка] Назва: {Title}, Годин: {EstimatedHours}, Завдань: {Tasks.Count}");
    }
}

// Основний клас програми
class Program
{
    static List<Project> projects = new List<Project>();

    static void Main()
    {
        string filePath = "projects.xml";
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("\nМеню:");
            Console.WriteLine("1. Додати новий проект");
            Console.WriteLine("2. Вивести список проектів");
            Console.WriteLine("3. Показати деталі проекту");
            Console.WriteLine("4. Додати завдання до проекту");
            Console.WriteLine("5. Обчислити вартість проекту");
            Console.WriteLine("6. Зберегти проекти у файл");
            Console.WriteLine("7. Завантажити проекти з файлу");
            Console.WriteLine("8. Вийти");
            Console.Write("Оберіть опцію: ");

            switch (Console.ReadLine())
            {
                case "1":
                    AddProject();
                    break;
                case "2":
                    ListProjects();
                    break;
                case "3":
                    ShowProjectDetails();
                    break;
                case "4":
                    AddTaskToProject();
                    break;
                case "5":
                    CalculateProjectCost();
                    break;
                case "6":
                    SaveProjectsToFile(filePath);
                    break;
                case "7":
                    LoadProjectsFromFile(filePath);
                    break;
                case "8":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Неправильний вибір, спробуйте ще раз.");
                    break;
            }
        }
    }

    static void AddProject()
    {
        Console.Write("Введіть назву проекту: ");
        string title = Console.ReadLine();

        Console.Write("Введіть кількість годин, необхідних для завершення: ");
        int hours = int.Parse(Console.ReadLine());

        Console.Write("Тип проекту (1 - веб, 2 - мобільна): ");
        string type = Console.ReadLine();

        if (type == "1")
        {
            projects.Add(new WebDevelopmentProject(title, hours));
        }
        else if (type == "2")
        {
            projects.Add(new MobileDevelopmentProject(title, hours));
        }
        else
        {
            Console.WriteLine("Невірний тип проекту.");
        }
    }

    static void ListProjects()
    {
        Console.WriteLine("\nСписок проектів:");
        for (int i = 0; i < projects.Count; i++)
        {
            Console.Write($"{i + 1}. ");
            projects[i].DisplayInfo();
        }
    }

    static void ShowProjectDetails()
    {
        Console.Write("Введіть номер проекту: ");
        int index = int.Parse(Console.ReadLine()) - 1;

        if (index >= 0 && index < projects.Count)
        {
            projects[index].DisplayInfo();
            if (projects[index] is ITaskable taskable)
            {
                Console.WriteLine("Завдання:");
                foreach (var task in ((dynamic)projects[index]).Tasks)
                {
                    Console.WriteLine($"- {task}");
                }
            }
        }
        else
        {
            Console.WriteLine("Проект не знайдено.");
        }
    }

    static void AddTaskToProject()
    {
        Console.Write("Введіть номер проекту: ");
        int index = int.Parse(Console.ReadLine()) - 1;

        if (index >= 0 && index < projects.Count && projects[index] is ITaskable taskable)
        {
            Console.Write("Введіть завдання: ");
            string task = Console.ReadLine();
            taskable.AddTask(task);
        }
        else
        {
            Console.WriteLine("Проект не знайдено або не підтримує завдання.");
        }
    }

    static void CalculateProjectCost()
    {
        Console.Write("Введіть номер проекту: ");
        int index = int.Parse(Console.ReadLine()) - 1;

        if (index >= 0 && index < projects.Count && projects[index] is ICalculable calculable)
        {
            Console.Write("Введіть ставку за годину: ");
            decimal rate = decimal.Parse(Console.ReadLine());
            Console.WriteLine($"Вартість проекту: {calculable.CalculateCost(rate):C}");
        }
        else
        {
            Console.WriteLine("Проект не знайдено або не підтримує обчислення вартості.");
        }
    }

    static void SaveProjectsToFile(string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<Project>), new Type[] { typeof(WebDevelopmentProject), typeof(MobileDevelopmentProject) });

        using (FileStream fs = new FileStream(filePath, FileMode.Create))
        {
            serializer.Serialize(fs, projects);
        }

        Console.WriteLine("Проекти збережено.");
    }

    static void LoadProjectsFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Project>), new Type[] { typeof(WebDevelopmentProject), typeof(MobileDevelopmentProject) });

            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                projects = (List<Project>)serializer.Deserialize(fs);
            }

            Console.WriteLine("Проекти завантажено.");
        }
        else
        {
            Console.WriteLine("Файл не знайдено.");
        }
    }
}
