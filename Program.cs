using System.IO.Enumeration;
using System.Linq.Expressions;
using System.Text.Json;

NoteManager manager = new NoteManager();
manager.LoadFromFile();

while (true)
{
    Console.WriteLine("\nВыберите одно из действий:" +
        "\n* Создать файл для заметок [0]" +
        "\n* Добавить заметку [1] " +
        "\n* Посмотреть все заметки [2]" +
        "\n* Удалить заметку [3]" +
        "\n* Поиск [4]" +
        "\n* Редактировать заметку [5]" +
        "\n* Сортировать заметки [6]" +
        "\n* Выйти [7]");
    try
    {
        int Choose = Convert.ToInt32(Console.ReadLine());
        if (Choose == 7)
            break;
        if (Choose >= 8)
            Console.WriteLine("Ты обезьяна? Сказано выбрато только из семи!\n");

        switch (Choose)
        {
            case 0:
                manager.CreateFileNote();
                break;
            case 1: // Добавить заметку
                manager.AddNotes();
                break;
            case 2: // Посмотреть все заметки
                manager.ShowAllNotes();
                break;
            case 3: // Удалить заявку
                manager.DeleteNotes();
                break;
            case 4: //Поиск
                Console.WriteLine("\nВыберите одно из действий:" +
                "\n* По ключевому слову[0]" +
                "\n* По дате [1]" +
                "\n* Выйти [2]");
                Choose = Convert.ToInt32(Console.ReadLine());
                if (Choose == 2)
                    break;
                else if (Choose >= 3)
                    Console.WriteLine("Ты обезьяна? Сказано выбрато только из трех!\n");
                else
                {
                    switch (Choose)
                    {
                        case 0:
                            manager.SearchNotes();
                            break;
                        case 1:
                            manager.SearchDate();
                            break;
                    }

                }
                break;
            case 5: // Редактирование
                manager.EditNotes();
                break;
            case 6:
                Console.WriteLine("\nВыберите одно из действий:" +
            "\n* Сначала новые[0]" +
            "\n* Сначала старые [1]" +
            "\n* Выйти [2]");
                Choose = Convert.ToInt32(Console.ReadLine());
                if (Choose == 2)
                    break;
                else if (Choose >= 3)
                    Console.WriteLine("Ты обезьяна? Сказано выбрато только из трех!\n");
                else
                {
                    switch (Choose)
                    {
                        case 0:
                            manager.SortNoteByDateNew();
                            break;
                        case 1:
                            manager.SortNoteByDateOld();
                            break;
                    }
                }
                break;
        }
    }
    catch (IOException ex)
    {
        Console.WriteLine($"Завязывай писать всякое, а то ошибка вылезла: {ex.Message}");
    }
}
class NoteManager
{
    private Dictionary<int, Note> strings = new Dictionary<int, Note>();
    private static string fileName = "notes.json";
    private string? filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);

    public void LoadFromFile() //Загрузка файла 
    {
        try
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                if (!string.IsNullOrEmpty(json))
                {
                    var noteList = JsonSerializer.Deserialize<List<Note>>(json);
                    strings = noteList.ToDictionary(note => note.ID);
                }
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Ошибка при открытии файла: {ex.Message}");
        };
    }
    public void CreateFileNote() // Создание файла 
    {
        try
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                File.Create(filePath);
                File.WriteAllText(filePath, "[]");
            }
            else
            {
                strings.Clear();
                File.WriteAllText(filePath, "[]");
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Ошибка при создании файла: {ex.Message}");
        }
        ;
    }
    public void AddNotes() // Добавить заметку
    {
        Console.WriteLine("Оставьте заметку (или введите 'q' для выхода в меню)");
        string? anywords = Console.ReadLine();
        qExit(anywords);
        if (string.IsNullOrWhiteSpace(anywords)) {
            Console.WriteLine("Поле с заметкой не должно быть пустым");
            return;
        }
        int newId = strings.Any() ? strings.Keys.Max() + 1 : 1;
        Note newNote = new Note(newId, anywords, DateTime.Now);
        strings.Add(newId, newNote);
        try
        {
            SaveToFile();
            Console.WriteLine("* Заметка успешно создана *\n");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Ошибка при записи: {ex.Message}");
        }
    }
    public void ShowAllNotes() // Посмотреть все заметки
    {
        if (strings == null || strings.Count == 0)
            Console.WriteLine("Тут пока пусто...\n");
        else
        {
            foreach (var n in strings.Values)
            {
                Console.WriteLine("--------------");
                Console.WriteLine($"{n.ID}. {n.Text} ({n.DateAt})");
            }
        }
    }
    public void DeleteNotes() // Удалить заявку 
    {
        if (strings.Count == 0)
        {
            Console.WriteLine("Нет заметок для поиска");
            return;
        }
        Console.WriteLine("Выберите номер заметки для удаления (или введите 'q' для выхода в меню)");

        string ChooseID = Console.ReadLine();
        qExit(ChooseID);
        try
        {
            int ChooseDel = Convert.ToInt32(ChooseID);
            if (strings.Remove(ChooseDel))
            {
                SaveToFile();
                Console.WriteLine("Заметка удалена");
            }
            else
                Console.WriteLine("Заметка с таким ID не найдена");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    public void SearchNotes()//Поиск
    {
        if (strings.Count == 0)
        {
            Console.WriteLine("Нет заметок для поиска");
            return;
        }
        Console.WriteLine(@"Введите ключевое слово для поиска (или введите 'q' для выхода в меню)");
        string? keyword = Console.ReadLine();
        qExit(keyword);
        var SearchWord = strings.Values.Where(n => n.Text.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
        if (SearchWord.Count == 0)
        {
            Console.WriteLine("Совпадений не найдено");
            return;
        }
        else
        {
            Console.WriteLine("Все найденные совпадения");
            foreach (var word in SearchWord)
            {
                Console.WriteLine("----------");
                Console.WriteLine($"{word.ID}. {word.Text} ({word.DateAt})");
                Console.WriteLine("----------");
            }
        }
    }
    public void SearchDate()
    {
        if (strings.Count == 0)
        { Console.WriteLine("Заметок нет"); return; }
        Console.WriteLine("Введите дату (или введите 'q' для выхода в меню)");
        string? input = Console.ReadLine();
        qExit(input);
        try {
            DateTime? keydate = null;
            if (DateTime.TryParse(input, out DateTime parsedDate))
            {
                keydate = parsedDate;
                var SearchDate = strings.Values.Where(n => n.DateAt.Date == parsedDate.Date).ToList();
                if (SearchDate.Count == 0)
                    Console.WriteLine("Заметок на эту дату нет");
                else
                {
                    foreach (var date in SearchDate)
                    {
                        Console.WriteLine("----------");
                        Console.WriteLine($"{date.ID}. {date.Text} ({date.DateAt})");
                        Console.WriteLine("----------");
                    }
                }
            }
            else
                Console.WriteLine("Некорректная введена дата");


        }
        catch (Exception ex){
            Console.WriteLine($"Ошибка в поиске: {ex.Message}");
        } 
    }
   
    public void EditNotes() // Редактирование
    {
        if (strings.Count == 0)
        {
            Console.WriteLine("Нет заметок для изменения");
            return;
        }
        Console.WriteLine("Для изменения записи введите её ID (или введите 'q' для выхода в меню)");
        string? KeyWord = Console.ReadLine();
        qExit(KeyWord);
        try
        {
            int idKeyWord = int.Parse(KeyWord);
            Note Searchid = strings.Values.FirstOrDefault(n => n.ID == idKeyWord);
            if (Searchid != null)
            {
                Console.WriteLine("++++++++++");
                Console.WriteLine($"{Searchid.ID}. {Searchid.Text} ({Searchid.DateAt})");
                Console.WriteLine("++++++++++\n");
                Console.WriteLine("Запись найдена, теперь можете внести изменение");
                string? editText = Console.ReadLine();
                Searchid.Text = editText;
                Searchid.DateAt = DateTime.Now;

                SaveToFile();

                Console.WriteLine("Запись была успешно измененна!");
            }
            else
                Console.WriteLine("Запись с таким ID не найдена");
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Ошибка при изменении:{ex.Message}");
        }
        
    }
    public void SortNoteByDateOld()
    {
        var sorter = strings.Values.OrderByDescending(n => n.DateAt);
        
        foreach (var n in sorter)
        {
            Note srt = new Note(n.ID, n.Text, n.DateAt);
            Console.WriteLine("--------------");
            Console.WriteLine($"{n.ID}. {n.Text} ({n.DateAt})");
        }
        SaveToFile();
    }
    public void SortNoteByDateNew()
    {
        var sorter = strings.Values.OrderBy(n => n.DateAt);
        foreach (var n in sorter)
        {
            Note srt = new Note(n.ID, n.Text, n.DateAt);
            Console.WriteLine("--------------");
            Console.WriteLine($"{n.ID}. {n.Text} ({n.DateAt})");
        }
        SaveToFile();

    }

    private void SaveToFile()
    {
        string json = JsonSerializer.Serialize(strings.Values, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }
    private void qExit(string q)
    {
        if (q == "q" || q == "Q")
            return;
    }
    
}
public class Note
{
    public int ID { get; set; }
    public string Text { get; set; }
    public DateTime DateAt { get; set; }

    public Note() { }

    public Note(int id, string text, DateTime date)
    {
        ID = id;
        Text = text;
        DateAt = date;
    }
}